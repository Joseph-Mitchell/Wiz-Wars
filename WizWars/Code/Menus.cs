using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WizWars
{
    class MainMenu : Menu
    {
        public bool ExitGame
        {
            get;
            private set;
        }

        public bool GoToInstructions
        {
            get;
            set;
        }

        public bool GoToChoosePlayers
        {
            get;
            set;
        }

        public MainMenu(Texture2D[] buttonTexture, Point centerPosition):base(buttonTexture, centerPosition)
        {
        }

        protected override void Button0Events()
        {
            GoToChoosePlayers = true;
        }
        protected override void Button1Events()
        {
            GoToInstructions = true;
        }
        protected override void Button2Events()
        {
            ExitGame = true;
        }
    }

    class PlayerMenu : Menu
    {

        public bool Two
        {
            get;
            set;
        }

        public bool Three
        {
            get;
            set;
        }

        public bool Four
        {
            get;
            set;
        }

        public PlayerMenu(Texture2D[] buttonTexture, Point centerPosition) : base(buttonTexture, centerPosition)
        {
        }

        protected override void Button0Events()
        {
            Two = true;
        }
        protected override void Button1Events()
        {
            Three = true;
        }
        protected override void Button2Events()
        {
            Four = true;
        }
    }
}
