using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Connect4
{
    public class Game
    {
        public Game()
        {
            SB = new StringBuilder();
        }
        public StringBuilder SB { get; set; }
        
        public async Task HumanVsMachine(int depth)
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

                if (player1ToPlay) //human to play
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
                }

                else //computer AI
                {
                    Console.WriteLine("Computer thinking about a move...");
                    Console.WriteLine(position.PlayerToMove);
                    await Task.Delay(500);
                    (ulong move, int score) = solver.Solve(position, depth);
                    SB.AppendLine($"Score = {score}");                    
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
            //only used for loggging of whole game
            //var path = @"c:\utvikling\connect4.txt";
            //File.WriteAllText(path, SB.ToString());
        }

        public async Task MachineVsMachine(int depth)
        {
            //player 1 is human
            //player 2 is machine
            Console.WriteLine("Match started....\n");

            var solver = new Solver();
            var position = new Position();
            bool player1ToPlay = true;

            while (true)
            {
                Console.WriteLine($"Number of nodes processed: {solver.NodeCount}");
                if (position.Moves >= 42)
                {
                    Console.WriteLine("Game ended in a draw");
                    return;
                }

                if (player1ToPlay) //Machine 1 to play
                {
                    Console.WriteLine("Player 1 thinking about a move...");                    
                    await Task.Delay(500);
                    (ulong move, int score) = solver.Solve(position, depth);                    
                    SB.AppendLine($"Score = {score}");                   
                    PrintColumnPlayed(move, position.PlayerToMove);
                    position.Play(move);
                }

                else //Machine 2 to play
                {
                    Console.WriteLine("Player 2 thinking about a move...");
                    await Task.Delay(500);
                    (ulong move, int score) = solver.Solve(position, depth);
                    SB.AppendLine($"Score = {score}");
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

            var whoWon = player1ToPlay ? "Player 1 won!" : "Player 2 won";
            Console.WriteLine(whoWon);
            //only used for loggging of whole game
            //var path = @"c:\utvikling\connect4.txt";
            //File.WriteAllText(path, SB.ToString());
        }


        private void PrintColumnPlayed(ulong move, Player player)
        {
            for (int i = 0; i < 7; i++)
            {
                var res = Position.ColumnMask(i) & move;
                if (res != 0)
                {
                    SB.AppendLine($"{player} played in column {i + 1}");
                    Console.WriteLine($"{player} played in column {i + 1}");
                    break;
                }
            }
        }
    }
}
