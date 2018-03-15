using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicTacToe
{
    class GameHistories
    {
        Random rng = new Random(42);
        public class GameMove
        {
            Board.Player player;            
            uint position;
            uint wins = 0;
            uint losses = 0;

            GameMove(Player pl, uint pos)
            {
                Player = pl;
                Position = pos;
                Wins = 0;
                Losses = 0;
            }

            GameMove(Player pl, uint pos, uint wins, uint losses)
            {
                Player = pl;
                Position = pos;
                Wins = wins;
                Losses = losses;
            }

            public Board.Player Player
            {
                get { return player; }
                set { player = value; }                
            }

            public uint Position
            {
                get { return position; }
                set { position = value; }
            }

            public uint Wins
            {
                get { return wins; }
                set { wins = value; }
            }

            public uint Losses
            {
                get { return losses; }
                set { losses = value; }
            }

            void RegisterWin() { Wins += 1; }
            void RegisterLosses() { Losses += 1; }
            static GameMove Random(Board brd)
            {
                //get indexes of all open positions
                var openPositions = brd.GameBoard
                    .Zip(Enumerable.Range(0, 9), (mv, i) => Tuple.Create(mv, i))
                    .Where(pr => pr.Item1 == Move.Sp)
                    .Select(pr => pr.Item2).ToArray();

                //randomly choose an open position 
                return new GameMove(player, openPositions[rng.Next(openPositions.Count())]);
            }

            float Worth()
            {
                if (wins == 0 && losses == 0)
                    return 0.0;
                else if (wins > 0 && losses == 0)
                    return (float)wins;
                else if (wins == 0 && losses > 0)
                    return (float)-losses;
                else if (losses > wins)
                    return -(float)losses / (float)wins;
                else
                    return (float)wins / (float)losses;
            }
        }       

        
        //records a board position and all moves made from that board by players
        public class GameTree
        {
            Board board;
            Dictionary<GameMove, GameTree> tree;
            
            GameTree Parent;

            internal Dictionary<GameMove, GameTree> Tree
            {
                get { return tree; }
                set { tree = value; }
            }

            public Board Board
            {
                get {  return board; }
                set { board = value; }
            }

            GameTree(Board brd)
            {
                Board = brd;
                Tree = new Dictionary<GameMove, GameTree>();
            }

            GameTree MakeMove(GameMove mv, Board brd)
            {
                GameTree res = null;
                if (tree.Contains(mv))
                {
                    res = tree[mv];
                    res.Parent = this;
                }
                else
                {
                    res = new GameTree(brd);
                    tree[mv] = res;
                    res.Parent = this;                    
                }
                return res;
            }

            GameMove GetMove(bool best)
            {
                var moves = tree.Keys;
                var possibleMoves = moves.Select(k => Tuple.Create(k, k.Worth())).Order(pr => pr.Item2);
                if (best)
                    return possibleMoves.First().Item1;
                else
                {
                    int pos = rng.Next(possibleMoves.Count());
                    return possibleMoves.Nth(pos).Item1;
                }
                return GameMove.Random(board);                                
            }
        }


        Queue<GameMove> gameHistory = new Queue<GameMove>();


    }
}
