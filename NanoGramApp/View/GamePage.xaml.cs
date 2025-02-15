using NanoGramApp.ViewModel;

namespace NanoGramApp.View;

public partial class GamePage : ContentPage
{
	public GamePage()
	{
		InitializeComponent();
        BindingContext = new NanoGramViewModel();
    }
}