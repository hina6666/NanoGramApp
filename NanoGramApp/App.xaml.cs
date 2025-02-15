using NanoGramApp.View;

namespace NanoGramApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new GamePage();
        }
    }
}
