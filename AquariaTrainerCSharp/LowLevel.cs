/* This file keeps definitions for code elements which are part of the low-level features of the trainer. */
using RAMvader.CodeInjection;
using System;
using System.Collections.Generic;

namespace AquariaTrainerCSharp
{
    /// <summary>Identifiers for all cheats available in the trainer.</summary>
    public enum ECheat
    {
        /// <summary>Identifier for the cheat: God Mode.</summary>
        [CheatTypeInfo( ECodeCave.evCaveGodMode, 0xD9, 0x80, 0xE4, 0x1A, 0x00, 0x00 )]
        evCheatGodMode,
        /// <summary>Identifier for the cheat: Infinite items (when using them).</summary>
        [CheatTypeInfo( 0x83, 0x40, 0x3C, 0xFF )]
        evCheatInfiniteItemsUse,
        /// <summary>Identifier for the cheat: Infinite items (when cooking recipes with them).</summary>
        [CheatTypeInfo( 0x83, 0x40, 0x3C, 0xFF )]
        evCheatInfiniteItemsCook,
        /// <summary>Identifier for the cheat: Instant charge attacks.</summary>
        [CheatTypeInfo( ECodeCave.evCaveInstantChargeAttacks, 0xD9, 0x96, 0x78, 0x43, 0x00, 0x00 )]
        evCheatInstantChargeAttacks,
        /// <summary>Identifier for the cheat: Dual Form's Naija attack always enabled.</summary>
        [CheatTypeInfo( ECodeCave.evCaveDualFormKillCountHack, 0xDB, 0x80, 0x3C, 0x15, 0x00, 0x00 )]
        evCheatDualFormKillCountHack,
        /// <summary>Identifier for the cheat: Override player's velocity factor.</summary>
        [CheatTypeInfo( ECodeCave.evCaveOverrideVelocity, 0xD9, 0x81, 0x98, 0x11, 0x00, 0x00 )]
        evCheatOverrideVelocity,
        /// <summary>Identifier for the cheat: Override player's damage increase.</summary>
        [CheatTypeInfo( ECodeCave.evCaveOverrideEnergyShotDamage, 0xD8, 0x99, 0xA8, 0x11, 0x00, 0x00 )]
        evCheatOverrideEnergyShotDamage,
        /// <summary>Identifier for the cheat: Override pet power.</summary>
        [CheatTypeInfo( ECodeCave.evCaveOverridePetPower, 0xD9, 0x80, 0xB4, 0x11, 0x00, 0x00 )]
        evCheatOverridePetPower,

    }


    /// <summary>Identifiers for all of the code caves injected into the game process' memory space,
    /// once the trainer gets attached to the game.</summary>
    public enum ECodeCave
    {
        /// <summary>Identifier for the code cave used by the #ECheat.evCheatGodMode cheat.</summary>
        [CodeCaveDefinition( 0x52, 0x8B, 0x90, 0xE8, 0x1A, 0x00, 0x00, 0x89, 0x90, 0xE4, 0x1A, 0x00, 0x00, 0x5A, 0xD9, 0x80, 0xE4, 0x1A, 0x00, 0x00, 0xC3, 0x90 )]
        evCaveGodMode,
        /// <summary>Identifier for the code cave used by the #ECheat.evCheatInstantChargeAttacks cheat.</summary>
        [CodeCaveDefinition( 0xDD, 0xD8, 0xC7, 0x86, 0x78, 0x43, 0x00, 0x00, 0x00, 0x00, 0x48, 0x42, 0xC3 )]
        evCaveInstantChargeAttacks,
        /// <summary>Identifier for the code cave used by the #ECheat.evCheatDualFormKillCountHack cheat.</summary>
        [CodeCaveDefinition( 0x51, 0x31, 0xC9, 0xB1, 0x03, 0x89, 0x88, 0x3C, 0x15, 0x00, 0x00, 0x59, 0xDB, 0x80, 0x3C, 0x15, 0x00, 0x00, 0xC3 )]
        evCaveDualFormKillCountHack,
        /// <summary>Identifier for the code cave used by the #ECheat.evCheatOverrideVelocity cheat.</summary>
        [CodeCaveDefinition( 0x50, 0xA1, EVariable.evVarOverrideVelocity, 0x89, 0x81, 0x98, 0x11, 0x00, 0x00, 0x58, 0xD9, 0x81, 0x98, 0x11, 0x00, 0x00, 0xC3 )]
        evCaveOverrideVelocity,
        /// <summary>Identifier for the code cave used by the #ECheat.evCheatOverrideEnergyShotDamage cheat.</summary>
        [CodeCaveDefinition( 0x50, 0xA1, EVariable.evVarOverrideEnergyShotDamage, 0x89, 0x81, 0xA8, 0x11, 0x00, 0x00, 0x58, 0xD8, 0x99, 0xA8, 0x11, 0x00, 0x00, 0xC3 )]
        evCaveOverrideEnergyShotDamage,
        /// <summary>Identifier for the code cave used by the #ECheat.evCheatOverridePetPower cheat.</summary>
        [CodeCaveDefinition( 0x51, 0x8B, 0x88, 0x40, 0x1F, 0x00, 0x00, 0x81, 0xF9, 0x5A, 0x02, 0x00, 0x00, 0x75, 0x21, 0x31, 0xC9, 0xB1, 0x16, 0x51, 0xDB, 0x04, 0x24,
            0x59, 0xD9, 0x05, EVariable.evVarOverridePetPower, 0xDF, 0xF1, 0xDD, 0xD8, 0x72, 0x0C, 0xC7, 0x80, 0xB4, 0x11, 0x00, 0x00, 0x00, 0x00, 0xB0, 0x41, 0xEB, 0x0C,
            0x8B, 0x0D, EVariable.evVarOverridePetPower, 0x89, 0x88, 0xB4, 0x11, 0x00, 0x00, 0x59, 0xD9, 0x80, 0xB4, 0x11, 0x00, 0x00, 0xC3 )]
        evCaveOverridePetPower,
    }


