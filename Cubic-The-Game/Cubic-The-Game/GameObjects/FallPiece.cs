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
        private const float MIDLEN = 1f;
        private const float DEPTH = -5f;
        private const float FALLSPEED = -0.05f;
        private const float OFFSCREENOFFSET = 5f;
        #endregion

        #region statics
        private readonly static Color inactiveColor = Color.White;
        #endregion

        #region members
        private Vector3 movement = Vector3.Zero;
        #endregion

        public static void loadBuffers()
        {
            cubeBuffer = new VertexBuffer(GameObject.device, typeof(VertexPositionColorTexture), 4, BufferUsage.None);
            cubeEffect = new BasicEffect(GameObject.device);
        }

        public FallPiece(Vector2 origin)
            : base(MIDLEN / 2 * new Vector3(-1f, 1f, 0f), MIDLEN)
        {
            interactingPlayer = -1;
            position3 = new Vector3(origin,DEPTH);   
        }

        #region members
        public override Matrix GetWorldTranslation { get { return Matrix.Identity; } }
            public override Vector3 GetCenter3 { get { return position3; } }
        private Matrix worldTranslation { get { return Matrix.CreateTranslation(position3); } }

        private bool grabbed;
        private bool expired;
        #endregion

        #region update and draw
        public bool Grab(Player grabbingPlayer)
        {
            return grabbed = !grabbed && intersects(grabbingPlayer.center);
        }

        public bool Drop(Player grabbingPlayer)
        {
            //if (grabbed && intersects(grabbingPlayer.center))
            //{
                grabbed = false;
                interactingPlayer = -1;
            //}
            return false;
        }


        public void Move(Vector2 tohere)
        {
            // TODO: convert thismuch from screenspace back into worldSpace
            //movement += new Vector3(thismuch.X, -thismuch.Y, 0) / 45.0f;
            Move(GetWorldSpace(tohere,position3.Z,GetWorldTranslation));
        }
        public void Move(Vector3 tohere)
        {
            // TODO: convert thismuch from screenspace back into worldSpace
            tohere *= 15;
            float Zdiff = position3.Z - tohere.Z;
            Matrix Zfix = Matrix.CreateTranslation(Zdiff*Vector3.UnitZ);
            position3 =  Vector3.Transform(tohere, Zfix);
        }

        public bool IsExpired()
        {
            return (center2.Y > screenSize.Y + OFFSCREENOFFSET)||expired;
        }

        protected override void Update()
        {
            if (grabbed) ;
            //position3 = movement;
            else
                position3 += new Vector3(0.0f, FALLSPEED, 0.0f);
            movement = Vector3.Zero;
            color = isIntersected? interactedColor : inactiveColor ;
        }

        public void Draw(Camera camera)
        {
            GameObject.device.SetVertexBuffer(cubeBuffer);

            cubeEffect.World = worldTranslation;
            cubeEffect.View = camera.view;
            cubeEffect.Projection = camera.projection;
            cubeEffect.DiffuseColor = color.ToVector3();
            //cubeEffect.DiffuseColor = Color.Red.ToVector3();
            cubeEffect.TextureEnabled = true;
            cubeEffect.Texture = texture;


            foreach (EffectPass pass in cubeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GameObject.device.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleStrip, cubeFront, 0, 2);

            }


        }

        #endregion

        internal bool Match(int id)
        {
            return this.pieceID == id;
        }

        internal void Expire()
        {
            expired=true;
        }

        public bool intersects(Player[] players)
        {
            if (isIntersected && interactingPlayer >= 0 && intersects(players[interactingPlayer].center)) ; // yeah this is blank... for now
            else
            {
                isIntersected = false;
                interactingPlayer = -1;
                for (byte i = 0; i < players.Length; ++i)
                    if (players[i] != null)
                        if (intersects(players[i].center))
                        {
                            isIntersected = true;
                            interactingPlayer = i;
                            players[i].Attach(this);
                            interactedColor = players[i].fadedColor;
                            break;
                        }
            }
            return isIntersected;
        }
    }
}
