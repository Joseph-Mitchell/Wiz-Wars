using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

#if DEBUG
using Microsoft.Xna.Framework.Input;
#endif

namespace WizWars
{
    enum GameState
    {
        Menu,
        Instructions,
        ChoosePlayers,
        Playing
    }

    public class Game1 : Game
    {
        #region Constants
        const int TILESIZE = 48;
        const int MAPSIZE = 13;
        const int MAXPLAYERS = 4;

        private const int CAULDRONMAXFUSETIME = 120;

        const int EXPLOSIONFRAMECOUNT = 12;

        const int CAULDROMFRAMECOUNT = 12;
        #endregion

        GameState gameState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

#if DEBUG
        SpriteFont debugFont;
        MouseState mouse;
#endif

        Random RNG;
        Rectangle screenBounds;
        Texture2D pixel;

        Input input, inputOld;

        StaticGraphic background;

        #region Menu
        StaticGraphic title, menuControls;
        Texture2D[] menuButtonTextures;
        MainMenu mainMenu;
        #endregion
        #region Instructions
        StaticGraphic instructionPage;
        #endregion
        #region Choose Players
        StaticGraphic playerPage;
        Texture2D[] playerButtonTextures;
        PlayerMenu playerMenu;
        #endregion
        #region In Game
        SoundEffect cauldronBubble;
        SoundEffect[] explosionSound;
        SoundEffectInstance[] explosionSoundInstance;
        Texture2D[] cauldronTxtr, explosionTxtr0, powerUpTextures, playerSprites;
        List<Texture2D> tiles;

        bool gameOver;
        int currentPlayers, xAccounter, yAccounter, countdown, timer;
        int[] bombCounter;

        Map map;
        StaticGraphic countdown3, countdown2, countdown1, backButton;
        StaticGraphic[] playerWinGraphics, playerStatusBars;
        Player[] player;
        List<Bomb> bombs;
        List<Explosion> explosions;
        List<Powerup> powerUps;
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            IsMouseVisible = true;
            //graphics.IsFullScreen = true;
        }

