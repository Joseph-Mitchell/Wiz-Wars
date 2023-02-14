using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WizWars
{
    enum PowerUpType
    {
        Speed,
        Range,
        Cauldron
    }

    class Powerup : HitBoxObject
    {
        public Rectangle HitBox
        {
            get => m_hitBox;
        }

        public PowerUpType PowerType
        {
            get;
            private set;
        }

        public Powerup(Texture2D texture, Point position, int powerType) : base(texture, position)
        {
            PowerType = (PowerUpType)powerType;
        }
    }
}
