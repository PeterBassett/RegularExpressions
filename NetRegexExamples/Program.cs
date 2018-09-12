using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace NetRegexExamples
{
    class Program
    {
        static void Main(string[] args)
        {
            var slides = new Action []
            {
                ()=>{
                    Section("Regular Expressions");
                    Explain("Navigation");
                    Explain("Next Slide : Right Key");
                    Explain("Previous Slide : Left Key");
                    Explain("First Slide : Up Key");
                    Explain("Last Slide : Down Key");
                    Explain("Exit : Esc Key");
                },

                ()=>{
                    Section("Plain Character Literals");
                    Explain("Matches just the character you specify and nothing else.");
                    RunRegex("a", "a", true); // will match
                    RunRegex("a", "b"); // will not match            
                },
                ()=>{
                    Section("Escaped Character Literals");
                    Explain("Allow you to enter difficult to type characters or as shortcuts to Character Classes (as seen below)");
                    RunRegex("\t", "this\thas some\ttabs in \t\tit", true);
                    RunRegex(@"\d", "a1b2b3", true);
                },
                ()=>{
                    Section("Escaped characters used in regex syntax");
                    Explain("Regex syntax uses several characters to define operations. If you want to match them, you just escape them with a backslash");
                    RunRegex(@"\*", "a*c", true);
                    RunRegex(@"\*", "abc", true);
                    RunRegex(@"\+", "a+c", true);
                },
                ()=>{
                    Section("Escaped backslash");
                    Explain("Even the backslash can be escaped into a plain literal for matching");
                    RunRegex(@"\\", @"\", true);
                },
                
                ()=> {
                    Section("Anchors (Assertions)");
                    
                    Explain("These consume no input but assert that the current matching is at a position in the stream");
                    Explain("^ matches only at the start of the string");
                    RunRegex(@"^\d+", "123-456");
                    Explain("$ matches only at the start of the string");
                    RunRegex(@"\d+$", "123-456");
                },

                ()=> {
                    Section("Anchors (Assertions) - Continued");
                    
                    Explain("These anchors can appear anywhere in a pattern, they dont have to be at the start/end");
                    Explain("^ matches only at the start of the string");
                    RunRegex(@"(^YES|^NO$)", "YES THIS IS....");
                    RunRegex(@"(^YES|^NO$)", "NO");
                },

                ()=> {
                    Section("Anchors (Assertions) - Continued");
                                        
                    Explain(@"\b matches work boundaries");
                    RunRegex(@"Bob.*\bcat\b", "Bob ate the cat");
                    RunRegex(@"Bob.*\bcat\b", "Bobcat");

                    Explain(@"Almost but not quite the same and a lot more difficult to test");
                    RunRegex(@"Bob.+ cat( |$)", "Bob ate the cat");
                },

                ()=>{
                    Section("Character Classes");
                    Explain("A character class matches any one of a SET of characters.");            
                    RunRegex(@"[aeiou]", @"abcdefghijklmnopqrstuvwxyz");
                },
                ()=>{
                    Section("Character Classes - Continued");
                    Explain("You can specify Ranges in Character Classes with a dash (-)");
                    RunRegex(@"[a-d]", @"abcdefghijklmnopqrstuvwxyz");
                },
                ()=>{
                    Section("Character Classes - Continued");
                    Explain("Multiple ranges union together");
                    RunRegex(@"[a-dk-l]", @"abcdefghijklmnopqrstuvwxyz");
                },
                ()=>{
                    Section("Character Classes - Continued");
                    Explain("You can negate or invert a character class with a ^ at the start of the set");
                    RunRegex(@"[^a-dk-l]", @"abcdefghijklmnopqrstuvwxyz");
                },
                ()=>{
                    Section("Character Classes - Continued");

                    Explain("You can subtract nested character classes. [base_group - [excluded_group]]");
                    RunRegex(@"[a-z-[pb]]", @"abcdefghijklmnopqrstuvwxyz");
                    RunRegex(@"[a-z-[x-z]]", @"abcdefghijklmnopqrstuvwxyz");
                    RunRegex(@"[a-z-[aeiou]]", @"abcdefghijklmnopqrstuvwxyz");
                    RunRegex(@"[a-z-[d-w-[m-o]]]", @"abcdefghijklmnopqrstuvwxyz");

                    RunRegex(@"[0-9-[1-3-[3]]]", @"0123456789");
                    RunRegex(@"[0-9-[2468]]+", @"1234128937941987329471365137845");

                    Explain(".NET for not support class intersection i.e. [a-z&&[^aeiou]] which would leave just the consonants.");                    
                },

                ()=>{
                    Section("Character Class Shortcuts");

                    Explain("You can write several character classes in a shorter way");

                    var str = "abc!d e1f£2gh$3  ij%4kl5^m   n6o&p7qr*8st9(uv0w)xyz-+=";
                    Explain(". matches any character");
                    RunRegex(@".", str);

                    Explain(@"\d matches any number");
                    RunRegex(@"\d", str);

                    Explain(@"\D matches any NON number");
                    RunRegex(@"\D", str);

                    Explain(@"\w matches any 'word' character");
                    RunRegex(@"\w", str);

                    Explain(@"\W matches any NON 'word' character");
                    RunRegex(@"\W", str);

                    Explain(@"\s matches any whitespace character");
                    RunRegex(@"\s", str);

                    Explain(@"\S matches any NON whitespace character");
                    RunRegex(@"\S", str);
                },

                ()=>{
                    Section("Quantifiers");
                    Explain("Quantifiers specify how many instances of a character, group, or character class must be present in the input for a match to be found");
                    Explain("they are GREEDY. They always match as much as possible. They will always consume as much of the input while still matching.");
                    Explain("* is Zero Or More - Note the match found after the end of the string (it is zero length)");
                    RunRegex(@"a*", @"aaa", true);
                    RunRegex(@"a*", "aaaaaaaaaaaaa", true);
                    Explain("+ is One Or More");
                    RunRegex(@"a+", @"aaa", true);
                    Explain("? is Zero Or One");
                    RunRegex(@"a?", @"aaa", true);
                },
                ()=>{
                    Section("Quantifiers - Continued");
                    Explain("And those quantifiers can be LAZY if required, where they consume as little as possible.");
                    RunRegex(@"a*?", @"aaa");
                    RunRegex(@"a*?", "aaaaaaaaaaaaa");

                    Explain("+ is One Or More");
                    RunRegex(@"a+?", @"aaa");

                    Explain("? is Zero Or One");
                    RunRegex(@"a??", @"aaa");
                },
                ()=> {
                    Section("Quantifiers - Continued");
                    Explain("There are also quantifiers to specify explicit counts.");
                    Explain("Between zero and two 'a's.");
                    RunRegex(@"a{0,2}", @"aaaaaa");
                    Explain("Exactly two'a's.");
                    RunRegex(@"a{2}", @"aaaaaa");
                    Explain("Two or more 'a's.");
                    RunRegex(@"a{2,}", @"aaaaaa");

                    Explain("Beween Two and Four 'a's.");
                    RunRegex(@"a{2,4}", @"aaaaaa");
                },

                ()=> {
                    Section("Alternation");
                    
                    Explain("You can use the vertical bar (|) character to match any one of a series of patterns, where the | character separates each pattern.");
                    Explain("It's a switch statement for a regex");

                    RunRegex(@"(a|b|c|d)", @"axbyhfhdc");
                    Explain("And the sub patterns can be full regexes in their own right");
                    RunRegex(@"(moose|squirrel)", @"moose");
                    RunRegex(@"(^a$|b+)", @"bbbbbbb");

                    RunRegex(@"([aeiou]|[z])", @"The quick brown fox jumped over the lazy dog");
                },

                ()=> {
                    Section("Conditional Matching");
                    
                    Explain("This attempts to match one of two patterns based on whether it can match an initial pattern.");
                    Explain("It's syntax is: (?( expression ) yes | no )");
                    Explain("It's an if statement : (?'if'( expression ) truepattern else falsepattern )");

                    RunRegex(@"\b(?(\d{2}-)\d{2}-\d{7}|\d{3}-\d{2}-\d{4})\b", "01-9999999 02-3339333 777-88-9999");
                    RunRegex(@"\b(?(\d{2}-)\d{2}-\d{7}|\d{3}-\d{2}-\d{4})\b", "NO---MATCH 02-3339333 777-88-9999");
                    //RunRegex(@"(?(A)X|Y)", "AXY");
                    //RunRegex(@"(?(A)X|Y)", "1XY");
                    //RunRegex(@"(?(condition))(then1|then2|then3)|(else1|else2|else3)", "condition then1 else2");
                    RunRegex(@"(?(condition))(then1|then2|then3)|(else1|else2|else3)", "codndition then1 else2");
                },

                ()=> {
                    Section("Unnamed Capturing Subexpressions");

                    Explain("Any use of a braced subexpression creates a capture which is available in the Match object");
                    Explain("These captures are available from the Match.Groups collection, but have no name. e.g. Match.Groups[1]");

                    RunRegex(@"((abc)\d+)?(xyz)", "abc9292934xyzJSJDHF SDDF45 7df", true);

                    Explain("Here we have three capturing groups.");
                    Explain("1 captures the abc and the folloing numbers");
                    Explain("2 captures just the abc");
                    Explain("3 captures the following xyz");

                    RunRegex(@"(a|b|c|d)", @"abcd", true);
                    Explain("Event just plain alternation counts as a capture");
                },

                ()=> {
                    Section("Named Capturing Subexpressions");
                    
                    Explain("These capture the result of a subexpression and make it available by name.");
                    Explain("They are available from the Match.Groups collection. e.g. Match.Groups[\"GroupName\"]"); 
                    Explain("The name must begin with a letter and consist only of letters and numbers");
                    
                    RunRegex(@"(?<One>(?<Two>abc)\d+)?(?<Three>xyz)", "abc9292934xyzJSJDHF SDDF45 7df", true);

                    Explain("Here we have three named capturing groups.");
                    Explain("One captures the abc and the folloing numbers");
                    Explain("Two captures just the abc");
                    Explain("Three captures the following xyz");

                    RunRegex(@"(?<character>a|b|c|d)", @"abcd", true);
                },

                ()=> {
                    Section("Back references");
                    
                    Explain("This is where the pain begins");

                    Explain("Back references provide a convenient way to identify a repeated character or substring within a string.");
                    Explain("example, if the input string contains multiple occurrences of an arbitrary substring, you can match the first");
                    Explain("occurrence with a capturing group, and then use a back reference to match subsequent occurrences of the substring.");

                    RunRegex(@"(\w)\1", "The moon will soon go kaboom", true);

                    Explain(@"Unnamed captures you use a back reference with just a \number");
                    Explain(@"e.g. \1 above");
                },

                ()=> {
                    Section("Named Back references");
                    
                    Explain(@"Named captures require you to use named back references with the syntax");
                    Explain(@"e.g. \k<name>");

                    RunRegex(@"(?<char>\w)\k<char>", "The moon will soon go kaboom", true);

                    Explain(@"You can mix and match, but dont do this.");
                    RunRegex(@"(?<char>\w)\1", "The moon will soon go kaboom");
                },

                ()=> {
                    Section("Conditional Matching Based on a Valid Captured Group");

                    Explain(@"This matches one of two patterns depending on whether it has matched a previously specified capturing group");
                    Explain(@"e.g. (?(name) yes | no )");
                    Explain(@"It's an if statement with a reference as its condition rather than an a subexpression as seen earlier.");

                    Explain(@"If we can match two digits and a dash, THEN match 7 digits only otherwise match 3-2-4 digits");

                    RunRegex(@"(?<name>\d{2}-)?(?(name)\d{7}|\d{3}-\d{2}-\d{4})", "01-9999999 020-333333 777-88-9999");
                    RunRegex(@"(?<name>\d{2}-)?(?(name)\d{7}|\d{3}-\d{2}-\d{4})", "a1-9999999 020-333333 777-88-9999");
                    RunRegex(@"(?<name>(condition))?(?(name)(then1|then2|then3)|(else1|else2|else3))", "condition then1 else2");
                    RunRegex(@"(?<name>(condition))?(?(name)(then1|then2|then3)|(else1|else2|else3))", "Condit1ion then1 else2");
                },

                ()=> {
                    Section("The End");
                    RunRegex(@"\b\w", @"Thats All Folks!");
                }
            };

            Present(slides);
        }

        #region Presentation Machinery
        private static void Present(Action [] slides)
        {
            int currentSlide = 0;
            do
            {
                DisplaySlide(currentSlide, slides);

                HandleInput(ref currentSlide, slides.Length);
            }
            while (currentSlide < slides.Length);
        }

        private static void DisplaySlide(int currentSlide, Action[] slides)
        {
            Console.Clear();
            Console.Write("{0} of {1}", currentSlide + 1, slides.Length);
            slides[currentSlide]();
        }

        private static void HandleInput(ref int currentSlide, int slideCount)
        {
            var key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.RightArrow:
                    currentSlide++;
                    break;
                case ConsoleKey.LeftArrow:
                    currentSlide--;
                    break;
                case ConsoleKey.UpArrow:
                    currentSlide=0;
                    break;
                case ConsoleKey.DownArrow:
                    currentSlide = slideCount - 1;
                    break;
                case ConsoleKey.Escape:
                    currentSlide = slideCount;
                    break;
            }

            if (currentSlide < 0)
                currentSlide = 0;
        }

        private static void Section(string sectionName)
        {
            WriteLine("");
            WriteHR();
            var colour = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            WriteLine(sectionName);
            Console.ForegroundColor = colour;
        }

        private static void Explain(string explanation)
        {
            WriteLine(explanation);         
        }

        static void RunRegex(string pattern, string input, bool showCaptures = false)
        {
            WriteHR();

            var regEx = new Regex(pattern);

            var matches = regEx.Matches(input);

            WriteLine("{0} -> {1}", pattern, input);

            Indent();

            if(matches.Count == 0)
                WriteLine("FAIL");

            var positions = new HashSet<Tuple<int, int>>();
            foreach (Match match in matches)
            {
                foreach (Capture capture in match.Captures)
                    positions.Add(new Tuple<int, int>(capture.Index, capture.Length));
            
                foreach (Group group in match.Groups)
                    positions.Add(new Tuple<int, int>(group.Index, group.Length));
            }

            if (showCaptures)
            {
                var namedGroups = regEx.GetGroupNames();

                foreach (Match match in matches)
                {
                    WriteLine(match.Success ? "SUCCESS" : "FAIL");
                    WriteLine("Matched \"{0}\" - Length {1}", match.Value, match.Length);

                    Indent();
                    foreach (Capture capture in match.Captures)
                    {
                        WriteLine("Capture \"{0}\" at Position {1}", capture.Value, capture.Index);
                    }

                    
                    foreach (var name in namedGroups)
                    {
                        var group = match.Groups[name];
                        WriteLine("Group {0} \"{1}\" at Position {2}", name, group.Value, group.Index);
                    }                    

                    Outdent();
                }
            }

            if (matches.Count > 0)
            {
                WriteMatchPositions(input, positions);
            }

            ResetIndenting();            
        }

        private static void WriteHR()
        {
            var width = Console.BufferWidth - 1;
            WriteLine(new string('-', width));
        }

        private static void WriteMatchPositions(string input, HashSet<Tuple<int, int>> positions)
        {
            var marker = (new string('_', input.Length + input.Count(c => c == '\t') + 1)).ToArray();
            foreach (var p in positions)
            {
                var tabToLeft = positions.Where(p2 => p2.Item1 < p.Item1 && input[p2.Item1] == '\t').Count();

                var start = p.Item1 + tabToLeft;
                marker[start] = '^';

                for (int i = 1; i < p.Item2; i++)
                {
                    marker[start + i] = '-';
                }
            }

            if (!positions.Any(p => p.Item1 == input.Length))
                marker[marker.Length - 1] = ' ';

            WriteLine(input);
            WriteIndent();
            foreach (var character in marker)
            {
                var colour = Console.ForegroundColor;
                if (character == '^')
                    Console.ForegroundColor = ConsoleColor.Red;

                if (character == '-')
                    Console.ForegroundColor = ConsoleColor.Yellow;

                Console.Write(character);

                Console.ForegroundColor = colour;
            }
            Console.WriteLine();
        }

        static int _indent = 0;
        static void Indent()
        {
            _indent++;
        }

        static void Outdent()
        {
            _indent--;
        }
        
        static void ResetIndenting()
        {
            _indent = 0;
        }

        static void WriteLine(string format, params object[] args)
        {
            Write(format, args);
            Console.WriteLine();
        }


        static void Write(string format, params object[] args)
        {
            WriteIndent();
            var newArgs = new object[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                newArgs[i] = args[i];

                if (args[i] is string)
                    newArgs[i] = args[i].ToString().Replace("\t", "\\t");
            }

            Console.Write(format.Replace("\t", "\\t"), newArgs);
        }

        private static void WriteIndent()
        {
            Console.Write(new string('\t', _indent));
        }
        #endregion
    }
}
