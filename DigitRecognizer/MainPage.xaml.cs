using System.Diagnostics;

namespace DigitRecognizer
{
    public partial class MainPage : ContentPage
    {


        public MainPage()
        {
            InitializeComponent();
        }


        async public void OnClearCanvas(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("ActionSheet: SavePhoto?", "Cancel", "Delete", "Photo Roll", "Email");
            Console.WriteLine("Action: " + action);
        }

    }

}
