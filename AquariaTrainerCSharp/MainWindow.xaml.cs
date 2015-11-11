using System;
using System.Diagnostics;
using System.Windows;
using System.Linq;

namespace AquariaTrainerCSharp
{
    /// <summary>
    /// Implements the logic behind the trainer's MainWindow.
    /// </summary>
    public partial class MainWindow : Window
    {
        #region PRIVATE CONSTANTS
        /// <summary>The name of the process which runs the game.</summary>
        private const string GAME_PROCESS_NAME = "Aquaria";
        #endregion





        #region PUBLIC METHODS
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion





        #region EVENT CALLBACKS
        private void ButtonClickAttachToGame( object sender, RoutedEventArgs e )
        {
            // Try to find the game's process
            Process gameProcess = Process.GetProcessesByName( GAME_PROCESS_NAME ).FirstOrDefault();
            if ( gameProcess != null )
                Console.WriteLine( "Found game's process, PID = {0}", gameProcess.Id );
            else
                Console.WriteLine( "Game's not running" );
        }
        #endregion
    }
}
