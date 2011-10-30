#region description
//-----------------------------------------------------------------------------
// Player.cs
//
// Written by bwrsandman(Sandy)
//-----------------------------------------------------------------------------
#endregion


#region using
using Microsoft.Xna.Framework;            //  for Color, Vectors
using Microsoft.Xna.Framework.Graphics;   //  for Texture2D
#endregion

namespace Cubic_The_Game
{
    class Player : CollisionObject
    {
        #region constants
        #endregion

        #region statics
        public static Texture2D texture { private get; set; }
        #endregion

        #region members
        private Color color;
        private Vector2 movement = Vector2.Zero;
        #endregion

        #region constructors
        public Player(Vector2 position, Color color)
            : base(position)
        {
            this.color = color;
        }
        #endregion

        #region update
        public void Move(Vector2 thismuch)
        {
            movement += thismuch;
        }
        protected override void Update()
        {
            position += movement;
            movement = Vector2.Zero;
        }
        public void SetPos(float x, float y)
        {
            position.X = x;
            position.Y = y;
        }
        #endregion

        #region draw
        protected override void Draw() 
        {
            spriteBatch.Draw(texture, position, color);
        }
        #endregion
    }
}