        protected override void Initialize()
        {
            gameState = GameState.Menu;

            RNG = new Random();
            screenBounds = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            input = new Input();
            inputOld = new Input();

            #region Menu
            menuButtonTextures = new Texture2D[3];
            #endregion
            #region Choose Players
            playerButtonTextures = new Texture2D[3];
            #endregion
            #region In Game
            explosionSound = new SoundEffect[4];
            explosionSoundInstance = new SoundEffectInstance[4];
            cauldronTxtr = new Texture2D[4];
            explosionTxtr0 = new Texture2D[4];
            powerUpTextures = new Texture2D[3];
            playerSprites = new Texture2D[4];
            playerWinGraphics = new StaticGraphic[5];
            playerStatusBars = new StaticGraphic[4];
            tiles = new List<Texture2D>();

            currentPlayers = 2;
            countdown = 3;
            bombCounter = new int[4];

            map = new Map(screenBounds);
            player = new Player[4];

            bombs = new List<Bomb>();
            explosions = new List<Explosion>();
            powerUps = new List<Powerup>();
            #endregion

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pixel = Content.Load<Texture2D>("Textures\\Other\\Pixel");

#if DEBUG
            debugFont = Content.Load<SpriteFont>("Fonts\\Arial07");
#endif

            background = new StaticGraphic(Content.Load<Texture2D>("Textures\\Other\\Background"), Point.Zero);

            #region Menu
            title = new StaticGraphic(Content.Load<Texture2D>("Textures\\UI\\Menu\\Title"), new Point(screenBounds.Center.X - 130, 60));
            menuControls = new StaticGraphic(Content.Load<Texture2D>("Textures\\UI\\Menu\\MenuControls"), Point.Zero);

            menuButtonTextures[0] = Content.Load<Texture2D>("Textures\\UI\\Menu\\PlayButton");
            menuButtonTextures[1] = Content.Load<Texture2D>("Textures\\UI\\Menu\\InstructionsButton");
            menuButtonTextures[2] = Content.Load<Texture2D>("Textures\\UI\\Menu\\QuitButton");

            mainMenu = new MainMenu(menuButtonTextures, new Point(screenBounds.Center.X, 450));

            #endregion
            #region Instructions
            instructionPage = new StaticGraphic(Content.Load<Texture2D>("Textures\\UI\\Instructions\\InstructionPage"), Point.Zero);
            #endregion
            #region Choose Players
            playerPage = new StaticGraphic(Content.Load<Texture2D>("Textures\\UI\\PlayerChoose\\PlayerChoosePage"), Point.Zero);

            playerButtonTextures[0] = Content.Load<Texture2D>("Textures\\UI\\PlayerChoose\\2Players");
            playerButtonTextures[1] = Content.Load<Texture2D>("Textures\\UI\\PlayerChoose\\3Players");
            playerButtonTextures[2] = Content.Load<Texture2D>("Textures\\UI\\PlayerChoose\\4Players");

            playerMenu = new PlayerMenu(playerButtonTextures, screenBounds.Center);
            #endregion
            #region In Game
            cauldronBubble = Content.Load<SoundEffect>("Sounds\\CauldronBubble");

            explosionSound[0] = Content.Load<SoundEffect>("Sounds\\PurpleExplosion");
            explosionSound[1] = Content.Load<SoundEffect>("Sounds\\OrangeExplosion");
            explosionSound[2] = Content.Load<SoundEffect>("Sounds\\BlueExplosion");
            explosionSound[3] = Content.Load<SoundEffect>("Sounds\\GreyExplosion");

            for (int i = 0; i < explosionSoundInstance.Length; i++)
                explosionSoundInstance[i] = explosionSound[i].CreateInstance();

            for (int i = 0; i < MAXPLAYERS; i++)
            {
                cauldronTxtr[i] = Content.Load<Texture2D>("Textures\\Bomb\\Cauldron" + i);
                explosionTxtr0[i] = Content.Load<Texture2D>("Textures\\Bomb\\Explosion" + i);
            }

            countdown3 = new StaticGraphic(Content.Load<Texture2D>("Textures\\UI\\Countdown\\3"), new Point(screenBounds.Center.X - 93, screenBounds.Center.Y - 200));
            countdown2 = new StaticGraphic(Content.Load<Texture2D>("Textures\\UI\\Countdown\\2"), new Point(screenBounds.Center.X - 108, screenBounds.Center.Y - 200));
            countdown1 = new StaticGraphic(Content.Load<Texture2D>("Textures\\UI\\Countdown\\1"), new Point(screenBounds.Center.X - 48, screenBounds.Center.Y - 200));
            backButton = new StaticGraphic(Content.Load<Texture2D>("Textures\\UI\\GameOver\\BackButton"), Point.Zero);

            powerUpTextures[0] = Content.Load<Texture2D>("Textures\\Powers\\SpeedUp");
            powerUpTextures[1] = Content.Load<Texture2D>("Textures\\Powers\\RangeUp");
            powerUpTextures[2] = Content.Load<Texture2D>("Textures\\Powers\\CauldronCapUp");

            for (int i = 0; i < playerSprites.Length; i++)
                playerSprites[i] = Content.Load<Texture2D>("Textures\\Players\\Wizard" + i);

            playerWinGraphics[0] = new StaticGraphic(Content.Load<Texture2D>("Textures\\UI\\GameOver\\PurpleWins"), new Point(screenBounds.Center.X - 220, screenBounds.Center.Y - 220));
            playerWinGraphics[1] = new StaticGraphic(Content.Load<Texture2D>("Textures\\UI\\GameOver\\OrangeWins"), new Point(screenBounds.Center.X - 220, screenBounds.Center.Y - 220));
            playerWinGraphics[2] = new StaticGraphic(Content.Load<Texture2D>("Textures\\UI\\GameOver\\BlueWins"), new Point(screenBounds.Center.X - 220, screenBounds.Center.Y - 220));
            playerWinGraphics[3] = new StaticGraphic(Content.Load<Texture2D>("Textures\\UI\\GameOver\\GreyWins"), new Point(screenBounds.Center.X - 220, screenBounds.Center.Y - 220));
            playerWinGraphics[4] = new StaticGraphic(Content.Load<Texture2D>("Textures\\UI\\GameOver\\NoOneWins"), new Point(screenBounds.Center.X - 220, screenBounds.Center.Y - 220));

            playerStatusBars[0] = new StaticGraphic(Content.Load<Texture2D>("Textures\\UI\\PlayerStats\\playerBar0"), Point.Zero);
            playerStatusBars[1] = new StaticGraphic(Content.Load<Texture2D>("Textures\\UI\\PlayerStats\\playerBar1"), new Point(screenBounds.Width - 380, screenBounds.Height - 48));
            playerStatusBars[2] = new StaticGraphic(Content.Load<Texture2D>("Textures\\UI\\PlayerStats\\playerBar2"), new Point(screenBounds.Width - 380, 0));
            playerStatusBars[3] = new StaticGraphic(Content.Load<Texture2D>("Textures\\UI\\PlayerStats\\playerBar3"), new Point(0, screenBounds.Height - 48));

            tiles.Add(Content.Load<Texture2D>("Textures\\Tiles\\Blank"));
            tiles.Add(Content.Load<Texture2D>("Textures\\Tiles\\Floor"));
            tiles.Add(Content.Load<Texture2D>("Textures\\Tiles\\Wall"));
            tiles.Add(Content.Load<Texture2D>("Textures\\Tiles\\Block"));

            xAccounter = (screenBounds.Width - (TILESIZE * MAPSIZE)) / 2;
            yAccounter = (screenBounds.Height - (TILESIZE * MAPSIZE)) / 2;

            for (int i = 0; i < MAXPLAYERS; i++)
            {
                player[i] = new Player(playerSprites[i], new Point((map.PlayerStarts[i].X * TILESIZE) + xAccounter + 5, (map.PlayerStarts[i].Y * TILESIZE) + yAccounter + 5), TILESIZE, 1, 0);
            }
            #endregion
        }

