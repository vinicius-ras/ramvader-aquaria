/* This file keeps definitions for code elements which are part of the low-level features of the trainer. */
using System;
using RAMvader.CodeInjection;

namespace AquariaTrainerCSharp
{
    /// <summary>Identifiers for all cheats available in the trainer.</summary>
    public enum ECheat
    {
        /// <summary>Identifier for the cheat: God Mode.</summary>
        evCheatGodMode,
        /// <summary>Identifier for the cheat: Infinite items (when using them).</summary>
        evCheatInfiniteItemsUse,
        /// <summary>Identifier for the cheat: Infinite items (when cooking recipes with them).</summary>
        evCheatInfiniteItemsCook,
        /// <summary>Identifier for the cheat: Instant charge attacks.</summary>
        evCheatInstantChargeAttacks,
        /// <summary>Identifier for the cheat: Dual Form's Naija attack always enabled.</summary>
        evCheatDualFormKillCountHack,
        /// <summary>Identifier for the cheat: Override player's velocity factor.</summary>
        evCheatOverrideVelocity,
        /// <summary>Identifier for the cheat: Override player's damage increase.</summary>
        evCheatOverrideEnergyShotDamage,
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
        [CodeCaveDefinition( 0x50, 0xA1, EVariable.evVarOverrideEnergyShotDamage, 0x89, 0x81, 0xA8, 0x11, 0x00, 0x00, 0x58, 0xD9, 0x81, 0xA8, 0x11, 0x00, 0x00, 0xC3 )]
        evCaveOverrideEnergyShotDamage,
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
    }
}