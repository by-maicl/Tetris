using System.IO;

namespace Tetris
{
    public class GameState
    {
        private Block currentBlock;

        public Block CurrentBlock
        {
            get => currentBlock;
            private set
            {
                currentBlock = value;
                currentBlock.Reset();

                for (int i = 0; i < 2; i++)
                {
                    currentBlock.Move(1, 0);

                    if (!BlockFits())
                    {
                        currentBlock.Move(-1, 0);
                    }
                }
            }
        }

        public GameGrid GameGrid { get; }
        public BlockQueue BlockQueue { get; }
        public bool GameOver { get; private set; }
        public int Score { get; private set; }
        public static int BestScore { get; private set; }
        public Level Level { get; private set; }

        public GameState(Level level)
        {
            GameGrid = new GameGrid(22, 10);
            Level = level;
            BlockQueue = new BlockQueue(level);
            CurrentBlock = BlockQueue.GetAndUpdate();
        }

        private bool BlockFits()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                if (!GameGrid.IsEmpty(p.Row, p.Column))
                {
                    return false;
                }
            }

            return true;
        }

        public void RotateBlockCW()
        {
            CurrentBlock.RotateCW();

            if (!BlockFits())
            {
                CurrentBlock.RotateCCW();
            }
        }

        public void MoveBlockLeft()
        {
            CurrentBlock.Move(0, -1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, 1);
            }
        }

        public void MoveBlockRight()
        {
            CurrentBlock.Move(0, 1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, -1);
            }
        }

        private bool IsGameOver()
        {
            return !(GameGrid.IsRowEmpty(0) && GameGrid.IsRowEmpty(1));
        }

        private void PlaceBlock()
        {
            foreach (Position p in CurrentBlock.TilePositions())
            {
                GameGrid[p.Row, p.Column] = CurrentBlock.Id;
            }

            GetScore(GameGrid.ClearFullRows());

            if (IsGameOver())
            {
                GameOver = true;
            } else
            {
                CurrentBlock = BlockQueue.GetAndUpdate();
            }
        }

        public void MoveBlockDown()
        {
            CurrentBlock.Move(1, 0);

            if (!BlockFits())
            {
                CurrentBlock.Move(-1, 0);
                PlaceBlock();
            }
        }

        private int TileDropDistance(Position p)
        {
            int drop = 0;

            while (GameGrid.IsEmpty(p.Row + drop + 1, p.Column))
            {
                drop++;
            }

            return drop;
        }

        public int BlockDropDistance()
        {
            int drop = GameGrid.Rows;

            foreach (Position p in CurrentBlock.TilePositions())
            {
                drop = System.Math.Min(drop, TileDropDistance(p));
            }

            return drop;
        }

        public void DropBlock()
        {
            CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock();
        }

        public void GetScore(int numRows)
        {
            switch (numRows) {
                case 1:
                    Score += 100;
                    break;
                case 2: 
                    Score += 300;
                    break;
                case 3:
                    Score += 700;
                    break;
                case 4:
                    Score += 1500;
                    break;
                case 5:
                    Score += 2500;
                    break;
                default:
                    break;
            }

            int oldLevel = Level.CurrentLevel;

            if (Level.UpdateLevel(Score) && Level.CurrentLevel != oldLevel)
            {
                GameGrid.Clear();
            }
        }

        public int GetBestScore(int score, string fileName)
        {
            GetBestScoreFromFile(fileName);

            if (score > BestScore)
            {
                BestScore = score;
                WriteBestScoreToFile(fileName, BestScore);
            }

            return BestScore;
        }

        public int GetDelay(int level)
        {
            int delay = 500;
            if (level == 2)
            {
                delay -= 50;
            } else if (level == 3)
            {
                delay -= 100;
            }

            return delay;
        }

        private void GetBestScoreFromFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                BestScore = 0;
                File.WriteAllText(fileName, BestScore.ToString());
            }

            BestScore = int.Parse(File.ReadAllText(fileName));
        }
        
        private void WriteBestScoreToFile(string fileName, int bestScore)
        {
            File.WriteAllText(fileName, bestScore.ToString());
        }
    }
}
