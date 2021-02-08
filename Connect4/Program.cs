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
            //bottom test
            //var bottom = Position.bottom();
            //Console.WriteLine(bottom);
            //var t1 = Utils.StateToBinaryString(4432676798593);
            //Console.WriteLine(t1);

            //cpp version - Todo: Does not work yet :(
            Solver solver = new Solver();
            bool weak = false;
            bool analyze = false;
            Position P = new Position();
            var positons = new char[700];
            //explore(P, positons, 4);
            //Console.WriteLine(positons.Length);
            var rnd = new Random();
            var moves = Enumerable.Range(1, 7).Select(i => "1");
            var randMoves = Enumerable.Range(1, 50).Select(i => rnd.Next(1,8).ToString());
            P.play(randMoves);
            //var eval = solver.analyze(P, weak);
            for (int i = 0; i < 7; i++)
            {
                var score = solver.solve(P);
                Console.WriteLine($"Score is {score}");
                P.playCol(i);
                //Utils.PrintPosToConsole(false, P.current_position, P.current_position ^ P.mask); 
            }
            Utils.PrintPosToConsole(false,P.current_position, P.current_position ^ P.mask);
            ////P.playCol(col);
            ////Console.WriteLine(position);
            //for (int i = 0; i < 4; i++)
            //{
            //    if (analyze)
            //    {
            //        var scores = solver.analyze(P, weak);
            //        for (int j = 0; j < Position.WIDTH; j++)
            //            Console.WriteLine(scores[j]);
            //        //std::cout << " " << scores[i];
            //    }
            //    else
            //    {
            //        int score = solver.solve(P, weak);
            //        Console.WriteLine(score);
            //    }

            //    solver.solve(P);
            //    //P.play(1);
            //    var board = Utils.StateToBinaryString(P.current_position);
            //    Console.WriteLine(board);
            //}

            //Console.Title = "Connect 4 - testes av Tine små";
            //Console.WriteLine("Connect 4 - play the game against a computer!");
            //var ai = new Game(6, 7);
            ////var t1 = Utils.StateToBinaryString(ai.TopMask(0)); //will not show with a row size set to 6
            ////Console.WriteLine(t1);
            //var whoWon = HumanVsMachine(ai);
            ////var whoWon = RandomGame(ai);
            //Console.WriteLine($"{whoWon} won the game after {ai.MoveList.Count} moves");
            //var lastMove = ai.MoveList.Peek();
            //var board = Utils.StateToBinaryString(lastMove);
            //Console.WriteLine($"Last moved played was\n {board}");
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



        static void explore(Position P, char[] pos_str, int depth)
        {
            var solver = new Solver();
            var visited = new HashSet<ulong>();

            //ulong key = P.key3();
            //if (!visited.insert(key).second)
            //    return; // already explored position

            int nb_moves = P.nbMoves();
            if (nb_moves <= depth)
                Console.WriteLine(pos_str); // should probably define another outstream here...
            if (nb_moves >= depth) return;  // do not explore at further depth

            for (int i = 0; i < Position.WIDTH; i++) // explore all possible moves
                if (P.canPlay(i) && !P.isWinningMove(i))
                {
                    Position P2 = new Position(P);
                    var svar = solver.analyze(P2);
                    var test = svar.Min();
                    Console.WriteLine(test);
                    P2.playCol(i);
                    Console.WriteLine(P2.moveScore(P2.current_position));
                    pos_str[nb_moves] = (char)(1 +  i);
                    explore(P2, pos_str, depth);
                    pos_str[nb_moves] = (char)0;
                }
        }
    }
}
