using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace NanoGramApp.Model
{
    internal class GameBoard : ObservableObject
    {
        #region Fields
        private int Lives;
        private bool[,] Board;
        public int BoardSize;
        private string _gameStatus;
        #endregion

        #region Properties
        public Cell[,] GuessdBoard { get; private set; }

        public List<int>[] Columns { get; private set ; }
        public List<int>[] Rows { get; private set; }
        public bool Flag { get; private set; }

        public string GameStatus
        {
            get { return _gameStatus; }
            set
            {
                if (_gameStatus != value)
                {
                    _gameStatus = value;
                    OnPropertyChanged();
                }
            }
        }
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
            Complete(row, column);
            if (Board[row, column] != Flag)
            {
                Wrong();
            }
            if (CheckWin())
            {
                GameStatus = $"Good Job you won with {Lives} more lives";
                return true;
            }
            return Board[row, column] != Flag;
        }
        private void Wrong()
        {
            Lives--;
            GameStatus = "Lives: " + Lives;
            if (Lives == 0)
            {
                IsGameOver = true;
                GameStatus = "You Lost :(";
            }
        }
        private void Complete(int row, int column)
        {
            /*
            if (Board[row, column])
            {
                GuessdBoard[row, column].ButtonColor = Colors.Green;  // Correct guess: green color
            }
            else
            {
                GuessdBoard[row, column].ButtonColor = Colors.Red; // Incorrect guess: red color
            }*/

            GuessdBoard[row, column].Value = Board[row, column] ? "█" : "X"; // Show symbol (█ or X)
            // Disable the cell
        }
        public bool CheckRow(int row)
        {
            for (int i = 0; i < BoardSize; i++)
            {
                if (Board[row, i] != (GuessdBoard[row, i].Value == "█"))
                {
                    return false;
                }
            }
            for (int i = 0; i < BoardSize; i++)
            {
                if (GuessdBoard[row, i].IsEnabled && !Board[row, i ])
                {
                    GuessdBoard[row, i].Value = "X";
                }
            }
            return true;
        }
        public bool CheckColumn(int column)
        {
            for (int i = 0; i < BoardSize; i++)
            {
                if (Board[i, column] != (GuessdBoard[i, column].Value == "█"))
                {
                    return false;
                }
            }
            for (int i = 0; i < BoardSize; i++)
            {
                if (GuessdBoard[i, column].IsEnabled && !Board[i,column])
                {
                    GuessdBoard[i, column].Value = "X";
                }
            }
            return true;
        }

        public void DisableCell(int row, int col) { GuessdBoard[row, col].IsEnabled = false; }

        public bool CheckWin()
        {
            foreach (var cell in GuessdBoard)
                if (cell.IsEnabled) 
                    return false;
                            GameStatus = $"Good Job you won with {Lives} more lives";
            return true;
        }
        public void ToggleMode()
        {
            Flag = !Flag;
        }
        #endregion
    }
}
