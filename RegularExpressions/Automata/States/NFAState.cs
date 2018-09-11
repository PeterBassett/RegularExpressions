using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegularExpressions.Ast;

namespace RegularExpressions.Automata
{
    /// <summary>
    /// An NFAState is a node with edges going to other NFAStates. Each outgoing edge is character labeled 
    /// and can only be followed by consuming an appropriate character from the input or it is an empty edge
    /// which can be followed without consuming anything.
    /// </summary>
    public class NFAState
    {
        // debugging niceities.
        public static int ID = 0;
        public readonly int instanceID;
        private RegExType type;

        // actual functionality
        public bool isFinalState = false;
        private Dictionary<char, List<NFAState>> namedEdges = new Dictionary<char, List<NFAState>>();
        private List<NFAState> anonEdges = new List<NFAState>();        

        public NFAState(RegExType type)
        {
            ID++;
            instanceID = ID;
            this.type = type;
        }

        /// Add a transition edge from this state to next which consumes the character c.
        internal void AddNamedEdge(char character, NFAState next)
        {
            Ensure(character);
            namedEdges[character].Add(next);
        }

        // make sure we have setup the list for the char in question
        private void Ensure(char character)
        {
            if (!namedEdges.ContainsKey(character))
                namedEdges.Add(character, new List<NFAState>());
        }

        //Add a transition edge from this state to next that does not consume a character.
        internal void AddAnonymousEdge(NFAState next)
        {
            anonEdges.Add(next);
        }

        internal virtual bool Matches(string input, bool matchEntireString)
        {
            return Matches(input, matchEntireString, 0, input.Length);
        }

        internal virtual bool Matches(string input, bool matchEntireString, int position, int originalLength)
        {
            return Matches(input, matchEntireString, position, originalLength, new HashSet<NFAState>());
        }

        internal virtual IEnumerable<NFAState> StatesForChar(char c)
        {
            if (namedEdges.ContainsKey(c))
                return namedEdges[c];

            return Enumerable.Empty<NFAState>();
        }

        internal virtual bool Matches(string input, bool matchEntireString, int position, int originalLength, HashSet<NFAState> visited)
        {
            // We move char by char through the string
            //
            // If we have reached the end of the input we need to check if
            // we are in a final state or if we can get to a final state along
            // anonmous edges only. if we can we have sucessfully matched and we return true.
            // 
            // If we have yet to reach the end, we try to consume a character
            // and match the rest of the input recursivly.
            // 
            // If that fails, we use the anonymous edges if we can reach the end of the string.
            // If we can we return true;
            // 
            // If even that fails, then the whole match process fails and we return false..
            // 
            // Because the NFA is a directed graph with explicitly includes looped 
            // paths we need to keep track of the nodes we have visited via anaonymous edges.
            // if we find ourselves at a node we have already visited, we have gone round in a loop
            // and we need to backtrack, returning false.

            // if we have already visited this node, we just exit early.
            // we go here by folloing a looped path in the graph
            if (visited.Contains(this))
                return false;

            // record that we have vissited this node.
            visited.Add(this);

            // is there any content to consume?
            if (!input.Any())
            {
                // no, the string is now empty.

                // we can only have a successful match if 
                // 1) we are in a final state               
                if (isFinalState)
                    return true;

                // or
                // 2) we can get to one via anonymouse edges. i.e. while consuming
                // no more input.
                return EmptyStateMatches("", matchEntireString, position, originalLength, visited);
            }
            else
            {
                // there is still input to consume
                // get the first character anc attempt to match along a named path
                var character = input[0];
                foreach (NFAState next in StatesForChar(character))
                {
                    // remove the first character of the string and recurse, incresing the recorded position in the string.
                    if (next.Matches(input.Substring(1), matchEntireString, position + 1, originalLength))
                        return true;
                }

                // no named edge resulted in a complete match
                // we attempt to find an anonymous edge which leads to a match
                if (EmptyStateMatches(input, matchEntireString, position, originalLength, visited))
                    return true;

                // still no match.
                // are we in Match Entire String Mode?
                // if not and we are at a final node, we have a match.
                if (!matchEntireString && isFinalState)
                    return true;

                return false;
            }
        }

        // iterate the anonymouse edges in turn, recursing on each
        internal virtual bool EmptyStateMatches(string s, bool matchEntireString, int position, int originalLength, HashSet<NFAState> visited)
        {
            foreach (NFAState next in anonEdges)
            {
                // pass visited here which keeps track of states visited via empty transitions
                if (next.Matches(s, matchEntireString, position, originalLength, visited))
                    return true;
            }

            return false;
        }
    }
}
