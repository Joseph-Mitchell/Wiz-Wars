using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WizWars
{
    enum PlayerColour
    {
        Purple,
        Orange,
        Cyan,
        Grey
    }

    class Bomb : HitBoxObjectAnimatedWithEdges
    {
        public Point Position
        {
            get => m_position;
        }

        public Rectangle HitBox
        {
            get => m_hitBox;
        }

        public PlayerColour Colour
        {
            get;
            private set;
        }

        public int FuseTimer
        {
            get;
            set;
        }

        public int Range
        {
            get;
            private set;
        }

        public bool RemoveThis
        {
            get;
            private set;
        }

        public bool[] ExplosionCanSpawn
        {
            get;
            private set;
        }

        public bool[] DontCollide
        {
            get;
            set;
        }

        public Bomb(Texture2D texture, Point position, int frameSize, int frameCount, int time, int player, int range) : base (texture, position, frameSize, frameCount, time)
        {
            m_time = 10;

            ExplosionCanSpawn = new bool[17];
            for (int i = 0; i < ExplosionCanSpawn.Length; i++)
                ExplosionCanSpawn[i] = true;

            DontCollide = new bool[4];
            for (int i = 0; i < DontCollide.Length; i++)
                DontCollide[i] = true;

            FuseTimer = 0;

            RemoveThis = false;

            Colour = (PlayerColour)player;

            Range = range;
        }

        public void Update(int tileSize, int xAccounter, int yAccounter, int[,] wallMap, int[,] blockMap, int maxFuseTime)
        {

            //Move to center of closest tile (xAccounter and yAccounter account for the distance from screen edge to map edge)
            if ((m_position.X - xAccounter) % tileSize < tileSize / 2)
                m_position.X -= (m_position.X - xAccounter) % tileSize;
            else if ((m_position.X - xAccounter) % tileSize >= tileSize / 2)
                m_position.X += tileSize - ((m_position.X - xAccounter) % tileSize);
            if ((m_position.Y - yAccounter) % tileSize < tileSize / 2)
                m_position.Y -= (m_position.Y - yAccounter) % tileSize;
            else if ((m_position.Y - yAccounter) % tileSize >= tileSize / 2)
                m_position.Y += tileSize - (m_position.Y - yAccounter) % tileSize;


            if (FuseTimer < maxFuseTime)
                FuseTimer += 1;
            else
            {
                RemoveThis = true;

                #region Explosion Check Setup
                int tileX = (m_position.X - xAccounter) / tileSize;
                int tileY = (m_position.Y - yAccounter) / tileSize;

                Point[] surroundingTiles = new Point[17];
                surroundingTiles[0] = new Point(tileX, tileY);
                surroundingTiles[1] = new Point(tileX, tileY - 1);
                surroundingTiles[2] = new Point(tileX + 1, tileY);
                surroundingTiles[3] = new Point(tileX, tileY + 1);
                surroundingTiles[4] = new Point(tileX - 1, tileY);
                surroundingTiles[5] = new Point(tileX, tileY - 2);
                surroundingTiles[6] = new Point(tileX + 2, tileY);
                surroundingTiles[7] = new Point(tileX, tileY + 2);
                surroundingTiles[8] = new Point(tileX - 2, tileY);
                surroundingTiles[9] = new Point(tileX, tileY - 3);
                surroundingTiles[10] = new Point(tileX + 3, tileY);
                surroundingTiles[11] = new Point(tileX, tileY + 3);
                surroundingTiles[12] = new Point(tileX - 3, tileY);
                surroundingTiles[13] = new Point(tileX, tileY - 4);
                surroundingTiles[14] = new Point(tileX + 4, tileY);
                surroundingTiles[15] = new Point(tileX, tileY + 4);
                surroundingTiles[16] = new Point(tileX - 4, tileY);
                #endregion

                //Checks which tiles can spawn explosions
                for (int i = 0; i < surroundingTiles.Length; i++)
                {
                    if (i != 0)
                    {
                        if (i > 4)
                        {
                            if (ExplosionCanSpawn[i - 4])
                            {
                                if (blockMap[surroundingTiles[i - 4].X, surroundingTiles[i - 4].Y] == (int)TileType.B || wallMap[surroundingTiles[i].X, surroundingTiles[i].Y] == (int)TileType.W)
                                    ExplosionCanSpawn[i] = false;
                            }
                            else
                                ExplosionCanSpawn[i] = false;
                        }
                        else
                        {
                            if (wallMap[surroundingTiles[i].X, surroundingTiles[i].Y] == (int)TileType.W)
                                ExplosionCanSpawn[i] = false;
                        }
                    }
                }
            }

            base.Update();
        }

#if DEBUG
        public void DrawDebug(SpriteBatch sb, Texture2D pixel, SpriteFont debugFont)
        {
            for (int i = 0; i < m_edgeHitBoxes.Length; i++)
                sb.Draw(pixel, m_edgeHitBoxes[i], Color.White);
        }
#endif
    }

    class Explosion : HitBoxObjectAnimated
    {
        private const int LIFETIME = 60;

        private int lifeTimer;

        public Rectangle HitBox
        {
            get => m_hitBox;
        }

        public bool RemoveThis
        {
            get;
            private set;
        }

        public Explosion(Texture2D texture, Point position, int frameSize, int frameCount, int time):base(texture, position, frameSize, frameCount, time)
        {
            lifeTimer = 0;

            m_time = LIFETIME / frameCount;
        }

        public override void Update()
        {
            if (lifeTimer < LIFETIME)
                lifeTimer += 1;
            else
                RemoveThis = true;

            base.Update();
        }

#if DEBUG
        public void DrawDebug(SpriteBatch sb, Texture2D pixel)
        {
            sb.Draw(pixel, m_hitBox, Color.Purple * 0.5f);
        }
#endif
    }
}
