﻿using System;
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
            nodeCount = 0;
            //for (int i = 0; i < Position.WIDTH; i++) // initialize the column exploration order, starting with center columns
            //{
            //    var half = Position.WIDTH / 2;
            //    var mid = 1 - (2 * (i % 2));
            //    var end = (i + 1) / 2;
            //    var order = half + mid * end;
            //    columnOrder[i] = order; // example for WIDTH=7: columnOrder = {3, 4, 2, 5, 1, 6, 0}            
            //}

            columnOrder[0] = 3;
            columnOrder[1] = 4;
            columnOrder[2] = 2;
            columnOrder[3] = 5;
            columnOrder[4] = 1;
            columnOrder[5] = 6;
            columnOrder[6] = 0;
        }

        //static int TABLE_SIZE = 24;
        //TranspositionTable<uint_t<Position::WIDTH*(Position::HEIGHT + 1) - TABLE_SIZE >, Position::position_t, uint8_t, TABLE_SIZE > transTable;
        ulong nodeCount; // counter of explored nodes.
        int[] columnOrder = new int[Position.WIDTH];

        int negamax(Position P, int alpha, int beta)
        {
            Debug.Assert(alpha < beta);
            Debug.Assert(!P.canWinNext());
            nodeCount++;
            var possible = P.possibleNonLosingMoves();
            if (possible == 0) // if no possible non losing move, opponent wins next move
                return (-(Position.WIDTH * Position.HEIGHT - P.nbMoves()) / 2);
            if (P.nbMoves() >= Position.WIDTH * Position.HEIGHT - 2) // check for draw game
                return (0);

            int min = -(Position.WIDTH * Position.HEIGHT - 2 - P.nbMoves()) / 2;  // lower bound of score as opponent cannot win next move
            if (alpha < min)
            {
                alpha = min;                     // there is no need to keep alpha below our max possible score.
                if (alpha >= beta) return (alpha);  // prune the exploration if the [alpha;beta] window is empty.
            }

            int max = (Position.WIDTH * Position.HEIGHT - 1 - P.nbMoves()) / 2;   // upper bound of our score as we cannot win immediately
            if (beta > max)
            {
                beta = max;                     // there is no need to keep beta above our max possible score.
                if (alpha >= beta) return (beta);  // prune the exploration if the [alpha;beta] window is empty.
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
            int bestCol = -1;
            MoveSorter moves = new MoveSorter();
            for (int i = Position.WIDTH - 1; i >= 0; i--)
            {
                var move = possible & Position.column_mask(columnOrder[i]);
                if (move != 0ul)
                    moves.add(move, P.moveScore(move));
            }

            ulong next;
            while ((next = moves.getNext()) != 0ul)
            {
                Position p2 = new Position(P); //unsure if this is correct translation from c++ -- got it -> copy constructor from c++
                p2.play(next);
                int score = -negamax(p2, -beta, -alpha);
                if (score >= beta)
                {
                    //transTable.put(key, score + Position.MAX_SCORE - 2 * Position.MIN_SCORE + 2); // save the lower bound of the position
                    return score;  // prune the exploration if we find a possible move better than what we were looking for.
                }
                if (score > alpha) alpha = score; // reduce the [alpha;beta] window for next exploration, as we only
                                                  // need to search for a position that is better than the best so far.
            }


            return alpha;
        }

        public static int INVALID_MOVE = -1000;

        public ulong getNodeCount() => nodeCount;

        public int solve(Position P, bool weak = false)
        {
            if (P.canWinNext()) // check if win in one move as the Negamax function does not support this case.
                return (Position.WIDTH * Position.HEIGHT + 1 - P.nbMoves()) / 2;
            int min = -(Position.WIDTH * Position.HEIGHT - P.nbMoves()) / 2;
            int max = (Position.WIDTH * Position.HEIGHT + 1 - P.nbMoves()) / 2;
            if (weak)
            {
                min = -1;
                max = 1;
            }

            while (min < max)
            {                    // iteratively narrow the min-max exploration window
                int med = min + (max - min) / 2;
                if (med <= 0 && min / 2 < med) med = min / 2;
                else if (med >= 0 && max / 2 > med) med = max / 2;
                int r = negamax(P, med, med + 1);   // use a null depth window to know if the actual score is greater or smaller than med
                if (r <= med) max = r;
                else min = r;
            }
            return min;
        }

        public int[] analyze(Position P, bool weak = false)
        {
            var scores = new int[Position.WIDTH]; // { Position.WIDTH, Solver.INVALID_MOVE };
            for (int col = 0; col < Position.WIDTH; col++)
                if (P.canPlay(col))
                {
                    if (P.isWinningMove(col)) scores[col] = (Position.WIDTH * Position.HEIGHT + 1 - P.nbMoves()) / 2;
                    else
                    {
                        var P2 = new Position(P);
                        P2.playCol(col);
                        scores[col] = -solve(P2, weak);
                    }
                }
            return scores;
        }

        public void reset()
        {
            nodeCount = 0;
            //transTable.reset();
        }

        public void loadBook(string book_file)
        {
            //book.load(book_file);
        }
    }
}