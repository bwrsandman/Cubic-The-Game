﻿#region description
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
        readonly Color inactiveColor = new Color(1,14,33); //black
        readonly Color backColor = Color.White; //gray
        #endregion

        #region statics
        #endregion

        #region members
        private float rotOffset;

        public override Matrix GetWorldTranslation { get { return worldTranslation; } }
        private Matrix worldTranslation = Matrix.CreateTranslation(0, 0, 2);
        public override Vector3 GetCenter3 { get { return new Vector3(position3.X + midLen / 2, position3.Y - midLen / 2, position3.Z); } }
        private Vector3 position3;


        //private Color[] playerColors;
        private bool[] playersSelecting; //players that are hovering over this
        /// <summary>
        /// When a Match Piece is created:
        ///     - Randomly select an identity (texture)
        ///     - Set its facing direction (front, side, back)
        ///     - Set it's offset from the middle
        ///     - Create a backsurface and front texture key
        /// </summary>
        public MatchPiece(float XOffset, float size, int facingDirection, float ZOffset)
            : base(new Vector3(XOffset, 0.0f, ZOffset), size)
        {
            position3 = new Vector3(XOffset, 0, 0);
            rotOffset = (float)(facingDirection * Math.PI / 2.0);

            //initialize the world transform, for drawing and collision detection
         //   worldTranslation = Matrix.CreateRotationY(rotOffset) * Matrix.CreateTranslation(position3);
            playersSelecting = new bool[MAXPLAYERS];
        }

        #endregion

        #region update and draw
        public void Draw(Camera camera, Matrix matWorld)
        {
            GameObject.device.SetVertexBuffer(cubeBuffer);
            
            Matrix rotMatrix = Matrix.CreateRotationY(rotOffset);
            this.worldTranslation = rotMatrix * matWorld;

            cubeEffect.World = worldTranslation;
            cubeEffect.View = camera.view;
            cubeEffect.Projection = camera.projection;
            
            // Render background of Square
            cubeEffect.DiffuseColor = color.ToVector3();
            cubeEffect.TextureEnabled = true;
            cubeEffect.Texture = cubeTex;

            cubeEffect.DiffuseColor = backColor.ToVector3();
            foreach (EffectPass pass in cubeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GameObject.device.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleStrip, cubeFront, 0, 2);

            }


            // Render foreground of Square with shape
            cubeEffect.DiffuseColor = color.ToVector3();
            cubeEffect.TextureEnabled = true;
            cubeEffect.Texture = texture;


            foreach (EffectPass pass in cubeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GameObject.device.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleStrip, cubeFront, 0, 2);

            }

        }

        public bool intersects(Player player, Matrix matWorld)
        {
            Vector2[] polygon = new Vector2[cubeFront.Length];
            for (int i = 0; i < cubeFront.Length; i++)
               polygon[i] = GameObject.GetScreenSpace(cubeFront[i].Position, worldTranslation);

            if (GlobalFuncs.PointInPolygonCollision2D(player.center, polygon))
            {
                playersSelecting[player.index] = true;
                interactedColor = new Color(player.color.R / 4 + 128, player.color.G / 4 + 128, player.color.B / 4 + 128);
            }
            else
                playersSelecting[player.index] = false;

            return playersSelecting[player.index];
        }

        public new void Update()
        {
            //color = isIntersected ? interactedColor : inactiveColor;
            bool someoneSelecting = false;
            for (int i = 0; i < playersSelecting.Length; i++)
            {
                if (playersSelecting[i])
                    someoneSelecting = true;
            }
            color = someoneSelecting ? interactedColor : inactiveColor;

            for(int i=0; i < cubeFront.Length; i++)
                cubeFront[i].Color = color;
        }
        #endregion
    }
}
