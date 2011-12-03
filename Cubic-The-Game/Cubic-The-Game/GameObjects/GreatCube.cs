#region description
//-----------------------------------------------------------------------------
// GreatCube.cs
//
// Written by bwrsandman(Sandy)
//-----------------------------------------------------------------------------
#endregion


#region using
using System;
using Microsoft.Xna.Framework;            //  for Color, Vectors

#endregion

namespace Cubic_The_Game
{
    class GreatCube : GameObject
    {

        #region constants
        float ROT_SPEED = 2.5f;
        #endregion

        #region statics
        #endregion

        #region members
        CubeSegment[] cubeSegments;
        /// <summary>
        /// x, y, z: the top center of the cube
        /// </summary>
        /// <param name="levels"></param>
        /// <param name="squaresAcross"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        protected Vector3 position3;
        #endregion

        #region update and draw
        protected override void Draw()
        {
            for (int i = 0; i < cubeSegments.Length; i++)
            {
                cubeSegments[i].DrawPieces();
                cubeSegments[i].Draw();
            }
        }
        public void Update(float seconds)
        {
            //for now, rotate all segments at once
            for (int i = 0; i < cubeSegments.Length; i++)
            {
                //if((int)elapsedTime %7 != 0)
                cubeSegments[i].Rotate((float)(ROT_SPEED * (Math.PI / 180) * seconds));
                cubeSegments[i].Update(seconds);
                //speed += 1;
                //speed *= -1;
            }
        }

        public void intersects(Player[] players)
        {
            //foreach (CubeSegment segment in cubeSegments)
            for(int i=0; i < cubeSegments.Length; i++)
                cubeSegments[i].intersects(players);
        }
        #endregion

        #region constructors
        public GreatCube(int levels, int squaresAcross, float squareWidth, Vector3 position)
            : base()
        {
            CubeSegment.LoadStaticContent(squaresAcross, squareWidth);
            cubeSegments = new CubeSegment[levels];
            position3 = position;
            for (int i = 0; i < levels; i++)
            {
                cubeSegments[i] = new CubeSegment(new Vector3(position.X, position.Y - (i * squareWidth), position.Z), i%2 == 0);
            }
        }
        #endregion


    }
}
