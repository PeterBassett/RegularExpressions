using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegularExpressions.Ast
{
    public class Prefix : AstNode
    {
	    public AstNode contained;

	    public Prefix(AstNode re)
	    {
		    this.contained = re;
		    this.type = RegExType.Prefix;
	    }
    }
}