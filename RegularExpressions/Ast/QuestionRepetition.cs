using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegularExpressions.Visitor;

namespace RegularExpressions.Ast
{
    public class QuestionRepetition : AstNode
    {
	    public AstNode child;

	    public QuestionRepetition(AstNode child)
	    {
		    this.child = child;
	    }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}