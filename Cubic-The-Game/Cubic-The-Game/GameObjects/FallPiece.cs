#region description
//-----------------------------------------------------------------------------
// FallPiece.cs
//
// Written by bwrsandman(Sandy)
//-----------------------------------------------------------------------------
#endregion


#region using
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Cubic_The_Game
{
    class FallPiece : Piece
    {
        #region constants
        private const float OFFSET = 50f;
        private const float SIDERADIUS = 0.5f;
        private const float DEPTH = -5f;
        private const float FALLSPEED = -0.05f;
        private const float OFFSCREENOFFSET = 5f;
        #endregion

        #region statics
        private readonly static Color inactiveColor = Color.White;
        #endregion

        #region members
        public int interactingPlayer{get; private set;}
        private Color color;
        private Color interactedColor;
        private Vector3 movement = Vector3.Zero;
        #endregion

        public FallPiece(Vector2 origin)
        {
            interactingPlayer = -1;
            center3 = new Vector3(origin,DEPTH);

            cubeFront = new VertexPositionColor[4];

            cubeBuffer = new VertexBuffer(GameObject.device, typeof(VertexPositionColor), cubeFront.Length, BufferUsage.None);
            cubeEffect = new BasicEffect(GameObject.device);
        }

        #region members
        public Matrix worldTranslation = Matrix.CreateTranslation(0, 0, 2);
        public Vector3 center3;
        public Vector2 center2 
        {
            get { return GameObject.GetScreenSpace(center3, worldTranslation); } 
        }
        VertexPositionColor[] cubeFront;
        VertexBuffer cubeBuffer;
        BasicEffect cubeEffect;
        private bool isIntersected;
        private bool grabbed;
        #endregion

        #region update and draw
        public bool intersects(Vector2 cntr)
        {
            return ((center2 - cntr).Length() <= OFFSET);
        }
        public bool Grab(Player grabbingPlayer, bool grabDrop)
        {
            if (grabDrop) // if Grabbing
            {
                if (!grabbed && intersects(grabbingPlayer.center))
                {
                    return grabbed = true;
                }
            }
            else //if dropping
                if (grabbed && interactingPlayer == grabbingPlayer.index)
                {
                    grabbed = false;
                    interactingPlayer = -1;
                }
            return false;
        }
        public bool intersects(Player[] players)
        {
            if (isIntersected && interactingPlayer >= 0 && intersects(players[interactingPlayer].center));
            else {
                isIntersected = false;
                interactingPlayer = -1;
                for (byte i = 0; i<players.Length; ++i) if (players[i]!=null)
                    if (intersects(players[i].center))
                    {
                        isIntersected = true;
                        interactingPlayer = i;
                        players[i].Attach(this);
                        interactedColor = new Color(players[i].color.R/4+128, players[i].color.G/4+128, players[i].color.B/4+128);
                        break;
                    }
            }
            return isIntersected;
        }

        public void Move(Vector2 thismuch)
        {
            // TODO: convert thismuch from screenspace back into worldSpace
            movement += new Vector3(thismuch.X, -thismuch.Y, 0)/45.0f;
        }

        public bool OutOfBounds()
        {
            return (center2.Y > screenSize.Y + OFFSCREENOFFSET);
        }

        protected override void Update()
        {
            if (grabbed)
                center3 += movement;
            else
                center3 += new Vector3(0.0f, FALLSPEED, 0.0f);
            movement = Vector3.Zero;
            color = isIntersected? interactedColor : inactiveColor ;
            cubeFront[0] = new VertexPositionColor(new Vector3(-SIDERADIUS, SIDERADIUS, 0) + center3, color);
            cubeFront[1] = new VertexPositionColor(new Vector3(SIDERADIUS, SIDERADIUS, 0) + center3, color);
            cubeFront[2] = new VertexPositionColor(new Vector3(-SIDERADIUS, -SIDERADIUS, 0) + center3, color);
            cubeFront[3] = new VertexPositionColor(new Vector3(SIDERADIUS, -SIDERADIUS, 0) + center3, color);
        }

        public void Draw(Camera camera)
        {
            GameObject.device.SetVertexBuffer(cubeBuffer);

            cubeEffect.World = worldTranslation;
            cubeEffect.View = camera.view;
            cubeEffect.Projection = camera.projection;
            cubeEffect.DiffuseColor = color.ToVector3();


            foreach (EffectPass pass in cubeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GameObject.device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, cubeFront, 0, 2);

            }


        }

        #endregion

    }
}
