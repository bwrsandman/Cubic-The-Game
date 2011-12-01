using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Cubic_The_Game
{
    class InstructionsScreen : MenuScreen
    {

        //Rectangle titleRect;   //rect for main title
        //Rectangle title2Rect;  //rect for 2nd title "PC controls"
        //Rectangle title3Rect; //rect for 3rd title "X-box controls"

        Texture2D bckgrnd;
        public InstructionsScreen()
            :base(" ")
        {
   
        }

        public override void LoadContent()
        {
            base.LoadContent();
            bckgrnd = ScreenManager.Game.Content.Load<Texture2D>("background_instructions");
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            
            spriteBatch.Begin();

            spriteBatch.Draw(bckgrnd, new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, 
                ScreenManager.GraphicsDevice.Viewport.Height), Color.White);
        
            spriteBatch.End();
        }
    }
}
