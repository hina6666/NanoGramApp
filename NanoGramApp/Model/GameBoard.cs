using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace NanoGramApp.Model
{
    internal class GameBoard 
    {
        private int Lives;
        private bool[,] Board;
        public int BoardSize;
        #region Properties
        public Cell[,] GuessdBoard { get; private set; }

        public List<int>[] Columns { get; private set ; }
        public List<int>[] Rows { get; private set; }
        public bool Flag { get; private set; }

        public string GameStatus { get; private set; }
        public bool IsGameOver { get; private set; }

        #endregion

        #region Constractor
        public GameBoard(int boardSize, int lives)
        {
            BoardSize = boardSize;
            this.GenrateBoard();
            this.GenerateRow();
            this.GenerateColumn();
            GuessdBoard = new Cell[BoardSize, BoardSize];
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    GuessdBoard[i, j] = new Cell();
                }
            }

            this.Lives = lives;
        }
        #endregion

        #region Generator 
        private void GenrateBoard()
        {
            Random rand = new Random();
            this.Board = new bool[BoardSize, BoardSize];
            bool hasTrue;
            for (int i = 0; i < BoardSize; i++)
            {
                hasTrue = false;
                for (int j = 0; j < BoardSize; j++)
                {
                    Board[i, j] = rand.Next(0, 2) == 1;
                    if (Board[i, j])
                    {
                        hasTrue = true;
                    }
                }
                if (!hasTrue)
                {
                    Board[i, rand.Next(0, BoardSize)] = true;
                }
            }
            for (int j = 0; j < BoardSize; j++)
            {
                hasTrue = false;
                for (int i = 0; i < BoardSize; i++)
                {
                    if (Board[i, j])
                    {
                        hasTrue = true;
                    }
                }
                if (!hasTrue)
                {
                    Board[rand.Next(0, BoardSize), j] = true;
                }
            }
        }
        private void GenerateRow()
        {
            Rows = new List<int>[BoardSize];
            int count;
            for (int i = 0; i < BoardSize; i++)
            {
                count = 0;
                Rows[i] = new List<int>();
                for (int j = 0; j < BoardSize; j++)
                {
                    if (Board[i, j])
                    {
                        count++;
                    }
                    else if (count != 0)
                    {
                        Rows[i].Add(count);
                        count = 0;
                    }
                }
                if (count != 0)
                {
                    Rows[i].Add(count);
                }
            }
        }
        private void GenerateColumn()
        {
            Columns = new List<int>[BoardSize];
            int count;
            for (int j = 0; j < BoardSize; j++)
            {
                count = 0;
                Columns[j] = new List<int>();
                for (int i = 0; i < BoardSize; i++)
                {
                    if (Board[i, j])
                    {
                        count++;
                    }
                    else if (count != 0)
                    {
                        Columns[j].Add(count);
                        count = 0;
                    }
                }
                if (count != 0)
                {
                    Columns[j].Add(count);
                }
            }
        }
        #endregion

        #region Function

        public bool Guess(int row, int column) 
        {
            if(IsGameOver || !GuessdBoard[row,column].IsEnabled) return false;
            GuessdBoard[row, column].Value = Board[row, column] ? "█" : "X";
            GuessdBoard[row, column].IsEnabled = false;
            this.CheckRow(row);
            this.CheckColumn(column);
            if (Board[row, column] != Flag)
            {
                Lives--;
                GameStatus = "Lives: " + Lives;
            }
            if (CheckWin())
            {
                GameStatus = $"Good Job you won with {Lives} more lives";
                IsGameOver = true;
                return true;
            }
            else if (Lives == 0)
            {
                GameStatus = "You Lost :(";
                IsGameOver = true;
            }
            return Board[row, column] != Flag;
        }

        public void CheckRow(int row)
        {
            for (int i = 0; i < BoardSize; i++)
            {
                if (Board[row, i] != (GuessdBoard[row, i].Value == "█"))
                {
                    return;
                }
            }
            for (int i = 0; i < BoardSize; i++)
            {
                if (GuessdBoard[row, i].IsEnabled && !Board[row, i ])
                {
                    GuessdBoard[row, i].Value = "X";
                    GuessdBoard[row, i].IsEnabled = false;
                }
            }
        }
        public void CheckColumn(int column)
        {
            for (int i = 0; i < BoardSize; i++)
            {
                if (Board[i, column] != (GuessdBoard[i, column].Value == "█"))
                {
                    return;
                }
            }
            for (int i = 0; i < BoardSize; i++)
            {
                if (GuessdBoard[i, column].IsEnabled && !Board[i,column])
                {
                    GuessdBoard[i, column].Value = "X";
                    GuessdBoard[i, column].IsEnabled = false;
                }
            }
        }

        public bool CheckWin()
        {
            foreach (var cell in GuessdBoard)
                if (cell.IsEnabled) return false;
            return true;
        }
        public void ToggleMode()
        {
            Flag = !Flag;
        }
        #endregion
    }
}
