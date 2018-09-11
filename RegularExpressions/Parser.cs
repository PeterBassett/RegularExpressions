using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegularExpressions.Ast;
using RegularExpressions.Exceptions;

namespace RegularExpressions
{
    internal class Parser
    {
        private readonly string _pattern;
        private int _position;

        public Parser(string pattern)
        {
            _pattern = pattern;
            _position = 0;
        }

        #region Internal Parser Machinery
        /// <summary>
        /// take one character of input and throws if the char is not what we expect it to be.
        /// </summary>
        protected void Consume(char expected)
        {
            var c = _pattern[_position];

            if (expected == c)
                _position++;
            else
                throw new UnexpectedCharacterParseException(_pattern, _position);
        }

        /// <summary>
        /// get the current char in the input without moving to the next character
        /// </summary>
        protected char Peek()
        {
            return _pattern[_position];
        }

        /// <summary>
        /// peeks the next character and checks if it is any of the options provided 
        /// </summary>
        protected bool PeekIsAny(params char[] options)
        {
            var c = Peek();
            return options.Any(o => o == c);
        }

        /// <summary>
        /// move to the next character in the input stream
        /// </summary>
        protected char Advance()
        {
            var c = Peek();
            Consume(c);
            return c;
        }

        /// <summary>
        /// returns true if there are more characters in the input to process
        /// </summary>
        protected bool More()
        {
            return _position <= _pattern.Length - 1;
        }
        #endregion

        /// <summary>
        /// recursive descent parser levels cribbed from the antlr grammar here
        /// https://github.com/bkiers/pcre-parser/blob/master/src/main/antlr4/nl/bigo/pcreparser/PCRE.g4
        /// </summary>
        public AstNode Parse()
        {
            return Regex();
        }

        #region Parser Productions
        private AstNode Regex()
        {
            return Alternation();
        }

        private AstNode Alternation()
        {
            var exprNode = Expr();

            if (More() && PeekIsAny('|'))
            {
                Consume('|');
                var regexNode = Regex();
                return new Choice(exprNode, regexNode);
            }

            return exprNode;            
        }

        private AstNode Expr()
        {
            AstNode current = null;

            while (More() && !PeekIsAny('|', ')'))
            {
                // expr can't start with | or ) as these are handled above and below			    
                var elementNode = Element();

                if (current == null)
                    current = elementNode;
                else
                    current = new Sequence(current, elementNode);
            }
            return current;
        }

        private AstNode Element()
        {
            var atomNode = Atom();
            while (More() && PeekIsAny('*', '+', '?'))
            {
                var character = Peek();
                switch (character)
                {
                    case '*':
                        atomNode = new KleenStarRepetition(atomNode);
                        Consume('*');
                        break;
                    case '+':
                        atomNode = new PlusRepetition(atomNode);
                        Consume('+');
                        break;
                    case '?':
                        atomNode = new QuestionRepetition(atomNode);
                        Consume('?');
                        break;
                    default:
                        break;
                }
            }
            return atomNode;
        }

        private AstNode Atom()
        {
            switch (Peek())
            {
                case '(':
                {
                    Consume('(');
                    var regexNode = Regex();
                    Consume(')');
                    return regexNode;
                }
                case '\\':
                {
                    Consume('\\');
                    var character = Advance();
                    return ParseSpecialChars(character);
                }
                case '.':
                {
                    Consume('.');
                    return new AnyChar();
                }
                case '^':
                {
                    Consume('^');
                    return new StartOfInput();
                }
                case '$':
                {
                    Consume('$');
                    return new EndOfInput();
                }
                case '[':
                {
                    Consume('[');
                    var characterClassNode = MultiCharacterClass();
                    Consume(']');
                    return characterClassNode;
                }
                default:
                    return new Primitive(Advance());
            }
        }

        private CharacterClass ParseSpecialChars(char character)
        {
            switch (character)
            {
                // we are escaping a backspash
                case '\\':
                    return new CharacterClass('\\');
                case 'd': // digits
                    return new CharacterClass('0', '9');
                case 'w': // alpha numerics and dash
                    return CharacterClass.Union( 
                            new CharacterClass('A', 'Z'),
                            new CharacterClass('a', 'z'),
                            new CharacterClass('0', '9'),
                            new CharacterClass('-') 
                        );
                case 's': // whitespace
                    return CharacterClass.Union( 
                            new CharacterClass(' '),
                            new CharacterClass('\t'),
                            new CharacterClass('\r'),
                            new CharacterClass('\n'),
                            new CharacterClass('\f')
                        );
                default:
                    throw new UnexpectedCharacterParseException(string.Format("Unexpected escape sequence {0}", character));
            }
        }

        private CharacterClass MultiCharacterClass()
        {
            bool negative = false;
            // are we dealing with a negated character class?
            if (Peek() == '^')
            {
                negative = true;
                Consume('^');
            }

            var output = new CharacterClass();
            // keep going until we find the last closing bracket
            while (More() && Peek() != ']')
            {
                // parse a single charclass out
                var single = SingleCharacterClass();
                
                // union them together
                output = CharacterClass.Union(output, single);
                
                // have we got a intersection or substraction?
                while (More() && PeekIsAny('&', '-'))
                {
                    switch (Peek())
                    {
                        case '&':
                            // intersection
                            Consume('&');
                            Consume('&');
                            var and = IntersectionWithClass();
                            output = CharacterClass.Intersect(output, and);
                            break;
                        case '-':
                            // subtraction
                            Consume('-');
                            var subtract = MultiCharacterClass();
                            output = CharacterClass.Subtract(output, subtract);
                            break;
                    }
                }
            }

            if (negative)
                output = CharacterClass.Negate(output);

            return output;
        }

        private CharacterClass SingleCharacterClass()
        {
            // nested character classes?
            if (Peek() == '[')
            {
                Consume('[');
                // recurse to parse another character class
                var nestedCharacterClass = MultiCharacterClass();
                Consume(']');
                return nestedCharacterClass;
            }

            // we are expecting a single character, a dash or an escape sequence
            var characterStart = Advance();

            // if we have an escape sequence
            if (characterStart == '\\')
            {
                var character = Advance();
                return CharacterClass.Union(ParseSpecialChars(character));
            }
            // have we got a dash?
            else if (Peek() == '-')
            {
                Consume('-');
                // get the end of the char range
                var characterEnd = Advance();
                return new CharacterClass(characterStart, characterEnd);
            }
            else
                return new CharacterClass(characterStart, characterStart);
        }

        private CharacterClass IntersectionWithClass()
        {
            var output = new CharacterClass();

            while (More() && !PeekIsAny(']', '&'))
            {
                output = CharacterClass.Union(output, SingleCharacterClass());
            }

            return output;
        }
        #endregion
    }
}
