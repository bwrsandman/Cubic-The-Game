using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Cubic_The_Game
{
    class TestCube
    {

        public static GraphicsDevice device;
        
        public Matrix worldTranslation = Matrix.CreateTranslation(0,0,2);
        public Color color = Color.White;


        public Vector3 normal;
        public Vector3 center;

        float offset = 5f;

        VertexPositionColor[] cubeFront;
        VertexBuffer cubeBuffer;
        BasicEffect cubeEffect;


        public TestCube()
        {
            Vector3 offcenter = new Vector3(5,2,-10);
            cubeFront = new VertexPositionColor[4];
            cubeFront[0] = new VertexPositionColor(new Vector3(-2, 2, 0) + offcenter, color);
            cubeFront[1] = new VertexPositionColor(new Vector3(2, 2, 0) + offcenter, color);
            cubeFront[2] = new VertexPositionColor(new Vector3(-2, -2, 0) + offcenter, color);
            cubeFront[3] = new VertexPositionColor(new Vector3(2, -2, 0) + offcenter, color);

            //Vector3 v1 = cubeFront[1].Position - cubeFront[0].Position;
            //Vector3 v2 = cubeFront[2].Position - cubeFront[0].Position;
            center = offcenter;
            //center = new Vector3(worldTranslation.Translation.X, worldTranslation.Translation.Y, worldTranslation.Translation.Z);

            getNormal();

            cubeBuffer = new VertexBuffer(device, typeof(VertexPositionColor), cubeFront.Length, BufferUsage.None);
            cubeEffect = new BasicEffect(device);

        }

        #region calculateNormal
        public void getNormal()
        {
            Vector3 v1 = Vector3.Transform(cubeFront[0].Position, worldTranslation);
            Vector3 v2 = Vector3.Transform(cubeFront[1].Position, worldTranslation);
            Vector3 v3 = Vector3.Transform(cubeFront[2].Position, worldTranslation);
            Vector3 vA = v2-v1;
            Vector3 vB = v3-v1;
            normal = Vector3.Cross(vA, vB);
            normal.Normalize();
        }


        public bool intersects(Vector2 cntr)
        {
            Vector2 cubeScreenCenter = GameObject.GetScreenSpace(center, worldTranslation);
            Vector2 cntrDifference = cubeScreenCenter - cntr;
            
            return (cntrDifference.Length() <= offset);     
            
        }
        #endregion


        #region update
        public void Update()
        {    
        }
        #endregion

        #region draw
        public void Draw(Camera camera)
        {
            device.SetVertexBuffer(cubeBuffer);

            cubeEffect.World = worldTranslation;
            cubeEffect.View = camera.view;
            cubeEffect.Projection = camera.projection;
            cubeEffect.DiffuseColor = color.ToVector3();
            

            foreach (EffectPass pass in cubeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, cubeFront, 0, 2);

            }

 
        }
        #endregion
    }
}