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
        Color color = Color.Black;
        #endregion

        #region statics
        #endregion

        #region members
        VertexPositionColorTexture[] cubeFront;
        float pieceSize;
        float posOffset;
        float rotOffset;
        Vector3 faceOffset;


                //reference to the vertex array that this square is a part of (actually contains multiple squares)
        private VertexPositionColorTexture[] vertices;
        private short[] vertexIndices;
        private float width;//, height;


        public MatchPiece(int idxTopLeft, int idxBLeft, int idxBRight, int idxTopRight, VertexPositionColor[] pVertices)
        {
            vertexIndices = new short[4];
            vertexIndices[0] = (short) idxTopLeft;
            vertexIndices[1] = (short) idxBLeft;
            vertexIndices[2] = (short) idxBRight;
            vertexIndices[3] = (short) idxTopRight;
            vertices = new VertexPositionColorTexture[pVertices.Count()];
            for (int i = 0; i > pVertices.Count(); ++i)
            {
                vertices[i] = new VertexPositionColorTexture(pVertices[i].Position, pVertices[i].Color, new Vector2((i%2),(int)(i/2)));
            }
            width = Math.Abs(vertices[idxTopRight].Position.X - vertices[idxTopLeft].Position.X); 
        }

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
            faceOffset = new Vector3(0, -size, midLen);
            rotOffset = (float)(facingDirection * Math.PI / 2.0);
            
        }

        public void SetVertexIndices(int idxTopLeft, int idxBLeft, int idxBRight, int idxTopRight)
        {
            vertexIndices[0] = (short)idxTopLeft;
            vertexIndices[1] = (short)idxBLeft;
            vertexIndices[2] = (short)idxBRight;
            vertexIndices[3] = (short)idxTopRight;
        }
        /// <summary>
        /// Get top left X position
        /// </summary>
        /// <returns></returns>
        public float GetX()
        {
            return vertices[(int)vertexIndices[0]].Position.X;
        }
        /// <summary>
        /// Get topleft Y coordinate of square
        /// </summary>
        /// <returns></returns>
        public float GetY()
        {
            return vertices[(int)vertexIndices[0]].Position.Y;
        }

        public float GetWidth()
        {
            return width;
            //return Math.abs(vertices[(int)vertexIndices[3]].Position.X - vertices[(int)vertexIndices[1]].Position.X);
        }
        //public float GetHeight()
        //{
        //    return height;
        //   // return Math.abs(vertices[(int)vertexIndices[3]].Position.Y - vertices[(int)vertexIndices[2]].Position.Y);
        //}
        public void SetColor(byte r, byte g, byte b)
        {
            for (int i = 0; i < vertexIndices.Length; i++)
            {
                vertices[(int)vertexIndices[i]].Color.R = r;
                vertices[(int)vertexIndices[i]].Color.G = g;
                vertices[(int)vertexIndices[i]].Color.B = b;
            }
        }

        #endregion

        #region update and draw
        public void Draw(Camera camera, Matrix worldTranslation)
        {
            GameObject.device.SetVertexBuffer(cubeBuffer);
            
            Matrix rotMatrix = Matrix.CreateRotationY(rotOffset);
            cubeEffect.World = rotMatrix * worldTranslation;
            cubeEffect.View = camera.view;
            cubeEffect.Projection = camera.projection;
            cubeEffect.DiffuseColor = color.ToVector3();
            //cubeEffect.TextureEnabled = true;
            //cubeEffect.Texture = texture;


            foreach (EffectPass pass in cubeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GameObject.device.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleStrip, cubeFront, 0, 2);

            }


        }


        public void Update()
        {
            cubeFront[0] = new VertexPositionColorTexture(new Vector3(posOffset, pieceSize, 0)+faceOffset , color, new Vector2(0, 0));
            cubeFront[1] = new VertexPositionColorTexture(new Vector3(posOffset + pieceSize, pieceSize, 0) + faceOffset, color, new Vector2(1, 0));
            cubeFront[2] = new VertexPositionColorTexture(new Vector3(posOffset, 0, 0) + faceOffset, color, new Vector2(0, 1));
            cubeFront[3] = new VertexPositionColorTexture(new Vector3(posOffset + pieceSize, 0, 0) + faceOffset, color, new Vector2(1, 1));
        }
        #endregion
    }
}
