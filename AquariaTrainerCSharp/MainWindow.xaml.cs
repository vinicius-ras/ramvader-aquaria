using RAMvader;
using RAMvader.CodeInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

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
        /// <summary>DependencyProperty associated with the value of the "#EVariable.evVarOverridePetPower" injected variable.</summary>
        public static readonly DependencyProperty PetPowerProperty = RegisterInjectedVariableDependencyProperty(
            "PetPower", EVariable.evVarOverridePetPower );
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
        private void DetachFromGame()
        {
            // Detach from the target process
            if ( GameMemoryIO.IsAttached() )
            {
                // If the game's process is still running, all cheats must be disabled
                if ( GameMemoryIO.TargetProcess.HasExited == false )
                {
                    foreach ( ECheat curCheat in Enum.GetValues( typeof( ECheat ) ) )
                        SetCheatEnabled( curCheat, false );
                }

                // Release injected memory, cleanup and detach
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


        /// <summary>This method is called to enable or disable cheats, altering the game's process' memory space accordingly.</summary>
        /// <param name="cheatID">The identifier of the cheat that needs to be enabled/disabled on the game.</param>
        /// <param name="bEnable">A flag specifying if the cheat is to be enabled or disabled.</param>
        private void SetCheatEnabled( ECheat cheatID, bool bEnable )
        {
            FieldInfo fieldInfo = typeof( ECheat ).GetField( cheatID.ToString() );
            CheatTypeInfo cheatTypeInfo = fieldInfo.GetCustomAttribute<CheatTypeInfo>();

            IntPtr targetInstructionAddress = LowLevelConstants.GetCheatTargetInstructionAddress( cheatID, GameMemoryIO.TargetProcess.MainModule.BaseAddress );

            // Verify if we're enabling or disabling the cheat...
            if ( bEnable == false )
            {
                // Disabling the cheat is just a matter of writing the original bytes of the instruction into the right place
                GameMemoryIO.WriteToTarget( targetInstructionAddress, cheatTypeInfo.OriginalInstructionBytes );
            }
            else
            {
                // Enable the cheat based on its type...
                switch ( cheatTypeInfo.CheatType )
                {
                    case ECheatType.evCheatTypeNOP:
                        {
                            // Enabling a NOP cheat is just a matter of replacing the instruction's original bytes by NOP instructions
                            byte [] arrayOfNOPs = Enumerable.Repeat<byte>( LowLevelConstants.INSTRUCTION_NOP, cheatTypeInfo.OriginalInstructionBytes.Length ).ToArray();
                            GameMemoryIO.WriteToTarget( targetInstructionAddress, arrayOfNOPs );
                            break;
                        }
                    case ECheatType.evCheatTypeCodeCave:
                        GameMemoryInjector.WriteCodeCaveDetour( targetInstructionAddress, cheatTypeInfo.CodeCave, cheatTypeInfo.OriginalInstructionBytes.Length );
                        break;
                    default:
                        throw new NotImplementedException( string.Format( "[{0}] Activation of cheats of type \"{1}\" are not yet implemented!",
                            this.GetType().Name, cheatTypeInfo.CheatType.ToString() ) );
                }
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
                DetachFromGame();
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
                DetachFromGame();
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

                        // When the game's process exits, the MainWindow's dispatcher is used to invoke the DetachFromGame() method
                        // in the same thread which "runs" our MainWindow
                        GameMemoryIO.TargetProcess.EnableRaisingEvents = true;
                        GameMemoryIO.TargetProcess.Exited += ( caller, args ) => {
                            this.Dispatcher.Invoke( () => { this.DetachFromGame(); } );
                        };

                        #if DEBUG
                            // In debug mode, print some useful debug information
                            Console.WriteLine( "[DEBUG]" );
                            Console.WriteLine( "Attached to process: {0} (PID: {1}", GameMemoryIO.TargetProcess.ProcessName, GameMemoryIO.TargetProcess.Id );
                            Console.WriteLine( "Base injection address: 0x{0}", ( (long) GameMemoryInjector.BaseInjectionAddress ).ToString( "X8" ) );
                            Console.WriteLine( "Injected code caves:" );
                            foreach ( ECodeCave curCodeCave in Enum.GetValues( typeof( ECodeCave ) ) )
                                Console.WriteLine( "   {0} at 0x{1}", curCodeCave, ( (long) GameMemoryInjector.InjectedCodeCaveAddress[curCodeCave] ).ToString( "X8" ) );
                            Console.WriteLine( "Injected variables:" );
                            foreach ( EVariable curVariable in Enum.GetValues( typeof( EVariable ) ) )
                                Console.WriteLine( "   {0} at 0x{1}", curVariable, ( (long) GameMemoryInjector.InjectedVariableAddress[curVariable] ).ToString( "X8" ) );
                        #endif

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


        /// <summary>
        /// Called whenever any of the CheckBoxes used to activate/deactivate cheats from the trainer gets checked or unchecked.
        /// </summary>
        /// <param name="sender">Object which sent the event.</param>
        /// <param name="e">Arguments from the event.</param>
        private void CheckBoxCheatToggled( object sender, RoutedEventArgs e )
        {
            // Only proceed if the trainer is attached...
            if ( GameMemoryIO.Attached == false )
                return;

            // Retrieve information which will be used to enable or disable the cheat
            CheckBox chkBox = (CheckBox) e.Source;
            ECheat cheatID = (ECheat) chkBox.Tag;
            bool bEnableCheat = ( chkBox.IsChecked == true );

            SetCheatEnabled( cheatID, bEnableCheat );
        }
#endregion
    }
}
