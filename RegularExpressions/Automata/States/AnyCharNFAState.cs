using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegularExpressions.Ast;

namespace RegularExpressions.Automata.States
{
    class AnyCharNFAState : NFAState
    {
        private NFAState exit;
        public AnyCharNFAState(NFAState exit)
            : base(RegExType.AnyChar)
        {
            this.exit = exit;
        }

        internal override IEnumerable<NFAState> StatesForChar(char c)
        {
            return new[] { this.exit };
        }
    }
}
