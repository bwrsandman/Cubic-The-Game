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

        public GreatCube(int levels, int squaresAcross, float squareWidth, float x, float y, float z)
            :base(new Vector2(x, y))
        {
            cubeSegments = new CubeSegment[levels];
            float putY = y;
            for (int i = 0; i < levels; i++)
            {
                cubeSegments[i] = new CubeSegment(squareWidth, squaresAcross, x, putY, z);
                putY += squareWidth;
            }
        }

        public new void Draw()
        {
            for (int i = 0; i < cubeSegments.Length; i++)
            {
                cubeSegments[i].Draw(GameObject.camera);
            }
        }
        public void Update(float seconds)
        {
            //for now, rotate all segments at once
            for (int i = 0; i < cubeSegments.Length; i++)
            {
                cubeSegments[i].Rotate((float)(5 * (Math.PI / 180) * seconds)); 
            }
        }
        #endregion

        #region constructors
        public GreatCube()
            : base(new Vector2())
        {
        }
        #endregion


    }
}
