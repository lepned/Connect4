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
    public enum Player
    {
        PlayerA,
        PlayerB
    }

    public class Position
    {
        public Position(Position pos)
        {
            CurrentPosition = pos.CurrentPosition;
            Mask = pos.Mask;
            Moves = pos.Moves;
            MoveList = new List<ulong>();
            PlayerToMove = pos.PlayerToMove;
            ColumnOrder = pos.ColumnOrder;
        }
        public Position()
        {
            Debug.Assert(WIDTH < 8, "Board width must be 7 columns");
            Debug.Assert(WIDTH * (HEIGHT + 1) < 64, "Board does not fit in 64bits bitboard");
            InitPosition();
        }

        private void InitPosition()
        {
            CurrentPosition = 0;
            Mask = 0;
            Moves = 0;
            MoveList = new List<ulong>();
            PlayerToMove = Player.PlayerA;
            //works like an opening book for connect 4 - better to place your pieces close to the center
            ColumnOrder[0] = 3;
            ColumnOrder[1] = 4;
            ColumnOrder[2] = 2;
            ColumnOrder[3] = 5;
            ColumnOrder[4] = 1;
            ColumnOrder[5] = 6;
            ColumnOrder[6] = 0;
        }


        public static int WIDTH = 7;  // width of the board
        public static int HEIGHT = 6; // height of the board
        
        public int[] ColumnOrder { get; set; } = new int[Position.WIDTH];
        public List<ulong> MoveList { get; set; }
        public Player PlayerToMove { get; set; }
        public Bitboard CurrentPosition { get; set; } //bitmap of the current player stones
        public Bitboard Mask { get; set; } // bitmap of all the already played spots

        public int Moves { get; set; } // number of moves played since the beginning of the game.

        public bool CanPlay(int col) => (Mask & TopMaskCol(col)) == 0;

        public void PlayCol(int col)
        {
            Play((Mask + BottomMaskCol(col)) & ColumnMask(col));
        }

        public bool ConnectedFour(ulong pos)
        {
            //vertical four in a row
            var m = pos & BitOperations.RotateRight(pos, 1);
            if ((m & BitOperations.RotateRight(m, 2)) != 0ul)
            {
                return true;
            }

            //horisontal four in a row
            m = pos & BitOperations.RotateRight(pos, 7);
            if ((m & BitOperations.RotateRight(m, 14)) != 0ul)
            {
                return true;
            }

            //diagonal up-left four in a row
            m = pos & BitOperations.RotateRight(pos, 6);
            if ((m & BitOperations.RotateRight(m, 12)) != 0ul)
            {
                return true;
            }

            //diagonal up-right four in a row
            m = pos & BitOperations.RotateRight(pos, 8);
            if ((m & BitOperations.RotateRight(m, 16)) != 0ul)
            {
                return true;
            }

            return false;
        }

        public bool GameWon()
        {
            var posBeforeMove = CurrentPosition ^ Mask;
            return IsWinningGame(posBeforeMove);
        }

        public IEnumerable<ulong> GetMoves()
        {
            for (int i = 0; i < 7; i++)
            {
                var col = ColumnOrder[i];
                var columnValue = Position.ColumnMask(col);
                var isValid = (Mask & columnValue) != columnValue;

                if (isValid)
                {
                    var count = BitOperations.PopCount(columnValue & Mask);
                    var move = BitTesting.RowMask(count) & columnValue;
                    yield return move;
                }
            }
        }

        public void Play(Bitboard move)
        {
            MoveList.Add(move);
            CurrentPosition ^= Mask;
            Mask |= move;
            Moves++;
            PlayerToMove = PlayerToMove == Player.PlayerA ? Player.PlayerB : Player.PlayerA;
        }

        private bool IsWinningGame(Bitboard position)
        {
            var pos = position;

            //vertical four in a row
            var m = pos & BitOperations.RotateRight(pos, 1);
            if ((m & BitOperations.RotateRight(m, 2)) != 0ul)
            {
                return true;
            }

            //horisontal four in a row
            m = pos & BitOperations.RotateRight(pos, 7);
            if ((m & BitOperations.RotateRight(m, 14)) != 0ul)
            {
                return true;
            }

            //diagonal up-left four in a row
            m = pos & BitOperations.RotateRight(pos, 6);
            if ((m & BitOperations.RotateRight(m, 12)) != 0ul)
            {
                return true;
            }

            //diagonal up-right four in a row
            m = pos & BitOperations.RotateRight(pos, 8);
            if ((m & BitOperations.RotateRight(m, 16)) != 0ul)
            {
                return true;
            }

            return false;
        }

        private Bitboard TopMaskCol(int col) => 1ul << ((HEIGHT - 1) + col * (HEIGHT + 1));
        public static Bitboard BottomMaskCol(int col) => 1ul << col * (HEIGHT + 1);
        public static Bitboard ColumnMask(int col) => ((1ul << HEIGHT) - 1) << col * (HEIGHT + 1);

        
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

