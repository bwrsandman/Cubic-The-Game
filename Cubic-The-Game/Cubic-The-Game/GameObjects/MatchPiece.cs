#region description
//-----------------------------------------------------------------------------
// MatchPiece.cs
//
// Written by bwrsandman(Sandy)
//-----------------------------------------------------------------------------
#endregion


#region using
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Cubic_The_Game
{
    class MatchPiece : Piece
    {

        #region constants
        readonly Color inactiveColor = Color.Black;
        readonly Color backColor = Color.Gray;
        #endregion

        #region statics
        #endregion

        #region members
        private VertexPositionColorTexture[] cubeFront;
        private float pieceSize;
        private float posOffset;
        private float rotOffset;
        private Vector3 faceOffset;

        public override Matrix GetWorldTranslation { get { return worldTranslation; } }
        private Matrix worldTranslation = Matrix.CreateTranslation(0, 0, 2);
        public override Vector3 GetCenter3 { get { return new Vector3(position3.X + pieceSize / 2, position3.Y - pieceSize / 2, position3.Z); } }
        private Vector3 position3;


        /// <summary>
        /// When a Match Piece is created:
        ///     - Randomly select an identity (texture)
        ///     - Set its facing direction (front, side, back)
        ///     - Set it's offset from the middle
        ///     - Create a backsurface and front texture key
        /// </summary>
        public MatchPiece(float posOffset, float size, int facingDirection, float midLen)
        {
            cubeFront = new VertexPositionColorTexture[4];
            this.pieceSize = size;
            this.posOffset = posOffset;
            position3 = new Vector3(posOffset, 0, 0);
            faceOffset = new Vector3(0, -size, midLen);
            rotOffset = (float)(facingDirection * Math.PI / 2.0);
            
        }

        #endregion

        #region update and draw
        public void Draw(Camera camera, Matrix worldTranslation)
        {
            GameObject.device.SetVertexBuffer(cubeBuffer);
            
            Matrix rotMatrix = Matrix.CreateRotationY(rotOffset);
            this.worldTranslation = rotMatrix * worldTranslation;
            cubeEffect.World = this.worldTranslation;
            cubeEffect.View = camera.view;
            cubeEffect.Projection = camera.projection;
            
            
            
            
            // Render background of Square
            cubeEffect.DiffuseColor = color.ToVector3();
            cubeEffect.TextureEnabled = false;
            //cubeEffect.Texture = texture;

            cubeEffect.DiffuseColor = backColor.ToVector3();
            foreach (EffectPass pass in cubeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GameObject.device.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleStrip, cubeFront, 0, 2);

            }


            // Render foreground of Square with shape
            cubeEffect.DiffuseColor = color.ToVector3();
            cubeEffect.TextureEnabled = true;
            //cubeEffect.Texture = texture;


            foreach (EffectPass pass in cubeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GameObject.device.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleStrip, cubeFront, 0, 2);

            }

        }


        public new void Update()
        {
            color = isIntersected ? interactedColor : inactiveColor;
            cubeFront[0] = new VertexPositionColorTexture(new Vector3(posOffset, pieceSize, 0)+faceOffset , color, new Vector2(0, 0));
            cubeFront[1] = new VertexPositionColorTexture(new Vector3(posOffset + pieceSize, pieceSize, 0) + faceOffset, color, new Vector2(1, 0));
            cubeFront[2] = new VertexPositionColorTexture(new Vector3(posOffset, 0, 0) + faceOffset, color, new Vector2(0, 1));
            cubeFront[3] = new VertexPositionColorTexture(new Vector3(posOffset + pieceSize, 0, 0) + faceOffset, color, new Vector2(1, 1));
        }
        #endregion
    }
}
