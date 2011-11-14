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
                cubeSegments[i].Draw();
            }
        }
        public void Update(float seconds)
        {
            //for now, rotate all segments at once
            float speed = 15;
            for (int i = 0; i < cubeSegments.Length; i++)
            {
                cubeSegments[i].Rotate((float)(speed * (Math.PI / 180) * seconds));
                //speed += 1;
                //speed *= -1;
            }
        }
        #endregion

        #region constructors
        public GreatCube(int levels, int squaresAcross, float squareWidth, Vector3 position)
            : base()
        {
            cubeSegments = new CubeSegment[levels];
            position3 = position;
            for (int i = 0; i < levels; i++)
            {
                cubeSegments[i] = new CubeSegment(squareWidth, squaresAcross, new Vector3(position.X, position.Y + i * squareWidth, position.Z));
            }
        }
        #endregion


    }
}
