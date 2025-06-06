﻿namespace Tetris.Pentomino
{
    public class YBlock_P : Block
    {
        private readonly Position[][] tiles = new Position[][]
        {
            new Position[] { new(0,1), new(1,1), new(1,0), new(2,1), new(3,1) },
            new Position[] { new(1,0), new(1,1), new(1,2), new(1,3), new(0,2) },
            new Position[] { new(0,0), new(1,0), new(2,0), new(3,0), new(2,1) },
            new Position[] { new(0,0), new(0,1), new(0,2), new(0,3), new(1,1) }
        };

        public override int Id => 4;
        public override int PreviewId => 18;
        protected override Position StartOffset => new Position(0, 3);
        protected override Position[][] Tiles => tiles;
    }
}
