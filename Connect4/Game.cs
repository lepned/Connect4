using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Connect4
{
    public enum MoveRes
    {
        None = 0,
        Player1 = 1,
        Player2 = 2,
    }
    public class Game
    {
        public Game(int rows, int cols)
        {
            GameRows = rows;
            GameCols = cols;
            Debug.Assert(cols < 8, "Board width must be 7 columns");
            Debug.Assert(rows < 7, "Board does not fit in 64bits bitboard");
            MoveList = new Stack<ulong>();
            MoveHistory = InitMoveHistory();
        }

        private MoveRes[,] InitMoveHistory()
        {
            var arr = new MoveRes[6, 7];
            //rows in history
            for (int row = 0; row < 6; row++)
            {
                //cols in history
                for (int col = 0; col < 7; col++)
                {
                    arr[row, col] = MoveRes.None;
                }
            }
            return arr;
        }

        public bool IsWinning { get; set; }
        public int GameRows { get; init; }
        public int GameCols { get; init; }
        public int Moves { get; set; }
        public ulong CurrentPosition { get; set; }
        public ulong CurrentMask { get; set; }
        public Stack<ulong> MoveList { get; set; }
        public MoveRes[,] MoveHistory { get; set; }
        public bool Player1ToMove { get; set; } = true;

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

        public ulong TopMask(int col) => (1UL << (this.GameCols - 1) << col * (this.GameRows + 1));
        public ulong BottomMask(int col) => 1UL << col * (this.GameRows + 1);  
        public bool CanPlay(int col, ulong mask) => (mask & TopMask(col)) == 0UL;
        public (ulong newPos, ulong newMask) MakeAMove(ulong pos, ulong mask, int col)
        {
            var newPos = pos ^ mask; //for the opponent
            var newMove = mask + (1UL << (col * 7));
            var newMask = mask | newMove;
            var boardPlacement = ~mask & newMove;
            IsWinning = ConnectedFour(newPos ^ newMask);
            Player1ToMove = !Player1ToMove;
            MoveList.Push(boardPlacement);
            return (newPos, newMask);
        }

        public void MoveScore(ulong pos) //todo - implement a scoring system
        {
            BitOperations.PopCount(pos);
        }

    }
}
