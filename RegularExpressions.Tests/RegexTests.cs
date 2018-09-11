using System;
using NUnit.Framework;

namespace RegularExpressions.Tests
{
    [TestFixture]
    public class RegexTests
    {
        [TestCase("a", "a", true)]

        [TestCase("a", "a-andthemsome", false)]

        [TestCase("a", "b", false)]
        [TestCase("a+", "a", true)]
        [TestCase("a*", "a", true)]
        [TestCase("a*", "aaa", true)]
        [TestCase("a*b", "aab", true)]
        [TestCase("a*b", "aac", false)]
        [TestCase("a.c", "abc", true)]
        [TestCase("a.c", "adc", true)]
        [TestCase("a.c", "bdc", false)]
        [TestCase("abc", "abc", true)]
        [TestCase("abc", "xbc", false)]
        [TestCase("abc", "axc", false)]
        [TestCase("abc", "abx", false)]
        [TestCase("a|b", "a", true)]
        [TestCase("a|b", "b", true)]
        [TestCase("a|b*", "bbbb", true)]
        [TestCase("a|b*", "a", true)]
        [TestCase("a*|b", "aaaaa", true)]
        [TestCase("a|b*", "bbbbb", true)]
        [TestCase("a*|b", "b", true)]
        [TestCase("a|b*|c", "bbbb", true)]
        [TestCase("(Fido|Dave)", "Fido", true)]
        [TestCase("(Fido|Dave)", "Dido", false)]
        [TestCase("C|SQL|HtML", "SQL", true)]
        [TestCase("a|b*|c", "a", true)]
        [TestCase("a|b*|c", "b", true)]
        [TestCase("a|b*|c", "c", true)]
        [TestCase("a|b*|c", "aa", false)]
        [TestCase("a|b*|c", "bbbbbb", true)]
        [TestCase("a|b*|c", "", true)]
        [TestCase("a|b*|c", "cc", false)]        
        [TestCase("a|b+|c", "a", true)]
        [TestCase("a|b+|c", "b", true)]
        [TestCase("a|b+|c", "c", true)]
        [TestCase("a|b+|c", "aa", false)]
        [TestCase("a|b+|c", "bbbbbb", true)]
        [TestCase("a|b+|c", "", false)]
        [TestCase("a|b+|c", "cc", false)]
        [TestCase("a|bf+|c", "a", true)]
        [TestCase("a|bf+|c", "c", true)]
        [TestCase("a|bf+|c", "bf", true)]
        [TestCase("a|bf+|c", "bfffff", true)]
        [TestCase("a|bf+|c", "bfg", false)]
        [TestCase("([a-z][a-z0-9]*,)+", "a5,b7,c9,", true)]
        [TestCase("[a-z0-9]*", "a53j5j7j3", true)]
        [TestCase("[a-z0-9]*", "---", false)]

        [TestCase("^a", "a", true)]
        [TestCase("a$", "a", true)]

        [TestCase("(0|(1(01*0)*1))*", "0"   , true)]
        [TestCase("(0|(1(01*0)*1))*", "00"  , true)]
        [TestCase("(0|(1(01*0)*1))*", "11"  , true)]
        [TestCase("(0|(1(01*0)*1))*", "000" , true)]
        [TestCase("(0|(1(01*0)*1))*", "011" , true)]
        [TestCase("(0|(1(01*0)*1))*", "110" , true)]
        [TestCase("(0|(1(01*0)*1))*", "0000", true)]
        [TestCase("(0|(1(01*0)*1))*", "0011", true)]
        [TestCase("(0|(1(01*0)*1))*", "0110", true)]
        [TestCase("(0|(1(01*0)*1))*", "1001", true)]
        [TestCase("(0|(1(01*0)*1))*", "1100", true)]
        [TestCase("(0|(1(01*0)*1))*", "1111", true)]
        [TestCase("[a-z]", "a", true)]
        [TestCase("[b-z]", "a", false)]
        [TestCase("([a-c0-3])+", "abc123", true)]
        [TestCase("([a-c0-3])+", "abc1234", false)]
        [TestCase("([a-c0-3])+", "abcd123", false)]
        [TestCase("[abc]", "a", true)]
        [TestCase("[abc]", "b", true)]
        [TestCase("[abc]", "c", true)]
        [TestCase("[abc]", "d", false)]
        [TestCase("[a-c&&c-e]", "a", false)]
        [TestCase("[a-c&&c-e]", "b", false)]
        [TestCase("[a-c&&c-e]", "c", true)]
        [TestCase("[a-c&&c-e]", "d", false)]
        [TestCase("[a-c&&c-e]", "e", false)]
        [TestCase("[a-c-[b]]", "a", true)]
        [TestCase("[a-c-[b]]", "b", false)]
        [TestCase("[a-c-[b]]", "c", true)]
        
