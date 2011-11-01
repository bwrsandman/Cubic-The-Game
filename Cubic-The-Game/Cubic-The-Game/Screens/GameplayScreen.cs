#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Cubic_The_Game
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region constants
        // Default keybindings for each player, up to 4
        readonly Keys[] controlsUp =         { Keys.Up,              Keys.W,             Keys.NumPad8,   Keys.I      };
        readonly Keys[] controlsDown =       { Keys.Down,            Keys.S,             Keys.NumPad5,   Keys.K      };
        readonly Keys[] controlsLeft =       { Keys.Left,            Keys.A,             Keys.NumPad4,   Keys.J      };
        readonly Keys[] controlsRight =      { Keys.Right,           Keys.D,             Keys.NumPad6,   Keys.L      };
        readonly Keys[] controlsActivate =   { Keys.RightControl,    Keys.LeftControl,   Keys.NumPad0,   Keys.Space  };
        #endregion 

        #region Fields

        ContentManager content;
        SpriteFont gameFont;
        int playerNum;
        Random random = new Random();

        float pauseAlpha;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(int[] gameOptions)
        {
            // Number of players, 1, 2, 4
            switch (gameOptions[0])
            {
                case 0:
                    playerNum = 1;
                    break;
                case 1:
                    playerNum = 2;
                    break;
                case 2:
                    playerNum = 4;
                    break;
                default:
                    playerNum = 1;
                    break;
            }
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            GameObject.device = ScreenManager.GraphicsDevice;
            GameObject.spriteBatch = ScreenManager.SpriteBatch;
            GameObject.NewGame();

            // Adding player by number, at this point we assume player configurations 1 or 1,2 or 1,2,3,4
            // TODO: configurations of players 2 or 3 or 4 or 2,3 or 3,4 or 2,3,4
            for (byte i = 0; i < playerNum; ++i)
                GameObject.AddPlayer(i);
            

            gameFont = content.Load<SpriteFont>("gamefont");
            GameObject.LoadStaticContent(content);

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            //Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {

                GameObject.UpdateStaticContent();
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            //int playerIndex = (int)ControllingPlayer.Value;

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            foreach (byte i in GameObject.playerList)
            {
                KeyboardState keyboardState = input.CurrentKeyboardStates[i];
                GamePadState gamePadState = input.CurrentGamePadStates[i];

                bool gamePadDisconnected = !gamePadState.IsConnected &&
                                           input.GamePadWasConnected[i];

                if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
                {
                    ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
                    break;
                }
                else
                {
                    // Otherwise move the player position.
                    Vector2 movement = Vector2.Zero;

                    if (keyboardState.IsKeyDown(controlsLeft[i]))
                        movement.X--;

                    if (keyboardState.IsKeyDown(controlsRight[i]))
                        movement.X++;

                    if (keyboardState.IsKeyDown(controlsUp[i]))
                        movement.Y--;

                    if (keyboardState.IsKeyDown(controlsDown[i]))
                        movement.Y++;
                    

                    Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                    movement.X += thumbstick.X;
                    movement.Y -= thumbstick.Y;

                    if (movement.Length() > 1)
                        movement.Normalize();

                    GameObject.MovePlayer(i, movement * GameObject.PLAYERSPEED);

                }
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            // This is where the heavy lifting happens
            GameObject.DrawStaticContent();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }


        #endregion
    }
}
