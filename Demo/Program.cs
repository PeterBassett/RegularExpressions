using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegularExpressions;
using RegularExpressions.Automata;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            NFA pat = NFA.Sequence(NFA.ZeroOrMany(NFA.Alternation("foo", "bar")), "");

	        var strings = new [] {
	            "foo", "bar", "foobar", "farboo", "boofar", "barfoo",
	            "foofoobarfoo1", "foofoobarfoo"
	        };

            Console.WriteLine("PATTERN : {0}", "(foo|bar)*");
	        foreach (string s in strings) 
            {
                Console.WriteLine(s + "\t:\t" + pat.Matches(s, matchEntireString: true));
	        }

            Console.ReadLine();
        }
    }
}
