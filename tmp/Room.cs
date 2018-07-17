using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pair = System.Tuple<int, int>;

namespace ConsoleApp2.Map
{
    public class Room
    {
        Module module;
        int row;
        int col;

        public int Row { get => row; set => row = value; }
        public int Col { get => col; set => col = value; }
        public Module Module { get => module; }

        public Room(Module module, int row, int col)
        {
            this.module = module;
            this.row = row;
            this.col = col;
        }

        internal IEnumerable<Pair> GetExits(int[,] map)
        {
            return module.GetExitsForPosition(map, Row, Col);
        }
    }
}
