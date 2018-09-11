using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegularExpressions.Visitor;

namespace RegularExpressions.Ast
{
    public class Primitive : AstNode
    {
	    public char character;

	    public Primitive(char character)
	    {
		    this.character = character;
	    }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}