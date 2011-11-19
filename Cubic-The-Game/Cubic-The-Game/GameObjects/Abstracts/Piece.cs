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

        #region constants
        #endregion

        #region statics
        protected static VertexBuffer cubeBuffer;
        protected static BasicEffect cubeEffect;
        #endregion

        #region members
        protected Texture2D texture;
        #endregion

  
    }
}
