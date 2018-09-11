using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegularExpressions.Ast;

namespace RegularExpressions.Automata.States
{
    class StartOfStringNFAState : NFAState
    {
        private NFAState exit;
        public StartOfStringNFAState(NFAState exit)
            : base(RegExType.StartOfString)
        {
            this.exit = exit;
        }

        internal override bool Matches(string s, bool matchEntireString, int position, int originalLength)
        {
            if (position == 0)
                return exit.Matches(s, matchEntireString, position, originalLength);

            return false;
        }

        internal override bool Matches(string s, bool matchEntireString, int position, int originalLength, HashSet<NFAState> visited)
        {
            if (position == 0)
                return exit.Matches(s, matchEntireString, position, originalLength, visited);

            return false;
        }
    }
}
