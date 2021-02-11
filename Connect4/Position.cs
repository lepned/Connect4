using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Bitboard = System.UInt64;

namespace Connect4
{
    public class Position
    {
        public Position(Position pos)
        {
            CurrentPosition = pos.CurrentPosition;
            Mask = pos.Mask;
            Moves = pos.Moves;
            MoveList = new List<ulong>();
        }
        public Position()
        {
            Debug.Assert(WIDTH < 8, "Board width must be 7 columns");
            Debug.Assert(WIDTH * (HEIGHT + 1) < 64, "Board does not fit in 64bits bitboard");
            CurrentPosition = 0;
            Mask = 0;
            Moves = 0;
            MoveList = new List<ulong>();

        }
        public static int WIDTH = 7;  // width of the board
        public static int HEIGHT = 6; // height of the board
        public List<ulong> MoveList { get; set; }

        //public static int MIN_SCORE = -(WIDTH * HEIGHT) / 2 + 3;
        //public static int MAX_SCORE = (WIDTH * HEIGHT + 1) / 2 - 3;

        public Bitboard CurrentPosition { get; set; } //public Bitboard current_position; // bitmap of the current_player stones

        public Bitboard Mask { get; set; } // bitmap of all the already palyed spots

        public int Moves { get; set; } // number of moves played since the beinning of the game.

        public bool CanPlay(int col) => (Mask & TopMaskCol(col)) == 0;

        public void PlayCol(int col)
        {
            Play((Mask + BottomMaskCol(col)) & ColumnMask(col));
        }

        public bool IsWinningMove(int col)
        {
            return (Winning_position() & Possible() & ColumnMask(col)) != 0ul;
        }

        public void Play(Bitboard move)
        {
            MoveList.Add(move);
            CurrentPosition ^= Mask;
            Mask |= move;
            Moves++;
        }

        //not in use yet
        public int Play(IEnumerable<string> seq)
        {
            for (int i = 0; i < seq.Count(); i++)
            {
                int col = Convert.ToInt32(seq.ElementAt(i)) - 1;
                if (IsWinningMove(col))
                {
                    Console.WriteLine($"Game over with the next move in column {col} after {Moves} moves");
                }
                if (col < 0 || col >= Position.WIDTH || !CanPlay(col) || IsWinningMove(col))
                {
                    if (!CanPlay(col)) // invalid move
                    {
                        Console.WriteLine($"Invalid move in col {col} after {Moves} moves");
                        continue;
                    }
                    return i;
                }
                PlayCol(col);

            }
            return seq.Count();
        }

        public bool CanWinNext()
        {
            return (Winning_position() & Possible()) != 0ul;
        }

        public Bitboard Key()
        {
            return CurrentPosition + Mask;
        }

        public int MoveScore(Bitboard move)
        {
            return BitOperations.PopCount(ComputeWinningPosition(CurrentPosition | move, Mask));
        }

        public Bitboard PossibleNonLosingMoves()
        {
            //Debug.Assert(!canWinNext());
            Bitboard possible_mask = Possible();
            Bitboard opponent_win = Opponent_winning_position();
            Bitboard forced_moves = possible_mask & opponent_win;
            if (forced_moves != 0ul)
            {
                if ((forced_moves & (forced_moves - 1)) != 0ul) // check if there is more than one forced move
                    return 0;                           // the opponnent has two winning moves and you cannot stop him
                else possible_mask = forced_moves;    // enforce to play the single forced move
            }
            return possible_mask & ~(opponent_win >> 1);  // avoid to play below an opponent winning spot
        }

        Bitboard Winning_position() => ComputeWinningPosition(CurrentPosition, Mask);

        Bitboard Opponent_winning_position() => ComputeWinningPosition(CurrentPosition ^ Mask, Mask);

        Bitboard Possible() => (Mask + BottomMask(WIDTH, HEIGHT)) & BoardMask;

        static Bitboard ComputeWinningPosition(Bitboard position, Bitboard mask)
        {
            // vertical;
            Bitboard r = (position << 1) & (position << 2) & (position << 3);

            //horizontal
            Bitboard p = (position << (HEIGHT + 1)) & (position << 2 * (HEIGHT + 1));
            r |= p & (position << 3 * (HEIGHT + 1));
            r |= p & (position >> (HEIGHT + 1));
            p = (position >> (HEIGHT + 1)) & (position >> 2 * (HEIGHT + 1));
            r |= p & (position << (HEIGHT + 1));
            r |= p & (position >> 3 * (HEIGHT + 1));

            //diagonal 1
            p = (position << HEIGHT) & (position << 2 * HEIGHT);
            r |= p & (position << 3 * HEIGHT);
            r |= p & (position >> HEIGHT);
            p = (position >> HEIGHT) & (position >> 2 * HEIGHT);
            r |= p & (position << HEIGHT);
            r |= p & (position >> 3 * HEIGHT);

            //diagonal 2
            p = (position << (HEIGHT + 2)) & (position << 2 * (HEIGHT + 2));
            r |= p & (position << 3 * (HEIGHT + 2));
            r |= p & (position >> (HEIGHT + 2));
            p = (position >> (HEIGHT + 2)) & (position >> 2 * (HEIGHT + 2));
            r |= p & (position << (HEIGHT + 2));
            r |= p & (position >> 3 * (HEIGHT + 2));

            return r & (BoardMask ^ mask);
        }

        static Bitboard TopMaskCol(int col) => 1ul << ((HEIGHT - 1) + col * (HEIGHT + 1));

        public static Bitboard BottomMaskCol(int col) => 1ul << col * (HEIGHT + 1);

        public static Bitboard ColumnMask(int col) => ((1ul << HEIGHT) - 1) << col * (HEIGHT + 1);

        public static Bitboard BottomMask(int width, int height) => 4432676798593ul | 1ul << (width - 1) * (height + 1);
        static Bitboard BoardMask => BottomMask(WIDTH, HEIGHT) * ((1ul << HEIGHT) - 1);

    }
}

