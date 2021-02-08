using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using position_t = System.UInt64;

namespace Connect4
{
    public class Position
    {
        public Position(Position pos)
        {
            current_position = pos.current_position;
            mask = pos.mask;
            moves = pos.moves;
            ColumnPlayed = pos.ColumnPlayed;
        }
        public Position()
        {
            Debug.Assert(WIDTH < 8, "Board width must be 7 columns");
            Debug.Assert(WIDTH * (HEIGHT + 1) < 64, "Board does not fit in 64bits bitboard");
            current_position = 0;
            mask = 0;
            moves = 0;

        }
        public static int WIDTH = 7;  // width of the board
        public static int HEIGHT = 6; // height of the board

        public static int MIN_SCORE = -(WIDTH * HEIGHT) / 2 + 3;
        public static int MAX_SCORE = (WIDTH * HEIGHT + 1) / 2 - 3;
        public position_t current_position; // bitmap of the current_player stones
        public position_t mask;             // bitmap of all the already palyed spots
        int moves;        // number of moves played since the beinning of the game.
        public int ColumnPlayed { get; set; }

        public bool canPlay(int col) => (mask & top_mask_col(col)) == 0;

        public void playCol(int col)
        {
            ColumnPlayed = col;
            play((mask + bottom_mask_col(col)) & column_mask(col));
        }

        public bool isWinningMove(int col)
        {
            return (winning_position() & possible() & column_mask(col)) != 0ul;
        }

        public void play(position_t move)
        {
            current_position ^= mask;
            mask |= move;
            moves++;
        }

        public int play(IEnumerable<string> seq)
        {
            for (int i = 0; i < seq.Count(); i++)
            {
                int col = Convert.ToInt32(seq.ElementAt(i)) - 1;
                if (isWinningMove(col))
                {
                    Console.WriteLine($"Game over with the next move in column {col} after {nbMoves()} moves");
                }
                if (col < 0 || col >= Position.WIDTH || !canPlay(col) || isWinningMove(col))
                {
                    if (!canPlay(col)) // invalid move
                    {
                        Console.WriteLine($"Invalid move in col {col} after {nbMoves()} moves");
                        continue;
                    }
                    return i;
                }
                playCol(col);

            }
            return seq.Count();
        }

        public bool canWinNext()
        {
            return (winning_position() & possible()) == 1ul;
        }

        public int nbMoves()
        {
            return moves;
        }

        public position_t key()
        {
            return current_position + mask;
        }

        public int moveScore(position_t move)
        {
            return BitOperations.PopCount(compute_winning_position(current_position | move, mask));
        }

        public position_t possibleNonLosingMoves()
        {
            Debug.Assert(!canWinNext());
            position_t possible_mask = possible();
            position_t opponent_win = opponent_winning_position();
            position_t forced_moves = possible_mask & opponent_win;
            if (forced_moves == 1ul)
            {
                if ((forced_moves & (forced_moves - 1)) == 1ul) // check if there is more than one forced move
                    return 0;                           // the opponnent has two winning moves and you cannot stop him
                else possible_mask = forced_moves;    // enforce to play the single forced move
            }
            return possible_mask & ~(opponent_win >> 1);  // avoid to play below an opponent winning spot
        }

        position_t winning_position() => compute_winning_position(current_position, mask);

        position_t opponent_winning_position() => compute_winning_position(current_position ^ mask, mask);

        position_t possible() => (mask + bottom_mask(WIDTH, HEIGHT)) & board_mask;

        static position_t compute_winning_position(position_t position, position_t mask)
        {
            // vertical;
            position_t r = (position << 1) & (position << 2) & (position << 3);

            //horizontal
            position_t p = (position << (HEIGHT + 1)) & (position << 2 * (HEIGHT + 1));
            r |= p & (position << 3 * (HEIGHT + 1));
            r |= p & (position >> (HEIGHT + 1));
            p = (position >> (HEIGHT + 1)) & (position >> 2 * (HEIGHT + 1));
            r |= p & (position << (HEIGHT + 1));
            r |= p & (position >> 3 * (HEIGHT + 1));

            //diagonal 1
            p = (position << HEIGHT) & (position << 2 * HEIGHT);
            r |= p & (position << 3 * HEIGHT);
            r |= p & (position >> HEIGHT);
            p = (position >> HEIGHT) & (position >> 2 * HEIGHT);
            r |= p & (position << HEIGHT);
            r |= p & (position >> 3 * HEIGHT);

            //diagonal 2
            p = (position << (HEIGHT + 2)) & (position << 2 * (HEIGHT + 2));
            r |= p & (position << 3 * (HEIGHT + 2));
            r |= p & (position >> (HEIGHT + 2));
            p = (position >> (HEIGHT + 2)) & (position >> 2 * (HEIGHT + 2));
            r |= p & (position << (HEIGHT + 2));
            r |= p & (position >> 3 * (HEIGHT + 2));

            return r & (board_mask ^ mask);
        }




        static position_t top_mask_col(int col) => 1ul << ((HEIGHT - 1) + col * (HEIGHT + 1));

        static position_t bottom_mask_col(int col) => 1ul << col * (HEIGHT + 1);

        public static position_t column_mask(int col) => ((1ul << HEIGHT) - 1) << col * (HEIGHT + 1);

        static position_t bottom_mask(int width, int height) => 4432676798593ul | 1ul << (width - 1) * (height + 1);
        static position_t board_mask => bottom_mask(WIDTH, HEIGHT) * ((1ul << HEIGHT) - 1);

        public static position_t bottom()
        {
            var start = 1ul << 0;
            for (int i = 0; i < WIDTH; i++)
            {
                start = start | 1ul << 7 * i;
            }
            return start;
        }

    }
}