        [TestCase(@"\d", "0", true)]
        [TestCase(@"\d", "9", true)]
        [TestCase(@"\d", "a", false)]

        [TestCase(@"\w", "a", true)]
        [TestCase(@"\w", "z", true)]
        [TestCase(@"\w", "A", true)]
        [TestCase(@"\w", "Z", true)]
        [TestCase(@"\w", "0", true)]
        [TestCase(@"\w", "9", true)]
        [TestCase(@"\w", " ", false)]
        
        [TestCase(@"\s", " ", true)]
        [TestCase(@"\s", "\t", true)]
        [TestCase(@"\s", "\r", true)]
        [TestCase(@"\s", "\n", true)]
        [TestCase(@"\s", "\f", true)]
        [TestCase(@"\s", "a", false)]
        [TestCase(@"\\", "a", false)]
        [TestCase(@"\\", "\\", true)]
        [TestCase(@"[\\]", "a", false)]
        [TestCase(@"[\\]", "\\", true)]
        [TestCase(@"[[a\\]&&[\\]]", "a", false)]
        [TestCase(@"[[a\\]&&[\\]]", "\\", true)]

        [TestCase(@"[\d&&\w]", "0", true)]
        [TestCase(@"[\d&&\w]", "9", true)]
        [TestCase(@"[\d&&\w]", "a", false)]
        [TestCase(@"[\d&&\w]", "z", false)]

        [TestCase(@"\u{41}", "A", true)]
        [TestCase(@"\u{41}", "a", false)]
        [TestCase(@"[\u{41}-\u{5A}]+", "ABCDEFGHIJKLMNOPQRSTUVWXYZ", true)]
        [TestCase(@"[\u{41}-\u{5A}]+", "abcdefghijklmnopqrstuvwxyz", false)]
        [TestCase(@"[\u{5A}-\\]+", "\x5a\x5b\x5c", true)]
        [TestCase(@"[\u{5A}-\\]+", "\x5a\x5b\x5c\x5d", false)]
        public void IsMatchWholeStringTest(string pattern, string input, bool expected)
        {
            var target = new Regex(pattern);

            Assert.AreEqual(expected, target.IsMatchWholeString(input));
        }

