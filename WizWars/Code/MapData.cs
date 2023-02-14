using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace WizWars
{
    enum TileType
    {
        V,
        F,
        W,
        B
    }

    class Map
    {
        private const int TILESIZE = 48;

        Map1 map;

        private int[,] m_baseLayout;
        private int[,] m_blockLayout;
        private readonly int[,] m_initialBlockLayout;
        private readonly int m_width;
        private readonly int m_height;
        private readonly Rectangle[] m_wallRects;
        private Rectangle[] m_blockRects;
        private Rectangle m_screenBounds;
        private Point[] m_playerStarts;
        private bool[] m_blockRemoved;

        public Rectangle[] WallRectangles
        {
            get => m_wallRects;
        }

        public Rectangle[] BlockRectangles
        {
            get => m_blockRects;
        }

        public int[,] BaseLayout
        {
            get => m_baseLayout;
        }
        public int[,] BlockLayout
        {
            get => m_blockLayout;
        }

        public Point[] PlayerStarts
        {
            get => m_playerStarts;
        }

        public Map(Rectangle screenBounds)
        {
            map = new Map1();

            int m_wallRectCount = 0;
            int m_blockRectCount = 0;

            m_playerStarts = map.PlayerStarts;

            m_width = map.Base.GetLength(0);
            m_height = map.Base.GetLength(1);
            m_screenBounds = screenBounds;

            m_baseLayout = new int[m_width, m_height];
            m_blockLayout = new int[m_width, m_height];

            for (int x = 0; x < m_width; x++)
                for (int y = 0; y < m_height; y++)
                {
                    m_baseLayout[x, y] = map.Base[y, x];
                    m_blockLayout[x, y] = map.Blocks[y, x];
                    if (m_baseLayout[x, y] == (int)TileType.W)
                        m_wallRectCount += 1;
                    if (m_blockLayout[x, y] == (int)TileType.B)
                        m_blockRectCount += 1;
                }

            m_initialBlockLayout = map.Blocks;

            m_wallRects = new Rectangle[m_wallRectCount];
            m_blockRects = new Rectangle[m_blockRectCount];
            m_blockRemoved = new bool[m_blockRectCount];

            m_wallRectCount = 0;
            m_blockRectCount = 0;

            //Creating all wall and block hit boxes
            for (int y = 0; y < m_width; y++)
                for (int x = 0; x < m_height; x++)
                {
                    if (m_baseLayout[x, y] == (int)TileType.W)
                    {
                        m_wallRects[m_wallRectCount] = new Rectangle
                            (x * TILESIZE + (screenBounds.Width - TILESIZE * m_baseLayout.GetLength(0)) / 2,
                            y * TILESIZE + (screenBounds.Height - TILESIZE * m_baseLayout.GetLength(1)) / 2, 48, 48);
                        m_wallRectCount += 1;
                    }
                    if (m_blockLayout[y, x] == (int)TileType.B)
                    {
                        m_blockRects[m_blockRectCount] = new Rectangle
                            (x * TILESIZE + (screenBounds.Width - TILESIZE * m_baseLayout.GetLength(0)) / 2,
                            y * TILESIZE + (screenBounds.Height - TILESIZE * m_baseLayout.GetLength(1)) / 2, 48, 48);
                        m_blockRectCount += 1;
                    }
                }
        }

        public void BlockRemoveTexture(int i)
        {
            //Removes the texture of a block after being destroyed

            m_blockRects[i] = new Rectangle(-1, -1, 0, 0);

            int m_blockCounter = 0;

            for (int y = 0; y < m_width; y++)
                for (int x = 0; x < m_height; x++)
                    if (m_initialBlockLayout[x, y] == (int)TileType.B)
                    {
                        if (m_blockCounter == i)
                        {
                            m_blockLayout[x, y] = (int)TileType.V;
                            m_blockRemoved[i] = true;
                            m_blockCounter += 1;
                        }
                        if (!m_blockRemoved[i])
                            m_blockCounter += 1;
                    }
        }

        public void GameEndReset()
        {
            int m_wallRectCount = 0;
            int m_blockRectCount = 0;

            m_playerStarts = map.PlayerStarts;

            m_baseLayout = new int[m_width, m_height];
            m_blockLayout = new int[m_width, m_height];

            for (int x = 0; x < m_width; x++)
                for (int y = 0; y < m_height; y++)
                {
                    m_baseLayout[x, y] = map.Base[y, x];
                    m_blockLayout[x, y] = map.Blocks[y, x];
                    if (m_baseLayout[x, y] == (int)TileType.W)
                        m_wallRectCount += 1;
                    if (m_blockLayout[x, y] == (int)TileType.B)
                        m_blockRectCount += 1;
                }

            m_blockRects = new Rectangle[m_blockRectCount];
            m_blockRemoved = new bool[m_blockRectCount];

            m_wallRectCount = 0;
            m_blockRectCount = 0;

            for (int y = 0; y < m_width; y++)
                for (int x = 0; x < m_height; x++)
                {
                    if (m_baseLayout[x, y] == (int)TileType.W)
                    {
                        m_wallRects[m_wallRectCount] = new Rectangle
                            (x * TILESIZE + (m_screenBounds.Width - TILESIZE * m_baseLayout.GetLength(0)) / 2,
                            y * TILESIZE + (m_screenBounds.Height - TILESIZE * m_baseLayout.GetLength(1)) / 2, 48, 48);
                        m_wallRectCount += 1;
                    }
                    if (m_blockLayout[y, x] == (int)TileType.B)
                    {
                        m_blockRects[m_blockRectCount] = new Rectangle
                            (x * TILESIZE + (m_screenBounds.Width - TILESIZE * m_baseLayout.GetLength(0)) / 2,
                            y * TILESIZE + (m_screenBounds.Height - TILESIZE * m_baseLayout.GetLength(1)) / 2, 48, 48);
                        m_blockRectCount += 1;
                    }
                }
        }

        public void Draw(SpriteBatch sb, List<Texture2D> tiles)
        {
            for (int x = 0; x < m_width; x++)
                for (int y = 0; y < m_height; y++)
                {
                    sb.Draw(tiles[m_baseLayout[x, y]],
                        new Vector2(x * tiles[0].Width + (m_screenBounds.Width - TILESIZE * m_baseLayout.GetLength(0)) / 2,
                        y * tiles[0].Height + (m_screenBounds.Height - TILESIZE * m_baseLayout.GetLength(1)) / 2), Color.White);

                    sb.Draw(tiles[m_blockLayout[x, y]],
                        new Vector2(x * tiles[0].Width + (m_screenBounds.Width - TILESIZE * m_blockLayout.GetLength(0)) / 2,
                        y * tiles[0].Height + (m_screenBounds.Height - TILESIZE * m_blockLayout.GetLength(1)) / 2), Color.White);
                }
        }

#if DEBUG
        public void DrawDebug(SpriteBatch sb, Texture2D pixel, SpriteFont debugFont, List<Texture2D> tiles)
        {
            for (int x = 0; x < m_width; x++)
                for (int y = 0; y < m_height; y++)
                {
                    sb.DrawString(debugFont, "[" + x + ", " + y + "]", new Vector2
                        (x * tiles[0].Width + (m_screenBounds.Width - TILESIZE * m_blockLayout.GetLength(0)) / 2,
                        y * tiles[0].Height + (m_screenBounds.Height - TILESIZE * m_blockLayout.GetLength(1)) / 2),
                        Color.White);
                }
            for (int i = 0; i < m_blockRects.Length; i++)
            {
                if (!m_blockRemoved[i])
                {
                    sb.Draw(pixel, m_blockRects[i], Color.Green * 0.5f);
                    sb.DrawString(debugFont, "\n{" + i + "}", new Vector2(m_blockRects[i].X, m_blockRects[i].Y), Color.White);
                }
            }
        }
#endif
    }

    class Map1
    {
        private const int V = (int)TileType.V;
        private const int F = (int)TileType.F;
        private const int W = (int)TileType.W;
        private const int B = (int)TileType.B;

        public Point[] PlayerStarts
        {
            get;
            private set;
        }

        public int[,] Base
        {
            get;
            private set;
        }
        public int[,] Blocks
        {
            get;
            private set;
        }

        public Map1()
        {
            PlayerStarts = new Point[4];
            PlayerStarts[0] = new Point(1, 1);
            PlayerStarts[1] = new Point(11, 11);
            PlayerStarts[2] = new Point(11, 1);
            PlayerStarts[3] = new Point(1, 11);

            Base = new int[13, 13]
            {
                { W, W, W, W, W, W, W, W, W, W, W, W, W},
                { W, F, F, F, F, F, F, F, F, F, F, F, W},
                { W, F, W, F, W, F, W, F, W, F, W, F, W},
                { W, F, F, F, F, F, F, F, F, F, F, F, W},
                { W, F, W, F, W, F, W, F, W, F, W, F, W},
                { W, F, F, F, F, F, F, F, F, F, F, F, W},
                { W, F, W, F, W, F, W, F, W, F, W, F, W},
                { W, F, F, F, F, F, F, F, F, F, F, F, W},
                { W, F, W, F, W, F, W, F, W, F, W, F, W},
                { W, F, F, F, F, F, F, F, F, F, F, F, W},
                { W, F, W, F, W, F, W, F, W, F, W, F, W},
                { W, F, F, F, F, F, F, F, F, F, F, F, W},
                { W, W, W, W, W, W, W, W, W, W, W, W, W},
            };

            Blocks = new int[13, 13]
            {
                { V, V, V, V, V, V, V, V, V, V, V, V, V},
                { V, V, V, B, B, B, B, B, B, B, V, V, V},
                { V, V, V, B, V, B, V, B, V, B, V, V, V},
                { V, B, B, V, B, V, B, V, B, V, B, B, V},
                { V, B, V, B, V, B, V, B, V, B, V, B, V},
                { V, B, B, V, B, V, B, V, B, V, B, B, V},
                { V, B, V, B, V, B, V, B, V, B, V, B, V},
                { V, B, B, V, B, V, B, V, B, V, B, B, V},
                { V, B, V, B, V, B, V, B, V, B, V, B, V},
                { V, B, B, V, B, V, B, V, B, V, B, B, V},
                { V, V, V, B, V, B, V, B, V, B, V, V, V},
                { V, V, V, B, B, B, B, B, B, B, V, V, V},
                { V, V, V, V, V, V, V, V, V, V, V, V, V},
            };
        }


    }
}
