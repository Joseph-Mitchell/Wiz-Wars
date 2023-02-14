using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WizWars
{
    class StaticGraphic
    {
        protected Texture2D m_texture;
        protected Point m_position;

        public StaticGraphic(Texture2D texture, Point position)
        {
            m_texture = texture;
            m_position = position;
        }

        public virtual void Draw(SpriteBatch sb)
        {
            sb.Draw(m_texture, new Vector2(m_position.X, m_position.Y), Color.White);
        }
    }

    class StaticGraphicColoured : StaticGraphic
    {

        public StaticGraphicColoured(Texture2D texture, Point position):base(texture, position)
        {
        }

        public void Draw(SpriteBatch sb, Color color)
        {
            sb.Draw(m_texture, new Vector2(m_position.X, m_position.Y), color);
        }
    }

    class HitBoxObject : StaticGraphic
    {
        protected Rectangle m_hitBox;

        public HitBoxObject(Texture2D texture, Point position) : base(texture, position)
        {
            m_hitBox = new Rectangle(m_position.X, m_position.Y, texture.Width, texture.Height);
        }

        public virtual void Update()
        {
            UpdateHitBox();
        }

        protected virtual void UpdateHitBox()
        {
            m_hitBox.X = m_position.X;
            m_hitBox.Y = m_position.Y;
        }
    }

    class HitBoxObjectAnimated : HitBoxObject
    {
        private Rectangle m_source;

        protected int m_time;
        private int m_timer;

        public HitBoxObjectAnimated(Texture2D texture, Point position, int frameSize, int frameCount, int time) : base(texture, position)
        {
            m_hitBox.Width = texture.Width / frameCount;

            m_source = new Rectangle(0, 0, frameSize, frameSize);

            m_time = time;
        }

        public override void Update()
        {
            base.Update();

            m_timer++;
            //Move to next sprite in spritesheet after "m_time" frames
            if (m_timer >= m_time)
            {
                m_source.X += m_source.Width;

                //All spritesheets only use x dimension, so y dimension incrementing is not needed
                if (m_source.X > m_texture.Width)
                    m_source.X = 0;

                m_timer = 0;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.Draw(m_texture, new Vector2(m_position.X, m_position.Y), m_source, Color.White);
        }
    }

    class HitBoxObjectAnimatedWithEdges : HitBoxObjectAnimated
    {
        protected Rectangle[] m_edgeHitBoxes;

        private readonly int m_frameCount;

        public HitBoxObjectAnimatedWithEdges(Texture2D texture, Point position, int frameSize, int frameCount, int time) : base(texture, position, frameSize, frameCount, time)
        {
            m_edgeHitBoxes = new Rectangle[4];
            m_edgeHitBoxes[0] = new Rectangle(position.X + 1, position.Y + 1, (texture.Width / frameCount) - 2, 1);
            m_edgeHitBoxes[1] = new Rectangle(position.X + (texture.Width / frameCount) - 1, position.Y, 1, texture.Height - 2);
            m_edgeHitBoxes[2] = new Rectangle(position.X + 1, position.Y + texture.Height - 1, (texture.Width / frameCount) - 2, 1);
            m_edgeHitBoxes[3] = new Rectangle(position.X, position.Y + 1, 1, texture.Height - 2);

            m_frameCount = frameCount;
        }

        protected override void UpdateHitBox()
        {
            base.UpdateHitBox();

            //Keep edges in correct positions incase of object movement
            m_edgeHitBoxes[0].X = m_position.X + 1;
            m_edgeHitBoxes[0].Y = m_position.Y + 1;
            m_edgeHitBoxes[1].X = m_position.X + (m_texture.Width / m_frameCount) - 1;
            m_edgeHitBoxes[1].Y = m_position.Y;
            m_edgeHitBoxes[2].X = m_position.X + 1;
            m_edgeHitBoxes[2].Y = m_position.Y + m_texture.Height - 1;
            m_edgeHitBoxes[3].X = m_position.X;
            m_edgeHitBoxes[3].Y = m_position.Y + 1;
        }
    }

    class Menu
    {
        private StaticGraphicColoured[] m_button;

        private int m_buttonSelected;

        public Menu(Texture2D[] buttonTexture, Point centerPosition)
        {
            m_button = new StaticGraphicColoured[3];

            //Since both menus use exactly three buttons there is no reason to use a dynamic list
            m_button[0] = new StaticGraphicColoured(buttonTexture[0], new Point(centerPosition.X - (buttonTexture[0].Width / 2), centerPosition.Y - 100));
            m_button[1] = new StaticGraphicColoured(buttonTexture[1], new Point(centerPosition.X - (buttonTexture[1].Width / 2), centerPosition.Y));
            m_button[2] = new StaticGraphicColoured(buttonTexture[2], new Point(centerPosition.X - (buttonTexture[2].Width / 2), centerPosition.Y + 100));
        }

        public void Update(Input input, Input oldInput)
        {
            if (input.Up[0] && !oldInput.Up[0] && !input.Down[0])
                m_buttonSelected -= 1;
            if (input.Down[0] && !oldInput.Down[0] && !input.Up[0])
                m_buttonSelected += 1;

            if (m_buttonSelected < 0)
                m_buttonSelected = 2;
            if (m_buttonSelected > 2)
                m_buttonSelected = 0;

            if (input.Action[0] && !oldInput.Action[0])
            {
                if (m_buttonSelected == 0)
                    Button0Events();
                if (m_buttonSelected == 1)
                    Button1Events();
                if (m_buttonSelected == 2)
                    Button2Events();
            }
        }

        protected virtual void Button0Events()
        {
        }
        protected virtual void Button1Events()
        {
        }
        protected virtual void Button2Events()
        {
        }

        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < m_button.Length; i++)
            {
                if (m_buttonSelected == i)
                    m_button[i].Draw(sb);
                else
                    m_button[i].Draw(sb, Color.Gray);
            }
        }
    }
}
