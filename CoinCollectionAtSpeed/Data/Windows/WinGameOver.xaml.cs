using System.Windows;

namespace CoinCollectionAtSpeed.Data.Windows
{
    public partial class WinGameOver : Window
    {
        public WinGameOver(int PlayerScore)
        {
            InitializeComponent();
            Score.Text += PlayerScore.ToString();
        }

        private void CloseGame(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void RestartGame(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
