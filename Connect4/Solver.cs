using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connect4
{
    public class Solver
    {
        public Solver()
        {
            Watch = new Stopwatch();
            NodeCount = 0;
            //works like an opening book for connect 4 - better to place your pieces close to the center
            ColumnOrder[0] = 3;
            ColumnOrder[1] = 4;
            ColumnOrder[2] = 2;
            ColumnOrder[3] = 5;
            ColumnOrder[4] = 1;
            ColumnOrder[5] = 6;
            ColumnOrder[6] = 0;
        }

        public Stopwatch Watch { get; set; }

        public static int INVALID_MOVE = -1000;
        public ulong BestMove { get; set; }
        public int NodeCount { get; set; } // counter of explored nodes.

        //static int TABLE_SIZE = 24;
        //TranspositionTable<uint_t<Position::WIDTH*(Position::HEIGHT + 1) - TABLE_SIZE >, Position::position_t, uint8_t, TABLE_SIZE > transTable;
        public int[] ColumnOrder { get; set; } = new int[Position.WIDTH];

        int Negamax(Position P, int alpha, int beta, int depth)
        {
            if (depth == 0)
            {
                return alpha;
            }

            //Debug.Assert(alpha < beta);
            //Debug.Assert(!P.canWinNext());
            NodeCount++;
            var possible = P.PossibleNonLosingMoves();
            if (possible == 0) // if no possible non losing move, opponent wins next move
            {
                //Console.WriteLine("You win next");
                return -(Position.WIDTH * Position.HEIGHT - P.Moves) / 2;
            }

            if (P.Moves >= Position.WIDTH * Position.HEIGHT - 2) // check for draw game
            {
                //Console.WriteLine("Position is a draw");
                return 0;
            }

            int min = -(Position.WIDTH * Position.HEIGHT - 2 - P.Moves) / 2;  // lower bound of score as opponent cannot win next move
            if (alpha < min)
            {
                alpha = min; // there is no need to keep alpha below our max possible score.
                if (alpha >= beta)
                {
                    return alpha;  // prune the exploration if the [alpha;beta] window is empty.
                }
            }

            int max = (Position.WIDTH * Position.HEIGHT - 1 - P.Moves) / 2;   // upper bound of our score as we cannot win immediately
            if (beta > max)
            {
                beta = max; // there is no need to keep beta above our max possible score.
                if (alpha >= beta)
                {
                    return beta;  // prune the exploration if the [alpha;beta] window is empty.
                }
            }

            //var key = P.key();
            //if (int val = transTable.get(key)) {
            //    if (val > Position.MAX_SCORE - Position.MIN_SCORE + 1)
            //    { // we have an lower bound
            //        min = val + 2 * Position.MIN_SCORE - Position.MAX_SCORE - 2;
            //        if (alpha < min)
            //        {
            //            alpha = min;                     // there is no need to keep beta above our max possible score.
            //            if (alpha >= beta) return alpha;  // prune the exploration if the [alpha;beta] window is empty.
            //        }
            //    }
            //    else
            //    { // we have an upper bound
            //        max = val + Position.MIN_SCORE - 1;
            //        if (beta > max)
            //        {
            //            beta = max;                     // there is no need to keep beta above our max possible score.
            //            if (alpha >= beta) return beta;  // prune the exploration if the [alpha;beta] window is empty.
            //        }
            //    }
            //}

            //if (int val = book.get(P)) return val + Position.MIN_SCORE - 1; // look for solutions stored in opening book
            //int bestCol = -1;

            MoveSorter moves = new MoveSorter();
            for (int i = Position.WIDTH - 1; i >= 0; i--)
            {
                var move = possible & Position.ColumnMask(ColumnOrder[i]);
                if (move != 0ul)
                    moves.Add(move, P.MoveScore(move));
            }

            ulong next;
            int score = Int32.MinValue;
            while ((next = moves.GetNext()) != 0ul)
            {
                Position p2 = new Position(P);
                p2.Play(next);
                score = Math.Max(score, -Negamax(p2, -beta, -alpha, depth - 1));
                if (score >= beta)
                {
                    //transTable.put(key, score + Position.MAX_SCORE - 2 * Position.MIN_SCORE + 2); // save the lower bound of the position
                    BestMove = next;
                    return score; // prune the exploration if we find a possible move better than what we were looking for.
                }
                if (score > alpha)
                {
                    alpha = score;
                    BestMove = next;
                }
            }
            //transTable.put(key, alpha - Position::MIN_SCORE + 1); // save the upper bound of the position
            return alpha;
        }


        public (ulong move, int score) Solve(Position P, int depth, bool weak = false)
        {
            //Watch.Start();
            if (P.CanWinNext()) // check if win in one move as the Negamax function does not support this case.
            {
                Console.WriteLine("There is a win for you");
                return (BestMove, (Position.WIDTH * Position.HEIGHT + 1 - P.Moves) / 2);
            }

            var min = -(Position.WIDTH * Position.HEIGHT - P.Moves) / 2;
            var max = (Position.WIDTH * Position.HEIGHT + 1 - P.Moves) / 2;
            if (weak)
            {
                min = -1;
                max = 1;
            }

            while (min < max)
            {   // iteratively narrow the min-max exploration window
                var med = min + (max - min) / 2;
                if (med <= 0 && min / 2 < med) med = min / 2;
                else if (med >= 0 && max / 2 > med) med = max / 2;
                var score = Negamax(P, med, med + 1, depth);   // use a null depth window to know if the actual score is greater or smaller than med                
                if (score <= med) max = score;
                else min = score;
            }
            return (BestMove, min);
        }

        public void Reset()
        {
            NodeCount = 0;
            //transTable.reset();
        }

        //public void LoadBook(string book_file)
        //{
        //    //book.load(book_file);
        //}
    }
}
