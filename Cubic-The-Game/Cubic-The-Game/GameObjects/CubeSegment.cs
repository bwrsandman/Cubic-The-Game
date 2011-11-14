#region description
//-----------------------------------------------------------------------------
// CubeSegment.cs
//
// Written by ecote (Eric)
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
    class CubeSegment : MovingObject
    {
        #region constants
        #endregion

        #region statics
        #endregion

        #region members
        private float rotation = 0; //represents how much the slab is rotated
        private MatchPiece[] squares;
        private VertexPositionColor[] vertices; //vertices that make up the slab
        private Vector3[] normals;
        private Vector3 position3;   //TODO: replace with GameObject.Position, once that is a Vector3
        private int numSquaresAcross;
        private int numSquaresTotal;
        private VertexBuffer vertexBuff;
  //      private IndexBuffer indexBuff;
        private Matrix matWorld;
        private Color color = new Color(157, 19, 168);
       
        BasicEffect segmentEffect;
        private RasterizerState wireFrameRasterizer = new RasterizerState { CullMode = CullMode.None, FillMode = FillMode.WireFrame };
        public void Rotate(float amount)
        {
            //hack to stop it from rotating too much. for some reason gamescreen
            ////stops updating for a while, and then the time-elapsed ends up being huge
            //if (amount > 0.5f)
            //    amount = 0.5f;
            rotation += amount;
            if (rotation > (float)Math.PI * 2)
                rotation = 0;//((float)Math.PI * 2) - rotation;
            else if (rotation < 0)
                rotation = (float) Math.PI * 2;//((float)Math.PI * 2) + rotation;

            Matrix rotMatrix = Matrix.CreateRotationY(rotation);
            matWorld = rotMatrix * Matrix.CreateTranslation(position3);

        
            //rotate the normals
            for (int i = 0; i < 4; i++)
            {
                normals[i] = Vector3.TransformNormal(normals[i], rotMatrix);
            }
        }
        public void HighlightSquare(int index)
        {
            squares[index].SetColor(150, 36, 230);
        }

        public new void Draw()
        {
            segmentEffect.World = matWorld;
            segmentEffect.View = camera.view;
            segmentEffect.Projection = camera.projection;
            segmentEffect.DiffuseColor = color.ToVector3();

            device.SetVertexBuffer(vertexBuff);
        //    device.Indices = indexBuff;
            RasterizerState backupState = device.RasterizerState;
            device.RasterizerState = wireFrameRasterizer; 
       //   device.RasterizerState.FillMode = FillMode.WireFrame;
            foreach(EffectPass pass in segmentEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, numSquaresTotal*2 +9);//+2
            }
            device.RasterizerState = backupState;
        }
        protected void UpdateNormals()
        {
            Matrix rotMatrix = Matrix.CreateRotationY(rotation);
            //set the normals to the rotation
            for (int i = 0; i < 4; i++)
            {
                normals[i] = Vector3.TransformNormal(normals[i], rotMatrix);
            }
        }
        
        #endregion

        #region constructors
        /// <summary>
        /// numAcross: how many squares on one face.
        /// x, y, z: center position of the cube segment
        /// </summary>
        public CubeSegment(float squareWidth, int numAcross, Vector3 position)
        {
            position3 = position;

            Matrix rotMatrix = Matrix.CreateRotationY(rotation);
            matWorld = Matrix.CreateTranslation(position3) * rotMatrix;
            //setup normals for each side, from front to left-side
            normals = new Vector3[4];
            normals[0] = new Vector3(0, 0, -1);
            normals[1] = new Vector3(1, 0, 0);
            normals[2] = new Vector3(0, 0, 1);
            normals[3] = new Vector3(-1, 0, 0);
            segmentEffect = new BasicEffect(device); //initialize the basic effect
           
            ///--- Now initialize the vertex/index buffers ----
            numSquaresAcross = numAcross;
            numSquaresTotal = numAcross * 4;// total number of squares: create a 4-sided cube segment 

            //numVerts: total number of vertices
            //+ 2 to close up the polygons. +10 to make up the polygons for the top and bottom. only 8 would be needed, but 
            //we need to use 2 of them to generate "degenerate triangles". When using triangle strips, every point added uses 
            //the last 3 points to make a new triangle. We generate a degenerate triangle (one that is rejected by the pipeline)
            //by putting one in the same spot as the previous point. This allows us to move to the top/bottom of the cube segment
            //without generating extra geometry. 
            //int numVerts = (numSquaresTotal * 2) + 2 + 10;
            int numVertsForSquares = (numSquaresTotal * 2) + 2;
            //we use numVertsForSquares + 10, because we need vertices for top and bottom
            vertices = new VertexPositionColor[numVertsForSquares + 10];// + 5];
            squares = new MatchPiece[numSquaresTotal];
            float putX = -(squareWidth * (float)numAcross * 0.5f);
            float putY = 0;
            float putZ = squareWidth * (float)numAcross * 0.5f;
            
            //set up all vertices for squares, from front to left side
            int side = 0;

            //Setup the first 3
            vertices[0] = new VertexPositionColor(new Vector3(putX, putY - squareWidth, putZ), color);
            vertices[1] = new VertexPositionColor(new Vector3(putX, putY, putZ), color);
            vertices[2] = new VertexPositionColor(new Vector3(putX + squareWidth, putY - squareWidth, putZ), color);
            putX += squareWidth;

            float incX=squareWidth, incZ=0;
            int squaresCrossed = 0;
            int i = 3;
            for (; i < numVertsForSquares-1; i+=2)
            {
                vertices[i] = new VertexPositionColor(new Vector3(putX, putY, putZ), color);
                
                //is it time to change direction?
                squaresCrossed++;
                if (squaresCrossed == numSquaresAcross)
                {
                    side++;
                    squaresCrossed = 0;
                }
                switch (side)
                {
                    case 0:
                        incX = squareWidth;
                        incZ = 0;
                        break;
                    case 1:
                        incX = 0;
                        incZ = -squareWidth;
                        break;
                    case 2:
                        incX = -squareWidth;
                        incZ = 0;
                        break;
                    case 3:
                        incX = 0;
                        incZ = squareWidth;
                        break;
                    case 4:
                        continue;
                }
                putX += incX;
                putZ += incZ;
                
                vertices[i + 1] = new VertexPositionColor(new Vector3(putX, putY - squareWidth, putZ), color);
                //vertices[i + ].Position.X = putX;
                //vertices[i + totalSquares].Position.Y = putY + squareWidth;
                //vertices[i + totalSquares].Position.Z = putZ;
            }
            //add one last vertex
            int idx = numVertsForSquares - 1;
            float segWidth = squareWidth * numSquaresAcross;
            vertices[idx] = new VertexPositionColor(new Vector3(putX, putY, putZ), color);

            //add vertices for top and bottom
            vertices[++idx] = new VertexPositionColor(new Vector3(putX, putY, putZ), color);//degenerate triangle
            //vertices for top
            vertices[++idx] = new VertexPositionColor(new Vector3(putX, putY, putZ - segWidth), color);
            vertices[++idx] = new VertexPositionColor(new Vector3(putX + segWidth, putY, putZ), color);
            vertices[++idx] = new VertexPositionColor(new Vector3(putX + segWidth, putY, putZ - segWidth), color);
    //        vertices[++idx] = new VertexPositionColor(new Vector3(putX + segWidth, putY, putZ), color);

            //degenerate at triangle at top right
            vertices[++idx] = new VertexPositionColor(new Vector3(putX + segWidth, putY, putZ - segWidth), color);
            //another degenerate triangle at the bottom right
            vertices[++idx] = new VertexPositionColor(new Vector3(putX + segWidth, putY - squareWidth, putZ - segWidth), color);
            
            //another degenerate, and the start of an actual triangle.
            //Bottom vertices
            vertices[++idx] = new VertexPositionColor(new Vector3(putX + segWidth, putY - squareWidth, putZ - segWidth), color);
            vertices[++idx] = new VertexPositionColor(new Vector3(putX, putY - squareWidth, putZ - segWidth), color);
            vertices[++idx] = new VertexPositionColor(new Vector3(putX + segWidth, putY - squareWidth, putZ), color);
            vertices[++idx ] = new VertexPositionColor(new Vector3(putX, putY-segWidth, putZ), color);

            ////add all this to the vertex buffer.
            vertexBuff = new VertexBuffer(device, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
            vertexBuff.SetData<VertexPositionColor>(vertices);        
        }


        #endregion

    }
}
