using System;

namespace Tetris
{
    public class BlockQueue
    {
        private readonly Block[] blocks = new Block[]
        {
            new Tetromino.IBlock(),
            new Tetromino.JBlock(),
            new Tetromino.LBlock(),
            new Tetromino.OBlock(),
            new Tetromino.SBlock(),
            new Tetromino.TBlock(),
            new Tetromino.ZBlock(),

            new Pentomino.FBlock_P(),
            new Pentomino.IBlock_P(),
            new Pentomino.LBlock_P(),
            new Pentomino.NBlock_P(),
            new Pentomino.PBlock_P(),
            new Pentomino.TBlock_P(),
            new Pentomino.UBlock_P(),
            new Pentomino.VBlock_P(),
            new Pentomino.WBlock_P(),
            new Pentomino.XBlock_P(),
            new Pentomino.YBlock_P(),
            new Pentomino.ZBlock_P()
        };

        private readonly Random random = new Random();
        private Level level = new Level();

        public Block NextBlock { get; private set; }

        public BlockQueue(Level level)
        {
            this.level = level;
            NextBlock = RandomBlock();
        }

        private Block RandomBlock()
        {
            int from = 0, to = 0;

            switch (level.CurrentLevel)
            {
                case 1:
                    to = 7;
                    break;
                case 2:
                    from = 8;
                    to = blocks.Length;
                    break;
                case 3:
                    to = blocks.Length;
                    break;
                default:
                    break;
            }

            return blocks[random.Next(from, to)];
        }

        public Block GetAndUpdate()
        {
            Block block = NextBlock;

            do
            {
                NextBlock = RandomBlock();
            }
            while (block.Id == NextBlock.Id);

            return block;
        }
    }
}
