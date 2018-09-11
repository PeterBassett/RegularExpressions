using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegularExpressions.Ast
{
	public class Suffix : AstNode
	{
		public AstNode contained;

		public Suffix(AstNode re)
		{
			this.contained = re;
			this.type = RegExType.Suffix;
		}
	}
}