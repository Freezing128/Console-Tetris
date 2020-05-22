# C# Console Application - Tetris remake
This is my take on the classic tetris game.

I decided to make it console-based, just to give it some kind of "retro" vibe instead of doing it in WFA, and also it looked  slighly more appealing to me.

The code may not be optimal and perhaps many things could've been done differently/more efficiently. However, I'm not a professional programmer (yet) and I'm still learning on the way. During the development, I tried to use a lot of handy tools and options that C# language provides and after finishing the game's algorithms and logic, I've also incorporated I/O handling and some kind of basic UI into the game. 

As for IDE, I used MS Visual Studio 2019 (and I've been using it for pretty much any C# based code since highschool) and git for version control (but I'm still trying to get used to it; almost managed to delete ALL of my work thanks to "git reset --hard" - lesson learned :D)

Feel free to use and modify the code for your own needs.
# Main menu

After the game starts up, console window pops up on the screen with simple Main menu.
Menu contains three buttons: 

  "Nová hra" - new game, 
  
  "Výsledky" - scoreboard with high scores 
  
  "Konec" - exit 

User is prompted to navigate throught the menu by UP and DOWN arrow keys and use ENTER key to confirm his/her choice.
# New game

Upon choosing the New game option, user is brought before input screen and asked to write down his/her name, that is later on used for assigning the final score for that player and writting it down into .txt file located inside the application's folder.
Input must be valid and in order to do so, no empty strings or spaces are allowed. User has to repeatedly write down valid name in order to proceed further into the game itself.
# Scoreboard

Upon visiting the scoreboard, console clears itself and shows simply formated table of player names and their scores.
The scores aren't sorted and it just simply reads the data from .txt file located inside the application's folder.

User is prompted to use ESCAPE key in order to get back to the Main menu.
# Exit

Choosing this option exits the game (it's more thoroughly explained in the code itself).
# Game

When the user writes down his/her name and program accepts it, the game finally starts. Playing area is shown on the console screen with score text on the side. First block spawns and slowly falls down. 

The player can use LEFT, RIGHT and DOWN ARROW keys to move the block left/right or to make it instantly fall down. Player also has an option to ROTATE the block 90 degress clockwise by presssing the SPACE key.

After the currently controlled block falls down to the floor or on top of another block, score for placing that block is added and another block spawns at random location at the top of the playing area. 

This goes on until, at some point of the game, blocks stack up all the way to the ceiling of the playing area. When that happens, player loses control over the game and the whole playing area is then filled from the bottom to the top with a little animation. Player's score is written into the file containing all the scores, if it's the player's first attempt OR if the player's current score beats his/her highest. Then, the little endgame menu appears on screen with choices to either go back into the main menu or straight up exit the application.
