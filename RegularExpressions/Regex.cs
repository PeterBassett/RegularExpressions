using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegularExpressions.Ast;
using RegularExpressions.Automata;
using RegularExpressions.Visitor;

namespace RegularExpressions
{
    public class Regex
    {
        public static AstNode Blank = new Blank();
        private NFA _matcher;

        public Regex(string pattern)
        {            
            var parser = new Parser(pattern);
            var root = parser.Parse();
            _matcher = ConvertRegexAstToNFA(root);            
        }

        private NFA ConvertRegexAstToNFA(AstNode root)
        {
            var visitor = new RegexToNFAVisitor();
            root.Accept(visitor);
            return visitor.Root;
        }
        
        public bool IsMatchWholeString(string input)
        {
            return _matcher.Matches(input, matchEntireString: true);
        }

        public bool IsMatchAnywhere(string input)
        {
            if (string.IsNullOrEmpty(input))
                if(_matcher.Matches("", matchEntireString: false))
                    return true;        

            for (int i = 0; i < input.Length; i++)
            {
                if (_matcher.Matches(input.Substring(i), matchEntireString:false))
                    return true;
            }

            return false;
        }
    }
}
