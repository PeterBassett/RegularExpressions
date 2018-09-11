using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegularExpressions.Visitor;

namespace RegularExpressions.Ast
{
    public abstract class AstNode : IVisitable
    {
        public abstract void Accept(IVisitor visitor);
    }
}