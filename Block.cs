/***
 * C# Console application - Tetris remake
 * @Author: Freezing
 * @Date: 22/05/2020
 * 
 * Block.cs
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Block
    {   
        /*_____________________________________________________________________________________________________*/
        // VARIABLES
        /*_____________________________________________________________________________________________________*/
        private int shape;
        private int block_width;
        private int block_height;
        private static Point p0 = new Point(0), p1 = new Point(1);
        public Point[,] blockGrid;
        // All available blocks
        private static Point[,] I = new Point[1, 4] { { p1, p1, p1, p1 } };
        private static Point[,] L = new Point[3, 3] { { p1, p1, p1 },
                                                      { p0, p0, p1 },
                                                      { p0, p0, p0 }, };
        private static Point[,] T = new Point[3, 3] { { p1, p0, p0 },
                                                      { p1, p1, p1 },
                                                      { p1, p0, p0 }, };
        private static Point[,] S = new Point[3, 3] { { p0, p1, p0 },
                                                      { p1, p1, p0 },
                                                      { p1, p0, p0 }, };
        private static Point[,] Z = new Point[3, 3] { { p1, p0, p0 },
                                                      { p1, p1, p0 },
                                                      { p0, p1, p0 }, };
        private static Point[,] J = new Point[3, 3] { { p0, p0, p1 },
                                                      { p1, p1, p1 },
                                                      { p0, p0, p0 }, };
        private static Point[,] O = new Point[2, 2] { { p1, p1 },
                                                      { p1, p1 }, };
        public static List<Point[,]> blocks = new List<Point[,]>() { I, L, T, S, Z, J, O };
        public int Shape
        {
            get { return shape; }
            set { shape = value; }
        }
        /***
         * Block's default constructor
         */
        public Block()
        {
            Random RNG = new Random();
            shape = RNG.Next(0, 7);
            blockGrid = blocks[shape];
            block_height = blockGrid.GetLength(0);
            block_width = blockGrid.GetLength(1);
        }
        /***
         * Prints current block onto the screen 
         * @param int x - horizontal position of the current block
         * @param int y - vertical postition of the current block
         */
        public void printBlock(int x, int y)
        {
            block_height = blockGrid.GetLength(0);
            block_width = blockGrid.GetLength(1);
            for (int i = 0; i < block_height; i++)
            {
                for (int j = 0; j < block_width; j++)
                {
                    Point current = blockGrid[i, j];
                    if (current.State == 1)
                    {
                        Console.SetCursorPosition(x + i, y + j);
                        Console.Write("X");
                    }
                }
            }
        }
        /***
         * Clears current block from the screen - used inside movement functions to accurately draw
         * the current block onto the new location to which it has moved
         * @param int x - horizontal position of the current block
         * @param int y - vertical postition of the current block
         */
        public void clearBlock(int x, int y)
        {
            block_height = blockGrid.GetLength(0);
            block_width = blockGrid.GetLength(1);
            for (int i = 0; i < block_height; i++)
            {
                for (int j = 0; j < block_width; j++)
                {
                    Point current = blockGrid[i, j];
                    if (current.State == 1)
                    {
                        Console.SetCursorPosition(x + i, y + j);
                        Console.Write(" ");
                    }
                }
            }
        }
        /***
         * Saves the current block into the grid by changing grid's values from empty blocks (0)
         * to already occupied, filled blocks (1)
         * @param int x - horizontal position of the current block
         * @param int y - vertical postition of the current block
         * @param int[,] droppedGrid - playing area storing information about filled blocks (1) and available space (0)
         */
        public void saveToGrid(int x, int y, int[,] droppedGrid)
        {
            block_height = blockGrid.GetLength(0);
            block_width = blockGrid.GetLength(1);
            for (int i = 0; i < block_height; i++)
            {
                for (int j = 0; j < block_width; j++)
                {
                    Point current = blockGrid[i, j];
                    if (current.State == 1)
                        droppedGrid[i + x, j + y] = 1;
                }
            }
        }
        /***
         * Calculates score for each block's pixel - used in determining the "price" for each placed block
         * @returns value (or "price") of current block in terms of scoring
         */
        public int calcScoreForBlock()
        {
            int pieces = 0;
            block_height = blockGrid.GetLength(0);
            block_width = blockGrid.GetLength(1);
            for (int i = 0; i < block_height; i++)
            {
                for (int j = 0; j < block_width; j++)
                {
                    Point current = blockGrid[i, j];
                    if (current.State == 1)
                        pieces++;
                }
            }
            return pieces * 2;
        }
        /***
         * Performs check whether there's available space one pixel to the right of the current block
         * @param int x - horizontal position of the current block
         * @param int y - vertical postition of the current block
         * @param int[,] droppedGrid - playing area storing information about filled blocks (1) and available space (0)
         * @returns true if there's no available space ("Is the block 1 pixel to the right filled => true")
         *          false if there is some actual space ("Is the block 1 pixel to the right filled ? => false")
         */
        public bool isFilledRight(int x, int y, int[,] droppedGrid)
        {
            block_height = blockGrid.GetLength(0);
            block_width = blockGrid.GetLength(1);
            for (int i = 0; i < block_height; i++)
            {
                for (int j = block_width - 1; j >= 0; j--)
                {
                    // Temporary variable storing data about the current point
                    Point current = blockGrid[i, j];
                    // If there's filled point one pixel to the right of the current point, there's no space
                    if (current.State == 1 && droppedGrid[i + x + 1, j + y] == 1)
                        return true;
                }
            }
            return false;
        }
        /***
         * Performs check whether there's available space one pixel to the left of the current block
         * @param int x - horizontal position of the current block
         * @param int y - vertical postition of the current block
         * @param int[,] droppedGrid - playing area storing information about filled blocks (1) and available space (0)
         * @returns true if there's no available space ("Is the block 1 pixel to the left filled => true")
         *          false if there is some actual space ("Is the block 1 pixel to the left filled ? => false")
         */
        public bool isFilledLeft(int x, int y, int[,] droppedGrid)
        {
            block_height = blockGrid.GetLength(0);
            block_width = blockGrid.GetLength(1);
            for (int i = 0; i < block_height; i++)
            {
                for (int j = 0; j < block_width; j++)
                {
                    // Temporary variable storing data about the current point
                    Point current = blockGrid[i, j];
                    // If there's filled point one pixel to the left of the current point, there's no space
                    if (current.State == 1 && droppedGrid[i + x - 1, j + y] == 1)
                        return true;
                }
            }
            return false;
        }
        /***
         * Performs check whether there's available space one pixel below the current block
         * @param int x - horizontal position of the current block
         * @param int y - vertical postition of the current block
         * @param int[,] droppedGrid - playing area storing information about filled blocks (1) and available space (0)
         * @returns true if there's no available space ("Is the block 1 pixel below current block filled => true")
         *          false if there is some actual space ("Is the block 1 pixel below current block filled ? => false")
         */
        public bool isFilledUnder(int x, int y, int[,] droppedGrid)
        {
            block_height = blockGrid.GetLength(0);
            block_width = blockGrid.GetLength(1);
            for (int i = 0; i < block_height; i++)
            {
                for (int j = block_width - 1; j >= 0; j--)
                {
                    // Temporary variable storing data about the current point
                    Point current = blockGrid[i, j];
                    // If there's filled point one pixel under the current point, the block reached it's bottom
                    if (current.State == 1 && droppedGrid[i + x, j + y + 1] == 1)
                        return true;
                }
            }
            return false;
        }
        /***
         * Pushes the current block one pixel to the right
         * @param int x - horizontal position of the current block
         * @param int y - vertical postition of the current block
         */
        public void pushRight(int x, int y)
        {
            clearBlock(x, y);
            printBlock(x + 1, y);
        }
        /***
         * Pushes the current block one pixel to the left
         * @param int x - horizontal position of the current block
         * @param int y - vertical postition of the current block
         */
        public void pushLeft(int x, int y)
        {
            clearBlock(x, y);
            printBlock(x - 1, y);
        }
        /***
         * Pushes the current block one pixel down
         * @param int x - horizontal position of the current block
         * @param int y - vertical postition of the current block
         */
        public void pushDown(int x, int y)
        {
            clearBlock(x, y);
            printBlock(x, y + 1);
        }
        /***
         * Rotates the blocks that don't have the same width as size ("I" blocks)
         * @param int x - horizontal position of the current block
         * @param int y - vertical postition of the current block
         * @param int width - width of the playing area
         * @parma int height - height of the playing area
         */
        public void Rotate(int x, int y, int width, int height)
        {
            // Vertical and horizontal sizes of current block
            block_height = blockGrid.GetLength(0);
            block_width = blockGrid.GetLength(1);
            if (block_height != block_width)
            {
                int horizontal_cnt = block_height;
                int vertical_cnt = block_width;
                if (horizontal_cnt == 1 && x < width - vertical_cnt)
                {
                    blockGrid = new Point[4, 1] { { p1 }, { p1 }, { p1 }, { p1 } };
                }
                if (horizontal_cnt == 4 && y + horizontal_cnt - 1 < height - 1)
                {
                    blockGrid = new Point[1, 4] { { p1, p1, p1, p1 } };
                }
            }
            else
            {
                Transpose();
                ReverseCols();
            }
        }
        /***
         * Transpositions the indexes inside of the block's grid
         */
        public void Transpose()
        {
            // Vertical and horizontal sizes of current block
            block_height = blockGrid.GetLength(0);
            block_width = blockGrid.GetLength(1);
            Point[,] tempGrid = new Point[block_width, block_height];
            for (int i = 0; i < block_height; i++)
                for (int j = 0; j < block_width; j++)
                    tempGrid[j, i] = blockGrid[i, j];
            blockGrid = tempGrid;
        }
        /***
         * Reverses columns inside of the block's grid
         */
        public void ReverseCols()
        {
            // Vertical and horizontal sizes of current block
            block_height = blockGrid.GetLength(0);
            block_width = blockGrid.GetLength(1);
            Point[,] tempGrid = new Point[block_width, block_height];
            for (int i = 0; i < block_height; i++)
                for (int j = 0; j < block_width; j++)
                    tempGrid[i, j] = blockGrid[block_width - 1 - i, j];
            blockGrid = tempGrid;
        }
        /***
         * Reverses rows inside of the block's grid
         */
        public void ReverseRows()
        {
            // Vertical and horizontal sizes of current block
            block_height = blockGrid.GetLength(0);
            block_width = blockGrid.GetLength(1);
            Point[,] tempGrid = new Point[block_height, block_width];
            for (int i = 0; i < block_width; i++)
                for (int j = 0; j < block_height; j++)
                    tempGrid[j, i] = blockGrid[j, block_width - 1 - i];
            blockGrid = tempGrid;
        }
        /***
         * Rotates blocks clockwise by 90 degrees
         */
        public void Rotate()
        {
            Transpose();
            ReverseCols();
        }
        /***
         * Rotates blocks counter-clockwise by 90 degrees
         */
        public void ReverseRotation()
        {
            Transpose();
            ReverseRows();
        }
        /***
         * Performs check whether the block is rotatable or not (if there's available space near it)
         * by actually rotating it and then going pixel by pixel and comparing each pixel with corresponding
         * coordinates to the playing area and checks if there's collision or not
         * @param int x - horizontal position of the current block
         * @param int y - vertical postition of the current block
         * @param int[,] droppedGrid - playing area storing information about filled blocks (1) and available space (0)
         * @param Block current_block - current block that is meant to be rotated
         * @returns true if the block is rotatable 
         *          false if not
         */
        public bool isRotatable(int x, int y, int[,] droppedGrid, Block current_block)
        {
            // Vertical and horizontal sizes of current block
            block_height = blockGrid.GetLength(0);
            block_width = blockGrid.GetLength(1);

            // Rotate the block
            Block rotated_block = current_block;
            rotated_block.Rotate();

            // Compare it pixel by pixel to the playing area with corresponding coordinates
            for (int i = 0; i < rotated_block.block_height; i++)
            {
                for (int j = 0; j < rotated_block.block_width; j++)
                {
                    Point rotated_block_point = rotated_block.blockGrid[i, j];
                    try
                    {
                        if (rotated_block_point.State == 1 && droppedGrid[i + x, j + y] == 1)
                        {
                            rotated_block.ReverseRotation();
                            return false;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            // Rotate the block back
            rotated_block.ReverseRotation();
            return true;
        }
    }
}
