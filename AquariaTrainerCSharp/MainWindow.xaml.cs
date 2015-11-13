using System;
using System.Diagnostics;
using System.Windows;
using System.Linq;
using RAMvader;
using RAMvader.CodeInjection;

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





        #region PUBLIC PROPERTIES
        /// <summary>An object used for performing I/O operations on the game process' memory. </summary>
        public RAMvaderTarget GameMemoryIO { get; private set; }
        /// <summary>An object used for injecting and managing code caves and arbitrary variables into the
        /// game process' memory.</summary>
        public Injector<ECodeCave,EVariable> GameMemoryInjector { get; private set; }
        #endregion





        #region PRIVATE CALLBACKS
        /// <summary>Called when the trainer needs to be detached from the game's process.</summary>
        private void detachFromGame()
        {
            if ( GameMemoryIO.IsAttached() )
            {
                GameMemoryInjector.ResetAllocatedMemoryData();
                GameMemoryIO.DetachFromProcess();
            }
        }
        #endregion





        #region PUBLIC METHODS
        public MainWindow()
        {
            // Initialize objects which will perform operations on the game's memory
            GameMemoryIO = new RAMvaderTarget();

            GameMemoryInjector = new Injector<ECodeCave, EVariable>();
            GameMemoryInjector.SetTargetProcess( GameMemoryIO );

            // Initialize RAMvaderTarget
            GameMemoryIO.SetTargetEndianness( EEndianness.evEndiannessLittle );
            GameMemoryIO.SetTargetPointerSize( EPointerSize.evPointerSize32 );

            // Initialize window
            InitializeComponent();
        }
        #endregion





        #region EVENT CALLBACKS
        private void ButtonClickAttachToGame( object sender, RoutedEventArgs e )
        {
            // Is the trainer currently attached to the game's process?
            if ( GameMemoryIO.Attached )
                detachFromGame();
            else
            {
                // Try to find the game's process
                Process gameProcess = Process.GetProcessesByName( GAME_PROCESS_NAME ).FirstOrDefault();
                if ( gameProcess != null )
                {
                    // Try to attach to the game's process
                    if ( GameMemoryIO.AttachToProcess( gameProcess ) )
                        GameMemoryInjector.Inject();
                    else
                        MessageBox.Show( this,
                            Properties.Resources.strMsgFailedToAttach, Properties.Resources.strMsgFailedToAttachCaption,
                            MessageBoxButton.OK, MessageBoxImage.Exclamation );
                }
                else
                    MessageBox.Show( this,
                        Properties.Resources.strMsgGamesProcessNotFound, Properties.Resources.strMsgGamesProcessNotFoundCaption,
                        MessageBoxButton.OK, MessageBoxImage.Exclamation );
            }
        }
        #endregion
    }
}
