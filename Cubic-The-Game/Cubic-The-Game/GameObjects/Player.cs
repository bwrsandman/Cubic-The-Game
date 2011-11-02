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
        public Color color{get; private set;}
        private Vector2 movement = Vector2.Zero;
        protected override TwoInt size{ get { return new TwoInt(texture.Width, texture.Height); } }
        private FallPiece grabPiece;
        private bool grabbing;
        private int index;
        #endregion

        #region accessors
        public void Attach(FallPiece piece)
        {
            grabPiece = piece;
        }

        #endregion

        #region constructors
        public Player(int index, Vector2 position, Color color)
            : base(position)
        {
            this.index = index;
            this.color = color;
        }
        #endregion

        #region update
        public void Move(Vector2 thismuch)
        {
            movement += thismuch;
        }
        public void GrabDrop()
        {
            if (grabPiece != null)
                grabbing = !grabbing && grabPiece.intersects(center);
        }
        protected override void Update()
        {
            position += movement;
            if(grabPiece!=null && grabbing)
                grabPiece.Move(movement);
            movement = Vector2.Zero;
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
