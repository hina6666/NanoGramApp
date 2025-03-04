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
        private const int Lives = 3;
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
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            // Add Hints (First Row and First Column)
            for (int i = 1; i <= BoardSize; i++) // Start at 1 to skip (0,0)
            {
                // Column Hint (Above the Grid)
                Border colHint = CreateHintCell(i-1, true);
                Grid.SetRow(colHint, 0);
                Grid.SetColumn(colHint, i);
                grid.Children.Add(colHint);

                // Row Hint (Left of the Grid)
                Border rowHint = CreateHintCell(i-1, false);
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
                        WidthRequest = 300 / BoardSize,
                        HeightRequest = 300 / BoardSize,
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

            return grid;
        }

        // Helper Method to Create Hint Cells
        private Border CreateHintCell(int index, bool IsCol)
        {
            List<int> Hints = (IsCol ? game.Columns : game.Rows)[index];
            string text = " ";
            for (int i = 0; i < Hints.Count; i++)
            {
                text += (IsCol ? "\n" : " ") + Hints[i].ToString();
            }

            return new Border
            {
                Stroke = Colors.Black,
                StrokeThickness = 2,
                BackgroundColor = Colors.Transparent,
                HeightRequest = (IsCol ? 60 : 25),
                WidthRequest = (IsCol ? 25 : 60),
                Padding = 0,
                Margin = 0,
                Content = new Label
                {
                    Text = text,
                    FontSize = 15,
                    HorizontalTextAlignment = (IsCol ? TextAlignment.Center : TextAlignment.End),
                    VerticalTextAlignment = (IsCol ? TextAlignment.End : TextAlignment.Center),
                    Padding = 0,
                    Margin = 0
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
