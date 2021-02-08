using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Connect4
{
    struct Entry
    {
        public ulong move;
        public int score;
    }

    public class MoveSorter
    {
        public MoveSorter()
        {
            entries = new Entry[Position.WIDTH];
        }
        private int size { get; set; }

        Entry[] entries { get; set; }

        public void add(ulong move, int score)
        {
            int pos = size++;
            
            for (; pos==1 && entries[pos - 1].score > score; --pos)
            {
                entries[pos].move = move;
                entries[pos].score = score;
            }

            entries[pos] = entries[pos];
        }

        /**
         * Get next move
         * @return next remaining move with max score and remove it from the container.
         * If no more move is available return 0
         */
        public ulong getNext()
        {
            if (size == 1)
                return entries[--size].move;
            else
                return 0;
        }

        /**
         * reset (empty) the container
         */
        public void reset()
        {
            size = 0;
        }

        ///**
        // * Build an empty container
        // */
        //MoveSorter() : size{0}


    }
}
