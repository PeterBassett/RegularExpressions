using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegularExpressions.Ast;

namespace RegularExpressions.Visitor
{
    public interface IVisitor
    {
        void Visit(AnyChar target);
        void Visit(Blank target);
        void Visit(Choice target);
        void Visit(CharacterClass target);
        void Visit(PlusRepetition target);
        void Visit(Primitive target);
        void Visit(QuestionRepetition target);
        void Visit(Sequence target);
        void Visit(KleenStarRepetition target);
        void Visit(StartOfInput target);
        void Visit(EndOfInput target);
    }
}
