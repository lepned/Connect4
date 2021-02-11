using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Connect4
{
    public class BitTesting
    {
        public static void RunTest1()
        {
            var board = 1UL << 2;
            var row0 = board & 0x01010101010101UL;
            row0 += row0 >> 32;
            row0 += row0 >> 16;
            row0 += row0 >> 8;
            row0 &= 0xffUL;
            var row1 = 0xffUL;

            var boardString = Utils.BitboardToString(board);
            Console.WriteLine(boardString);
            var state = Utils.BitboardToString(row0);
            Console.WriteLine(state);
            var state1 = Utils.BitboardToString(row1);
            Console.WriteLine(state1);
        }

        public static void FindMovesInRow(int row)
        {
            var bottomRow = RowMask(row);
            var board = 1UL;
            var res = board & bottomRow;
            var boardString = Utils.BitboardToString(res);
            Console.WriteLine(boardString);
        }

        public static int CountMovesInRow(int row)
        {
            var bottomRow = RowMask(row);
            var board = 1UL;
            var res = board & bottomRow;
            return BitOperations.PopCount(res);
        }

        public static void MakeMove(int col)
        {
            var boardState = 1UL >> 7;
            var board = (boardState + Position.BottomMaskCol(col)) & Position.ColumnMask(col);
            var boardString = Utils.BitboardToString(board);
            Console.WriteLine(boardString);
        }

        public static void BottomTest()
        {
            var t1 = Utils.BitboardToString(4432676798593);
            Console.WriteLine(t1);
        }

        public static void ColumnTest(int col)
        {
            var boardString = Utils.BitboardToString(Position.ColumnMask(col));
            Console.WriteLine(boardString);
        }

        public static void SetAllBits()
        {
            var board = 0UL;
            for (int i = 0; i < 64; i++)
            {
                board = board | 1UL << i;
                var state = Utils.BitboardToString(board);
                Console.WriteLine(i);
                Console.WriteLine(state);
            }
        }

        public static ulong RowMask(int row)
        {
            var start = 0ul;
            for (int i = 0; i < 7; i++)
            {
                start = start | 1ul << (7 * i + row) ;
            }
            return start;
        }
    }
}
