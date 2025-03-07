using NanoGramApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace NanoGramApp.ViewModel
{
    internal class NanoGramViewModel : ObservableObject
    {
        private const int BoardSize = 5;
        private const int Lives = 20;
        private readonly GameBoard game;

        public Grid? GameGrid { get; private set; }
        public string? gameStatus { get; private set; }
        public string Flag => (game.Flag ? "█" : "X");


        public string? GameStatus
        {
            get => gameStatus;
            set
            {
                if (gameStatus != value)
                {
                    gameStatus = value;
                    OnPropertyChanged();
                }
            }
        }
        public ICommand? CellTappedCommand { get; }
        public ICommand? ToggleModeCommand { get; }



        public NanoGramViewModel()
        {
            game = new GameBoard(BoardSize, Lives);

            gameStatus = "Nothing is clicked";
            CellTappedCommand = new Command<int>(OnCellTapped);
            ToggleModeCommand = new Command(OnToggleMode);
            GameGrid = GenerateDynamicGrid();
        }
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
                        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(BoardSize *0.24, GridUnitType.Star) }); // Flexible height for the first row
                    }
                    else
                    {
                        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Button height (adjust ratio as needed)
                    }

                    // Column Definitions
                    if (i == 0) // First column for row hints
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(BoardSize *0.24, GridUnitType.Star) }); // Flexible width for the first column
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
                Frame colHint = CreateHintCell(i-1, true);
                Grid.SetRow(colHint, 0);
                Grid.SetColumn(colHint, i);
                //colHint.HeightRequest = CalculateHintHeight(game.Columns[i - 1].Count);

                grid.Children.Add(colHint);

                // Row Hint (Left of the Grid)
                Frame rowHint = CreateHintCell(i-1, false);
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
                        BorderColor = Colors.Black,
                        BorderWidth = 1,
                        CornerRadius = 0, // 🔹 Makes the button edges square
                        Padding = 0,
                        Margin = 0
                    };

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


        private void OnCellTapped(int index)
        {
            int row = index / BoardSize;
            int col = index % BoardSize;
            

            if (game.Guess(row, col))
            {
                GameStatus = game.GameStatus;
                OnPropertyChanged(nameof(GameStatus));
            }
        }



        private void OnToggleMode()
        {
            game.ToggleMode();
            OnPropertyChanged(nameof(Flag)); // Notify UI of change
        }
    }
}
