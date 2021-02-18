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

        private static void RunBitTesting()
        {
            RunTest1();
            MakeMove(1);
            BottomTest();
            SetAllBits();
            ColumnTest(1);
            FindMovesInRow(0);
            //var res = BitTesting.CountMovesInRow(0);
        }

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
                start |= 1ul << (7 * i + row) ;
            }
            return start;
        }

        public static int ColumnPlayed(ulong move)
        {
            for (int i = 0; i < 7; i++)
            {
                var res = Position.ColumnMask(i) & move;
                if (res != 0)
                {
                    return i;
                }
            }

            throw new ArgumentNullException();
        }


        public static int RowPlayed(ulong move)
        {
            for (int i = 0; i < 7; i++)
            {
                var res = RowMask(i) & move;
                if (res != 0)
                {
                    return i;
                }
            }

            throw new ArgumentNullException();
        }


        //public bool CanWinNext()
        //{
        //    return (WinningPosition() & Possible()) != 0ul;
        //}

        //public Bitboard Key()
        //{
        //    return CurrentPosition + Mask;
        //}

        //public Bitboard PossibleNonLosingMoves()
        //{
        //    Bitboard possible_mask = Possible();
        //    Bitboard opponent_win = OpponentWinningPosition();
        //    Bitboard forced_moves = possible_mask & opponent_win;
        //    if (forced_moves != 0ul)
        //    {
        //        if ((forced_moves & (forced_moves - 1)) != 0ul) // check if there is more than one forced move
        //            return 0;                           // the opponnent has two winning moves and you cannot stop him
        //        else possible_mask = forced_moves;    // enforce to play the single forced move
        //    }
        //    return possible_mask & ~(opponent_win >> 1);  // avoid to play below an opponent winning spot
        //}

        //public Bitboard WinningPosition() => ComputeWinningPosition(CurrentPosition, Mask);

        //Bitboard OpponentWinningPosition() => ComputeWinningPosition(CurrentPosition ^ Mask, Mask);

        //public Bitboard Possible() => (Mask + BottomMask(WIDTH, HEIGHT)) & BoardMask;

        //static Bitboard ComputeWinningPosition(Bitboard position, Bitboard mask)
        //{
        //    // vertical;
        //    Bitboard r = (position << 1) & (position << 2) & (position << 3);

        //    //horizontal
        //    Bitboard p = (position << (HEIGHT + 1)) & (position << 2 * (HEIGHT + 1));
        //    r |= p & (position << 3 * (HEIGHT + 1));
        //    r |= p & (position >> (HEIGHT + 1));
        //    p = (position >> (HEIGHT + 1)) & (position >> 2 * (HEIGHT + 1));
        //    r |= p & (position << (HEIGHT + 1));
        //    r |= p & (position >> 3 * (HEIGHT + 1));

        //    //diagonal 1
        //    p = (position << HEIGHT) & (position << 2 * HEIGHT);
        //    r |= p & (position << 3 * HEIGHT);
        //    r |= p & (position >> HEIGHT);
        //    p = (position >> HEIGHT) & (position >> 2 * HEIGHT);
        //    r |= p & (position << HEIGHT);
        //    r |= p & (position >> 3 * HEIGHT);

        //    //diagonal 2
        //    p = (position << (HEIGHT + 2)) & (position << 2 * (HEIGHT + 2));
        //    r |= p & (position << 3 * (HEIGHT + 2));
        //    r |= p & (position >> (HEIGHT + 2));
        //    p = (position >> (HEIGHT + 2)) & (position >> 2 * (HEIGHT + 2));
        //    r |= p & (position << (HEIGHT + 2));
        //    r |= p & (position >> 3 * (HEIGHT + 2));

        //    return r & (BoardMask ^ mask);
        //}

        //public static Bitboard BottomMask(int width, int height) => 4432676798593ul | 1ul << (width - 1) * (height + 1);
        //static Bitboard BoardMask => BottomMask(WIDTH - 1, HEIGHT) * ((1ul << HEIGHT) - 1);
    }
}
