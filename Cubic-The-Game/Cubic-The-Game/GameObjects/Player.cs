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
        private Vector2 position;
        public Color color{get; private set;}
        private Vector2 movement = Vector2.Zero;
        protected override TwoInt size{ get { return new TwoInt(texture.Width, texture.Height); } }
        private FallPiece grabPiece;
        private bool grabbing;
        public int index { get; private set; }
        #endregion

        #region accessors
        public void Attach(Piece piece)
        {
            if (grabPiece == null || !grabbing)
                if (piece is FallPiece) grabPiece = (FallPiece)piece;
        }

        #endregion

        public Vector2 center //  object center = object position - 1/2 object size, if scaled you have to adjust size or to keep track of the scale (getScale)
        {
            get { return new Vector2(position.X + size.X /* * getScale*/ / 2, position.Y + size.Y /* * getScale*/ / 2); }
            protected set { position = new Vector2(value.X - size.X /* * getScale*/ / 2, value.Y - size.Y /* * getScale*/ / 2); }
        }

        #region constructors
        public Player(int index, Vector2 position, Color color)
        {
            this.position = position;
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
            {
                if (!grabbing) // Grab
                {
                    grabbing = grabPiece.Grab(this);
                    if (grabPiece != null && grabbing)
                        grabPiece.Move(center);
                }
                else
                {         // Drop
                    grabbing = grabPiece.Drop(this);
                    grabPiece = null;
                }
            }
        }
        protected override void Update()
        {
            if (movement != Vector2.Zero)
            {
                Vector2 newCenter = center + movement;
                if (newCenter.X > ((Vector2)GameObject.screenSize).X || newCenter.X < 0) movement.X = 0;
                if (newCenter.Y > ((Vector2)GameObject.screenSize).Y || newCenter.Y < 0) movement.Y = 0;
                position += movement;
                if (grabPiece != null && grabbing)
                    grabPiece.Move(center);
                movement = Vector2.Zero;
            }

        }
        #endregion

        #region draw
        protected override void Draw() 
        {
            spriteBatch.Draw(texture, position, color);
        }
        #endregion

        internal bool Match(int id)
        {
            return grabbing && grabPiece.Match(id);
        }
    }
}
