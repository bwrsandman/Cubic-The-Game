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

        private MenuEntry playerNumberMenuEntry, gameTimeMenuEntry, spawnIntervalsMenuEntry, playerSpeedMenuEntry, themeMenuEntry;

        static int[] spawnIntervals = { 100, 50, 25, 10 };
        static string[] playerNumber = { "single", "double", "triple", "quadruple" };
        static int[] gameTimes = { 60, 90, 150, 300, 600, 900 };
        static int[] playerSpeeds = { 2, 4, 6, 8, 10, 12 };
        static int currentPlayerNumber = 1;
        static int currentGameTime = 3;
        static int currentSpawnInterval = 2;
        static int currentPlayerSpeed = 2;

        static int currentTheme = 0;

        public static int[] getOptions { 
            get 
            { 
                return new int[] 
                    {
                        currentPlayerNumber, 
                        gameTimes[currentGameTime] , 
                        spawnIntervals[currentSpawnInterval],
                        playerSpeeds[currentPlayerSpeed],
                        currentTheme
                    };  
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
            playerSpeedMenuEntry = new MenuEntry(string.Empty);
            gameTimeMenuEntry = new MenuEntry(string.Empty);
            spawnIntervalsMenuEntry = new MenuEntry(string.Empty);

            themeMenuEntry = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            playerNumberMenuEntry.Selected += playerNumberMenuEntrySelected;
            playerSpeedMenuEntry.Selected += playerSpeedMenuEntrySelected;
            gameTimeMenuEntry.Selected += gameTimeMenuEntrySelected;
            spawnIntervalsMenuEntry.Selected += spawnIntervalsMenuEntrySelected;

            themeMenuEntry.Selected += themeMenuEntrySelected;

            back.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(playerNumberMenuEntry);
            MenuEntries.Add(playerSpeedMenuEntry);
            MenuEntries.Add(gameTimeMenuEntry);
            MenuEntries.Add(spawnIntervalsMenuEntry);
            MenuEntries.Add(themeMenuEntry);
            MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            playerNumberMenuEntry.Text = "Game Type: " + playerNumber[currentPlayerNumber];
            playerSpeedMenuEntry.Text = "Player Speed: " + playerSpeeds[currentPlayerSpeed];
            gameTimeMenuEntry.Text = "Game Time: " + gameTimes[currentGameTime];
            spawnIntervalsMenuEntry.Text = "Piece Spawn Intervals: " + spawnIntervals[currentSpawnInterval];

            themeMenuEntry.Text = "Theme: " + themes[currentTheme];
            gameTheme = currentTheme; //make sure font colors change with the theme
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

        void playerSpeedMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentPlayerSpeed = (currentPlayerSpeed + 1) % playerSpeeds.Length;

            SetMenuEntryText();
        }

        void gameTimeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentGameTime = (currentGameTime + 1) % gameTimes.Length;

            SetMenuEntryText();
        }
        
        void spawnIntervalsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentSpawnInterval = (currentSpawnInterval + 1) % spawnIntervals.Length;

            SetMenuEntryText();
        }

        void themeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentTheme = (currentTheme + 1) % themes.Length;

            SetMenuEntryText();
        }


        #endregion
    }
}
