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
        public List<int>[] RowHints { get; private set; }
        public List<int>[] ColumnHints { get; private set; }
        public string Flag => (game.Flag ? "█" : "X");
        public string RowHintsString
        {
            get
            {
                return string.Join(", ", RowHints.Select(r => string.Join(" ", r)));
            }
        }

        public string ColumnHintsString
        {
            get
            {
                return string.Join(", ", ColumnHints.Select(c => string.Join(" ", c)));
            }
        }


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
            RowHints = game.Rows;
            ColumnHints = game.Columns;

            gameStatus = "Nothing is clicked";
            CellTappedCommand = new Command<int>(OnCellTapped);
            ToggleModeCommand = new Command(OnToggleMode);
            GameGrid = GenerateDynamicGrid();
        }

        private Grid GenerateDynamicGrid()
        {
            var grid = new Grid
            {
                RowSpacing = 1,
                ColumnSpacing = 1
            };

            for (int i = 0; i < BoardSize; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    int index = row * BoardSize + col;

                    Button button = new Button
                    {
                        BindingContext = game.GuessdBoard[row, col],
                        WidthRequest = 60,
                        HeightRequest = 60,
                        FontSize = 25,
                        BackgroundColor = Colors.Transparent,
                        BorderColor = Colors.Black,
                        BorderWidth = 2,
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
