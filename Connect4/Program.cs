using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Connect4
{
    class Program
    {
        public static StringBuilder SB { get; set; }
        static void Main(string[] args)
        {
            SB = new StringBuilder();
            HumanVsMachine(12);
        }

        private static void HumanVsMachine(int depth)
        {
            //player 1 is human
            //player 2 is machine
            Console.WriteLine("Match started....\n");

            var solver = new Solver();
            var position = new Position();
            bool player1ToPlay = true;

            while (true)
            {
                if (position.Moves >= 42)
                {
                    Console.WriteLine("Game ended in a draw");
                    break;
                }

                if (player1ToPlay) //normally means human to play
                {
                    if (position.CurrentPosition == 0)
                    {
                        var pos = Utils.FillBoardWithPlayersTag(position.CurrentPosition, position.CurrentPosition ^ position.Mask);
                        Console.WriteLine("Startposition with 6 rows and 7 columns");
                        Console.WriteLine(pos);
                    }
                    Console.WriteLine("Player A: \nChoose a move between 1 .. 7");
                    var col = Console.ReadLine();
                    var cmd = Convert.ToInt32(col) - 1;
                    
                    if (position.CanPlay(cmd))
                    {
                        position.PlayCol(cmd);
                    }
                    
                    else
                    {
                        Console.WriteLine($"The move to col {cmd + 1} is not allowed");
                    }

                    //(ulong move, int score) = solver.Solve(position, depth);
                    //SB.AppendLine("");
                    //SB.AppendLine($"Score = {score}");
                    //if (move == 0UL)
                    //{
                    //    Console.WriteLine($"Null move not allowed {position.Moves}");
                    //    break;
                    //};
                    //PrintColumnPlayed(move, position.PlayerToMove);
                    //position.Play(move);
                }

                else //normally the computer AI
                {
                    Console.WriteLine("Computer thinking about a move...");
                    Console.WriteLine(position.PlayerToMove);
                    Thread.Sleep(1000);
                    (ulong move, int score) = solver.Solve(position, depth);
                    SB.AppendLine($"Score = {score}");
                    if (move == 0UL)
                    {
                        Console.WriteLine($"Null move not allowed {position.Moves}");
                        break;
                    }
                    PrintColumnPlayed(move, position.PlayerToMove);
                    position.Play(move);
                }

                var player1 = position.PlayerToMove == Player.PlayerA ? position.CurrentPosition : position.CurrentPosition ^ position.Mask;
                var player2 = position.PlayerToMove == Player.PlayerB ? position.CurrentPosition : position.CurrentPosition ^ position.Mask;
                if (position.GameWon())
                {
                    Console.WriteLine("Position is won");
                    var gameWon = Utils.FillBoardWithPlayersTag(player1, player2);
                    Console.WriteLine(gameWon);
                    break;
                }

                var game = Utils.FillBoardWithPlayersTag(player1, player2);
                Console.WriteLine(game);
                SB.AppendLine(game);
                player1ToPlay = !player1ToPlay;
            }

            var whoWon = player1ToPlay ? "You won!" : "Computer won";
            Console.WriteLine(whoWon);
            var path = @"c:\utvikling\connect4.txt";
            File.WriteAllText(path, SB.ToString());
        }

        private static void PrintColumnPlayed(ulong move, Player player)
        {
            for (int i = 0; i < 7; i++)
            {
                var res = Position.ColumnMask(i) & move;
                if (res != 0)
                {
                    SB.AppendLine($"{player} played in column {i}");
                    Console.WriteLine($"{player} played in column {i}");
                    break;
                }
            }
        }

        private static void RunBitTesting()
        {
            BitTesting.RunTest1();
            BitTesting.MakeMove(1);
            BitTesting.BottomTest();
            BitTesting.SetAllBits();
            BitTesting.ColumnTest(1);
            BitTesting.FindMovesInRow(0);
            var res = BitTesting.CountMovesInRow(0);
        }

        static void Print(ulong pos, ulong mask, int player)
        {
            Console.Write("Board state mask:");
            Console.WriteLine(Utils.BitboardToString(mask));
            Console.Write($"Position for player {player}:");
            Console.WriteLine(Utils.BitboardToString(pos));
        }

        //static void PrintAllMovesPlayed(Game ai)
        //{
        //    Console.WriteLine("Printing all moves played:\n");
        //    foreach (var item in ai.MoveList)
        //    {
        //        var move = Utils.StateToBinaryString(item);
        //        Console.Write($"Move by player:");
        //        Console.WriteLine(move);
        //    }
        //}
    }
}
