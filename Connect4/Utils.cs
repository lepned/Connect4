using System;
using System.Collections.Generic;
using System.Linq;
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
    public static class Utils
    {
        static string MoveResToString(MoveRes res)
        {
            return res switch
            {
                MoveRes.None => "*",
                MoveRes.Player1 => "X",
                MoveRes.Player2 => "O",
                _ => "Hmm",
            };
        }

        /// <summary>
        /// Mostly used for debugging
        /// </summary>
        /// <param name="bitboard as game position"></param>
        /// <returns></returns>
        public static string BitboardToString(ulong bitboard)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            int[,] board = new int[7, 7];
            const int mask = 1;
            var b = bitboard;

            for (int col = 0; col < 7; col++)
            {
                for (int row = 0; row < 7; row++)
                {
                    var bit = b & mask;
                    board[col, row] = (int)bit;
                    b >>= 1;
                }
            }

            //top row
            for (int i = 0; i < 7; i++)
            {
                board[i, 6] = 0;
            }

            var cols = board.GetUpperBound(0);

            for (int row = 5; row >= 0; row--)
            {
                for (int c = 0; c <= cols; c++)
                {
                    sb.Append(board[c, row] + " ");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static string FillBoardWithPlayersTag(ulong player1, ulong player2)
        {
            MoveRes[,] board = new MoveRes[7, 7];
            const int mask = 1;
            var both = new List<(ulong pos, MoveRes player)>() { (player1, MoveRes.Player1), (player2, MoveRes.Player2) };

            foreach (var (pos, player) in both)
            {
                var b = pos;
                for (int col = 0; col < 7; col++)
                {
                    for (int row = 0; row < 7; row++)
                    {
                        var bit = b & mask;
                        if (bit > 0)
                        {
                            board[row, col] = player;
                        }

                        b >>= 1;
                    }
                }
            }

            return CreateConsoleGUIFromBoard(board);
        }

        private static string CreateConsoleGUIFromBoard (MoveRes[,] board)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            var cols = board.GetUpperBound(1);

            for (int row = 5; row >= 0; row--)
            {
                for (int c = 0; c <= cols; c++)
                {
                    var res = board[row, c];
                    sb.Append(MoveResToString(res) + " ");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        static void Print(ulong pos, ulong mask, int player)
        {
            Console.Write("Board state mask:");
            Console.WriteLine(Utils.BitboardToString(mask));
            Console.Write($"Position for player {player}:");
            Console.WriteLine(Utils.BitboardToString(pos));
        }
    }
}
