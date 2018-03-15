using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    class Game
    {
        /// <summary>
        /// Ths class actually plays a game on the goame board class
        /// </summary>
        public class QuitGame : Exception
        {
            public QuitGame(string message): base(message)
            {               
            }
        }


        //Create current board for new game
        Board currentBoard = Board.StartGame;
        //Keep track of moves in game
        List<String> gameMoves = new List<String>();
        
        public Game() { }

        public void ConsoleWriteLine(string line, bool useNewLine = true)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            if (useNewLine)
                Console.WriteLine(line);
            else
                Console.Write(line);
            Console.ForegroundColor = color;

        }
        /// <summary>
        /// Run Game
        /// </summary>
        public void Run()
        {
            bool run = true;
            PrintBanner();

            //While games are being played
            while(run)
            {
                //make a move
                var player = PlayGame();

                //show move on board
                currentBoard.DisplayBoard();

                //do we habve a winner yet
                if (currentBoard.Winner(player))
                {
                    Console.Write("Congratulations: Player ");
                    if (player == Board.Player.O)
                        currentBoard.WriteMove(Board.Move.O);
                    else
                        currentBoard.WriteMove(Board.Move.X);
                    ConsoleWriteLine(" is the winner");
                    gameMoves.Add(currentBoard.ToString());
                    currentBoard = Board.StartGame;                    
                }

                //display a menu of choices to user
                Console.Write("Enter 'Q' to quit or 'Y' to continue 'H' to see history of last game");
                string input = Console.ReadLine();
                switch (input.Trim().ToUpper())
                { 
                    case "Q":
                        run = false;
                        break;
                    case "H":
                        int move = 1;
                        foreach(string mv in gameMoves)
                        {
                            Console.WriteLine("Move {0}", move);
                            move++;
                            new Board(Board.ReadBoard(mv)).DisplayBoard();
                            Console.Write("Press any key to see next move...");
                            Console.ReadKey(true);
                        }
                        break;
                    case "Y":
                        run = true;
                        break;
                    default:
                        ConsoleWriteLine("Unknown Command " + input.Trim().ToUpper());
                        break;
                }
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            ConsoleWriteLine("Good Bye");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        //Return Game Winner or if tie return Player.None
        public Board.Player PlayGame()
        {
            //play starts with X
            Board.Player player = Board.Player.O;
            
            try
            {
                //Alternate moves with computer until someone wins
                gameMoves = new List<string>();
                currentBoard.DisplayBoard();
                while (!currentBoard.Winner(player) && !currentBoard.IsTie())
                {
                    player = (player == Board.Player.O) ? Board.Player.X : Board.Player.O;                    
                    gameMoves.Add(currentBoard.ToString());
                    Tuple<int, int, Board.Move> move;
                    string prompt;
                    while (!GetNextMove(player, out move, out prompt))
                    {
                        ConsoleWriteLine(prompt);
                    }

                    currentBoard.DoMove(move.Item1, move.Item2, move.Item3);
                    currentBoard.DisplayBoard();
                    if (currentBoard.Winner(player)) break;
                    ConsoleWriteLine("The Computer moves.");
                    player = (player == Board.Player.O) ? Board.Player.X : Board.Player.O;
                    gameMoves.Add(currentBoard.ToString());
                    move = GetNextComputerMove(player);
                    currentBoard.DoMove(move.Item1, move.Item2, move.Item3);
                    currentBoard.DisplayBoard();
                }
            }
            catch(QuitGame)
            {
                ConsoleWriteLine("User has quit the game");
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                ConsoleWriteLine("Error in game" + ex.Message);
                Console.ForegroundColor = ConsoleColor.White;
            }

            //return winner
            if (currentBoard.IsTie())
                player = Board.Player.None;
            return player;
        }

        public Board.Move MoveFromPlayer(Board.Player player)
        {
            return player == Board.Player.O ? Board.Move.O : Board.Move.X;
        }

        private Tuple<int, int, Board.Move> GetNextComputerMove(Board.Player player)
        {
            int move = currentBoard.RandomMove();
            return Tuple.Create(move / 3, move % 3, MoveFromPlayer(player));
        }

        private void PrintBanner()
        {
            ConsoleWriteLine("TIC TAC TOE GAME Version 1.0.   \nLet the Gamess Begin!");
        }


        public bool GetNextMove(Board.Player player, out Tuple<int, int, Board.Move> mv, out string prompt)
        {
            prompt = String.Empty;
            mv = Tuple.Create(-1, -1, Board.Move.Sp);
            //keep trying until the user quits or enters a valid input
            while (true)
            {
                //prompt user for imnput
                ConsoleWriteLine("Enter row and column for your move: row , column. \nor enter 'Q' to quit the game: ", false);
                string input = Console.ReadLine();

                //if player is quiting throw quit exception
                if (input.Trim().ToUpper() == "Q")
                    throw new QuitGame("Quiting TicTacToe Game");

                //split inputs into row and col numbers
                var inputs = input.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                int digit = -1;
                List<int> move = new List<int>();
                foreach (string i in inputs)
                    if (Int32.TryParse(i, out digit))
                    {
                        move.Add(digit);
                    }

                //check move input for valid input
                if (move.Count == 2)
                {
                    if (currentBoard.ValidMove(move[0] -1, move[1] -1, player == Board.Player.O ? Board.Move.O : Board.Move.X))
                    {
                        mv = Tuple.Create(move[0] -1, move[1] -1, MoveFromPlayer(player));
                        return true;
                    }
                        
                    prompt = "\nInvalid Input read. Must enter an integer between 1 and 3 a comma and an integer between 1 and 3\n";
                }
                else if (move.Count > 2)
                {
                    prompt = 
                        "\nToo many inputs entered" + "\nUser input was " + input + 
                        "\nMust enter an integer between 0 and 2 a comma and an integer between 0 and 2\n";                    
                }
                else if (move.Count < 2)
                {
                    prompt = 
                        "\nToo few inputs entered" + "\nUser input was " + input +
                        "Must enter an integer between 0 and 2 a comma and an integer between 0 and 2\n";                
                }
            }
        }
    }
}
