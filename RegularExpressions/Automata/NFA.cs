using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegularExpressions.Ast;
using RegularExpressions.Automata.States;

namespace RegularExpressions.Automata
{
    /// <summary>
    /// An NFA is a container for a directed graph of NFAState objects.
    /// </summary>
    public class NFA 
    {
        private NFAState entry;
        private NFAState exit;
        private RegExType type;

        public NFA(NFAState entry, NFAState exit, RegExType type) 
        {
	        this.entry = entry;
	        this.exit  = exit;
            this.type = type;
        } 

        /// <summary>
        /// Main driver method.
        /// </summary>
        /// <param name="input">the string to match against the NFA graph</param>
        /// <param name="matchEntireString">flag, indicates that the NFA should match 
        /// the entire input stream or exit early at the first final state reached</param>
        /// <returns></returns>
        public bool Matches(string input, bool matchEntireString) 
        {
            return entry.Matches(input, matchEntireString);
        }
 
        #region Static NFA graph construction helper functions.
        public static NFA Character(char character) 
        {
	        var entry = new NFAState(RegExType.Primitive);
            var exit = new NFAState(RegExType.Primitive);
	        exit.isFinalState = true;
	        entry.AddNamedEdge(character,exit);
	        return new NFA(entry,exit, RegExType.Primitive);
        }

        public static NFA AnyCharacter()
        {
            var exit = new NFAState(RegExType.AnyChar);
            exit.isFinalState = true;
            var entry = new AnyCharNFAState(exit);
            return new NFA(entry, exit, RegExType.AnyChar);
        }

        internal static NFA StartOfStringAssertion()
        {
            var exit = new NFAState(RegExType.StartOfString);
            exit.isFinalState = true;
            var entry = new StartOfStringNFAState(exit);
            return new NFA(entry, exit, RegExType.StartOfString);
        }

        internal static NFA EndOfStringAssertion()
        {
            var exit = new NFAState(RegExType.StartOfString);
            exit.isFinalState = true;
            var entry = new EndOfStringNFAState();
            return new NFA(entry, exit, RegExType.StartOfString);
        }

        public static NFA CharacterClass(CharacterClass characterMap)
        {
            var exit = new NFAState(RegExType.CharacterClass);
            exit.isFinalState = true;
            var entry = new CharClassNFAState(characterMap, exit);
            return new NFA(entry, exit, RegExType.CharacterClass);
        }

        public static NFA CharacterClass(char start, char end, bool included = true)
        {
            var charMap = new CharacterClass(start, end);

            if (!included)
                charMap = Ast.CharacterClass.Negate(charMap);

            return NFA.CharacterClass(charMap);
        }
    
        public static NFA empty() 
        {
            var entry = new NFAState(RegExType.Blank);
            var exit = new NFAState(RegExType.Blank);
	        entry.AddAnonymousEdge(exit);
	        exit.isFinalState = true;
	        return new NFA(entry,exit, RegExType.Blank);
        }

        // Creates an NFA which matches zero or more repetitions of the given NFA.
        public static NFA ZeroOrMany(NFA nfa) 
        {
	        nfa.exit.AddAnonymousEdge(nfa.entry);
            nfa.entry.AddAnonymousEdge(nfa.exit);            
	        return nfa;	
        }
       
        // Creates an NFA which matches at least one or more repetitions of the given NFA.
        public static NFA OneOrMany(NFA nfa)
        {
            nfa.exit.AddAnonymousEdge(nfa.entry);
            return nfa;
        }

        // Creates an NFA which matches one zero or one times of the given NFA.
        public static NFA ZeroOrOne(NFA nfa)
        {
            nfa.entry.AddAnonymousEdge(nfa.exit);            
            return nfa;
        }

        // Creates an NFA that matches a sequence of the two provided NFAs.
        public static NFA Sequence(NFA first, NFA second) 
        {
	        first.exit.isFinalState = false;
	        second.exit.isFinalState = true;
	        first.exit.AddAnonymousEdge(second.entry);
	        return new NFA(first.entry,second.exit, RegExType.Sequence);
        }

        // Creates an NFA that matches either provided NFA.
        public static NFA Alternation(NFA a, NFA b) 
        {
	        a.exit.isFinalState = false;
	        b.exit.isFinalState = false;
            var entry = new NFAState(RegExType.Choice);
            var exit = new NFAState(RegExType.Choice);
	        exit.isFinalState = true;
	        entry.AddAnonymousEdge(a.entry);
	        entry.AddAnonymousEdge(b.entry);
	        a.exit.AddAnonymousEdge(exit);
	        b.exit.AddAnonymousEdge(exit);
	        return new NFA(entry,exit, RegExType.Choice);
        }

        // Top Level Helper Methods
        internal static NFA CastToNFA(object o) 
        {
	        if (o is NFA)
	            return (NFA)o;
	        else if (o is char)
	            return Character((char)o);
	        else if (o is string)
	            return SeqenceFromString((string)o);
	        else 
	            throw new Exception("Invalid object to create an NFA node from");
        }

        public static NFA Alternation(params object [] input) 
        {
	        var exp = CastToNFA(input[0]);
	        for (int i = 1; i < input.Length; i++) 
            {
                var r = CastToNFA(input[i]);

                if(r is NFA && exp is NFA)
                    exp = Alternation((NFA)exp, (NFA)r);
                else
                    exp = Alternation(exp, r);
	        }
	        return (NFA)exp;
        }

        public static NFA Sequence(params object[] rexps) 
        {
	        NFA exp = empty();
	        for (int i = 0; i < rexps.Length; i++) 
            {
                var r = CastToNFA(rexps[i]);
                if (r is NFA && exp is NFA)
                    exp = Sequence((NFA)exp, (NFA)r);
                else
	                exp = Sequence(exp,r);
	        }
	        return exp;
        }

        public static NFA SeqenceFromString(string input) 
        {
	        if (input.Length == 0)
	            return empty();

            return Sequence(CastToNFA(input[0]), SeqenceFromString(input.Substring(1)));
        }
        #endregion
    }
}
