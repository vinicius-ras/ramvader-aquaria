/*
 * Copyright (C) 2014 Vinicius Rogério Araujo Silva
 *
 * This file is part of AquaRipper.
 * 
 * AquaRipper is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * AquaRipper is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with AquaRipper.  If not, see <http://www.gnu.org/licenses/>.
 */


using System;

namespace AquaRipper
{
    /// <summary>Provides identifiers for all the types of cheats supported by the trainer.</summary>
    public enum ECheatType
    {
        /// <summary>Cheat type: NOP cheats. Cheats of this type replace instructions in the game's memory by NOP instructions.</summary>
        evCheatTypeNOP,
        /// <summary>Cheat type: code-cave based cheats. Cheats of this type replace instructions in the game's memory by calls to
        /// "code caves", which are small procedures that are injected by the trainer into the game's memory space.</summary>
        evCheatTypeCodeCave,
    }





    /// <summary>A special attribute to be applied to #ECheat enumerators in order to tell the trainer how
    /// each available cheat should be enabled or disabled.</summary>
    [AttributeUsage( AttributeTargets.Field, AllowMultiple = false )]
    public class CheatTypeInfo : Attribute
    {
        #region PUBLIC PROPERTIES
        /// <summary>Indicates the type of the cheat, which affects how the trainer "activates" it into the game's memory space.</summary>
        public ECheatType CheatType { get; private set; }
        /// <summary>Indicates the code cave related to that cheat. This property is only used if the cheat is of the
        /// type #ECheatType.evCheatTypeCodeCave. Otherwise, it is ignored.</summary>
        public ECodeCave CodeCave { get; private set; }
        /// <summary>Stores the bytes that compose the original instruction that is in the game's memory. These bytes are used
        /// when the user deactivates the cheat.</summary>
        public byte [] OriginalInstructionBytes { get; private set; }
        #endregion





        #region PRIVATE METHODS
        /// <summary>The class' main constructor. All other constructors of this class are redirected to this one.</summary>
        /// <param name="deducedCheatType">The type of the cheat, which is deduced by the public constructor used to instantiate
        /// the #CheatTypeInfo object.</param>
        /// <param name="codeCave">The code cave to be associated with the cheat. This code cave will only have any meaning to
        /// the trainer if the cheat is of type #ECheatType.evCheatTypeCodeCave.</param>
        /// <param name="originalInstructionBytes">The bytes that comprise the original instruction that is in the game's memory.
        /// They will be used when the trainer needs to deactivate the cheat.</param>
        private CheatTypeInfo( ECheatType deducedCheatType, ECodeCave codeCave, params byte[] originalInstructionBytes )
        {
            CheatType = deducedCheatType;
            CodeCave = codeCave;
            OriginalInstructionBytes = originalInstructionBytes;
        }
        #endregion





        #region PUBLIC METHODS
        /// <summary>Constructor for #CheatTypeInfo objects which hold information about cheats of type #ECheatType.evCheatTypeNOP.</summary>
        /// <param name="originalInstructionBytes">The bytes that comprise the original instruction that is in the game's memory.
        /// They will be used when the trainer needs to deactivate the cheat.</param>
        public CheatTypeInfo( params byte[] originalInstructionBytes )
            : this( ECheatType.evCheatTypeNOP, default( ECodeCave ), originalInstructionBytes )
        {
        }


        /// <summary>Constructor for #CheatTypeInfo objects which hold information about cheats of type #ECheatType.evCheatTypeCodeCave.</summary>
        /// <param name="codeCave">The code cave to be associated with the cheat.</param>
        /// <param name="originalInstructionBytes">The bytes that comprise the original instruction that is in the game's memory.
        /// They will be used when the trainer needs to deactivate the cheat.</param>
        public CheatTypeInfo( ECodeCave codeCave, params byte[] originalInstructionBytes )
            : this( ECheatType.evCheatTypeCodeCave, codeCave, originalInstructionBytes )
        {
        }
        #endregion
    }
}
