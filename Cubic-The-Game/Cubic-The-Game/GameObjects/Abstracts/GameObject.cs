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
        #endregion

        #region statics
        private static Player[] players;
        private static TwoInt screenSize;
        public static SpriteBatch spriteBatch{protected get; set;}
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
            players = new Player[4];
            players[0] = new Player(sSize / 2 - new TwoInt(100, 0), Color.Red);
            players[1] = new Player(sSize / 2 + new TwoInt(100, 0), Color.Blue);
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
            spriteBatch.Begin();
            //TODO: For i = 0 - MAX NUM OF PLAYERS -> if not null -> Draw()
            players[0].Update();
            players[1].Update();

            spriteBatch.End();
        }
        protected virtual void Update() { }
        #endregion

        #region draw
        public static void DrawStaticContent()
        {
            spriteBatch.Begin();
            //TODO: For i = 0 - MAX NUM OF PLAYERS -> if not null -> Draw()
            players[0].Draw();
            players[1].Draw();

            spriteBatch.End();
        }
        protected virtual void Draw() { }
        #endregion
    }
}
