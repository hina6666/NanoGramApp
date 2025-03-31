using NanoGramApp.Model;
using NanoGramApp.ViewModel;

namespace NanoGramApp.View;

public partial class GamePage : ContentPage
{
	public GamePage()
	{
		InitializeComponent();
        BindingContext = new NanoGramViewModel(Difficulty.Hard, 3);
    }
}