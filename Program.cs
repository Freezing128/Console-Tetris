/***
 * C# Console application - Tetris remake
 * @Author: Freezing
 * @Date: 22/05/2020
 * 
 * Program.cs
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Diagnostics;
using System.Media;
using System.Resources;
using System.IO;
using System.Reflection;

namespace Tetris
{
    static class Program
    {
        /*_____________________________________________________________________________________________________*/
        // VARIABLES
        /*_____________________________________________________________________________________________________*/
        public static Random RNG = new Random();
        public static DateTime nextMove = DateTime.Now.AddMilliseconds(timeDelay_ms);
        public static Block current_block = new Block();
        public static int[,] droppedGrid = new int[width, height];
        static int width = 24;
        static int height = 18;
        static int pos_x = RNG.Next(1, width - 4);
        static int pos_y = 0;
        static int decision = 0;
        static int lineNumber;
        static int timeDelay_ms = 300;
        static int clearedLines = 0;
        static int score = 0;
        static bool gameFinished = false;
        static bool firstFrame = true;
        static bool firstFrameMenu = true;
        static bool running = true;
        static bool endgameLoop = true;
        static string playerName = null;
        static string path = Directory.GetCurrentDirectory() + @"\scores.txt";
        static ConsoleKeyInfo userInput;
        /*_____________________________________________________________________________________________________*/
        // MISC OR UI FUNCTIONS
        /*_____________________________________________________________________________________________________*/
        /***
         * Helping function to draw floor border
         * @param int y - index of the row (y coordinate) where the floor is supposed to be
         * @param int x - index of each pixel on horizontal axis (x coordinate)
         * @returns 0 just for the sake of using the ternary operator to shorten condition inside DrawBorders() function
         */
        public static int drawFloor(int y, int x)
        {
            Console.SetCursorPosition(x, y);
            Console.Write('#');
            return 0;
        }
        /***
         * Helping function to draw side borders
         * @param int y - index of the row (y coordinate) where the borders are supposed to be
         * @returns 1 just for the sake of using the ternary operator to shorten condition inside DrawBorders() function
         */
        public static int drawSides(int y)
        {
            Console.SetCursorPosition(0, y);
            Console.Write('#');
            Console.SetCursorPosition(width - 1, y);
            Console.Write("#");
            return 1;
        }
        /***
         * Prints borders of the playing area into the console screen
         */
        public static void DrawBorders()
        {
            int decision;
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    decision = (y == height - 1) ? drawFloor(y, x) : drawSides(y);
        }
        /***
         * Prints user's current score to the screen
         */
        public static void showScore()
        {
            Console.SetCursorPosition(25, 10);
            Console.WriteLine("Score: " + score.ToString());
        }
        /***
         * Fills playing area with filled and empty blocks represented by 1 (filled) and 0 (empty)
         * It's executed at the start of the game in order to set up the whole game area
         */
        public static void FillDroppedGrid()
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++) // Fills sides and floor with "1" to distinct borders from empty blocks
                    droppedGrid[i, j] = ((i == 0 || i == width - 1) && j != height - 1) ? 1 : ((j == height - 1) ? 1 : 0);
        }
        /***
         * Prints inner grid (playing area surrounded with the borders) into the console screen
         * @param int[,] grid - array of the playing area
         */
        public static void DrawInnerGrid(int[,] grid)
        {
            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 0; j < height - 1; j++)
                {
                    Console.SetCursorPosition(i, j);
                    Console.Write(grid[i, j] == 1 ? "X" : " ");
                }
            }
        }
        /***
         * DEBUG FUNCTION - USED FOR TESTING
         * Prints the whole playing area with filled and empty blocks represented by 1 (filled) and 0 (empty)
         */
        public static void printDroppedGrid()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                    Console.Write(droppedGrid[j, i]);
                Console.WriteLine();
            }
        }
        /***
         * Animation that is triggered upon game over
         */
        public static void gameOverAnimation()
        {
            bool switchDirection = false;
            for (int j = height - 2; j >= 0; j--)
            {
                for (int i = 1; i < width - 1; i++)
                {
                    if (switchDirection)
                        Console.SetCursorPosition(i, j);
                    else
                        Console.SetCursorPosition(width - 1 - i, j);
                    Console.Write("▓");
                    System.Threading.Thread.Sleep(5);
                }
                switchDirection = switchDirection == false ? true : false;
            }
        }
        /*_____________________________________________________________________________________________________*/
        // I/O HANDLING
        /*_____________________________________________________________________________________________________*/
        /***
         * Checks if the string containg player's name isn't empty or that it doesn't contain spaces (' ')
         * @param string name - string with given player's name
         */
        public static bool isValidName(string name)
        {
            // Empty string check
            if (name.Length <= 0)
                return false;
            // Check for spaces - there can't be any spaces inside player's name
            for(int i = 0; i < name.Length; i++)
            {
                if (name[i] == ' ')
                    return false;
            }
            return true;
        }
        /***
         * Reads player's name from stdin
         */
        public static void readPlayerName()
        {
            bool readLine = false;
            Console.CursorVisible = true;

            // Get the player's name from the console stdin
            while (!readLine)
            {
                Console.SetCursorPosition(15, 10);
                Console.Write("Zadejte jmeno hrace: ");
                playerName = Console.ReadLine();

                // Check if the name is in valid format
                if (isValidName(playerName))
                    readLine = true;
            }

            // Check if the file with scores exists; if not, create it
            if (!File.Exists(path))
            {
                var newFile = File.Create(path);
                newFile.Close();
            }

            // Read each line and check if the given name is already there or not;
            // If it is, get the line number of it
            // If it isn't, append new line to the file with the player's name and 0 score
            string[] lines = File.ReadAllLines(path);
            if(lines.Length == 0) // File with scores is empty, so it adds first line
            {
                TextWriter tw = new StreamWriter(path, true);
                tw.WriteLine(playerName + ":0");
                tw.Close();
                lineNumber = 0;
            }
            else
            {
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (!string.IsNullOrEmpty(line) && line.Contains(playerName))
                    {
                        lineNumber = i;
                        break;
                    }
                    if (i == lines.Length - 1)
                    {
                        TextWriter tw = new StreamWriter(path, true);
                        tw.WriteLine(playerName + ":0");
                        tw.Close();
                        lineNumber = i + 1;
                    }
                }
            }
            Console.Clear();
        }
        /***
         * Saves player's final score into the file containing the high scores IF his/her score was better than before
         * or if it's the first entry for that player
         */
        public static void saveScore()
        {
            // Load all lines from the file containing scores
            string[] lines = File.ReadAllLines(path);

            // Read the saved score on current player's line from the file containing information about scoring and save that score 
            // into savedScore variable (converted from chars into int)
            int savedScore = 0;
            for(int i = lines[lineNumber].Length - 1; i >= 0; i--)
            {
                if ((lines[lineNumber])[i] != ':' && ('0' <= (int)(lines[lineNumber])[i] || (int)(lines[lineNumber])[i] <= '9'))
                    savedScore += ((int)(lines[lineNumber])[i] - '0') * (int)(Math.Pow(10, ((lines[lineNumber].Length - 1) - i)));
                else
                    break;
            }
            // Compare the saved score with player's current score, if the current score's higher, rewrite it in the file
            if(score > savedScore)
            {
                // Change the score in the corresponding line after the ":" symbol
                string updatedString = playerName + ":" + score.ToString();
                lines[lineNumber] = updatedString;
                File.WriteAllLines(path, lines);
            }
        }
        /*_____________________________________________________________________________________________________*/
        // MENU UI
        /*_____________________________________________________________________________________________________*/
        /***
         * Shows main menu of the game onto the console with interactive controls for the player
         */
        public static void MainMenu()
        {
            // Initial UI setup
            int xM = 15;
            int yM = 10;
            Console.CursorVisible = false;

            // Game runs until bool running is set to false, this way the player can be brought back to the main menu
            // even after failing at his/her gameplay, so that he/she can play again (the user is brought back here
            // also after looking at the scoreboard)
            while (running) 
            {
                // First frame of menu - my way of fixing some weird bug that made the control buttons dissapear
                if(firstFrameMenu)
                {
                    Console.SetCursorPosition(xM, yM);
                    Console.Write("> Nová hra");
                    Console.SetCursorPosition(xM, yM + 2);
                    Console.Write("  Výsledky");
                    Console.SetCursorPosition(xM, yM + 4);
                    Console.Write("  Konec");
                    firstFrameMenu = false;
                }

                // Get the input from the player - technically runs by the same logic as game's movement engine and endgame menu
                userInput = Console.ReadKey(true);
                switch(userInput.Key)
                {
                    case ConsoleKey.DownArrow:
                        if (yM < 14)
                        {
                            Console.SetCursorPosition(xM, yM);
                            Console.Write(" ");
                            yM += 2;
                            decision++;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if (yM > 10)
                        {
                            Console.SetCursorPosition(xM, yM);
                            Console.Write(" ");
                            yM -= 2;
                            decision--;
                        }
                        break;
                    case ConsoleKey.Enter:
                        // After hitting enter, what happens depends on the highlighted button
                        // decision = {0 (new game) | 1 (scoreboard) | 2 (exit)}
                        if (decision == 0) // Starts New game
                        {
                            endgameLoop = true;
                            Console.Clear();
                            readPlayerName();
                            GameLogic();
                        }
                        else if(decision == 1) // Shows scoreboard
                        {
                            Console.Clear();
                            showHighScores();
                            xM = 15;
                            yM = 10;
                        }
                        else if(decision == 2) // Exits the application
                        {
                            exitGame();
                        }
                        break;
                }

                // Cursor positioning ("highlighting" the current button)
                Console.SetCursorPosition(xM, yM);
                Console.Write("> ");
            }
        }
        /***
         * Shows the scoreboard saved in "scores.txt" file with high scores for each player that played the game
         */
        public static void showHighScores()
        {
            // Initital UI/technical setup
            bool getBack = false;
            int console_x = 20;
            int console_y = 10;
            int shift_x = 1;
            Console.SetCursorPosition(console_x, console_y - 6);
            Console.Write("Nejvyssi dosazene skore vsech zapsanych hracu");
            Console.SetCursorPosition(console_x, console_y - 3);
            Console.Write("Stiskni ESC pro navrat do menu");
            Console.SetCursorPosition(console_x, console_y - 1);
            Console.Write("______________________________________________________");

            // Print the whole, formatted, "scores.txt" file without ':' inbetween names and scores
            // Load all lines from the file containing scores
            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    if ((lines[i])[j] == ':')
                    {
                        shift_x = 5;
                    }
                    else
                    {
                        Console.SetCursorPosition(console_x + j + shift_x + 1, console_y + i + 1);
                        Console.Write((lines[i])[j]);
                    }
                }
                shift_x = 1;
            }

            // If the user presses "ESC" key, it brings him/her back to the main menu
            while (!getBack)
            {
                userInput = Console.ReadKey(true);
                if (userInput.Key == ConsoleKey.Escape)
                {
                    getBack = true;
                    firstFrameMenu = true;
                    decision = 0;
                }
            }
            Console.Clear();
        }
        /***
         * Exits application by flipping bool running to false => that ends the main menu's while loop, therefore app ends
         */
        public static void exitGame()
        {
            running = false;
            Console.Clear();
            Console.WriteLine("Diky za pouziti program a doufam, ze se ti libil!\nVytvoril Freezing v roce 2020");
            System.Threading.Thread.Sleep(1500);
        }
        /***
         * Little endgame menu that pops up after player fails at the game and the game's over
         */
        public static void EndgameUI()
        {
            // Initial UI/technical setup
            decision = 0;
            int xM = 15;
            int yM = 20;
            Console.CursorVisible = false;
            System.Threading.Thread.Sleep(100);

            // Write player's score into the file and reset it for the next playtrough
            saveScore();
            score = 0;

            // Cursor positioning and buttons
            Console.SetCursorPosition(15, 20);
            Console.Write("Konec hry!");
            Console.SetCursorPosition(xM, (yM += 5));
            Console.Write("> Navrat do menu");
            Console.SetCursorPosition(xM, yM + 2);
            Console.Write("  Konec");

            // Get the input from the player - technically runs by the same logic as game's movement engine and main menu
            while (endgameLoop)
            {
                userInput = Console.ReadKey(true);
                switch (userInput.Key)
                {
                    case ConsoleKey.DownArrow:
                        if (yM < 27)
                        {
                            Console.SetCursorPosition(xM, yM);
                            Console.Write(" ");
                            yM += 2;
                            decision++;
                        }
                        break;
                    case ConsoleKey.UpArrow:
                        if (yM > 25)
                        {
                            Console.SetCursorPosition(xM, yM);
                            Console.Write(" ");
                            yM -= 2;
                            decision--;
                        }
                        break;
                    case ConsoleKey.Enter:
                        // After hitting enter, what happens depends on the highlighted button
                        // decision = {0 (new game) | 1 (exit) }
                        if (decision == 0) // Brings the player back to the main menu
                        {
                            gameFinished = false;
                            firstFrameMenu = true;
                            endgameLoop = false;
                            Console.Clear();
                            return;
                        }
                        if (decision == 1)  // Exits the application
                        {
                            exitGame();
                        }
                        break;
                }

                // Cursor positioning ("highlighting" the current button)
                Console.SetCursorPosition(xM, yM);
                Console.Write("> ");
            }
        }
        /*_____________________________________________________________________________________________________*/
        // GAME ENGINE
        /*_____________________________________________________________________________________________________*/
        /***
         * Checks whether the playing area (grid) is filled to the top at some column or not
         * It only needs to check the ceiling of the whole grid in order to confirm that
         * @param int[,] grid - array of the playing area
         * @returns true when blocks are stacked way to the top of the grid somewhere
         *          false when no blocks were detected at the top of the grid
         */
        public static bool isGridFull(int[,] grid)
        {
            for (int j = 1; j < width - 1; j++)
                if (grid[j, 0] == 1)
                    return true;

            return false;
        }
        /***
         * Function performing full line checking by trying to find one or multiple rows with all the blocks marked 
         * as filled (1) inside of the playing area's grid, in order to clear them, and push all the other blocks
         * above these lines down by amount of cleared lines
         */
        public static void fullLineCheck()
        {
            int filledBlocks = 0;
            int fullLines = 0;
            int pushCoeficient = 0;
            int startHeight = 0;
            int endHeight = 0;

            // Iterate through the grid from the bottom to the top
            for (int i = height - 2; i >= 0; i--)
            {
                for (int j = 1; j < width - 1; j++)
                {
                    // Count filled blocks at each row
                    if (droppedGrid[j, i] == 1)
                        filledBlocks++;
                }
                // If there aren't any filled block at some row, end the loop, since there's no need to check remaining rows
                if (filledBlocks == 0)
                {
                    endHeight = i;
                    break;
                }
                // If there are (width - 2) filled blocks, one or multiple lines are to be cleared
                if (filledBlocks == width - 2)
                {
                    // Check all rows above the row with filled blocks and stop until there aren't any filled blocks (condition above)
                    // In case that more than one lines are completely filled, increase the push index
                    pushCoeficient++;
                    clearedLines++;
                    if (fullLines == 0)
                        startHeight = i;
                    fullLines++;
                }
                filledBlocks = 0;
            }
            // Push all blocks above complete lines in the range of rows with at least one
            // or more filled blocks one + push index lines down
            if (fullLines != 0)
            {
                System.Threading.Thread.Sleep(500);
                for (int i = startHeight; i >= endHeight; i--)
                {
                    for (int j = 1; j < width - 1; j++)
                    {
                        droppedGrid[j, i] = droppedGrid[j, i - pushCoeficient];
                    }
                }
                DrawInnerGrid(droppedGrid);

                // Increase difficulty with each two rows cleared - speed up the game by 5ms
                if ((clearedLines % 2 == 0 || clearedLines != 0) && (timeDelay_ms - 5 > 0))
                    timeDelay_ms -= 5;

                // Add score for completing one or more lines and show it
                score += fullLines * 5;
                showScore();
            }
            else // No full lines were found
            { return; }
        }
        /***
         * Controls timing, event of pushing each block down after
         * a certain amount of milliseconds and spawning of the next block when the current block
         * has fallen down to the floor, or on top of another block
         */
        public static void gameTimer() 
        {
            // Timing handling
            if (!gameFinished && DateTime.Now > nextMove)
            {
                // Block moves one pixel down
                if (!current_block.isFilledUnder(pos_x, pos_y, droppedGrid))
                {
                    current_block.pushDown(pos_x, pos_y);
                    pos_y++;
                }
                else // Block is at the floor or on top of another block
                {

                    // Add score for fallen block and show it
                    score += current_block.calcScoreForBlock();
                    showScore();

                    // Save current block into the grid and print it
                    current_block.saveToGrid(pos_x, pos_y, droppedGrid);
                    current_block.printBlock(pos_x, pos_y);

                    // Check line completion
                    fullLineCheck();

                    // Spawn another block
                    pos_x = RNG.Next(1, width - 4);
                    pos_y = 0;
                    current_block = new Block();

                    DrawInnerGrid(droppedGrid);
                    firstFrame = true;
                }
                // Reset the timer
                nextMove = DateTime.Now.AddMilliseconds(timeDelay_ms);
            }
            // DrawBorders();
            current_block.printBlock(pos_x, pos_y);
        }
        /***
         * Movement engine that handles logic behind movement based on user's key input
         */
        public static void movementEngine()
        {
            // Movement logic
            if (Console.KeyAvailable)
            {
                // Read key without displaying and decide what to do after that
                userInput = Console.ReadKey(true);
                switch (userInput.Key)
                {
                    // Move the block to the right - right arrow key
                    case ConsoleKey.RightArrow:
                        // Check if there's available space one pixel to the right
                        if (!current_block.isFilledRight(pos_x, pos_y, droppedGrid) && !Console.KeyAvailable)
                        {
                            current_block.pushRight(pos_x, pos_y);
                            pos_x += 1;
                            DrawBorders();
                        }
                        break;
                    // Move the block to the left - left arrow key
                    case ConsoleKey.LeftArrow:
                        // Check if there's available space one pixel to the right
                        if (!current_block.isFilledLeft(pos_x, pos_y, droppedGrid) && !Console.KeyAvailable)
                        {
                            current_block.pushLeft(pos_x, pos_y);
                            pos_x -= 1;
                            DrawBorders();
                        }
                        break;

                    // Push the block down to the lowest possible level - down arrow key
                    case ConsoleKey.DownArrow:
                        // Make sure that there's available space one pixel below the current block
                        while (!current_block.isFilledUnder(pos_x, pos_y, droppedGrid) && !Console.KeyAvailable)
                        {
                            current_block.pushDown(pos_x, pos_y);
                            pos_y++;
                        }
                        break;
                    // Rotate the block clockwise by 90 deg - spacebar key
                    case ConsoleKey.Spacebar:
                        // Edge case for the "I" block
                        if (current_block.blockGrid.GetLength(0) != current_block.blockGrid.GetLength(1))
                        {
                            current_block.Rotate(pos_x, pos_y, width, height);
                            current_block.printBlock(pos_x, pos_y);
                        }
                        else // Rotation for other blocks which arrays have the same width as height
                        {
                            // Make sure that there's available space to the left, right or below the block
                            if (current_block.isRotatable(pos_x, pos_y, droppedGrid, current_block))
                            {
                                current_block.Rotate();
                                current_block.printBlock(pos_x, pos_y);
                            }
                        }
                        DrawInnerGrid(droppedGrid);
                        break;
                    default: // As for other keys, nothing happens, really
                        break;
                }
            }
        }
        /***
         * Controls the flow of the game and keeps it going inside of the while loop until,
         * somewhere, the playing area get's filled from the bottom to the top
         */
        public static void GameLogic()
        {
            Console.CursorVisible = false;
            DrawBorders();
            FillDroppedGrid();
            showScore();

            // Game goes on until the filled blocks reach the top of the grid
            while (!gameFinished)
            {
                // Check if the grid is already filled - if that's the case, then game's over
                if (isGridFull(droppedGrid))
                {
                    gameFinished = true;
                    gameOverAnimation();
                    EndgameUI();
                    break;
                }
                // Fix for printing the first frame of newly created block, so the blocks all appear falling from the top
                if (firstFrame)
                {
                    current_block.printBlock(pos_x, pos_y);
                    firstFrame = false;
                }
                // Movement and timing logic
                movementEngine();
                gameTimer();
            }

        }
        /*_____________________________________________________________________________________________________*/
        /***
         * Main function
         */
        static void Main()
        {
             MainMenu();
        }
        /*_____________________________________________________________________________________________________*/
    }
}



