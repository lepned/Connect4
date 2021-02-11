using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Connect4
{
    public struct Entry
    {
        public ulong move;
        public int score;
    }

    public class MoveSorter
    {
        public MoveSorter()
        {
            Entries = new Entry[Position.WIDTH];
            Size = 0;
        }
        private int Size { get; set; }

        public Entry[] Entries { get; set; }

        public void Add(ulong move, int score)
        {
            int pos = Size++;

            for (; pos != 0 && Entries[pos - 1].score > score; --pos)
            {
                Entries[pos] = Entries[pos - 1];
            }

            Entries[pos].move = move;
            Entries[pos].score = score;
        }

        /**
         * Get next move
         * return next remaining move with max score and remove it from the container.
         * If no more move is available return 0
         */
        public ulong GetNext()
        {
            if (Size != 0)
                return Entries[--Size].move;
            else
                return 0;
        }

        /**
         * reset (empty) the container
         */
        public void Reset()
        {
            Size = 0;
        }
    }
}
