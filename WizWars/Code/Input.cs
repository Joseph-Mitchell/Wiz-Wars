using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace WizWars
{
    class Input
    {
        private KeyboardState keyboard;
        private GamePadState[] gamePad = new GamePadState[4];

        public bool[] Up
        {
            get;
            private set;
        }

        public bool[] Right
        {
            get;
            private set;
        }

        public bool[] Down
        {
            get;
            private set;
        }

        public bool[] Left
        {
            get;
            private set;
        }

        public bool[] Action
        {
            get;
            private set;
        }

        public bool Back
        {
            get;
            private set;
        }

        public Input()
        {
            Up = new bool[4];
            Right = new bool[4];
            Down = new bool[4];
            Left = new bool[4];
            Action = new bool[4];
        }

        public void Update()
        {
            keyboard = Keyboard.GetState();

            for (int i = 0; i < gamePad.Length; i++)
                gamePad[i] = GamePad.GetState((PlayerIndex)i);

            #region Up
            if (keyboard.IsKeyDown(Keys.W) || gamePad[0].DPad.Up == ButtonState.Pressed)
                Up[0] = true;
            else
                Up[0] = false;

            if (keyboard.IsKeyDown(Keys.Up) || gamePad[1].DPad.Up == ButtonState.Pressed)
                Up[1] = true;
            else
                Up[1] = false;

            if (keyboard.IsKeyDown(Keys.I) || gamePad[2].DPad.Up == ButtonState.Pressed)
                Up[2] = true;
            else
                Up[2] = false;

            if (keyboard.IsKeyDown(Keys.T) || gamePad[3].DPad.Up == ButtonState.Pressed)
                Up[3] = true;
            else
                Up[3] = false;
            #endregion
            #region Right
            if (keyboard.IsKeyDown(Keys.D) || gamePad[0].DPad.Right == ButtonState.Pressed)
                Right[0] = true;
            else
                Right[0] = false;

            if (keyboard.IsKeyDown(Keys.Right) || gamePad[1].DPad.Right == ButtonState.Pressed)
                Right[1] = true;
            else
                Right[1] = false;

            if (keyboard.IsKeyDown(Keys.L) || gamePad[2].DPad.Right == ButtonState.Pressed)
                Right[2] = true;
            else
                Right[2] = false;

            if (keyboard.IsKeyDown(Keys.H) || gamePad[3].DPad.Right == ButtonState.Pressed)
                Right[3] = true;
            else
                Right[3] = false;
            #endregion
            #region Down
            if (keyboard.IsKeyDown(Keys.S) || gamePad[0].DPad.Down == ButtonState.Pressed)
                Down[0] = true;
            else
                Down[0] = false;

            if (keyboard.IsKeyDown(Keys.Down) || gamePad[1].DPad.Down == ButtonState.Pressed)
                Down[1] = true;
            else
                Down[1] = false;

            if (keyboard.IsKeyDown(Keys.K) || gamePad[2].DPad.Down == ButtonState.Pressed)
                Down[2] = true;
            else
                Down[2] = false;

            if (keyboard.IsKeyDown(Keys.G) || gamePad[3].DPad.Down == ButtonState.Pressed)
                Down[3] = true;
            else
                Down[3] = false;
            #endregion
            #region Left
            if (keyboard.IsKeyDown(Keys.A) || gamePad[0].DPad.Left == ButtonState.Pressed)
                Left[0] = true;
            else
                Left[0] = false;

            if (keyboard.IsKeyDown(Keys.Left) || gamePad[1].DPad.Left == ButtonState.Pressed)
                Left[1] = true;
            else
                Left[1] = false;

            if (keyboard.IsKeyDown(Keys.J) || gamePad[2].DPad.Left == ButtonState.Pressed)
                Left[2] = true;
            else
                Left[2] = false;

            if (keyboard.IsKeyDown(Keys.F) || gamePad[3].DPad.Left == ButtonState.Pressed)
                Left[3] = true;
            else
                Left[3] = false;
            #endregion
            #region Action
            if (keyboard.IsKeyDown(Keys.LeftShift) || gamePad[0].Buttons.A == ButtonState.Pressed)
                Action[0] = true;
            else
                Action[0] = false;

            if (keyboard.IsKeyDown(Keys.NumPad0) || gamePad[1].Buttons.A == ButtonState.Pressed)
                Action[1] = true;
            else
                Action[1] = false;

            if (keyboard.IsKeyDown(Keys.RightShift) || gamePad[2].Buttons.A == ButtonState.Pressed)
                Action[2] = true;
            else
                Action[2] = false;

            if (keyboard.IsKeyDown(Keys.Space) || gamePad[3].Buttons.A == ButtonState.Pressed)
                Action[3] = true;
            else
                Action[3] = false;
            #endregion

            if (keyboard.IsKeyDown(Keys.Escape) || gamePad[0].Buttons.B == ButtonState.Pressed)
                Back = true;
            else
                Back = false;
        }
    }
}
