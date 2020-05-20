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
        public static Random RNG = new Random();
        static int width = 24;
        static int height = 18;
        static int pos_x = RNG.Next(1, width - 2);
        static int pos_y = 0;
        public static int[,] droppedGrid = new int[width, height];
        public static Block current_block = new Block();
        static ConsoleKeyInfo userInput;
        static bool gameFinished = false;
        const int timeDelay_ms = 300;
        public static DateTime nextMove = DateTime.Now.AddMilliseconds(timeDelay_ms);
        static int clearedLines = 0;

        public static void DrawBorders()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (i == height - 1)
                    {
                        Console.SetCursorPosition(j, i);
                        Console.Write('#');
                    }
                    else
                    {
                        Console.SetCursorPosition(0, i);
                        Console.Write('#');
                        Console.SetCursorPosition(width - 1, i);
                        Console.Write("#");
                        continue;
                    }
                }
            }
        }
        public static void printDroppedGrid()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                    Console.Write(droppedGrid[i, j]);
                Console.WriteLine();
            }
        }
        public static void FillDroppedGrid()
        {
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    droppedGrid[i, j] = 0;

            for (int i = 0; i < height; i++)
            {
                droppedGrid[0, i] = 1;
                droppedGrid[width - 1, i] = 1;
            }
            for (int i = 0; i < width; i++)
                droppedGrid[i, height - 1] = 1;
        }
        public static bool isGridFull(int[,] grid)
        {
            for (int j = 1; j < width - 1; j++)
                if (grid[j, 0] == 1)
                    return true;

            return false;
        }
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
            }
            else // No full lines were found
            { return; }

        }
        public static void GameLogic()
        {
            Console.CursorVisible = false;
            bool firstFrame = true;
            DrawBorders();
            FillDroppedGrid();

            // Game goes on until the filled blocks reach the top of the grid
            while (!gameFinished)
            {
                // Check if the grid is already filled - if that's the case, then game's over
                if (isGridFull(droppedGrid))
                {
                    gameFinished = true;
                    Console.Clear();
                    break;
                }
                // Fix for printing the first frame of newly created block, so the blocks all appear falling from the top
                if (firstFrame)
                {
                    current_block.printBlock(pos_x, pos_y);
                    firstFrame = false;
                }

                // Movement logic
                if (Console.KeyAvailable)
                {
                    // Read key without displaying and decide what to do after that
                    userInput = Console.ReadKey(true);
                    switch (userInput.Key)
                    {
                        // Movement to the right
                        case ConsoleKey.RightArrow:
                            // Check if there's available space one pixel to the right
                            if (!current_block.isFilledRight(pos_x, pos_y, droppedGrid) && !Console.KeyAvailable)
                            {
                                current_block.pushRight(pos_x, pos_y);
                                pos_x++;
                                DrawBorders();
                            }
                            break;
                        // Movement to the left
                        case ConsoleKey.LeftArrow:
                            // Check if there's available space one pixel to the right
                            if (!current_block.isFilledLeft(pos_x, pos_y, droppedGrid) && !Console.KeyAvailable)
                            {
                                current_block.pushLeft(pos_x, pos_y);
                                pos_x -= 1;
                                DrawBorders();
                            }
                            break;

                        // Instantly place block to the lowest possible level
                        case ConsoleKey.DownArrow:
                            while (!current_block.isFilledUnder(pos_x, pos_y, droppedGrid) && !Console.KeyAvailable)
                            {
                                current_block.pushDown(pos_x, pos_y);
                                pos_y++;
                            }
                            break;
                        // Rotate
                        case ConsoleKey.Spacebar:
                            if (current_block.blockGrid.GetLength(0) != current_block.blockGrid.GetLength(1))
                            {
                                current_block.Rotate(pos_x, pos_y, width, height);
                                current_block.printBlock(pos_x, pos_y);
                            }
                            else
                            {
                                if (current_block.isRotatable(pos_x, pos_y, droppedGrid, current_block))
                                {
                                    current_block.Rotate();
                                    current_block.printBlock(pos_x, pos_y);
                                }
                            }
                            DrawInnerGrid(droppedGrid);
                            break;
                        default:
                            break;
                    }
                }
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
                        current_block.saveToGrid(pos_x, pos_y, droppedGrid);
                        current_block.printBlock(pos_x, pos_y);

                        // Check line completion
                        fullLineCheck();

                        // Spawn another block
                        pos_x = RNG.Next(1, width - 2);
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
        }
        static void Main()
        {
            GameLogic();
        }
    }
}



