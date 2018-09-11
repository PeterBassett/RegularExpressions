using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegularExpressions.Visitor;

namespace RegularExpressions.Ast
{
    public class Choice : AstNode
    {
	    public AstNode one;
	    public AstNode two;

	    public Choice(AstNode one, AstNode two)
	    {
		    this.one = one;
		    this.two = two;		    
	    }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}