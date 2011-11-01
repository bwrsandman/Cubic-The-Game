#region description
//-----------------------------------------------------------------------------
// GameObject.cs
//
// Written by bwrsandman(Sandy)
//-----------------------------------------------------------------------------
#endregion


#region using
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;   // for Texture2D
using Microsoft.Xna.Framework;            // for Vectors
using System.Collections.Generic;         // For List
using System.Diagnostics;                 // For debug
#endregion

namespace Cubic_The_Game
{
    #region custom helper classes
    /// <summary>
    /// My own little class used to represent 2D integers and do simple operations on them
    /// </summary>
    class TwoInt 
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public TwoInt(int X, int Y) { this.X = X; this.Y = Y; }
        public static TwoInt operator +(TwoInt lhs, TwoInt rhs) { return new TwoInt(lhs.X + rhs.X, lhs.Y + rhs.Y); }
        public static TwoInt operator -(TwoInt rhs) { return new TwoInt(-rhs.X, -rhs.Y); }
        public static TwoInt operator -(TwoInt lhs, TwoInt rhs) { return lhs + -rhs; }
        public static TwoInt operator *(TwoInt lhs, float rhs) { return new TwoInt((int)(lhs.X * rhs), (int)(lhs.Y * rhs)); }
        public static TwoInt operator *(float lhs, TwoInt rhs) { return rhs * lhs; }
        public static TwoInt operator /(TwoInt lhs, float rhs) { return new TwoInt((int)(lhs.X / rhs), (int)(lhs.Y / rhs)); }
        public static implicit operator Vector2(TwoInt rhs) { return new Vector2(rhs.X, rhs.Y); }

    }
    #endregion

    /// <summary>
    /// This is the top superclass of any game object inside of the game
    /// Serves as an organized grouping of repeated calls and static calls
    /// Anything that is global to ALL game objects go here, otherwise in another abstarct subclass
    /// </summary>
    abstract class GameObject
    {

        #region constants
        public const byte MAXPLAYERS = 4;
        public const float PLAYERSPEED = 2.0f;
        #endregion

        #region statics
        private static Player[] players;
        private static TwoInt screenSize;
        public static GraphicsDevice device;
        public static SpriteBatch spriteBatch{protected get; set;}
        public static List<byte> playerList { private set; get; }
        
        #endregion

        #region members
        protected Vector2 position, size; // soon to be Vector3
        #endregion

        #region accessors
        public Vector2 center //  object center = object position - 1/2 object size, if scaled you have to adjust size or to keep track of the scale (getScale)
        {
            get { return new Vector2(position.X + size.X /* * getScale*/ / 2, position.Y + size.Y /* * getScale*/ / 2); }
            protected set { position = new Vector2(value.X - size.X /* * getScale*/ / 2, value.Y - size.Y /* * getScale*/ / 2); }
        }
        #endregion

        #region constructors
        public GameObject(Vector2 position)
        {
            this.position = position;
        }
        #endregion

        #region load
        /// <summary>
        /// Anything that is used only once for an entire class, such as a texture can be loaded here
        /// </summary>
        public static void LoadStaticContent(ContentManager content)
        {
            Player.texture = content.Load<Texture2D>("playerCursor");
        }

        public static void NewGame(TwoInt sSize)
        {
            screenSize = sSize;
            players = new Player[MAXPLAYERS];
            playerList = new List<byte>(4); 
        }
        /// <summary>
        /// Here we're indicating the colour of the player and the input(gamepad/keyboard) index
        /// 0 <= index < MAXPLAYERS
        /// </summary>
        public static void AddPlayer(byte index, Color colour)
        {
            Debug.Assert(!playerList.Contains(index), "Cannot add two players with the same index (index=" + index + ")");
            Debug.Assert(index < MAXPLAYERS, "Cannot add a player with an index greater or equal to the maximum allowed players (index=" + index + ">=" + MAXPLAYERS + ")");
            playerList.Add(index);
            players[index] = new Player(screenSize / 2 + new TwoInt(-100 + index % 2 * 200, 100 * (byte)(index / 2)), colour);
        }
        public static void AddPlayer(byte index)
        {
            Debug.Assert(index < MAXPLAYERS, "Cannot add a player with an index greater or equal to the maximum allowed players (index=" + index + ">=" + MAXPLAYERS + ")");
            Color playerColour;
            switch (index)
            {
                case 0:
                    playerColour = Color.Red;
                    break;
                case 1:
                    playerColour = Color.Blue;
                    break;
                case 2:
                    playerColour = Color.Green;
                    break;
                case 3:
                    playerColour = Color.Yellow;
                    break;
                default:
                    playerColour = Color.White;
                    break;
            }
            AddPlayer(index, playerColour);
        }

        #endregion

        #region update
        public static void MovePlayer(int index, Vector2 thismuch)
        {
            if(players[index] != null)
            {
                players[index].Move(thismuch);
            }

        }
        public static void UpdateStaticContent()
        {
            for (byte i = 0; i < MAXPLAYERS;++i )
                if (players[i] != null) players[i].Update();
        }
        protected virtual void Update() { }
        #endregion

        #region draw
        public static void DrawStaticContent()
        {
            spriteBatch.Begin();
            for (byte i = 0; i < MAXPLAYERS; ++i)
                if (players[i] != null) players[i].Draw();

            spriteBatch.End();
        }
        protected virtual void Draw() { }
        #endregion
    }
}
