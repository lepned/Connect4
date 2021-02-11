using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Connect4
{
    class Program
    {
        static void Main(string[] args)
        {
            HumanVsMachine(5);
        }

        private static void HumanVsMachine(int depth)
        {
            //player 1 is human
            //player 2 is machine
            Console.WriteLine("Match started....\n");
            
            var solver = new Solver();
            var position = new Position();
            bool humanToPlay = true;

            while (!position.CanWinNext())
            {
                if (humanToPlay) //means human to play
                {
                    if (position.CurrentPosition == 0) { Utils.PrintPosToConsole(true, position.CurrentPosition, position.CurrentPosition ^ position.Mask); }
                    Console.WriteLine("Choose a move between 1 .. 7");
                    var col = Console.ReadLine();
                    var cmd = Convert.ToInt32(col) - 1;
                    if (position.CanPlay(cmd))
                    {
                        position.PlayCol(cmd);
                        Utils.PrintPosToConsole(humanToPlay, position.CurrentPosition, position.CurrentPosition ^ position.Mask);
                    }
                    else
                    {
                        Console.WriteLine($"The move to col {cmd + 1} is not allowed");
                    }
                }
                
                else
                {
                    Console.WriteLine("Computer thinking about a move...");
                    Thread.Sleep(200);                    
                    (ulong move,int score) = solver.Solve(position, depth);
                    Console.WriteLine(score);
                    position.Play(move);
                    Utils.PrintPosToConsole(false, position.CurrentPosition, position.CurrentPosition ^ position.Mask);
                }
                
                humanToPlay = !humanToPlay;
            }
            
            var whoWon = humanToPlay ? "You won!" : "Computer won";
            Console.WriteLine(whoWon);
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
