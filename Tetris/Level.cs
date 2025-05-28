namespace Tetris
{
    public class Level
    {
        public int CurrentLevel;

        public Level()
        {
            CurrentLevel = 1;
        }

        public bool UpdateLevel(int score)
        {
            if (score >= 1500)
            {
                CurrentLevel = 2;
                return true;
            } else if (score >= 3000)
            {
                CurrentLevel = 3;
                return true;
            }
            return false;
        }
    }
}
