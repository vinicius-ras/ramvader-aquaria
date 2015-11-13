using RAMvader;
using RAMvader.CodeInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;

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





        #region PRIVATE STATIC FIELDS
        /// <summary>Maps a DependencyProperty to its associated injected variable.</summary>
        private static Dictionary<DependencyProperty,EVariable> sm_dependencyPropertyToInjectedVariable = new Dictionary<DependencyProperty, EVariable>();
        #endregion





        #region DEPENDENCY PROPERTIES
        /// <summary>DependencyProperty associated with the value of the "#EVariable.evVarOverrideVelocity" injected variable.</summary>
        public static readonly DependencyProperty PlayerVelocityFactorProperty = RegisterInjectedVariableDependencyProperty(
            "PlayerVelocityFactor", EVariable.evVarOverrideVelocity );
        /// <summary>DependencyProperty associated with the value of the "#EVariable.evVarOverrideEnergyShotDamage" injected variable.</summary>
        public static readonly DependencyProperty EnergyShotDamageFactorProperty = RegisterInjectedVariableDependencyProperty(
            "EnergyShotDamageFactor", EVariable.evVarOverrideEnergyShotDamage );
        #endregion





        #region PUBLIC PROPERTIES
        /// <summary>An object used for performing I/O operations on the game process' memory. </summary>
        public RAMvaderTarget GameMemoryIO { get; private set; }
        /// <summary>An object used for injecting and managing code caves and arbitrary variables into the
        /// game process' memory.</summary>
        public Injector<ECodeCave,EVariable> GameMemoryInjector { get; private set; }
        #endregion





        #region PRIVATE STATIC METHODS
        /// <summary>
        /// Registers a DependencyProperty which is associated to a given injected variable of the trainer.
        /// The default value and type for the property is retrieved from the associated injected variable.
        /// </summary>
        /// <param name="propertyName">The name of the DependencyProperty to be registered.</param>
        /// <param name="varID">The injected variable to be associated with the DependencyProperty.</param>
        /// <returns>Returns a DependencyProperty representing the injected variable.</returns>
        private static DependencyProperty RegisterInjectedVariableDependencyProperty( string propertyName, EVariable varID )
        {
            // Retrieve information about the variable
            MemberInfo enumeratorInfo = typeof( EVariable ).GetField( varID.ToString() );
            VariableDefinitionAttribute varDef = enumeratorInfo.GetCustomAttribute<VariableDefinitionAttribute>();
            Object defaultVarValue = varDef.InitialValue;
            Type varType = varDef.InitialValue.GetType();

            // Register the DependencyProperty
            DependencyProperty newDepProperty = DependencyProperty.Register( propertyName, varType, typeof( MainWindow ),
                new PropertyMetadata( defaultVarValue, InjectedVariableValueChanged ) );
            sm_dependencyPropertyToInjectedVariable.Add( newDepProperty, varID );
            return newDepProperty;
        }
        #endregion





        #region PRIVATE METHODS
        /// <summary>Called when the trainer needs to be detached from the game's process.</summary>
        private void detachFromGame()
        {
            // Detach from the target process
            if ( GameMemoryIO.IsAttached() )
            {
                GameMemoryInjector.ResetAllocatedMemoryData();
                GameMemoryIO.DetachFromProcess();
            }

            // Reset all of the values of the DependencyProperty objects which represent injected variables
            foreach ( KeyValuePair<DependencyProperty, EVariable> curPair in sm_dependencyPropertyToInjectedVariable )
            {
                // Retrieve the initial value of the injected variable
                DependencyProperty varDepProp = curPair.Key;
                EVariable varID = curPair.Value;

                FieldInfo enumeratorInfo = typeof( EVariable ).GetField( varID.ToString() );
                VariableDefinitionAttribute varDef = enumeratorInfo.GetCustomAttribute<VariableDefinitionAttribute>();
                
                // Reset the variable back to its initial value
                this.SetValue( varDepProp, varDef.InitialValue );
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
        /// <summary>
        /// Called when one of the DependencyProperty objects from the #MainWindow has its value changed.
        /// This method automatically updates the value of the corresponding injected variable into the game's process' memory space.
        /// </summary>
        /// <param name="depObj">A reference to the DependencyObject which owns the changed DependencyProperty. This parameter is
        /// effectivelly a reference to the #MainWindow object.</param>
        /// <param name="args">Contains data related to the event which changed the DependencyProperty's value.</param>
        private static void InjectedVariableValueChanged( DependencyObject depObj, DependencyPropertyChangedEventArgs args )
        {
            // If the trainer is attached to the game, update the value of the variable into the game's memory space
            MainWindow mainWnd = (MainWindow) App.Current.MainWindow;
            if ( mainWnd.GameMemoryIO.Attached )
            {
                // Retrieve the information we need to update the game's memory...
                EVariable varID = sm_dependencyPropertyToInjectedVariable[args.Property];
                IntPtr varAddressInGame = mainWnd.GameMemoryInjector.GetInjectedVariableAddress( varID );

                // Update the game's memory
                mainWnd.GameMemoryIO.WriteToTarget( varAddressInGame, args.NewValue );
            }
        }


        /// <summary>Called when the #MainWindow of the trainer is about to be closed (but before actually closing it).</summary>
        /// <param name="sender">Object which sent the event.</param>
        /// <param name="e">Arguments from the event.</param>
        private void WindowClosing( object sender, CancelEventArgs e )
        {
            // Detach the trainer, if it is attached
            if ( GameMemoryIO.Attached )
                detachFromGame();
        }


        /// <summary>
        /// Called when the user clicks the "'Attach to' (or 'Detach from') game" button.
        /// </summary>
        /// <param name="sender">Object which sent the event.</param>
        /// <param name="e">Arguments from the event.</param>
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
                    {
                        // Inject the trainer's code and variables into the game's memory!
                        GameMemoryInjector.Inject();

                        // When the game's process exits, the MainWindow's dispatcher is used to invoke the detachFromGame() method
                        // in the same thread which "runs" our MainWindow
                        GameMemoryIO.TargetProcess.EnableRaisingEvents = true;
                        GameMemoryIO.TargetProcess.Exited += ( caller, args ) => {
                            this.Dispatcher.Invoke( () => { this.detachFromGame(); } );
                        };
                    }
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
