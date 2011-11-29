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
using System.Diagnostics;
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
        public int[] owner { private set; get; }
        public bool[] isUnlocked { private set; get; }
        public Color[] colorOverlay { private set; get; }
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
            this.owner = new int[4];
            this.isUnlocked = new bool[4];
            this.colorOverlay = new Color[4];
            for (int i = 0; i < 4; ++i)
            {
                this.owner[i] = -1;
                this.isUnlocked[i] = true;
                this.colorOverlay[i] = Color.White;
            }
            this.isForward = isForward;
            this.position3 = position3;
            matWorld = Matrix.CreateTranslation(position3);


            squares = new MatchPiece[numSquaresTotal];
            for (int i = 0; i < numSquaresTotal; ++i)
              squares[i] = new MatchPiece(this, i % numSquaresAcross - (numSquaresAcross / 2.0f), squareWidth, i / numSquaresAcross, (numSquaresAcross / 2.0f));
      
            //setup normals for each side, from front to left-side
            normals = new Vector3[4];
            normals[0] = new Vector3(0, 0, 1);
            normals[1] = new Vector3(1, 0, 0);
            normals[2] = new Vector3(0, 0, -1);
            normals[3] = new Vector3(-1, 0, 0);


            //DEBUG - test
            //Matrix rotMatrix = Matrix.CreateRotationY((float)MathHelper.PiOver4);
            //matWorld = rotMatrix * Matrix.CreateTranslation(position3);
            ////rotate the normals
            //for (int i = 0; i < 4; i++)
            //{
            //    normals[i] = Vector3.TransformNormal(normals[i], rotMatrix);
            //}
            ////end debug


            float segWidth = numSquaresAcross* squareWidth/2;
            
            //top Cap
            VertexPositionColorTexture[] top = new VertexPositionColorTexture[4];
            top[0] = new VertexPositionColorTexture(new Vector3(segWidth, 0, -segWidth), color, new Vector2(0,0));
            top[1] = new VertexPositionColorTexture(new Vector3(segWidth, 0, segWidth), color, new Vector2(1,0));
            top[2] = new VertexPositionColorTexture(new Vector3(-segWidth, 0, -segWidth), color, new Vector2(0,1));
            top[3] = new VertexPositionColorTexture(new Vector3(-segWidth, 0, segWidth), color, new Vector2(1,1));
            topBuff = new VertexBuffer(device, typeof(VertexPositionColorTexture), top.Length, BufferUsage.WriteOnly);
            topBuff.SetData<VertexPositionColorTexture>(top);

            //bottom Cap
            VertexPositionColorTexture[] bottom = new VertexPositionColorTexture[4];
            bottom[0] = new VertexPositionColorTexture(new Vector3(segWidth, -squareWidth, segWidth), color, new Vector2(0, 0));
            bottom[1] = new VertexPositionColorTexture(new Vector3(segWidth, -squareWidth, -segWidth), color, new Vector2(1, 0));
            bottom[2] = new VertexPositionColorTexture(new Vector3(-segWidth, -squareWidth, segWidth), color, new Vector2(0, 1));
            bottom[3] = new VertexPositionColorTexture(new Vector3(-segWidth, -squareWidth, -segWidth), color, new Vector2(1, 1));
            bottomBuff = new VertexBuffer(device, typeof(VertexPositionColorTexture), bottom.Length, BufferUsage.WriteOnly);
            bottomBuff.SetData<VertexPositionColorTexture>(bottom);   
            
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
        }

        public void Update(float seconds)
        {
            // Update the segments
            foreach (MatchPiece piece in squares)
                if (piece != null)
                    piece.Update();
            for (int i = 0; i < 4; ++i)
                if (owner[i] != -1) ;
                    //GameObject.Score(owner[i], GameObject.LOCKEDPPSPS * numSquaresAcross*seconds);

        }


        public void intersects(Player[] players)
        {
       //     Vector3 worldPos = device.Viewport.Unproject(new Vector3((float)players[0].center.X, (float)players[0].center.Y, 0),
       //                                                         camera.projection, camera.view, Matrix.Identity);

            Vector3 worldPos = device.Viewport.Unproject(new Vector3(0, 0, 0), camera.projection, camera.view, Matrix.Identity);
            Ray ray = new Ray(worldPos, -camera.view.Forward);
            Vector3 norm = ray.Direction;

            int j = 0;
            for (int i = 0; i < normals.Length; i++)
            {
                //if the dot product between them is > 0, then the angle must be < 90 degrees, so 
                //this face is valid.
                if ((normals[i].X * norm.X) + (normals[i].Y * norm.Y) + (normals[i].Z * norm.Z) > 0)
                {
                    //test each square on this row
                    for (; j < (i + 1) * numSquaresAcross; j++)
                    {
                        foreach (Player p in players)
                        {
                            if (p != null)
                                squares[j].intersects(p);
                        }
                    }
                }
                else
                {
                    //(i+1) * numSquaresAcross: since the squares are set up in order, if i = 0, and there's 6
                    //squares across, we'll be skipping to index 6, the first square on the next side, which has a different normal
                    if (i < normals.Length - 1)
                        j = (i + 1) * numSquaresAcross;
                }

            }


            //foreach (MatchPiece piece in squares)
            //{
            //    //  foreach (Player p in players)
            //    piece.intersects(players);
            //}
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
            //segmentEffect.DiffuseColor = colorOverlay.ToVector3();
            segmentEffect.TextureEnabled = true;
            segmentEffect.Texture = cubeTex;

            //    device.Indices = indexBuff;
            device.RasterizerState = solidRasterizer;
            //   device.RasterizerState.FillMode = FillMode.WireFrame;
            foreach (EffectPass pass in segmentEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);//+2
            }

            //segmentEffect.DiffuseColor = color.ToVector3();
            //device.RasterizerState = wireFrameRasterizer;
            ////   device.RasterizerState.FillMode = FillMode.WireFrame;
            //foreach (EffectPass pass in segmentEffect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();
            //    device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);//+2
            //}
            //end top
        }

        public void DrawPieces()
        {
            foreach (MatchPiece piece in squares)
                if (piece != null) 
                    piece.Draw(camera, matWorld);        
        }



        //protected void UpdateNormals()
        //{
        //    Matrix rotMatrix = Matrix.CreateRotationY(rotation);
        //    //set the normals to the rotation
        //    for (int i = 0; i < 4; i++)
        //    {
        //        normals[i] = Vector3.TransformNormal(normals[i], rotMatrix);
        //    }
        //}
        #endregion 

    
        internal void FlipTo(Player player, int i)
        {
            GameObject.Score(player.index, GameObject.MATCHPOINTS);
            if (owner[i] != player.index)
            {
                // Row has been stolen
                // Allocate points
                GameObject.Score(player.index, GameObject.STEALPOINTS);
                owner[i] = player.index;
                isUnlocked[i] = false;
                colorOverlay[i] = player.fadedColor;
            }
            else GameObject.Score(player.index, GameObject.LOCKPOINTS);
            
            int numMatched = 1;
            for (int j = i*numSquaresAcross; j < (i+1)*numSquaresAcross; ++j)
            {
                if (!squares[j].isVirgin)
                {
                    squares[j].FlipVirgin(player);
                    ++numMatched;
                }
            }
            Debug.WriteLine(numMatched + "/" + numSquaresAcross);
            if (numMatched == numSquaresAcross)
            {
                // Player has filled in an entire row, reset side
                // Allocate points
                GameObject.Score(player.index, GameObject.COMPLETEPOINTS);
                ResetSide(i);
            }
        }

        public bool CanFlip(Player player, int i, bool affectedPieceIsVirgin)
        {
            return isUnlocked[i] || (owner[i] == player.index ^ !affectedPieceIsVirgin);
        }

        private void ResetSide(int j)
        {
            for (int i = j*numSquaresAcross; i < (j+1)*numSquaresAcross; ++i)
                squares[i] = new MatchPiece(this, i % numSquaresAcross - (numSquaresAcross / 2.0f), squareWidth, i / numSquaresAcross, (numSquaresAcross / 2.0f));
            owner[j] = -1;
            isUnlocked[j] = true;
            colorOverlay[j] = Color.White;
        }
    }
}
