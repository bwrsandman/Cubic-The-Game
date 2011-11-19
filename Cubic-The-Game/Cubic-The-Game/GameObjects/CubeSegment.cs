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
        private const byte NUMSIDES = 4;
        #endregion

        #region statics
        /// <summary>
        /// Each segment can share one:
        ///     - Rotation
        ///     - Number of Match Pieces
        ///     - Match Pieces Size
        ///     - colour
        ///     - vertex buffer
        ///     - effects
        ///     - Raster
        /// </summary>
        private static float rotation;     //represents how much the slab is rotated
        private static int numSquaresAcross;
        private static float squareWidth;
        private static Color color = new Color(157, 19, 168);
        private static VertexBuffer topBuff, bottomBuff;
        private static BasicEffect segmentEffect = new BasicEffect(device);
        private static RasterizerState wireFrameRasterizer = new RasterizerState { CullMode = CullMode.None, FillMode = FillMode.WireFrame };
        private static RasterizerState solidRasterizer = new RasterizerState { CullMode = CullMode.CullCounterClockwiseFace, FillMode = FillMode.Solid };
        #endregion

        #region members
        /// <summary>
        ///  Each separate segment has a distinct:
        ///  direction (isForward)
        ///  MatchPieces
        ///  Position
        ///  world Matrix
        ///  Top and bottom squares; 2*2=4 triangles
        /// </summary>
        private bool isForward;         // Does the segment rotate CW or CCW
        private MatchPiece[] squares;
        private Vector3[] normals;
        private Vector3 position3;   //TODO: replace with GameObject.Position, once that is a Vector3
        private Matrix matWorld;
        //      private IndexBuffer indexBuff;
        
        #endregion

        #region accessors
        private float rotationPrime
        {
            get
            {
                // This non linearality gives the cube a little personality
                // TODO, add randomness
                // theta + Sin (-2Theta)^16
                return ((isForward) ? 1 : -1)*(float)(rotation + (Math.Pow(Math.Sin(rotation * -2), 16.0)));
            }
        }
        private static int numSquaresTotal { get { return numSquaresAcross * NUMSIDES; } }
        #endregion

        #region constructors
        /// <summary>
        /// When Segment is created, we:
        ///     - determine directions, position
        ///     - Set top and bottom Faces with normals facing opposite directions
        ///     - Create MatchPieces
        /// numAcross: how many squares on one face.
        /// x, y, z: center position of the cube segment
        /// </summary>
        
        public static void LoadStaticContent(int sa, float sw)
        {
            numSquaresAcross = sa;
            squareWidth = sw;
        }
        public CubeSegment(Vector3 position3, bool isForward)
        {
            
            this.isForward = isForward;
            this.position3 = position3;
            matWorld = Matrix.CreateTranslation(position3);
            squares = new MatchPiece[numSquaresTotal];
            for (int i = 0; i < numSquaresTotal; ++i)
                squares[i] = new MatchPiece(i % numSquaresAcross - (numSquaresAcross / 2.0f), squareWidth, i / numSquaresAcross, (numSquaresAcross / 2.0f));
      
            //setup normals for each side, from front to left-side
            normals = new Vector3[4];
            normals[0] = new Vector3(0, 0, -1);
            normals[1] = new Vector3(1, 0, 0);
            normals[2] = new Vector3(0, 0, 1);
            normals[3] = new Vector3(-1, 0, 0);

            float segWidth = numSquaresAcross* squareWidth/2;
            
            //top Cap
            VertexPositionColor[] top = new VertexPositionColor[4];
            top[0] = new VertexPositionColor(new Vector3(segWidth, 0, -segWidth), color);
            top[1] = new VertexPositionColor(new Vector3(segWidth, 0, segWidth), color);
            top[2] = new VertexPositionColor(new Vector3(-segWidth, 0, -segWidth), color);
            top[3] = new VertexPositionColor(new Vector3(-segWidth, 0, segWidth), color);
            topBuff = new VertexBuffer(device, typeof(VertexPositionColor), top.Length, BufferUsage.WriteOnly);
            topBuff.SetData<VertexPositionColor>(top);

            //bottom Cap
            VertexPositionColor[] bottom = new VertexPositionColor[4];
            bottom[0] = new VertexPositionColor(new Vector3(segWidth, -squareWidth, segWidth), color);
            bottom[1] = new VertexPositionColor(new Vector3(segWidth, -squareWidth, -segWidth), color);
            bottom[2] = new VertexPositionColor(new Vector3(-segWidth, -squareWidth, segWidth), color);
            bottom[3] = new VertexPositionColor(new Vector3(-segWidth, -squareWidth, -segWidth), color);
            bottomBuff = new VertexBuffer(device, typeof(VertexPositionColor), bottom.Length, BufferUsage.WriteOnly);
            bottomBuff.SetData<VertexPositionColor>(bottom);   

        }


        #endregion

        #region update and draw
        public void Rotate(float amount)
        {
            // rotation only increases, rotation prime takes care of the rest
            rotation += amount;
            if (rotation > (float)Math.PI * 2)
                rotation -= (float)Math.PI * 2;

            Matrix rotMatrix = Matrix.CreateRotationY(rotationPrime);
            matWorld = rotMatrix * Matrix.CreateTranslation(position3);


            //rotate the normals
            for (int i = 0; i < 4; i++)
            {
                normals[i] = Vector3.TransformNormal(normals[i], rotMatrix);
            }

            // Update the segments
            foreach (MatchPiece piece in squares)
                if (piece != null)
                    piece.Update();
        }

        public void intersects(Player[] players)
        {
            foreach (MatchPiece piece in squares)
                piece.intersects(players);
        }
        public new void Draw()
        {
            segmentEffect.World = matWorld;
            segmentEffect.View = camera.view;
            segmentEffect.Projection = camera.projection;
            RasterizerState backupState = device.RasterizerState;

            //draw top
            DrawCap(true);
            //draw bottom
            DrawCap(false);

            device.RasterizerState = backupState;
        }

        private void DrawCap(bool top)
        {
            //Draw top square
            device.SetVertexBuffer((top) ? topBuff : bottomBuff);
            segmentEffect.DiffuseColor = Color.White.ToVector3();

            //    device.Indices = indexBuff;
            device.RasterizerState = solidRasterizer;
            //   device.RasterizerState.FillMode = FillMode.WireFrame;
            foreach (EffectPass pass in segmentEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);//+2
            }

            segmentEffect.DiffuseColor = color.ToVector3();
            device.RasterizerState = wireFrameRasterizer;
            //   device.RasterizerState.FillMode = FillMode.WireFrame;
            foreach (EffectPass pass in segmentEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);//+2
            }
            //end top
        }

        public void DrawPieces()
        {
            foreach (MatchPiece piece in squares)
                if (piece != null)
                    piece.Draw(camera, matWorld);
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

    }
}
