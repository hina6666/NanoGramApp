using NanoGramApp.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace NanoGramApp.ViewModel
{
    internal class NanoGramViewModel : ObservableObject
    {
        #region Fields
        private int BoardSize;
        private readonly int Lives;
        private readonly GameBoard? game;
        private string _gameStatus;
        #endregion

        #region Properties
        private static readonly Dictionary<string, (Color BackGroundColor, Color FontColor)> colorMacher =
            new Dictionary<string, (Color, Color)>
        {
                { "█", (Colors.Black, Colors.Black) },
                { "X", (Colors.Transparent, Colors.Black)},
                {"Wrong█", (Colors.Black, Colors.Black)},
                {"WrongX", (Colors.Transparent, Colors.Red) },
        };

        public Grid? GameGrid { get; private set; }
        public string Flag => (game.Flag ? "█" : "X");

        public string GameStatus
        {
            get { return _gameStatus; }
            set
            {
                if (_gameStatus != value)
                {
                    _gameStatus = value;
                    OnPropertyChanged();  // Notify that the GameStatus has changed
                }
            }
        }
        #endregion

        #region Commands
        public ICommand? CellTappedCommand { get; }
        public ICommand? ToggleModeCommand { get; }
        #endregion

        #region Constructor
        public NanoGramViewModel(Difficulty diff, int lives)
        {
            OnPropertyChanged();
            game = new GameBoard(diff, lives);
            Lives = lives;
            BoardSize = game.BoardSize;
            game.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(GameBoard.GameStatus))
                {
                    GameStatus = game.GameStatus;  // Update ViewModel's GameStatus when GameBoard's GameStatus changes
                }
            };
            //gameStatus = "Nothing is clicked";
            CellTappedCommand = new Command<int>(OnCellTapped);
            ToggleModeCommand = new Command(OnToggleMode);
            GameGrid = GenerateDynamicGrid();
        }
        #endregion

        #region BoardMaker
        private Grid GenerateDynamicGrid()
        {
            var grid = new Grid
            {
                RowSpacing = 0,
                ColumnSpacing = 0
            };


            // Define Row and Column Sizes
            for (int i = 0; i <= BoardSize; i++) // One extra for hints
            {
                {
                    // Row Definitions
                    if (i == 0) // First row for column hints
                    {
                        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(BoardSize * 0.24, GridUnitType.Star) }); // Flexible height for the first row
                    }
                    else
                    {
                        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Button height (adjust ratio as needed)
                    }

                    // Column Definitions
                    if (i == 0) // First column for row hints
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(BoardSize * 0.24, GridUnitType.Star) }); // Flexible width for the first column
                    }
                    else
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // Button width (adjust ratio as needed)
                    }
                }

            }

            // Add Hints (First Row and First Column)
            for (int i = 1; i <= BoardSize; i++) // Start at 1 to skip (0,0)
            {
                // Column Hint (Above the Grid)
                Frame colHint = CreateHintCell(i - 1, true);
                Grid.SetRow(colHint, 0);
                Grid.SetColumn(colHint, i);
                //colHint.HeightRequest = CalculateHintHeight(game.Columns[i - 1].Count);

                grid.Children.Add(colHint);

                // Row Hint (Left of the Grid)
                Frame rowHint = CreateHintCell(i - 1, false);
                Grid.SetRow(rowHint, i);
                Grid.SetColumn(rowHint, 0);

                grid.Children.Add(rowHint);
            }

            // Add Game Board Cells
            for (int row = 1; row <= BoardSize; row++)
            {
                for (int col = 1; col <= BoardSize; col++)
                {
                    int index = (row - 1) * BoardSize + (col - 1);

                    Button button = new Button
                    {
                        BindingContext = game.GuessdBoard[row - 1, col - 1],
                        FontSize = 25,
                        BackgroundColor = Colors.Transparent,
                        BorderColor = Colors.Grey,
                        BorderWidth = 0.5,
                        CornerRadius = 0, // 🔹 Makes the button edges square
                        Padding = 0,
                        Margin = 0
                    };
                    //button.SetBinding(Button.BackgroundColorProperty, "ButtonColor");
                    //button.SetBinding(Button.TextColorProperty, "TextColor");
                    button.SetBinding(Button.TextProperty, "Value");
                    button.SetBinding(Button.IsEnabledProperty, "IsEnabled");
                    button.Command = CellTappedCommand;
                    button.CommandParameter = index;

                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                    grid.Children.Add(button);
                }
            }
            grid.WidthRequest = 380;  // Adjust the width of the grid to fit the content
            grid.HeightRequest = 380; // Ensure height is the same, keeping the square shape

            return grid;
        }

        // Helper Methods for Dynamic Hint Size Calculation
        // Helper Method to Create Hint Cells
        private Frame CreateHintCell(int index, bool IsCol)
        {
            List<int> Hints = (IsCol ? game.Columns : game.Rows)[index];
            string text = "";

            for (int i = 0; i < Hints.Count; i++)
            {
                text += (IsCol ? "\n" : " ") + Hints[i].ToString();
            }

            double baseFontSize = Math.Max(8, Math.Min(15, 160 / BoardSize));

            double adjustedFontSize;
            if (IsCol)
            {
                // Column hints (Vertical) → Shrink more aggressively
                adjustedFontSize = baseFontSize - (Hints.Count > 3 ? (Hints.Count - 3) * 2 : 0);
            }
            else
            {
                // Row hints (Horizontal) → Shrink less aggressively
                adjustedFontSize = baseFontSize - (Hints.Count > 3 ? (Hints.Count - 3) * 1.5 : 0);
            }

            adjustedFontSize = Math.Max(8, adjustedFontSize); // Ensure it doesn't go below 8

            return new Frame
            {
                BorderColor = Colors.Black,
                BackgroundColor = Colors.Transparent,
                Padding = 0,
                Margin = 1, // No margin on the Frame itself
                CornerRadius = 5, // Rounded corners
                Content = new Label
                {
                    Text = text,
                    FontSize = adjustedFontSize,
                    FontAttributes = FontAttributes.Bold, // Make the text bold  
                    HorizontalTextAlignment = (IsCol ? TextAlignment.Center : TextAlignment.End),
                    VerticalTextAlignment = (IsCol ? TextAlignment.End : TextAlignment.Center),
                    Padding = 0,
                    Margin = 2.5
                }
            };
        }
        #endregion

        #region Functions
        private void OnCellTapped(int index)
        {
            int row = index / BoardSize;
            int col = index % BoardSize;
            if (game.IsGameOver || !game.GuessdBoard[row, col].IsEnabled) { return; }
            string color = (game.Guess(row, col) ? "Wrong" : string.Empty) + (game.GuessdBoard[row, col].Value);
            Console.WriteLine("in OnCell Tapeed");
            Color(row, col, color);
            CheckX(row, col);
        }

        private void CheckX(int row, int col)
        {
            if (game.CheckRow(row))
            {
                for (int i = 0; i < BoardSize; i++)
                {
                    if (game.GuessdBoard[row, i].IsEnabled && game.GuessdBoard[row, i ].Value != "█")
                        Color(row, i, "X");
                }
            }
            if (game.CheckColumn(col))
            {
                for (int i = 0; i < BoardSize; i++)
                {
                    if (game.GuessdBoard[i, col].IsEnabled && game.GuessdBoard[i, col].Value != "█")
                        Color(i, col, "X");
                }
            }
            game.CheckWin();
        }

        private void Color(int row, int col, string str)
        {
            Console.WriteLine(str);
            var button = GameGrid.Children
                        .OfType<Button>()
                        .FirstOrDefault(b => Grid.GetRow(b) == row + 1 && Grid.GetColumn(b) == col + 1);


            if (button != null)
            {
                game.DisableCell(row, col);
                button.IsEnabled = false;
                // Disable the button and change its background to red
                var (backgroundColor, textColor) = colorMacher[str];
                button.BackgroundColor = backgroundColor;
                button.TextColor = textColor;
            }
        }
        private void OnToggleMode()
        {
            game.ToggleMode();
            OnPropertyChanged(nameof(Flag)); // Notify UI of change
        }
        #endregion
    }
}
