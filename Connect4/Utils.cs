using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4
{
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
        
        public static string DebugInfo(ulong bitboard)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            int[,] board = new int[9, 7];
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

            for (int row = 7; row >= 0; row--)
            {
                for (int c = 0; c <= cols; c++)
                {
                    sb.Append(board[c, row] + " ");
                }
                sb.AppendLine();
            }

            return sb.ToString();            
        }
        public static string StateToBinaryString(ulong bitboard)
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

        public static void PrintPosToConsole(bool human, ulong toplay, ulong opponent)
        {
            var humanToPlay = human;
            MoveRes[,] board = new MoveRes[7, 7];
            const int mask = 1;
            var both = new List<ulong>() { toplay, opponent };
            
            foreach (var player in both)
            {
                var b = player;

                for (int col = 0; col < 7; col++)
                {
                    for (int row = 0; row < 7; row++)
                    {
                        var bit = b & mask;
                        if (bit > 0)
                        {
                            if (humanToPlay)
                            {
                                board[row, col] = MoveRes.Player1;
                            }
                            else
                            {
                                board[row, col] = MoveRes.Player2;
                            }
                        }
                        
                        b >>= 1;
                    }
                }

                humanToPlay = !humanToPlay;
            }

            DoWriteToConsole(board);
        }

        private static void DoWriteToConsole(MoveRes[,] board)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            var rows = board.GetUpperBound(0);
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

            var consoleBoard = sb.ToString();
            Console.WriteLine(consoleBoard);
        }
    }
}
