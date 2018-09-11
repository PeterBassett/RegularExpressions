using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegularExpressions.Ast;

namespace RegularExpressions.Automata.States
{
    class CharClassNFAState : NFAState
    {
        private CharacterClass charMap;
        private NFAState exit;

        public CharClassNFAState(CharacterClass charMap, NFAState exit)
            : base(RegExType.CharacterClass)
        {
            this.charMap = charMap;
            this.exit = exit;
        }

        internal override IEnumerable<NFAState> StatesForChar(char character)
        {
            if(charMap.Contains(character))
                return new[] { this.exit };

            return Enumerable.Empty<NFAState>();
        }
    }

}
