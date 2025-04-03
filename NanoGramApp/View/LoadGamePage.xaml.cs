using NanoGramApp.ViewModel;
using NanoGramApp.Model;

namespace NanoGramApp.View;

public partial class LoadGamePage : ContentPage
{
	public LoadGamePage()
	{
		InitializeComponent();
		BindingContext = new LoadGameViewModel();
	}
}