        protected override void Update(GameTime gameTime)
        {
            input.Update();

#if DEBUG
            mouse = Mouse.GetState();
#endif
            switch (gameState)
            {
                case GameState.Menu:
                    mainMenu.Update(input, inputOld);

                    if (mainMenu.ExitGame)
                        Exit();
                    if (mainMenu.GoToInstructions)
                    {
                        gameState = GameState.Instructions;
                        mainMenu.GoToInstructions = false;
                    }
                    if (mainMenu.GoToChoosePlayers)
                    {
                        mainMenu.GoToChoosePlayers = false;
                        gameState = GameState.ChoosePlayers;
                    }
                    break;
                case GameState.Instructions:
                    if (input.Back && !inputOld.Back)
                        gameState = GameState.Menu;
                    break;
                case GameState.ChoosePlayers:
                    playerMenu.Update(input, inputOld);

                    if (input.Back && !inputOld.Back)
                        gameState = GameState.Menu;

                    if (playerMenu.Two)
                    {
                        playerMenu.Two = false;
                        currentPlayers = 2;
                        gameState = GameState.Playing;
                    }
                    if (playerMenu.Three)
                    {
                        playerMenu.Three = false;
                        currentPlayers = 3;
                        gameState = GameState.Playing;
                    }
                    if (playerMenu.Four)
                    {
                        playerMenu.Four = false;
                        currentPlayers = 4;
                        gameState = GameState.Playing;
                    }
                    break;
                case GameState.Playing:
                    if (timer < 60)
                        timer++;
                    else
                    {
                        countdown--;
                        timer = 0;
                    }
                    if (countdown <= 0)
                    {
                        if (!gameOver)
                        {
                            #region PlayerUpdate
                            bool[] stoodOnBomb = new bool[currentPlayers];
                            for (int i = 0; i < stoodOnBomb.Length; i++)
                                stoodOnBomb[i] = false;

                            int alive = currentPlayers;

                            for (int i = 0; i < currentPlayers; i++)
                            {
                                player[i].Update(gameTime, input, map.WallRectangles, map.BlockRectangles, bombs, player, currentPlayers, i, xAccounter, yAccounter);

                                for (int j = 0; j < powerUps.Count; j++)
                                    if (player[i].StandingArea.Intersects(powerUps[j].HitBox))
                                    {
                                        if (powerUps[j].PowerType == PowerUpType.Speed)
                                            player[i].Speed += 1;
                                        if (powerUps[j].PowerType == PowerUpType.Range)
                                            player[i].BombRangeLevel += 1;
                                        if (powerUps[j].PowerType == PowerUpType.Cauldron)
                                            player[i].CauldronCapLevel += 1;
                                        powerUps.RemoveAt(j);
                                    }

                                for (int j = 0; j < bombs.Count; j++)
                                    if (player[i].StandingArea.Intersects(bombs[j].HitBox))
                                        stoodOnBomb[i] = true;

                                if (input.Action[i] && !inputOld.Action[i] && bombCounter[i] < player[i].CauldronCap && !player[i].Dead && !stoodOnBomb[i] && !player[i].Invulnerable)
                                {
                                    cauldronBubble.Play();
                                    bombs.Add(new Bomb(cauldronTxtr[i], new Point(player[i].StandingArea.Center.X - 24, player[i].StandingArea.Center.Y - 24), TILESIZE, CAULDROMFRAMECOUNT, 0, i, player[i].BombRange));
                                }

                                if (player[i].Dead)
                                    alive -= 1;

                                if (alive <= 1)
                                    gameOver = true;
                            }

                            for (int i = 0; i < powerUps.Count; i++)
                                powerUps[i].Update();
                            #endregion
                            #region BombUpdate
                            for (int i = 0; i < bombCounter.Length; i++)
                                bombCounter[i] = 0;

                            for (int i = 0; i < bombs.Count; i++)
                            {
                                bombs[i].Update(TILESIZE, (screenBounds.Width - (TILESIZE * MAPSIZE)) / 2, (screenBounds.Height - (TILESIZE * MAPSIZE)) / 2, map.BaseLayout, map.BlockLayout, CAULDRONMAXFUSETIME);

                                //Counts bombs, seperated into each colour
                                for (int j = 0; j < bombCounter.Length; j++)
                                    if (bombs[i].Colour == (PlayerColour)j)
                                        bombCounter[j]++;

                                //Players can phase through bombs once first placed, but cannot after leaving its hit box
                                for (int j = 0; j < currentPlayers; j++)
                                    if (!player[j].StandingArea.Intersects(bombs[i].HitBox))
                                        bombs[i].DontCollide[j] = false;

                                for (int j = 0; j < explosions.Count; j++)
                                    if (bombs[i].HitBox.Intersects(explosions[j].HitBox))
                                        bombs[i].FuseTimer = CAULDRONMAXFUSETIME;

                                if (bombs[i].RemoveThis)
                                {
                                    //Adds an explosion onto all relevant tiles
                                    if (explosionSoundInstance[(int)bombs[i].Colour].State != SoundState.Playing)
                                        explosionSoundInstance[(int)bombs[i].Colour].Play();

                                    if (bombs[i].Range >= 1)
                                        explosions.Add(new Explosion(explosionTxtr0[(int)bombs[i].Colour], new Point(bombs[i].Position.X, bombs[i].Position.Y), TILESIZE, EXPLOSIONFRAMECOUNT, 0));

                                    if (bombs[i].Range >= 2)
                                    {
                                        if (bombs[i].ExplosionCanSpawn[1])
                                            explosions.Add(new Explosion(explosionTxtr0[(int)bombs[i].Colour], new Point(bombs[i].Position.X, bombs[i].Position.Y - TILESIZE), TILESIZE, EXPLOSIONFRAMECOUNT, 0));
                                        if (bombs[i].ExplosionCanSpawn[2])
                                            explosions.Add(new Explosion(explosionTxtr0[(int)bombs[i].Colour], new Point(bombs[i].Position.X + TILESIZE, bombs[i].Position.Y), TILESIZE, EXPLOSIONFRAMECOUNT, 0));
                                        if (bombs[i].ExplosionCanSpawn[3])
                                            explosions.Add(new Explosion(explosionTxtr0[(int)bombs[i].Colour], new Point(bombs[i].Position.X, bombs[i].Position.Y + TILESIZE), TILESIZE, EXPLOSIONFRAMECOUNT, 0));
                                        if (bombs[i].ExplosionCanSpawn[4])
                                            explosions.Add(new Explosion(explosionTxtr0[(int)bombs[i].Colour], new Point(bombs[i].Position.X - TILESIZE, bombs[i].Position.Y), TILESIZE, EXPLOSIONFRAMECOUNT, 0));
                                    }

                                    if (bombs[i].Range >= 3)
                                    {
                                        if (bombs[i].ExplosionCanSpawn[5])
                                            explosions.Add(new Explosion(explosionTxtr0[(int)bombs[i].Colour], new Point(bombs[i].Position.X, bombs[i].Position.Y - (TILESIZE * 2)), TILESIZE, EXPLOSIONFRAMECOUNT, 0));
                                        if (bombs[i].ExplosionCanSpawn[6])
                                            explosions.Add(new Explosion(explosionTxtr0[(int)bombs[i].Colour], new Point(bombs[i].Position.X + (TILESIZE * 2), bombs[i].Position.Y), TILESIZE, EXPLOSIONFRAMECOUNT, 0));
                                        if (bombs[i].ExplosionCanSpawn[7])
                                            explosions.Add(new Explosion(explosionTxtr0[(int)bombs[i].Colour], new Point(bombs[i].Position.X, bombs[i].Position.Y + (TILESIZE * 2)), TILESIZE, EXPLOSIONFRAMECOUNT, 0));
                                        if (bombs[i].ExplosionCanSpawn[8])
                                            explosions.Add(new Explosion(explosionTxtr0[(int)bombs[i].Colour], new Point(bombs[i].Position.X - (TILESIZE * 2), bombs[i].Position.Y), TILESIZE, EXPLOSIONFRAMECOUNT, 0));
                                    }

                                    if (bombs[i].Range >= 4)
                                    {
                                        if (bombs[i].ExplosionCanSpawn[9])
                                            explosions.Add(new Explosion(explosionTxtr0[(int)bombs[i].Colour], new Point(bombs[i].Position.X, bombs[i].Position.Y - (TILESIZE * 3)), TILESIZE, EXPLOSIONFRAMECOUNT, 0));
                                        if (bombs[i].ExplosionCanSpawn[10])
                                            explosions.Add(new Explosion(explosionTxtr0[(int)bombs[i].Colour], new Point(bombs[i].Position.X + (TILESIZE * 3), bombs[i].Position.Y), TILESIZE, EXPLOSIONFRAMECOUNT, 0));
                                        if (bombs[i].ExplosionCanSpawn[11])
                                            explosions.Add(new Explosion(explosionTxtr0[(int)bombs[i].Colour], new Point(bombs[i].Position.X, bombs[i].Position.Y + (TILESIZE * 3)), TILESIZE, EXPLOSIONFRAMECOUNT, 0));
                                        if (bombs[i].ExplosionCanSpawn[12])
                                            explosions.Add(new Explosion(explosionTxtr0[(int)bombs[i].Colour], new Point(bombs[i].Position.X - (TILESIZE * 3), bombs[i].Position.Y), TILESIZE, EXPLOSIONFRAMECOUNT, 0));
                                    }

                                    if (bombs[i].Range >= 5)
                                    {
                                        if (bombs[i].ExplosionCanSpawn[13])
                                            explosions.Add(new Explosion(explosionTxtr0[(int)bombs[i].Colour], new Point(bombs[i].Position.X, bombs[i].Position.Y - (TILESIZE * 4)), TILESIZE, EXPLOSIONFRAMECOUNT, 0));
                                        if (bombs[i].ExplosionCanSpawn[14])
                                            explosions.Add(new Explosion(explosionTxtr0[(int)bombs[i].Colour], new Point(bombs[i].Position.X + (TILESIZE * 4), bombs[i].Position.Y), TILESIZE, EXPLOSIONFRAMECOUNT, 0));
                                        if (bombs[i].ExplosionCanSpawn[15])
                                            explosions.Add(new Explosion(explosionTxtr0[(int)bombs[i].Colour], new Point(bombs[i].Position.X, bombs[i].Position.Y + (TILESIZE * 4)), TILESIZE, EXPLOSIONFRAMECOUNT, 0));
                                        if (bombs[i].ExplosionCanSpawn[16])
                                            explosions.Add(new Explosion(explosionTxtr0[(int)bombs[i].Colour], new Point(bombs[i].Position.X - (TILESIZE * 4), bombs[i].Position.Y), TILESIZE, EXPLOSIONFRAMECOUNT, 0));
                                    }

                                    bombs.RemoveAt(i);
                                }
                            }
                            #endregion
                            #region ExplosionUpdate
                            for (int i = 0; i < explosions.Count; i++)
                            {
                                explosions[i].Update();

                                for (int j = 0; j < currentPlayers; j++)
                                    if (player[j].StandingArea.Intersects(explosions[i].HitBox) && !player[j].Invulnerable)
                                    {
                                        if (player[j].Shield <= 0)
                                            player[j].Kill(map.PlayerStarts[j], MAPSIZE, screenBounds);
                                        else
                                            player[j].LoseShield();
                                    }

                                if (explosions[i].RemoveThis)
                                    explosions.RemoveAt(i);
                            }
                            #endregion
                            #region MapUpdate
                            for (int i = 0; i < map.BlockRectangles.Length; i++)
                            {
                                bool removeBlock = false;

                                for (int j = 0; j < explosions.Count; j++)
                                    if (map.BlockRectangles[i].Intersects(explosions[j].HitBox))
                                        removeBlock = true;
#if DEBUG
                                if (map.BlockRectangles[i].Contains(mouse.Position) && mouse.LeftButton == ButtonState.Pressed)
                                    removeBlock = true;
#endif
                                if (removeBlock)
                                {
                                    if (RNG.Next(0, (12 / currentPlayers)) == 0)
                                    {
                                        int powerType = RNG.Next(0, 3);
                                        powerUps.Add(new Powerup(powerUpTextures[powerType], new Point(map.BlockRectangles[i].X, map.BlockRectangles[i].Y), powerType));
                                    }

                                    map.BlockRemoveTexture(i);
                                }
                            }

                            #endregion
                        }
                        else
                        {
                            if (input.Back && !inputOld.Back)
                            {
                                GameEndReset();
                                gameState = GameState.Menu;
                            }
                        }
                    }
                    break;
            }
            
            base.Update(gameTime);

            inputOld.Update();
        }

