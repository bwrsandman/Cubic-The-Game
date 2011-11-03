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

        public void Draw(Camera camera)
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
             //   device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, 0, vertices.Length, 0, numSquaresAcross);
                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, numSquaresTotal * 2);
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
            //since this will all wrap around, we'll need the same amount of vertices to make up the
            //top of the squares as we will have squares. so vertices * 2 to make up the bottom part as well
            int numVertsForSquares = (numSquaresTotal * 2) + 2;
            vertices = new VertexPositionColor[numVertsForSquares];
            squares = new MatchPiece[numSquaresTotal];
            float putX = -(squareWidth * (float)numAcross * 0.5f);
            float putY = 0;
            float putZ = -squareWidth * (float)numAcross * 0.5f;
            
            //set up all vertices for squares, from front to left side
            int side = 0;

            //Setup the first 3
            vertices[0] = new VertexPositionColor(new Vector3(putX, putY + squareWidth, putZ), color);
            vertices[1] = new VertexPositionColor(new Vector3(putX, putY, putZ), color);
            vertices[2] = new VertexPositionColor(new Vector3(putX + squareWidth, putY + squareWidth, putZ), color);
            putX += squareWidth;

            float incX=squareWidth, incZ=0;
            int squaresCrossed = 0;
            for (int i = 3; i < numVertsForSquares-1; i+=2)
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
                        incZ = squareWidth;
                        break;
                    case 2:
                        incX = -squareWidth;
                        incZ = 0;
                        break;
                    case 3:
                        incX = 0;
                        incZ = -squareWidth;
                        break;
                }
                putX += incX;
                putZ += incZ;
                
                
                vertices[i + 1] = new VertexPositionColor(new Vector3(putX, putY + squareWidth, putZ), color);
                //vertices[i + ].Position.X = putX;
                //vertices[i + totalSquares].Position.Y = putY + squareWidth;
                //vertices[i + totalSquares].Position.Z = putZ;
            }
        //    putX += incX;
     //       putZ += incZ;
            vertices[(numSquaresTotal * 2) + 1] = new VertexPositionColor(new Vector3(putX, putY, putZ), color);
            
            //add all this to the vertex buffer.
            vertexBuff = new VertexBuffer(device, typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
            vertexBuff.SetData<VertexPositionColor>(vertices);

             //Old, index buffer way:
            //for(int i = 0; i < totalSquares; i++)
            //{
            //    vertices[i].Position.X = putX;
            //    vertices[i].Position.Y = putY;
            //    vertices[i].Position.Z = putZ;

            //    //bottom row
            //    vertices[i + totalSquares].Position.X = putX;
            //    vertices[i + totalSquares].Position.Y = putY + squareWidth;
            //    vertices[i + totalSquares].Position.Z = putZ;
            //    if(i != 0 && i % numAcross == 0)
            //    {
            //        side++;
            //    }
            //    switch(side)
            //    {
            //        case 0:
            //            putX += squareWidth;
            //            break;
            //        case 1:
            //            putZ += squareWidth;
            //            break;
            //        case 2:
            //            putX -= squareWidth;
            //            break;
            //        case 3:
            //            putZ -= squareWidth;
            //            break;
            //    }
            //}


            //set up index buffer
       //     int total2 = totalSquares * 2;
       //     short[] idxBuff = new short[(totalSquares +1) * 2];// + 10

         //   int j = 0;

            //do the first 3 indices
            //The rest will be drawn going up, then over and down, then back up 
            //The drawing represents drawing order, not the actual numbering of the vertices.
            //Drawing order is done with indices, to tell the pipeline to draw a triangle strip,
            //At every point past the 2nd, a triangle is made with the last 3 indexed vertices.
            /*         1 ---3----5----7   
             *          |\  |  \  |\  |
             *          | \ |   \ | \ |
             *          |  \|    \|  \|
             *         0 ----2----4---6
             */
            //idxBuff[0] = (short)totalSquares;                      
            //idxBuff[1] = (short)0;
            //idxBuff[2] = (short)(totalSquares + 1);  
            //for (int i = 3; i < numSquaresTotal; i += 2)
            //{
            //    //idxBuff[i] = (short)i;

            //    int bRightIdx = i + numSquaresTotal + 1;
            //    ////make the bottom-right of the last square  use the bottom-left of the first square
            //    if (bRightIdx > vertices.Length - 1)
            //        bRightIdx = numSquaresTotal;
            //    //idxBuff[i + 1] = (short)bRightIdx;
            //    //top-right of last square uses top-left of first square
            //    //save these changes in the squares as well
            //  //  squares[i] = new MatchPiece(i, i + numSquaresTotal, bRightIdx, (i + 1) % numSquaresTotal, vertices);
            //}

         //   indexBuff = new IndexBuffer(device, typeof(short), idxBuff.Length, BufferUsage.WriteOnly);
         //   indexBuff.SetData<short>(idxBuff);

            //TODO: setup indices for top & bottom

 
        
        }


        #endregion

    }
}
