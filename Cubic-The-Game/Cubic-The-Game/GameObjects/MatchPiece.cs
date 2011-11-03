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
        #endregion

        #region statics
        #endregion

        #region members
                //reference to the vertex array that this square is a part of (actually contains multiple squares)
        private VertexPositionColor[] vertices;
        private short[] vertexIndices;
        private float width;//, height;
        /// <summary>
        /// Get topleft X coordinate of square
        /// </summary>
        /// <returns></returns>
        /// 
        
        ///<summary>
        ///Enter an array of vertex indices just for this square, and a 
        ///reference to the greater vertex array that this square is a part of
        /// </summary>

        //public Square(short[] pVertexIndices, VertexPositionColor[] pVertices)
        //{
        //    vertexIndices = pVertexIndices;
        //    vertices = pVertices;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idxTopLeft"></param>
        /// Index to the topleft vertex
        /// <param name="idxBLeft"></param>
        /// Index to the bottom-left vertex
        /// <param name="idxBRight"></param>
        /// index to the bottom-right vertex
        /// <param name="IdxTopRight"></param>
        /// index to the top-right vertex
        /// <param name="pVertices"></param>
        /// Array of vertices that this square is a part of
        public MatchPiece(int idxTopLeft, int idxBLeft, int idxBRight, int idxTopRight, VertexPositionColor[] pVertices)
        {
            vertexIndices = new short[4];
            vertexIndices[0] = (short) idxTopLeft;
            vertexIndices[1] = (short) idxBLeft;
            vertexIndices[2] = (short) idxBRight;
            vertexIndices[3] = (short) idxTopRight;
            vertices = pVertices;
            width = Math.Abs(vertices[idxTopRight].Position.X - vertices[idxTopLeft].Position.X); 
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
    }
}
