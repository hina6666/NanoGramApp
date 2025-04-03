using NanoGramApp.Model;
using NanoGramApp.ViewModel;

namespace NanoGramApp.View;

[QueryProperty(nameof(Difficulty), "diff")]

public partial class GamePage : ContentPage
{
    private Difficulty difficulty;

    // Store the string and convert it to Difficulty
    public Difficulty Difficulty
    {
        get => difficulty;
        set
        {
            if (difficulty != value)
            {
                difficulty = value;
            }
        }
    }

    public GamePage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        //Console.WriteLine($"Navigated to GamePage with Difficulty: {difficulty}");

        // Pass the Difficulty Enum to the ViewModel
        BindingContext = new NanoGramViewModel(difficulty, 3);
    }
}