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
        protected override float midLen { get { return MIDLEN; } }
        #endregion

        public static void loadBuffers()
        {
            cubeBuffer = new VertexBuffer(GameObject.device, typeof(VertexPositionColorTexture), 4, BufferUsage.None);
            cubeEffect = new BasicEffect(GameObject.device);
        }

        public FallPiece(Vector2 origin)
        {
            interactingPlayer = -1;
            center3 = new Vector3(origin,DEPTH);

            Vector3 offset = midLen / 2 * new Vector3(-1f,1f,0f);
            Vector2[] textCoord = { new Vector2(0, 0), 
                                    new Vector2(1, 0), 
                                    new Vector2(0, 1), 
                                    new Vector2(1, 1) };
            Vector2[] posCoord = { new Vector2(0, 0), 
                                    new Vector2(1, 0), 
                                    new Vector2(0, -1), 
                                    new Vector2(1, -1) };
            cubeFront = new VertexPositionColorTexture[4];
            for (byte i = 0; i < 4; ++i)
                cubeFront[i] = new VertexPositionColorTexture(offset + midLen * new Vector3(posCoord[i], 0f), color, textCoord[i]);

            texture = generateTexture();          
        }

        #region members
        public override Matrix GetWorldTranslation { get { return Matrix.Identity; } }
        public override Vector3 GetCenter3 { get { return center3; } }
        private Matrix worldTranslation { get { return Matrix.CreateTranslation(center3); } }
        private Vector3 center3;

        VertexPositionColorTexture[] cubeFront;
        private bool grabbed;
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

    }
}
