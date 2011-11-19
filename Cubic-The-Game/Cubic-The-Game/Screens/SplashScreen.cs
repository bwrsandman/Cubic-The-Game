#region description
//-----------------------------------------------------------------------------
// SplashScreen.cs
//
// Written by bwrsandman(Sandy)
//-----------------------------------------------------------------------------
#endregion

#region using
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Cubic_The_Game
{
    class SplashScreen : GameScreen
    {
        #region constants
        private const float SPLASHTIME = 2000.0f; //In Milliseconds
        #endregion

        #region Fields

        #endregion

        #region Initialization


        /// <summary>
        /// The constructor is private: loading screens should
        /// be activated via the static Load method instead.
        /// </summary>
        public SplashScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        ///// <summary>
        ///// Activates the loading screen.
        ///// </summary>
        //public static void Load(ScreenManager screenManager, bool loadingIsSlow,
        //                        PlayerIndex? controllingPlayer,
        //                        params GameScreen[] screensToLoad)
        //{
        //    // Tell all the current screens to transition off.
        //    foreach (GameScreen screen in screenManager.GetScreens())
        //        screen.ExitScreen();

        //    // Create and activate the loading screen.
        //    LoadingScreen loadingScreen = new LoadingScreen(screenManager,
        //                                                    loadingIsSlow,
        //                                                    screensToLoad);

        //    screenManager.AddScreen(loadingScreen, controllingPlayer);
        //}


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the loading screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // If all the previous screens have finished transitioning
            // off, it is time to actually perform the load.
            if (gameTime.TotalGameTime.TotalMilliseconds >= SPLASHTIME)
            {

                ExitScreen();
                //ScreenManager.AddScreen(new BackgroundScreen(), null);
                //ScreenManager.AddScreen(new MainMenuScreen(), null);

                // Once the load has finished, we use ResetElapsedTime to tell
                // the  game timing mechanism that we have just finished a very
                // long frame, and that it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
        }


        /// <summary>
        /// Draws the loading screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // If we are the only active screen, that means all the previous screens
            // must have finished transitioning off. We check for this in the Draw
            // method, rather than in Update, because it isn't enough just for the
            // screens to be gone: in order for the transition to look good we must
            // have actually drawn a frame without them before we perform the load.
            //if ((ScreenState == ScreenState.Active) &&
            //    (ScreenManager.GetScreens().Length == 1))
            //{
            //    otherScreensAreGone = true;
            //}

            // The gameplay screen takes a while to load, so we display a loading
            // message while that is going on, but the menus load very quickly, and
            // it would look silly if we flashed this up for just a fraction of a
            // second while returning from the game to the menus. This parameter
            // tells us how long the loading is going to take, so we know whether
            // to bother drawing the message.
                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
                SpriteFont font = ScreenManager.TitleFont;

                const string message = "CUBIC";

                // Center the text in the viewport.
                Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
                Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
                Vector2 textSize = font.MeasureString(message);
                Vector2 textPosition = (viewportSize - textSize) / 2;

                //Color color = Color.OrangeRed * TransitionAlpha;
                Color color = new Color(99, 90, 67) * TransitionAlpha;

                // Draw the text.
                spriteBatch.Begin();
                spriteBatch.DrawString(font, message, textPosition, color);
                spriteBatch.End();
        }


        #endregion
    }
}
