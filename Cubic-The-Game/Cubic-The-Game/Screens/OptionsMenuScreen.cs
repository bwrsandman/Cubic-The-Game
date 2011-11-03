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
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry playerNumberMenuEntry;

        static string[] playerNumber = { "single", "1v1", "2v2" };
        static int currentPlayerNumber = 1;

        public static int[] getOptions { 
            get 
            { 
                return new int[] {currentPlayerNumber};  
            } 
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            playerNumberMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            playerNumberMenuEntry.Selected += playerNumberMenuEntrySelected;
            back.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(playerNumberMenuEntry);
            MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            playerNumberMenuEntry.Text = "Game Type: " + playerNumber[currentPlayerNumber];
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Language menu entry is selected.
        /// </summary>
        void playerNumberMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentPlayerNumber = (currentPlayerNumber + 1) % playerNumber.Length;

            SetMenuEntryText();
        }


        #endregion
    }
}
