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
            //BitTesting.RunTest1();
            //BitTesting.MakeMove(1);
            //BitTesting.BottomTest();
            //BitTesting.SetAllBits();
            //BitTesting.ColumnTest(1);
            //BitTesting.FindMovesInRow(0);
            //var res = BitTesting.CountMovesInRow(0);
            HumanVsMachine();

        }

        private static void HumanVsMachine()
        {
            //player 1 is human
            //player 2 is machine
            //Console.WriteLine("Match started....\n");
            //ulong pos = position.current_position;
            //ulong mask = position.mask;
            
            var solver = new Solver();
            var position = new Position();
            bool humanToPlay = true;

            while (!position.canWinNext())
            {
                if (humanToPlay) //means human to play
                {
                    if (position.current_position == 0) { Utils.PrintPosToConsole(true, position.current_position, position.current_position ^ position.mask); }
                    Console.WriteLine("Choose a move between 1 .. 7");
                    var col = Console.ReadLine();
                    var cmd = Convert.ToInt32(col) - 1;
                    if (position.canPlay(cmd))
                    {
                        position.playCol(cmd);
                        Utils.PrintPosToConsole(humanToPlay, position.current_position, position.current_position ^ position.mask);
                    }
                    else
                    {
                        Console.WriteLine($"The move to col {cmd + 1} is not allowed");
                    }
                }
                
                else
                {
                    Console.WriteLine("Computer thinking about a move...");
                    Thread.Sleep(500);                    
                    (ulong move,int score) = solver.solve(position, 2);
                    //position.canWinNext()
                    Console.WriteLine(score);
                    position.play(move);
                    Utils.PrintPosToConsole(false, position.current_position, position.current_position ^ position.mask);
                }
                
                humanToPlay = !humanToPlay;
            }
            Console.WriteLine("Some player won :)");
        }


        static void Run(Solver solver)
        {
            var P = new Position();
            for (int i = 0; i < 20; i++)
            {
                (ulong move, int score) = solver.solve(P, 5);
                P.play(move);
                var state = Utils.StateToBinaryString(P.current_position);
                Console.WriteLine(state);
                Console.WriteLine($"Score is: {score}");
            }
        }

        private static (ulong pos, ulong mask) MakePredictedMove(Game ai, ulong pos, ulong mask)
        {
            ulong p, m;
            if (ai.Player1ToMove)
            {
                (p, m) = ai.MakeAMove(pos, mask, 1);
            }
            else
            {
                (p, m) = ai.MakeAMove(pos, mask, 2);
            }

            ai.IsWinning = ai.ConnectedFour(p ^ m);
            return (p, m);
        }

        private static (ulong pos, ulong mask) MakeRandomMove(Game ai, ulong pos, ulong mask)
        {
            ulong p, m;
            var rnd = new Random();
            var col = rnd.Next(0, 7);
            if (ai.CanPlay(col, mask))
            {
                (p, m) = ai.MakeAMove(pos, mask, col);
                ai.IsWinning = ai.ConnectedFour(p ^ m);
                return (p, m);
            }
            else
            {
                Console.WriteLine("Illegal move played");
                var test = Utils.DebugInfo(pos);
                Console.WriteLine(test);
                return (pos, mask);
            }
        }

        private static string RandomGame(Game ai)
        {
            ulong pos = 0;
            ulong mask = 0;

            while (!ai.IsWinning)
            {
                (pos, mask) = MakeRandomMove(ai, pos, mask);
                if (!ai.IsWinning)
                {
                    var p = ai.Player1ToMove ? 1 : 2;
                    Print(pos, mask, p);
                }
            }
            var player = !ai.Player1ToMove ? "Player 1" : "Player 2";
            Console.WriteLine("Final position:");
            var final = Utils.StateToBinaryString(pos ^ mask);
            Console.WriteLine($"{final}");
            return player;
        }



        private static string HumanVsMachine(Game ai)
        {
            ulong pos = 0;
            ulong mask = 0;

            //player 1 is human
            //player 2 is machine
            //Console.WriteLine("Match started....\n");

            while (!ai.IsWinning)
            {
                if (ai.Player1ToMove)
                {
                    if (pos == 0) { Utils.PrintPosToConsole(true, pos, pos ^ mask); }
                    Console.WriteLine("Choose a move between 1 .. 7");
                    var col = Console.ReadLine();
                    var cmd = Convert.ToInt32(col) - 1;
                    if (ai.CanPlay(cmd, mask))
                    {
                        (pos, mask) = ai.MakeAMove(pos, mask, cmd);
                        Utils.PrintPosToConsole(true, pos ^ mask, pos);
                    }
                    else
                    {
                        Console.WriteLine($"The move to col {cmd + 1} is not allowed");
                    }
                }
                else
                {
                    Console.WriteLine("Computer thinking about a move...");
                    Thread.Sleep(1000);
                    (pos, mask) = MakeRandomMove(ai, pos, mask);
                    Utils.PrintPosToConsole(false, pos ^ mask, pos);
                }
            }
            var player = !ai.Player1ToMove ? "Player 1" : "Player 2";
            return player;
        }

        static void Print(ulong pos, ulong mask, int player)
        {
            Console.Write("Board state mask:");
            Console.WriteLine(Utils.StateToBinaryString(mask));
            Console.Write($"Position for player {player}:");
            Console.WriteLine(Utils.StateToBinaryString(pos));
        }

        static void PrintAllMovesPlayed(Game ai)
        {
            Console.WriteLine("Printing all moves played:\n");
            foreach (var item in ai.MoveList)
            {
                var move = Utils.StateToBinaryString(item);
                Console.Write($"Move by player:");
                Console.WriteLine(move);
            }
        }



        //static void explore(Position P, char[] pos_str, int depth)
        //{
        //    var solver = new Solver();
        //    var visited = new HashSet<ulong>();

        //    //ulong key = P.key3();
        //    //if (!visited.insert(key).second)
        //    //    return; // already explored position

        //    int nb_moves = P.nbMoves();
        //    if (nb_moves <= depth)
        //        Console.WriteLine(pos_str); // should probably define another outstream here...
        //    if (nb_moves >= depth) return;  // do not explore at further depth

        //    for (int i = 0; i < Position.WIDTH; i++) // explore all possible moves
        //        if (P.canPlay(i) && !P.isWinningMove(i))
        //        {
        //            Position P2 = new Position(P);
        //            var svar = solver.analyze(P2);
        //            var test = svar.Min();
        //            Console.WriteLine(test);
        //            P2.playCol(i);
        //            Console.WriteLine(P2.moveScore(P2.current_position));
        //            pos_str[nb_moves] = (char)(1 +  i);
        //            explore(P2, pos_str, depth);
        //            pos_str[nb_moves] = (char)0;
        //        }
        //}
    }
}