    /// <summary>Identifiers for all variables injected into the game process' memory space,
    /// once the trainer gets attached to the game.</summary>
    public enum EVariable
    {
        /// <summary>Identifier for the variable used to control the player's velocity factor, in the #ECheat.evCheatOverrideVelocity cheat.</summary>
        [VariableDefinition( (Single) 1.0f )]
        evVarOverrideVelocity,
        /// <summary>Identifier for the variable used to control the player's Energy Shot damage, in the #ECheat.evCheatOverrideEnergyShotDamage cheat.</summary>
        [VariableDefinition( (Single) 0.0f )]
        evVarOverrideEnergyShotDamage,
        /// <summary>Identifier for the variable used to control the power of the player's pet, in the #ECheat.evCheatOverridePetPower cheat.</summary>
        [VariableDefinition( (Single) 0.0f )]
        evVarOverridePetPower,
    }


    /// <summary>
    /// A static class whose only purpose is to provide constants related to the trainer's low level features.
    /// </summary>
    public static class LowLevelConstants
    {
        #region PUBLIC CONSTANTS
        /// <summary>The byte value which represents the NOP instruction for a x86 processor.</summary>
        public const byte INSTRUCTION_NOP = 0x90;
        #endregion





        #region PRIVATE STATIC FIELDS
        /// <summary>Maps each cheat to the offset that must be added to the main module of the game's process, in order to find the
        /// instruction that needs to be modified by the trainer to activate or deactivate the given cheat.</summary>
        private static readonly Dictionary<ECheat, int> sm_cheatTargetInstructionAddress = new Dictionary<ECheat, int>()
        {
            { ECheat.evCheatGodMode, 0xFD877 },
            { ECheat.evCheatInfiniteItemsUse, 0xC52E0 },
            { ECheat.evCheatInfiniteItemsCook, 0xCB76C },
            { ECheat.evCheatInstantChargeAttacks, 0x74313 },
            { ECheat.evCheatDualFormKillCountHack, 0x65558 },
            { ECheat.evCheatOverrideVelocity, 0x5ECD3 },
            { ECheat.evCheatOverrideEnergyShotDamage, 0x65282 },
            { ECheat.evCheatOverridePetPower, 0x12D115 },
        };
        #endregion





        #region PUBLIC STATIC METHODS
        /// <summary>
        /// Retrieves the address of the instruction which the trainer needs to modify in order to enable or disable a given cheat.
        /// </summary>
        /// <param name="cheatID">The cheat whose target instruction is to be retrieved.</param>
        /// <param name="mainModuleBaseAddress">The base address of the game process's main module.</param>
        /// <returns>Returns the address of the instruction associated with the given cheat.</returns>
        public static IntPtr GetCheatTargetInstructionAddress( ECheat cheatID, IntPtr mainModuleBaseAddress )
        {
            return mainModuleBaseAddress + sm_cheatTargetInstructionAddress[cheatID];
        }
        #endregion
    }
}