        private void GameEndReset()
        {
            for (int i = 0; i < player.Length; i++)
                player[i].GameEndReset();

            while (bombs.Count > 0)
            {
                for (int i = 0; i < bombs.Count; i++)
                    bombs.RemoveAt(i);
            }
            while (explosions.Count > 0)
            {
                for (int i = 0; i < explosions.Count; i++)
                    explosions.RemoveAt(i);
            }
            while (powerUps.Count > 0)
            {
                for (int i = 0; i < powerUps.Count; i++)
                    powerUps.RemoveAt(i);
            }

            map.GameEndReset();

            gameOver = false;
            countdown = 3;
            timer = 0;

            for (int i = 0; i < bombCounter.Length; i++)
                bombCounter[i] = 0;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            background.Draw(spriteBatch);

            switch (gameState)
            {
                case GameState.Menu:
                    title.Draw(spriteBatch);
                    mainMenu.Draw(spriteBatch);
                    menuControls.Draw(spriteBatch);
                    break;
                case GameState.Instructions:
                    instructionPage.Draw(spriteBatch);
                    break;
                case GameState.ChoosePlayers:
                    playerPage.Draw(spriteBatch);
                    playerMenu.Draw(spriteBatch);
                    break;
                case GameState.Playing:
                    map.Draw(spriteBatch, tiles);

                    for (int i = 0; i < bombs.Count; i++)
                        bombs[i].Draw(spriteBatch);

                    for (int i = 0; i < powerUps.Count; i++)
                        powerUps[i].Draw(spriteBatch);

                    for (int i = 0; i < explosions.Count; i++)
                        explosions[i].Draw(spriteBatch);

                    for (int i = 0; i < currentPlayers; i++)
                        player[i].Draw(spriteBatch);

                    #region Player Status Bars
                    if (currentPlayers >= 1)
                    {
                        playerStatusBars[0].Draw(spriteBatch);

                        if (!player[0].Dead)
                        {
                            if (player[0].Shield >= 0)
                                spriteBatch.Draw(playerSprites[0], new Rectangle(20, 2, 30, 45), Color.White);
                            if (player[0].Shield >= 1)
                                spriteBatch.Draw(playerSprites[0], new Rectangle(60, 2, 30, 45), Color.White);
                            if (player[0].Shield >= 2)
                                spriteBatch.Draw(playerSprites[0], new Rectangle(100, 2, 30, 45), Color.White);
                        }

                        #region SpeedScrolls
                        if (player[0].Speed == 2)
                        {
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(165, 6, 36, 36), Color.White);
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(155, 6, 36, 36), Color.White);
                        }
                        else if (player[0].Speed == 1)
                        {
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(165, 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(155, 6, 36, 36), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(165, 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(155, 6, 36, 36), Color.White * 0.5f);
                        }
                        #endregion
                        #region CauldronScrolls
                        if (player[0].CauldronCapLevel == 2)
                        {
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(225, 6, 36, 36), Color.White);
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(215, 6, 36, 36), Color.White);
                        }
                        else if (player[0].CauldronCapLevel == 1)
                        {
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(225, 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(215, 6, 36, 36), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(225, 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(215, 6, 36, 36), Color.White * 0.5f);
                        }
                        #endregion
                        #region RangeScrolls
                        if (player[0].BombRangeLevel == 2)
                        {
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(285, 6, 36, 36), Color.White);
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(275, 6, 36, 36), Color.White);
                        }
                        else if (player[0].BombRangeLevel == 1)
                        {
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(285, 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(275, 6, 36, 36), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(285, 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(275, 6, 36, 36), Color.White * 0.5f);
                        }
                        #endregion
                    }
                    if (currentPlayers >= 2)
                    {
                        playerStatusBars[1].Draw(spriteBatch);

                        if (!player[1].Dead)
                        {
                            if (player[1].Shield >= 0)
                                spriteBatch.Draw(playerSprites[1], new Rectangle(screenBounds.Width - 50, screenBounds.Height - 47, 30, 45), Color.White);
                            if (player[1].Shield >= 1)
                                spriteBatch.Draw(playerSprites[1], new Rectangle(screenBounds.Width - 90, screenBounds.Height - 47, 30, 45), Color.White);
                            if (player[1].Shield >= 2)
                                spriteBatch.Draw(playerSprites[1], new Rectangle(screenBounds.Width - 130, screenBounds.Height - 47, 30, 45), Color.White);
                        }

                        #region SpeedScrolls
                        if (player[1].Speed == 2)
                        {
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(screenBounds.Width - 165 - 36, screenBounds.Height - 6 - 36, 36, 36), Color.White);
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(screenBounds.Width - 155 - 36, screenBounds.Height - 6 - 36, 36, 36), Color.White);
                        }
                        else if (player[1].Speed == 1)
                        {
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(screenBounds.Width - 165 - 36, screenBounds.Height - 6 - 36, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(screenBounds.Width - 155 - 36, screenBounds.Height - 6 - 36, 36, 36), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(screenBounds.Width - 165 - 36, screenBounds.Height - 6 - 36, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(screenBounds.Width - 155 - 36, screenBounds.Height - 6 - 36, 36, 36), Color.White * 0.5f);
                        }
                        #endregion
                        #region CauldronScrolls
                        if (player[1].CauldronCapLevel == 2)
                        {
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(screenBounds.Width - 36 - 225, screenBounds.Height - 36 - 6, 36, 36), Color.White);
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(screenBounds.Width - 36 - 215, screenBounds.Height - 36 - 6, 36, 36), Color.White);
                        }
                        else if (player[1].CauldronCapLevel == 1)
                        {
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(screenBounds.Width - 36 - 225, screenBounds.Height - 36 - 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(screenBounds.Width - 36 - 215, screenBounds.Height - 36 - 6, 36, 36), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(screenBounds.Width - 36 - 225, screenBounds.Height - 36 - 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(screenBounds.Width - 36 - 215, screenBounds.Height - 36 - 6, 36, 36), Color.White * 0.5f);
                        }
                        #endregion
                        #region RangeScrolls
                        if (player[1].BombRangeLevel == 2)
                        {
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(screenBounds.Width - 36 - 285, screenBounds.Height - 36 - 6, 36, 36), Color.White);
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(screenBounds.Width - 36 - 275, screenBounds.Height - 36 - 6, 36, 36), Color.White);
                        }
                        else if (player[1].BombRangeLevel == 1)
                        {
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(screenBounds.Width - 36 - 285, screenBounds.Height - 36 - 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(screenBounds.Width - 36 - 275, screenBounds.Height - 36 - 6, 36, 36), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(screenBounds.Width - 36 - 285, screenBounds.Height - 36 - 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(screenBounds.Width - 36 - 275, screenBounds.Height - 36 - 6, 36, 36), Color.White * 0.5f);
                        }
                        #endregion
                    }
                    if (currentPlayers >= 3)
                    {
                        playerStatusBars[2].Draw(spriteBatch);

                        if (!player[2].Dead)
                        {
                            if (player[2].Shield >= 0)
                                spriteBatch.Draw(playerSprites[2], new Rectangle(screenBounds.Width - 50, 2, 30, 45), Color.White);
                            if (player[2].Shield >= 1)
                                spriteBatch.Draw(playerSprites[2], new Rectangle(screenBounds.Width - 90, 2, 30, 45), Color.White);
                            if (player[2].Shield >= 2)
                                spriteBatch.Draw(playerSprites[2], new Rectangle(screenBounds.Width - 130, 2, 30, 45), Color.White);
                        }

                        #region SpeedScrolls
                        if (player[2].Speed == 2)
                        {
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(screenBounds.Width - 165 - 36, 6, 36, 36), Color.White);
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(screenBounds.Width - 155 - 36, 6, 36, 36), Color.White);
                        }
                        else if (player[2].Speed == 1)
                        {
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(screenBounds.Width - 165 - 36, 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(screenBounds.Width - 155 - 36, 6, 36, 36), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(screenBounds.Width - 165 - 36, 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(screenBounds.Width - 155 - 36, 6, 36, 36), Color.White * 0.5f);
                        }
                        #endregion
                        #region CauldronScrolls
                        if (player[2].CauldronCapLevel == 2)
                        {
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(screenBounds.Width - 36 - 225, 6, 36, 36), Color.White);
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(screenBounds.Width - 36 - 215, 6, 36, 36), Color.White);
                        }
                        else if (player[2].CauldronCapLevel == 1)
                        {
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(screenBounds.Width - 36 - 225, 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(screenBounds.Width - 36 - 215, 6, 36, 36), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(screenBounds.Width - 36 - 225, 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(screenBounds.Width - 36 - 215, 6, 36, 36), Color.White * 0.5f);
                        }
                        #endregion
                        #region RangeScrolls
                        if (player[2].BombRangeLevel == 2)
                        {
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(screenBounds.Width - 36 - 285, 6, 36, 36), Color.White);
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(screenBounds.Width - 36 - 275, 6, 36, 36), Color.White);
                        }
                        else if (player[2].BombRangeLevel == 1)
                        {
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(screenBounds.Width - 36 - 285, 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(screenBounds.Width - 36 - 275, 6, 36, 36), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(screenBounds.Width - 36 - 285, 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(screenBounds.Width - 36 - 275, 6, 36, 36), Color.White * 0.5f);
                        }
                        #endregion
                    }
                    if (currentPlayers >= 4)
                    {
                        playerStatusBars[3].Draw(spriteBatch);

                        if (!player[3].Dead)
                        {
                            if (player[3].Shield >= 0)
                                spriteBatch.Draw(playerSprites[3], new Rectangle(20, screenBounds.Height - 47, 30, 45), Color.White);
                            if (player[3].Shield >= 1)
                                spriteBatch.Draw(playerSprites[3], new Rectangle(60, screenBounds.Height - 47, 30, 45), Color.White);
                            if (player[3].Shield >= 2)
                                spriteBatch.Draw(playerSprites[3], new Rectangle(100, screenBounds.Height - 47, 30, 45), Color.White);
                        }

                        #region SpeedScrolls
                        if (player[3].Speed == 2)
                        {
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(165, screenBounds.Height - 6 - 36, 36, 36), Color.White);
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(155, screenBounds.Height - 6 - 36, 36, 36), Color.White);
                        }
                        else if (player[3].Speed == 1)
                        {
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(165, screenBounds.Height - 6 - 36, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(155, screenBounds.Height - 6 - 36, 36, 36), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(165, screenBounds.Height - 6 - 36, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[0], new Rectangle(155, screenBounds.Height - 6 - 36, 36, 36), Color.White * 0.5f);
                        }
                        #endregion
                        #region CauldronScrolls
                        if (player[3].CauldronCapLevel == 2)
                        {
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(225, screenBounds.Height - 36 - 6, 36, 36), Color.White);
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(215, screenBounds.Height - 36 - 6, 36, 36), Color.White);
                        }
                        else if (player[3].CauldronCapLevel == 1)
                        {
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(225, screenBounds.Height - 36 - 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(215, screenBounds.Height - 36 - 6, 36, 36), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(225, screenBounds.Height - 36 - 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[2], new Rectangle(215, screenBounds.Height - 36 - 6, 36, 36), Color.White * 0.5f);
                        }
                        #endregion
                        #region RangeScrolls
                        if (player[3].BombRangeLevel == 2)
                        {
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(285, screenBounds.Height - 36 - 6, 36, 36), Color.White);
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(275, screenBounds.Height - 36 - 6, 36, 36), Color.White);
                        }
                        else if (player[3].BombRangeLevel == 1)
                        {
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(285, screenBounds.Height - 36 - 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(275, screenBounds.Height - 36 - 6, 36, 36), Color.White);
                        }
                        else
                        {
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(285, screenBounds.Height - 36 - 6, 36, 36), Color.White * 0.5f);
                            spriteBatch.Draw(powerUpTextures[1], new Rectangle(275, screenBounds.Height - 36 - 6, 36, 36), Color.White * 0.5f);
                        }
                        #endregion
                    }
                    #endregion

                    if (countdown == 3)
                    {
                        spriteBatch.Draw(pixel, screenBounds, Color.Black * 0.5f);
                        countdown3.Draw(spriteBatch);
                    }
                    if (countdown == 2)
                    {
                        spriteBatch.Draw(pixel, screenBounds, Color.Black * 0.5f);
                        countdown2.Draw(spriteBatch);
                    }
                    if (countdown == 1)
                    {
                        spriteBatch.Draw(pixel, screenBounds, Color.Black * 0.5f);
                        countdown1.Draw(spriteBatch);
                    }

                    if (gameOver)
                    {
                        spriteBatch.Draw(pixel, screenBounds, Color.Black * 0.5f);
                        backButton.Draw(spriteBatch);
                        if (!player[0].Dead)
                            playerWinGraphics[0].Draw(spriteBatch);
                        else if (!player[1].Dead)
                            playerWinGraphics[1].Draw(spriteBatch);
                        else if (!player[2].Dead)
                            playerWinGraphics[2].Draw(spriteBatch);
                        else if (!player[3].Dead)
                            playerWinGraphics[3].Draw(spriteBatch);
                        else
                            playerWinGraphics[4].Draw(spriteBatch);
                    }
                    break;
            }
            spriteBatch.End();

#if DEBUG
            spriteBatch.Begin();
            switch (gameState)
            {
                case GameState.Playing:
                    spriteBatch.DrawString(debugFont, "Mouse Position: " + mouse.Position.X + "x " + mouse.Position.Y + "y"
                        + "\nexplosions: " + explosions.Count
                        + "\nP1: " + player[0].DebugString()
                        + "Game Over: " + gameOver, Vector2.Zero, Color.White);

                    map.DrawDebug(spriteBatch, pixel, debugFont, tiles);

                    for (int i = 0; i < currentPlayers; i++)
                        player[i].DrawDebug(spriteBatch, pixel);

                    for (int i = 0; i < bombs.Count; i++)
                        bombs[i].DrawDebug(spriteBatch, pixel, debugFont);

                    for (int i = 0; i < explosions.Count; i++)
                        explosions[i].DrawDebug(spriteBatch, pixel);
                    break;
            }
            spriteBatch.End();
#endif

            base.Draw(gameTime);
        }
    }
}
