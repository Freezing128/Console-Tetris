/***
 * C# Console application - Tetris remake
 * @Author: Freezing
 * @Date: 22/05/2020
 * 
 * Point.cs
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    /***
     * Class for storing the information about each pixel ("point") of some given block
     */
    class Point
    {
        // Class properties
        private int pos_x;
        private int pos_y;
        private int state;
        public int Pos_X
        {
            get { return pos_x; }
            set { pos_x = value; }
        }
        public int Pos_Y
        {
            get { return pos_y; }
            set { pos_y = value; }
        }
        public int State
        {
            get { return state; }
            set { state = value; }
        }
        // Default constructor
        public Point()
        {
            pos_x = 0;
            pos_y = 0;
            state = 0;
        }
        // Alternative constuctor #1
        public Point(int state, int x, int y)
        {
            pos_x = x;
            pos_y = y;
            this.state = state;
        }
        // Alternative constuctor #2
        public Point(int state)
        {
            this.state = state;
        }
    }
}
