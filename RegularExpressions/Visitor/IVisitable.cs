using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegularExpressions.Visitor
{
    public interface IVisitable
    {
        void Accept(IVisitor visitor);
    }
}
