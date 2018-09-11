using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegularExpressions.Exceptions
{
    [Serializable]
    public class UnexpectedCharacterParseException : Exception
    {
        public string Pattern { get; private set; }
        public int Position { get; private set; }
        public UnexpectedCharacterParseException(string pattern, int position)
        {
            Pattern = pattern;
            Position = position;
        }

        public UnexpectedCharacterParseException() { }
        public UnexpectedCharacterParseException(string message) : base(message) { }
        public UnexpectedCharacterParseException(string message, Exception inner) : base(message, inner) { }
        protected UnexpectedCharacterParseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
