using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegularExpressions.Visitor;

namespace RegularExpressions.Ast
{
    public class EndOfInput : AstNode
	{			
        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}