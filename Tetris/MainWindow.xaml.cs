using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Tetris
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative))
        };

        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative)),

            new BitmapImage(new Uri("Assets/FBlock_P.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/IBlock_P.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/LBlock_P.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/NBlock_P.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/PBlock_P.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TBlock_P.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/UBlock_P.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/VBlock_P.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/WBlock_P.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/XBlock_P.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/YBlock_P.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/ZBlock_P.png", UriKind.Relative))
        };

        private readonly Image[,] imageControls;

        private bool isPaused = false;

        private bool isGameLoopRunning = false;

        private readonly string bestScoreFile = "bestscore.txt";

        private Level level = new Level();
        private GameState gameState;
        
        public MainWindow()
        {
            InitializeComponent();

            gameState = new GameState(level);
            imageControls = SetupGameCanvas(gameState.GameGrid);
            
        }

        private Image[,] SetupGameCanvas(GameGrid grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            int cellSize = 25;

            for (int i = 0; i < grid.Rows; i++)
            {
                for (int j = 0; j < grid.Columns; j++)
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize
                    };

                    Canvas.SetTop(imageControl, (i - 2) * cellSize);
                    Canvas.SetLeft(imageControl, j * cellSize);
                    GameCanvas.Children.Add(imageControl);
                    imageControls[i, j] = imageControl;
                }
            }

            return imageControls;
        }

        private void DrawGrid(GameGrid grid)
        {
            for (int i = 0; i < grid.Rows; i++)
            {
                for (int j = 0; j < grid.Columns; j++)
                {
                    int id = grid[i, j];
                    imageControls[i, j].Source = tileImages[id];
                }
            }
        }

        private void DrawBlock(Block block)
        {
            foreach (Position p in block.TilePositions())
            {
                imageControls[p.Row, p.Column].Source = tileImages[block.Id];
            }
        }

        private void DrawNextBlock(BlockQueue blockQueue)
        {
            Block next = blockQueue.NextBlock;
            NextImage.Source = blockImages[next.PreviewId];
        }

        private void Draw(GameState gameState)
        {
            DrawGrid(gameState.GameGrid);
            DrawBlock(gameState.CurrentBlock);
            DrawNextBlock(gameState.BlockQueue);
            ScoreText.Text = gameState.Score.ToString();
            BestScoreText.Text = gameState.GetBestScore(gameState.Score, bestScoreFile).ToString();
            CurrentLevel.Text = $"Level {level.CurrentLevel.ToString()}";
        }

        private async Task GameLoop()
        {
            if (isGameLoopRunning)
                return;

            isGameLoopRunning = true;
            Draw(gameState);
            
            while(!gameState.GameOver)
            {
                if (isPaused)
                {
                    await Task.Delay(100);
                    continue;
                }

                int delay = gameState.GetDelay(level.CurrentLevel);
                await Task.Delay(delay);
                gameState.MoveBlockDown();
                Draw(gameState);
            }

            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Score: {gameState.Score}";

            isGameLoopRunning = false;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    gameState.MoveBlockLeft();
                    break;
                case Key.Right:
                    gameState.MoveBlockRight();
                    break;
                case Key.Down:
                    gameState.MoveBlockDown();
                    break;
                case Key.Up:
                    gameState.RotateBlockCW();
                    break;
                case Key.Enter:
                    gameState.DropBlock();
                    break;
                default:
                    return;
            }

            Draw(gameState);
        }

        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }

        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            level.CurrentLevel = 1;
            gameState = new GameState(level);
            GameOverMenu.Visibility = Visibility.Hidden;

            isPaused = false;
            SettingsMenu.Visibility = Visibility.Hidden;
            await GameLoop();
        }

        private void OpenSettings_Click(object sender, RoutedEventArgs e)
        {
            isPaused = true;
            SettingsMenu.Visibility = Visibility.Visible;
        }

        private void CloseSettings_Click(object sender, RoutedEventArgs e)
        {
            isPaused = false;
            SettingsMenu.Visibility = Visibility.Hidden;
        }

        private async void SelectLevel_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                var tagValue = element.Tag?.ToString();
                if (int.TryParse(tagValue, out int selectedLevel))
                {
                    level.CurrentLevel = selectedLevel;
                }
            }

            isPaused = false;
            SettingsMenu.Visibility = Visibility.Hidden;

            gameState = new GameState(level);
            await GameLoop();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
