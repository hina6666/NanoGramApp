using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NanoGramApp.Model;
using NanoGramApp.View;


namespace NanoGramApp.ViewModel
{
    internal class LoadGameViewModel : ObservableObject
    {
        private Difficulty Difficulty;
        private bool confirm;

        public ICommand GoToGameCommand { get; set; }

        public LoadGameViewModel()
        {
            //Console.WriteLine("LoadCreated");
            Difficulty = Difficulty.Easy;
            GoToGameCommand = new Command(async () => await StartGame());
        }

        public async Task StartGame()
        {
            //Console.WriteLine("in start game");
            confirm = await Shell.Current.DisplayAlert("תרומה", "תודה על התרומה", "אישור", "בטל");
            if (confirm)
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("diff", Difficulty.Medium);
                await Shell.Current.GoToAsync(nameof(GamePage), data);
            }            
        }
    }
}
