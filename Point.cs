using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    class Point
    {
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
        public Point()
        {
            pos_x = 0;
            pos_y = 0;
            state = 0;
        }
        public Point(int state, int x, int y)
        {
            pos_x = x;
            pos_y = y;
            this.state = state;
        }
        public Point(int state)
        {
            this.state = state;
        }
    }
}
