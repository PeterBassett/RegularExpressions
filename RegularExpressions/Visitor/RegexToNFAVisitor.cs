using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegularExpressions.Ast;
using RegularExpressions.Automata;
using RegularExpressions.Exceptions;

namespace RegularExpressions.Visitor
{
    class RegexToNFAVisitor : IVisitor
    {
        private Stack<NFA> stack = new Stack<NFA>();

        public NFA Root
        {
            get
            {
                return stack.First();
            }
        }

        public void Visit(AnyChar target)
        {
            stack.Push( NFA.AnyCharacter() );
        }

        public void Visit(Blank target)
        {
            stack.Push( NFA.empty() );
        }   

        public void Visit(Choice target)
        {
            target.one.Accept(this);
            var f1 = stack.Pop(); 
            target.two.Accept(this);
            var f2 = stack.Pop();

            stack.Push( NFA.Alternation(f1, f2) );
        }

        public void Visit(CharacterClass target)
        {
            stack.Push( NFA.CharacterClass(target) );
        }

        public void Visit(PlusRepetition target)
        {
            target.child.Accept(this);
            var f = stack.Pop();
            stack.Push( NFA.OneOrMany(f) );
        }

        public void Visit(Primitive target)
        {
            stack.Push( NFA.Character(target.character) );
        }

        public void Visit(QuestionRepetition target)
        {
            target.child.Accept(this);
            var f = stack.Pop();
            stack.Push( NFA.ZeroOrOne(f) );
        }

        public void Visit(Sequence target)
        {
            target.first.Accept(this);
            var f1 = stack.Pop();
            target.second.Accept(this);
            var f2 = stack.Pop();

            stack.Push( NFA.Sequence(f1, f2) );
        }

        public void Visit(KleenStarRepetition target)
        {
            target.child.Accept(this);
            var f = stack.Pop();

            stack.Push( NFA.ZeroOrMany(f) );
        }

        public void Visit(StartOfInput target)
        {
            stack.Push(NFA.StartOfStringAssertion());
        }

        public void Visit(EndOfInput target)
        {
            stack.Push(NFA.EndOfStringAssertion());
        }
    }
}
