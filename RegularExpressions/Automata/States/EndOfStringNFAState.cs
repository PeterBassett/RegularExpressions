using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegularExpressions.Ast;

namespace RegularExpressions.Automata.States
{
    class EndOfStringNFAState : NFAState
    {
        public EndOfStringNFAState()
            : base(RegExType.StartOfString)
        {
        }

        internal override bool Matches(string s, bool matchEntireString, int position, int originalLength)
        {
            if (position == originalLength)
                return true;

            return false;
        }

        internal override bool Matches(string s, bool matchEntireString, int position, int originalLength, HashSet<NFAState> visited)
        {
            if (position == originalLength)
                return true;

            return false;
        }
    } 
}
