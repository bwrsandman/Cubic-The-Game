#region description
//-----------------------------------------------------------------------------
// Piece.cs
//
// Written by bwrsandman(Sandy)
//-----------------------------------------------------------------------------
#endregion


#region using
using Microsoft.Xna.Framework;            // for Vectors
using Microsoft.Xna.Framework.Graphics;   // for Texture2D
#endregion

namespace Cubic_The_Game
{
    abstract class Piece : GameObject
    {
        protected float midLen;
        protected Vector3 position3;
        protected VertexPositionColorTexture[] cubeFront;
        protected Piece() { }

        protected Piece(Vector3 offset, float midLen)
        {
            this.midLen = midLen;
            Vector2[] textCoord = { new Vector2(0, 0), 
                                    new Vector2(1, 0), 
                                    new Vector2(0, 1), 
                                    new Vector2(1, 1) };
            Vector2[] posCoord = { new Vector2(0, 0), 
                                    new Vector2(1, 0), 
                                    new Vector2(0, -1), 
                                    new Vector2(1, -1) };
            cubeFront = new VertexPositionColorTexture[4];
            for (byte i = 0; i < 4; ++i)
                cubeFront[i] = new VertexPositionColorTexture(offset + midLen * new Vector3(posCoord[i], 0f), color, textCoord[i]);

            texture = generateTexture();

        }
        public Texture2D generateTexture()
        {
            return shapes[pieceID = rnd.Next(0, shapes.Length)];
        }

        #region constants
        private const float OFFSET = 50f;
        private const float OFFSET_SQUARED = 2500;
        #endregion

        #region statics
        protected static VertexBuffer cubeBuffer;
        protected static BasicEffect cubeEffect;
        #endregion

        #region members
        protected Texture2D texture;
        protected bool isIntersected;
        public int interactingPlayer { get; protected set; }
        protected Color interactedColor;
        protected Color color;
        public int pieceID { get; protected set; }

        public abstract Matrix GetWorldTranslation { get; }
        public abstract Vector3 GetCenter3{get;}
        public Vector2 center2
        {
            get { return GameObject.GetScreenSpace(GetCenter3, GetWorldTranslation); }
        }
        #endregion

        #region update and draw

        public virtual bool intersects(Vector2 cntr)
        {
            return ((center2 - cntr).LengthSquared() <= OFFSET_SQUARED);
        }
        #endregion


    }
}