        [TestCase("a", "123a456", true)]
        [TestCase("a", "123b456", false)]
        [TestCase("a", "a", true)]
        [TestCase("a", "a-andthemsome", true)]
        [TestCase("a", "b", false)]
        [TestCase("a+", "a", true)]
        [TestCase("a*", "a", true)]
        [TestCase("a*", "aaa", true)]
        [TestCase("a*b", "aab", true)]
        [TestCase("a*b", "aac", false)]
        [TestCase("a.c", "abc", true)]
        [TestCase("a.c", "adc", true)]
        [TestCase("a.c", "bdc", false)]
        [TestCase("abc", "abc", true)]
        [TestCase("abc", "xbc", false)]
        [TestCase("abc", "axc", false)]
        [TestCase("abc", "abx", false)]
        [TestCase("a|b", "a", true)]
        [TestCase("a|b", "b", true)]
        [TestCase("a|b*", "bbbb", true)]
        [TestCase("a|b*", "a", true)]
        [TestCase("a*|b", "aaaaa", true)]
        [TestCase("a|b*", "bbbbb", true)]
        [TestCase("a*|b", "b", true)]
        [TestCase("a|b*|c", "bbbb", true)]
        [TestCase("(Fido|Dave)", "Fido", true)]
        [TestCase("(Fido|Dave)", "Dido", false)]
        [TestCase("C|SQL|HtML", "SQL", true)]
        [TestCase("a|b*|c", "a", true)]
        [TestCase("a|b*|c", "b", true)]
        [TestCase("a|b*|c", "c", true)]
        [TestCase("a|b*|c", "aa", true)]
        [TestCase("a|b*|c", "bbbbbb", true)]
        [TestCase("a|b*|c", "", true)]
        [TestCase("a|b*|c", "cc", true)]
        [TestCase("a|b*|c", "d", true)]
        [TestCase("a|b+|c", "d", false)]
        [TestCase("a|b+|c", "a", true)]
        [TestCase("a|b+|c", "b", true)]
        [TestCase("a|b+|c", "c", true)]
        [TestCase("a|b+|c", "aa", true)]
        [TestCase("a|b+|c", "bbbbbb", true)]
        [TestCase("a|b+|c", "", false)]
        [TestCase("a|b+|c", "cc", true)]
        [TestCase("a|bf+|c", "a", true)]
        [TestCase("a|bf+|c", "c", true)]
        [TestCase("a|bf+|c", "bf", true)]
        [TestCase("a|bf+|c", "bfffff", true)]
        [TestCase("a|bf+|c", "bfg", true)]
        [TestCase("([a-z][a-z0-9]*,)+", "a5,b7,c9,", true)]
        [TestCase("[a-z0-9]*", "a53j5j7j3", true)]
        [TestCase("[a-z0-9]*", "---", true)]
        [TestCase("[a-z0-9]+", "---", false)]

        [TestCase("^a", "a", true)]
        [TestCase("a$", "a", true)]

        [TestCase("(0|(1(01*0)*1))*", "0", true)]
        [TestCase("(0|(1(01*0)*1))*", "00", true)]
        [TestCase("(0|(1(01*0)*1))*", "11", true)]
        [TestCase("(0|(1(01*0)*1))*", "000", true)]
        [TestCase("(0|(1(01*0)*1))*", "011", true)]
        [TestCase("(0|(1(01*0)*1))*", "110", true)]
        [TestCase("(0|(1(01*0)*1))*", "0000", true)]
        [TestCase("(0|(1(01*0)*1))*", "0011", true)]
        [TestCase("(0|(1(01*0)*1))*", "0110", true)]
        [TestCase("(0|(1(01*0)*1))*", "1001", true)]
        [TestCase("(0|(1(01*0)*1))*", "1100", true)]
        [TestCase("(0|(1(01*0)*1))*", "1111", true)]
        public void IsMatchAnywhereTest(string pattern, string input, bool expected)
        {
            var target = new Regex(pattern);

            Assert.AreEqual(expected, target.IsMatchAnywhere(input));

            var f = System.Text.RegularExpressions.Regex.IsMatch("d", "a|b*|c");
        }

        [Test] 
        public void SubtractedCharacterClassesTest()
        {
            var subtractedConsonants = new Regex("[a-z-[aeiuo]]");
            var explicitConsonants = new Regex("[b-df-hj-np-tv-z]");

            for (char c = 'a'; c <= 'z'; c++)
            {
                var s = new string(c, 1);
                Assert.AreEqual(explicitConsonants.IsMatchWholeString(s), subtractedConsonants.IsMatchWholeString(s));   
            }            
        }
        
        [Test] 
        public void IntersectedCharacterClassesTest()
        {
            var intersectedConsonants = new Regex("[a-z&&[^aeiuo]]");
            var explicitConsonants = new Regex("[b-df-hj-np-tv-z]");

            for (char c = 'a'; c <= 'z'; c++)
            {
                var s = new string(c, 1);
                Assert.AreEqual(explicitConsonants.IsMatchWholeString(s), intersectedConsonants.IsMatchWholeString(s));   
            }            
        }
    }
}
