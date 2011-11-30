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
using System;                             // For Random
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
        public static implicit operator Vector3(TwoInt rhs) { return new Vector3(rhs.X, rhs.Y, 0.0f); }

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
        public const float PLAYERSPEED = 6.0f;
        public static int fallSpawnInterval = 100;
        public const int NUMBEROFSHAPES = 8;

        public const int MATCHPOINTS = 50;
        public const int STEALPOINTS = 0;
        public const int LOCKPOINTS = 0;
        public const int LOCKEDPPSPS = 1; //points per second per shape
        public const int COMPLETEPOINTS = 1000;

        private static int gameDuration; // in seconds
        #endregion

        #region statics
        private static Player[] players;
        protected static TwoInt screenSize{get{return new TwoInt(device.Viewport.Width, device.Viewport.Height);}}
        public static GraphicsDevice device;
        public static SpriteBatch spriteBatch{protected get; set;}
        public static List<byte> playerList { private set; get; }
        private static int fallSpawnTimer;
        protected static Random rnd = new Random();

        private static List<FallPiece> fallPieceList;
        private static GreatCube theCube;
        public static Camera camera;

        
        public static Texture2D cubeTex;
        public static Texture2D backgroundTex;
        public static Texture2D[] shapes;
        public static SpriteFont font;
        public static float[] score{private set; get;}
        private static double elapsedTime;
        public static bool isGameover{private set; get;}
        //public static Texture2D[] maps;

        #endregion



        #region members
        protected virtual TwoInt size{get; set;}
        #endregion

        #region accessors
        #endregion

        #region constructors
        //public GameObject(Vector2 position)
        //{
        //    this.position = position;
        //}
        public GameObject()
        {
        }
        #endregion

        #region load
        /// <summary>
        /// Anything that is used only once for an entire class, such as a texture can be loaded here
        /// </summary>
        public static void LoadStaticContent(ContentManager content)
        {
            Player.texture = content.Load<Texture2D>("playerCursor");
            backgroundTex = content.Load<Texture2D>("game-bg");
            cubeTex = content.Load<Texture2D>("cubeTex");

            shapes = new Texture2D[NUMBEROFSHAPES];
            for (int i = 0; i < shapes.Length; ++i)
            {
                shapes[i] = content.Load<Texture2D>("shapes/" + (i+1));
            }
        }

        public static void NewGame(int gameTime, int fallInterval)
        {
            gameDuration = gameTime;
            elapsedTime = 0f;
            players = new Player[MAXPLAYERS];
            playerList = new List<byte>(MAXPLAYERS);
            score = new float[MAXPLAYERS];
            fallPieceList = new List<FallPiece>();
            fallSpawnInterval = fallInterval;
            fallSpawnTimer = fallSpawnInterval;
            theCube = new GreatCube(6, 6, 1.0f, new Vector3(0f, 2.0f, -5));
            FallPiece.loadBuffers();
            
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
            players[index] = new Player(index, screenSize / 2 + new TwoInt(-100 + index % 2 * 200, 100 * (byte)(index / 2)), colour);
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

        public static void PlayerGrabDrop(int index)
        {
            if (players[index] != null)
            {
                players[index].GrabDrop();
            }
        }

        public static void UpdateStaticContent(GameTime gameTime)
        {
            isGameover = ((elapsedTime += gameTime.ElapsedGameTime.TotalSeconds) > gameDuration);
            // Fall pieces
            if (--fallSpawnTimer <= 0)
            {
                fallSpawnTimer = fallSpawnInterval;
                fallPieceList.Add(new FallPiece(new Vector2((float)(rnd.NextDouble()*18f -9), 5.0f)));
            }
            
            List<FallPiece> expiredPieces = new List<FallPiece>();
            foreach (FallPiece piece in fallPieceList)
            {
                piece.Update();
                for (byte i = 0; i < MAXPLAYERS; ++i)
                    piece.intersects(players);
                if (piece.IsExpired()) expiredPieces.Add(piece);

            }
            foreach (FallPiece piece in expiredPieces)
            {
                fallPieceList.Remove(piece);
            }

            //PLAYERS
            for (byte i = 0; i < MAXPLAYERS; ++i)
            {
                if (players[i] != null)
                {
                    players[i].Update();
                }
            }

            //CUBE
            theCube.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            theCube.intersects(players);
        }
        protected virtual void Update() { }
        #endregion

        #region draw
        public static void DrawStaticContent()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTex, new Rectangle(device.Viewport.X,
                device.Viewport.Y, device.Viewport.Width, device.Viewport.Height), Color.White);
            spriteBatch.End();

            // Have the right thing render in front
            device.DepthStencilState = DepthStencilState.Default;
            theCube.Draw();
            device.DepthStencilState = DepthStencilState.None;

            foreach (FallPiece piece in fallPieceList)
                piece.Draw(GameObject.camera);

            

            spriteBatch.Begin();
            for (byte i = 0; i < MAXPLAYERS; ++i)
                if (players[i] != null) players[i].Draw();

            for (byte i = 0; i < MAXPLAYERS; ++i)
                if (players[i] != null) spriteBatch.DrawString(font, "score: " + (int)score[i], new Vector2((i % 2 == 0) ? 50 : device.Viewport.Width - 200, 50 + (int)(i / 2) * 50f), players[i].color);
            double timeleft = gameDuration - elapsedTime;
            spriteBatch.DrawString(font, string.Format("{0:00}", timeleft), new Vector2(device.Viewport.Width/2 - 5, 50), ((int)timeleft < 15 && ((int)(timeleft)%2) == 1 ) ? Color.OrangeRed : Color.White);

            spriteBatch.End();   
        }
        protected virtual void Draw() { }
        #endregion

        public static Vector2 GetScreenSpace(Vector3 cntr, Matrix world)
        {
            Vector3 projection = device.Viewport.Project(cntr, camera.projection, camera.view, world);
            return (new Vector2(projection.X, projection.Y));
        }
        public static Vector3 GetWorldSpace(Vector2 cntr, float Z, Matrix world)
        {
            return device.Viewport.Unproject(new Vector3(cntr,0), camera.projection, camera.view, world) + Vector3.Backward*Z;
        }

        protected static void Score(int i, float points)
        {
            Debug.WriteLine(points + " allocated to player#" + i);
            score[i] += points;
        }
    }
}
