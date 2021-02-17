using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Connect4
{
    public class Solver
    {
        public Solver()
        {
            //Watch = new Stopwatch();
            NodeCount = 0;
        }

        //public Stopwatch Watch { get; set; }
       
        public int NodeCount { get; set; } // counter of explored nodes.       

        int Negamax(Position P, int depth, int alpha, int beta, int color)
        {
            if (P.ConnectedFour(P.CurrentPosition))
            {
                return color * (1000  + EvaluateNode(P));
            }

            if (P.Moves >= 42)
            {
                return 0;
            }

            if (depth == 0)
            {
                return color * EvaluateNode(P);
            }

            NodeCount++;
            int best = Int32.MinValue;

            foreach (var move in P.GetMoves())
            {
                Position p2 = new Position(P);
                p2.Play(move);
                best = Math.Max(best, -Negamax(p2, depth - 1, -beta, -alpha, -color));
                alpha = Math.Max(alpha, best);
                if (alpha >= beta)
                {
                    break; // prune the exploration if we find a possible move better than what we were looking for.
                }
            }

            return best;
        }

        public (ulong move, int score) Solve(Position P, int depth)
        {
            var realDepth = Math.Min(depth, (Position.HEIGHT * Position.WIDTH - P.Moves));
            if (realDepth < depth)
            {
                Console.WriteLine($"Search depth reduced to {realDepth}");
            }
            var color = P.PlayerToMove == Player.PlayerA ? 1 : -1;
            var bestScore = Int32.MinValue;
            ulong bestMove = 0;
            var allMoves = P.GetMoves().ToList();
           
            foreach (var move in allMoves)
            {
                Position p2 = new Position(P);
                p2.Play(move);
                var value = -Negamax(p2, realDepth - 1, Int32.MinValue, Int32.MaxValue, -color);
                if (value > bestScore)
                {
                    bestScore = value;
                    bestMove = move;
                }
            }

            Console.WriteLine(bestScore);
            return (bestMove, bestScore);
        }

        private int EvaluateNode(Position pos)
        {
            var score = (Position.WIDTH * Position.HEIGHT - pos.Moves) / 2;
            return score;
        }

    }
}
