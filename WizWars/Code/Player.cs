using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace WizWars
{
    class Player : HitBoxObjectAnimatedWithEdges
    {
        #region Constants
        private const int INITIALPLAYERSPEED = 3;
        private const int INITIALRANGE = 3;
        private const int INITIALCAULDRONCAP = 2;
        private const int MAPSIZE = 13;
        private const int MAXSHIELD = 2;
        private const int INVTIME = 90;
        private const int MAXFREEZECHECK = 5000;
        #endregion

        private readonly Point m_startingPosition;
        private int m_playerSpeed;

        private int m_invCounter;

        private readonly int m_frameSize;

        public Rectangle StandingArea
        {
            get => m_hitBox;
        }

        public bool Dead
        {
            get;
            private set;
        }

        public bool Invulnerable
        {
            get;
            private set;
        }

        public int Shield
        {
            get;
            private set;
        }

        public int BombRange
        {
            get;
            private set;
        }

        public int BombRangeLevel
        {
            get;
            set;
        }

        public int CauldronCap
        {
            get;
            private set;
        }

        public int CauldronCapLevel
        {
            get;
            set;
        }

        public int Speed
        {
            get;
            set;
        }

        public Player(Texture2D texture, Point position, int frameSize, int frameCount, int time) : base(texture, position, frameSize, frameCount, time)
        {
            m_hitBox = new Rectangle(position.X + 4, position.Y + 4, 40, 40);

            m_edgeHitBoxes[0] = new Rectangle(position.X, position.Y - 3, 40, 3);
            m_edgeHitBoxes[1] = new Rectangle(position.X + 40, position.Y, 3, 40);
            m_edgeHitBoxes[2] = new Rectangle(position.X, position.Y + 40, 40, 3);
            m_edgeHitBoxes[3] = new Rectangle(position.X - 3, position.Y, 3, 40);

            m_startingPosition = position;

            m_playerSpeed = INITIALPLAYERSPEED;
            BombRange = INITIALRANGE;
            CauldronCap = INITIALCAULDRONCAP;

            Shield = MAXSHIELD;

            m_frameSize = frameSize;
        }

        public void Update(GameTime gt, Input current, Rectangle[] wallRects, Rectangle[] blockRects, List<Bomb> bombs, Player[] players, int currentPlayers, int playerNum, int xAccounter, int yAccounter)
        {
            #region Powers
            //Updates attributes based on powerups
            if (Speed > 2)
            {
                Speed = 2;
            }
            if (BombRangeLevel > 2)
            {
                BombRangeLevel = 2;
            }
            if (CauldronCapLevel > 2)
            {
                CauldronCapLevel = 2;
            }

            if (Speed == 1)
            {
                m_playerSpeed = 4;
            }
            if (Speed == 2)
            {
                m_playerSpeed = 6;
            }

            if (BombRangeLevel == 1)
            {
                BombRange = 4;
            }
            if (BombRangeLevel == 2)
            {
                BombRange = 5;
            }

            if (CauldronCapLevel == 1)
            {
                CauldronCap = 3;
            }
            if (CauldronCapLevel == 2)
            {
                CauldronCap = 4;
            }
            #endregion

            if (!Dead)
            {
                //Moves the player
                if (current.Up[playerNum])
                    m_position.Y -= m_playerSpeed;
                if (current.Right[playerNum])
                    m_position.X += m_playerSpeed;
                if (current.Down[playerNum])
                    m_position.Y += m_playerSpeed;
                if (current.Left[playerNum])
                    m_position.X -= m_playerSpeed;
            }

            UpdateHitBox();

            if (Invulnerable)
                m_invCounter += 1;
            if (m_invCounter >= INVTIME)
            {
                Invulnerable = false;
                m_invCounter = 0;
            }

            if (m_position.X < xAccounter || m_position.X > xAccounter + (m_frameSize * MAPSIZE) || m_position.Y < yAccounter || m_position.Y > yAccounter + (m_frameSize * MAPSIZE))
                m_position = m_startingPosition;

            #region Collision
            //Prevents Player from walking through walls, blocks, cauldrons and other players
            if (!Dead)
            {
                int freezeCheck = 0;

                for (int i = 0; i < wallRects.Length; i++)
                {
                    while (m_edgeHitBoxes[0].Intersects(wallRects[i]) || m_edgeHitBoxes[1].Intersects(wallRects[i]) || m_edgeHitBoxes[2].Intersects(wallRects[i]) || m_edgeHitBoxes[3].Intersects(wallRects[i]))
                    {
                        if (m_edgeHitBoxes[0].Intersects(wallRects[i]))
                        {
                            m_position.Y += 1;
                            UpdateHitBox();
                        }
                        if (m_edgeHitBoxes[1].Intersects(wallRects[i]))
                        {
                            m_position.X -= 1;
                            UpdateHitBox();
                        }
                        if (m_edgeHitBoxes[2].Intersects(wallRects[i]))
                        {
                            m_position.Y -= 1;
                            UpdateHitBox();
                        }
                        if (m_edgeHitBoxes[3].Intersects(wallRects[i]))
                        {
                            m_position.X += 1;
                            UpdateHitBox();
                        }

                        freezeCheck += 1;

                        if (freezeCheck > MAXFREEZECHECK)
                            break;
                    }
                }

                for (int i = 0; i < blockRects.Length; i++)
                {
                    while (m_edgeHitBoxes[0].Intersects(blockRects[i]) || m_edgeHitBoxes[1].Intersects(blockRects[i]) || m_edgeHitBoxes[2].Intersects(blockRects[i]) || m_edgeHitBoxes[3].Intersects(blockRects[i]))
                    {
                        if (m_edgeHitBoxes[0].Intersects(blockRects[i]))
                        {
                            m_position.Y += 1;
                            UpdateHitBox();
                        }
                        if (m_edgeHitBoxes[1].Intersects(blockRects[i]))
                        {
                            m_position.X -= 1;
                            UpdateHitBox();
                        }
                        if (m_edgeHitBoxes[2].Intersects(blockRects[i]))
                        {
                            m_position.Y -= 1;
                            UpdateHitBox();
                        }
                        if (m_edgeHitBoxes[3].Intersects(blockRects[i]))
                        {
                            m_position.X += 1;
                            UpdateHitBox();
                        }

                        freezeCheck += 1;

                        if (freezeCheck > MAXFREEZECHECK)
                            break;
                    }
                }

                if (!Invulnerable)
                {
                    for (int i = 0; i < bombs.Count; i++)
                    {
                        if (!bombs[i].DontCollide[playerNum])
                        {
                            while (m_edgeHitBoxes[0].Intersects(bombs[i].HitBox) || m_edgeHitBoxes[1].Intersects(bombs[i].HitBox) || m_edgeHitBoxes[2].Intersects(bombs[i].HitBox) || m_edgeHitBoxes[3].Intersects(bombs[i].HitBox))
                            {
                                if (m_edgeHitBoxes[0].Intersects(bombs[i].HitBox))
                                {
                                    m_position.Y += 1;
                                    UpdateHitBox();
                                }
                                if (m_edgeHitBoxes[1].Intersects(bombs[i].HitBox))
                                {
                                    m_position.X -= 1;
                                    UpdateHitBox();
                                }
                                if (m_edgeHitBoxes[2].Intersects(bombs[i].HitBox))
                                {
                                    m_position.Y -= 1;
                                    UpdateHitBox();
                                }
                                if (m_edgeHitBoxes[3].Intersects(bombs[i].HitBox))
                                {
                                    m_position.X += 1;
                                    UpdateHitBox();
                                }

                                freezeCheck += 1;

                                if (freezeCheck > MAXFREEZECHECK)
                                    break;
                            }
                        }
                    }

                    for (int i = 0; i < currentPlayers; i++)
                    {
                        if (!players[i].Invulnerable && !players[i].Dead)
                        {
                            while (m_edgeHitBoxes[0].Intersects(players[i].StandingArea) || m_edgeHitBoxes[1].Intersects(players[i].StandingArea) || m_edgeHitBoxes[2].Intersects(players[i].StandingArea) || m_edgeHitBoxes[3].Intersects(players[i].StandingArea))
                            {
                                if (m_edgeHitBoxes[0].Intersects(players[i].StandingArea))
                                {
                                    m_position.Y += 1;
                                    UpdateHitBox();
                                }
                                if (m_edgeHitBoxes[1].Intersects(players[i].StandingArea))
                                {
                                    m_position.X -= 1;
                                    UpdateHitBox();
                                }
                                if (m_edgeHitBoxes[2].Intersects(players[i].StandingArea))
                                {
                                    m_position.Y -= 1;
                                    UpdateHitBox();
                                }
                                if (m_edgeHitBoxes[3].Intersects(players[i].StandingArea))
                                {
                                    m_position.X += 1;
                                    UpdateHitBox();
                                }

                                freezeCheck += 1;

                                if (freezeCheck > MAXFREEZECHECK)
                                    break;
                            }
                        }
                    }
                }
            }
            #endregion
        }

        protected override void UpdateHitBox()
        {
            m_hitBox.X = m_position.X;
            m_hitBox.Y = m_position.Y;
            m_edgeHitBoxes[0].X = m_position.X;
            m_edgeHitBoxes[0].Y = m_position.Y - 3;
            m_edgeHitBoxes[1].X = m_position.X + 40;
            m_edgeHitBoxes[1].Y = m_position.Y;
            m_edgeHitBoxes[2].X = m_position.X;
            m_edgeHitBoxes[2].Y = m_position.Y + 40;
            m_edgeHitBoxes[3].X = m_position.X - 3;
            m_edgeHitBoxes[3].Y = m_position.Y;
        }

        public void Kill(Point resetPoint, int mapSize, Rectangle screenBounds)
        {
            Dead = true;
            m_position = m_startingPosition;
        }

        public void LoseShield()
        {
            Shield -= 1;
            m_position = m_startingPosition;
            Invulnerable = true;
        }

        public override void Draw(SpriteBatch sb)
        {
            if (!Dead)
            {
                if (Invulnerable)
                    sb.Draw(m_texture, new Vector2(m_position.X, m_position.Y), null, Color.White * 0.5f, 0, new Vector2(4, 26), 1, SpriteEffects.None, 1);
                else
                    sb.Draw(m_texture, new Vector2(m_position.X, m_position.Y), null, Color.White, 0, new Vector2(4, 26), 1, SpriteEffects.None, 1);
            }
        }

        public void GameEndReset()
        {
            m_position = m_startingPosition;
            m_invCounter = 0;

            m_playerSpeed = INITIALPLAYERSPEED;
            BombRange = INITIALRANGE;
            CauldronCap = INITIALCAULDRONCAP;

            Dead = false;
            Invulnerable = false;
            Shield = MAXSHIELD;
            BombRange = INITIALRANGE;
            BombRangeLevel = 0;
            CauldronCap = INITIALCAULDRONCAP;
            CauldronCapLevel = 0;
            Speed = 0;
        }

#if DEBUG
        public string DebugString()
        {
            return Dead.ToString()
                + "\n" + Shield;
        }

        public void DrawDebug(SpriteBatch sb, Texture2D pixel)
        {
            sb.Draw(pixel, StandingArea, Color.Green * 0.5f);

            for (int i = 0; i < m_edgeHitBoxes.Length; i++)
            {
                sb.Draw(pixel, m_edgeHitBoxes[i], Color.Blue * 0.5f);
            }
        }
#endif
    }
}
