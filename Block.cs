using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Block
    {
        private int shape;
        private int block_width;
        private int block_height;
        private static Point p0 = new Point(0), p1 = new Point(1);
        public Point[,] blockGrid;
        private static Point[,] I = new Point[1, 4] { { p1, p1, p1, p1 } };
        private static Point[,] L = new Point[3, 3] { { p0, p1, p0 },
                                                      { p0, p1, p0 },
                                                      { p0, p1, p1 }, };
        private static Point[,] T = new Point[3, 3] { { p1, p1, p1 },
                                                      { p0, p1, p0 },
                                                      { p0, p1, p0 }, };
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
        public Block()
        {
            Random RNG = new Random();
            shape = RNG.Next(0, 7);
            blockGrid = blocks[shape];
            block_height = blockGrid.GetLength(0);
            block_width = blockGrid.GetLength(1);
        }
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
        public void pushRight(int x, int y)
        {
            clearBlock(x, y);
            printBlock(x + 1, y);
        }
        public void pushLeft(int x, int y)
        {
            clearBlock(x, y);
            printBlock(x - 1, y);
        }
        public void pushDown(int x, int y)
        {
            clearBlock(x, y);
            printBlock(x, y + 1);
        }
        public void Rotate(int x, int y, int width, int height)
        {
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
        public void Transpose()
        {
            block_height = blockGrid.GetLength(0);
            block_width = blockGrid.GetLength(1);
            Point[,] tempGrid = new Point[block_width, block_height];
            for (int i = 0; i < block_height; i++)
                for (int j = 0; j < block_width; j++)
                    tempGrid[j, i] = blockGrid[i, j];
            blockGrid = tempGrid;
        }
        public void ReverseCols()
        {
            block_height = blockGrid.GetLength(0);
            block_width = blockGrid.GetLength(1);
            Point[,] tempGrid = new Point[block_width, block_height];
            for (int i = 0; i < block_height; i++)
                for (int j = 0; j < block_width; j++)
                    tempGrid[i, j] = blockGrid[block_width - 1 - i, j];
            blockGrid = tempGrid;
        }
        public void ReverseRows()
        {
            block_height = blockGrid.GetLength(0);
            block_width = blockGrid.GetLength(1);
            Point[,] tempGrid = new Point[block_height, block_width];
            for (int i = 0; i < block_width; i++)
                for (int j = 0; j < block_height; j++)
                    tempGrid[j, i] = blockGrid[j, block_width - 1 - i];
            blockGrid = tempGrid;
        }
        public void Rotate()
        {
            Transpose();
            ReverseCols();
        }
        public void ReverseRotation()
        {
            Transpose();
            ReverseRows();
        }
        public bool isRotatable(int x, int y, int[,] droppedGrid, Block current_block)
        {
            block_height = blockGrid.GetLength(0);
            block_width = blockGrid.GetLength(1);
            Block rotated_block = current_block;
            rotated_block.Rotate();
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
            rotated_block.ReverseRotation();
            return true;
        }
    }
}
