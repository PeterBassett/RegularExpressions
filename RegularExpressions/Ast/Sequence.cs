using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegularExpressions.Visitor;

namespace RegularExpressions.Ast
{
    public class Sequence : AstNode
    {
	    public AstNode first;
	    public AstNode second;

	    public Sequence(AstNode first, AstNode second)
	    {
		    this.first = first;
		    this.second = second;
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}