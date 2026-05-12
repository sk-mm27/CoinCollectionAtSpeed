using CoinCollectionAtSpeed.Data;
using CoinCollectionAtSpeed.Data.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CoinCollectionAtSpeed;

public partial class MainWindow : Window
{
    PlayerControl playerControl;

    int counterCoins = 0;
    private readonly int numberCoinsOnField = 10;
    private readonly int valueTimerInTick = 10;

    private DispatcherTimer timer;

    public MainWindow()
    {
        InitializeComponent();
        GameStart();
    }

    public void GameStart()
    {
        playerControl = new PlayerControl(PlayingField.RowDefinitions.Count, PlayingField.ColumnDefinitions.Count);

        RandCoin();

        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);

        timer.Tick += Timer_Tick;

        timer.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        Timer.Value -= valueTimerInTick;

        //При истечении таймера - завершаем игру.
        if (Timer.Value == Timer.Minimum)
        {
            timer.Stop();

            ShadingForGameOver.Visibility = Visibility.Visible;

            var restart = new WinGameOver(counterCoins).ShowDialog();

            if (restart != null && (bool)restart)
            {
                Restart();
            }
            else
            {
                Close();
            }
        }
    }

    private void Restart()
    {
        //Убераем затенение экрана игры.
        ShadingForGameOver.Visibility = Visibility.Hidden;

        //Обнуляем все динамические игровые значения.
        Timer.Value = Timer.Maximum;

        playerControl.Restart();

        counterCoins = 0;
        Score.Text = counterCoins.ToString();

        //Удаляем оставшиеся монеты и заполняем поле новыми.
        var remainingCoins = PlayingField.Children.Cast<UIElement>().Where(e => e != FigurePlayer).ToList();

        if (remainingCoins.Count > 0)
        {
            foreach (UIElement remoteCoin in remainingCoins)
            {
                PlayingField.Children.Remove(remoteCoin);
            }

            RandCoin();
        }

        //Возвращаем фигуру игрока в стартовую позицию.
        Grid.SetRow(FigurePlayer, 0);
        Grid.SetColumn(FigurePlayer, 0);

        //Запускаем игровой таймер.
        timer.Start();
    }


    private Border NewCoin()
    {
        Border coin = new Border()
        {
            BorderBrush = Brushes.Goldenrod,
            BorderThickness = new Thickness(5),
            CornerRadius = new CornerRadius(2000),
            Background = Brushes.Gold
        };

        coin.SetBinding(WidthProperty, new Binding("ActualHeight") { Source = coin });

        return coin;
    }

    private void RandCoin()
    {
        //Получаем координаты допустимых ячеек игрового поля для заполнения монетами.
        int rowMax = PlayingField.RowDefinitions.Count - 1;
        int columnMax = PlayingField.ColumnDefinitions.Count - 1;

        int forbiddenRow = Grid.GetRow(FigurePlayer);
        int forbiddenColumn = Grid.GetColumn(FigurePlayer);

        List<int> listRow = new List<int>();
        List<int> listColumn = new List<int>();

        for (int i = 0; listRow.Count <= rowMax; i++)
        {
            if (i == forbiddenRow)
                continue;

            listRow.Add(i);
        }
        for (int i = 0; listColumn.Count <= rowMax; i++)
        {
            if (i == forbiddenColumn)
                continue;

            listColumn.Add(i);
        }

        //Заполняем игровое поле монетами.
        List<(int, int)> coins = new List<(int, int)>();

        var rand = new Random();

        while (coins.Count < numberCoinsOnField)
        {
            (int, int) coordinates = (listRow[rand.Next(listRow.Count - 1)], listColumn[rand.Next(listColumn.Count - 1)]);

            //Если ячека не пуста - добовляем монету
            if (!coins.Any(x => x == coordinates))
            {
                coins.Add(coordinates);

                var newCoin = NewCoin();

                PlayingField.Children.Add(newCoin);

                Grid.SetRow(newCoin, coordinates.Item1);
                Grid.SetColumn(newCoin, coordinates.Item2);
            }
        }

    }

    private void Control(object sender, KeyEventArgs e)
    {

        switch (e.Key.ToString())
        {
            case "Up":
                Grid.SetRow(FigurePlayer, playerControl.Up());
                break;

            case "Down":
                Grid.SetRow(FigurePlayer, playerControl.Down());
                break;

            case "Left":
                Grid.SetColumn(FigurePlayer, playerControl.Left());
                break;

            case "Right":
                Grid.SetColumn(FigurePlayer, playerControl.Right());
                break;
        }

        //Удаляем монету если она есть в ячейке, затем увеличиваем счет игрока и уменьшаем значение таймера.
        var removedCoin = PlayingField.Children.Cast<UIElement>()
                    .FirstOrDefault(e => Grid.GetRow(e) == Grid.GetRow(FigurePlayer) &&
                                         Grid.GetColumn(e) == Grid.GetColumn(FigurePlayer) &&
                                         e != FigurePlayer);

        if (removedCoin != null)
        {
            PlayingField.Children.Remove(removedCoin);
            Score.Text = (++counterCoins).ToString();

            Timer.Value += 5;

            //Если Все монеты собраны - повторно заполняем игровое поле новыми монетами.
            if (counterCoins % 10 == 0)
            {
                RandCoin();
            }
        }
    }

    private void CloseGame(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void RestartGame(object sender, RoutedEventArgs e)
    {
        Restart();
    }
}