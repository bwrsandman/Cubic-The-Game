#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace Cubic_The_Game
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class CreditsMenuScreen : MenuScreen
    {
        #region Fields

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public CreditsMenuScreen()
            : base("Credits")
        {
            // Create our menu entries.
            MenuEntry blankMenuEntry = new MenuEntry("");
            MenuEntry developpersMenuEntry = new MenuEntry("Developpement", Color.White);
            MenuEntry ArtMenuEntry = new MenuEntry("Art Assets", Color.White);
            MenuEntry PkgMgmtMenuEntry = new MenuEntry("Version Management", Color.White);
            MenuEntry QAMenuEntry = new MenuEntry("Quality Control", Color.White);
            MenuEntry SoundMenuEntry = new MenuEntry("Audio Assets", Color.White);

            MenuEntry sandyMenuEntry = new MenuEntry("Sandy Carter", Color.Black);
            MenuEntry sandyMenuEntry2 = new MenuEntry("Sandy Carter", Color.Black);
            MenuEntry shaikahMenuEntry = new MenuEntry("Shaikah Bakerman", Color.Black);
            MenuEntry shaikahMenuEntry2 = new MenuEntry("Shaikah Bakerman", Color.Black);
            MenuEntry xavierMenuEntry = new MenuEntry("Xavier Dupont", Color.Black);
            MenuEntry xavierMenuEntry2 = new MenuEntry("Xavier Dupont", Color.Black);
            MenuEntry ericMenuEntry = new MenuEntry("Eric Cote", Color.Black);
            MenuEntry ericMenuEntry2 = new MenuEntry("Eric Cote", Color.Black);
            MenuEntry markMenuEntry = new MenuEntry("Mark Latimer", Color.Black);

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            back.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(blankMenuEntry);
            MenuEntries.Add(developpersMenuEntry);
            MenuEntries.Add(sandyMenuEntry);
            MenuEntries.Add(shaikahMenuEntry);
            MenuEntries.Add(xavierMenuEntry);
            MenuEntries.Add(ericMenuEntry);
            MenuEntries.Add(blankMenuEntry);
            
            MenuEntries.Add(ArtMenuEntry);
            MenuEntries.Add(shaikahMenuEntry2);
            MenuEntries.Add(blankMenuEntry);
            
            MenuEntries.Add(PkgMgmtMenuEntry);
            MenuEntries.Add(sandyMenuEntry2);
            MenuEntries.Add(blankMenuEntry);
            
            MenuEntries.Add(QAMenuEntry);
            MenuEntries.Add(xavierMenuEntry2);
            MenuEntries.Add(ericMenuEntry2);
            MenuEntries.Add(blankMenuEntry);
            
            MenuEntries.Add(SoundMenuEntry);
            MenuEntries.Add(markMenuEntry);
            MenuEntries.Add(blankMenuEntry);
            
            MenuEntries.Add(blankMenuEntry);
            MenuEntries.Add(back);
        }


        #endregion
    }
}
