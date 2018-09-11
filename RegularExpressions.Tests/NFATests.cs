using System;
using NUnit.Framework;
using RegularExpressions.Automata;

namespace RegularExpressions.Tests
{
    [TestFixture]
    public class NFATests
    {
        private void Test(string input, bool expected, NFA target, string pattern)
        {
            var actual = target.Matches(input, matchEntireString: true);
            var systemActual = System.Text.RegularExpressions.Regex.IsMatch(input, pattern);

            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected, systemActual);
        }

        [TestCase("", false)]
        [TestCase("a", true)]
        [TestCase("aa", false)]
        [TestCase("aaa", false)]
        [TestCase("b", false)]
        [TestCase("ab", false)]
        [TestCase(" aa", false)]
        [TestCase("aa aa", false)]
        [TestCase("aaaaaaaaaaaaaaaaaaaaa", false)]
        [TestCase("aaaaaaaaaaaaaaaaaaaaab", false)]
        public void JustAnA(string input, bool expected)
        {
            string pattern = "^a$";
            var target = NFA.Character('a');

            Test(input, expected, target, pattern);
        }


        [TestCase("a", true)]
        [TestCase("aa", false)]
        [TestCase("abbbbb", false)]
        [TestCase("b", false)]
        public void JustAnAWithoutEnforcinfItBeTheEntireString(string input, bool expected)
        {
            string pattern = "^a$";
            var target = NFA.Character('a');

            Test(input, expected, target, pattern);
        }

        [TestCase("", false)]
        [TestCase("b", true)]
        [TestCase("ba", false)]
        [TestCase("baa", false)]
        [TestCase("ab", false)]
        [TestCase(" b", false)]
        [TestCase("bb bb", false)]
        [TestCase("bbbbbbbbbbbbbbbbbbbbb", false)]
        [TestCase("bbbbbbbbbbbbbbbbbbbbbc", false)]
        public void JustAB(string input, bool expected)
        {
            string pattern = "^b$";
            var target = NFA.Character('b');

            Test(input, expected, target, pattern);
        }

        [TestCase("", true)]
        [TestCase("a", true)]
        [TestCase("aa", true)]
        [TestCase("aaa", true)]
        [TestCase("b", false)]
        [TestCase("ab", false)]
        [TestCase(" aa", false)]
        [TestCase("aa aa", false)]
        [TestCase("aaaaaaaaaaaaaaaaaaaaa", true)]
        [TestCase("aaaaaaaaaaaaaaaaaaaaab", false)]
        public void AZeroOrMoreRepeated(string input, bool expected)
        {
            string pattern = "^a*$";
            var target = NFA.ZeroOrMany(NFA.Character('a'));

            Test(input, expected, target, pattern);
        }

        [TestCase("", false)]
        [TestCase("a", true)]
        [TestCase("aa", true)]
        [TestCase("aaa", true)]
        [TestCase("b", false)]
        [TestCase("ba", false)]
        [TestCase("ab", false)]
        [TestCase(" aa", false)]
        [TestCase("aa aa", false)]
        [TestCase("aaaaaaaaaaaaaaaaaaaaa", true)]
        [TestCase("aaaaaaaaaaaaaaaaaaaaab", false)]
        public void AOneOrMoreRepeated(string input, bool expected)
        {
            string pattern = "^a+$";
            var target = NFA.OneOrMany(NFA.Character('a'));

            Test(input, expected, target, pattern);
        }

        [TestCase("a", true)]
        [TestCase("b", true)]
        [TestCase("c", false)]
        [TestCase("aa", false)]
        [TestCase("bb", false)]
        [TestCase("ab", false)]
        public void AlternationTest(string input, bool expected)
        {
            string pattern = "^(a|b)$";
            var target = NFA.Alternation('a', 'b');

            Test(input, expected, target, pattern);
        }

        [TestCase("a", true)]
        [TestCase("b", true)]
        [TestCase("c", true)]
        [TestCase("aa", false)]
        [TestCase("bb", false)]
        [TestCase("cc", false)]
        [TestCase("ab", false)]
        [TestCase("abc", false)]
        public void MultiAlternationTest(string input, bool expected)
        {
            string pattern = "^(a|b|c)$";
            var target = NFA.Alternation('a', 'b', 'c');

            Test(input, expected, target, pattern);
        }

        [TestCase("a", true)]
        [TestCase("b", true)]
        [TestCase("c", true)]
        [TestCase("aa", false)]
        [TestCase("bb", false)]
        [TestCase("cc", false)]
        [TestCase("ab", false)]
        [TestCase("abc", false)]
        public void MultiAlternationNoHelpersTest(string input, bool expected)
        {
            string pattern = "^(a|b|c)$";
            var target = NFA.Alternation(NFA.Character('a'), NFA.Character('b'), NFA.Character('c'));

            Test(input, expected, target, pattern);
        }

        [TestCase("foo", true)]
        [TestCase("bar", false)]
        [TestCase("foobar", false)]
        [TestCase("farboo", false)]
        [TestCase("boofar", false)]
        [TestCase("barfoo", false)]
        [TestCase("foofoobarfooX", false)]
        [TestCase("foofoobarfoo", false)]
        public void SequenceTest(string input, bool expected)
        {
            string pattern = "^foo$";
            var target = NFA.Sequence("foo");

            Test(input, expected, target, pattern);
        }

        [TestCase("foo", true)]
        [TestCase("bar", false)]
        [TestCase("foobar", false)]
        [TestCase("farboo", false)]
        [TestCase("boofar", false)]
        [TestCase("barfoo", false)]
        [TestCase("foofoobarfooX", false)]
        [TestCase("foofoobarfoo", false)]
        public void SequenceExplicitTest(string input, bool expected)
        {
            string pattern = "^foo$";
            var target = NFA.Sequence(NFA.Character('f'), NFA.Character('o'), NFA.Character('o'));

            Test(input, expected, target, pattern);
        }

        [TestCase("foo", true)]
        [TestCase("bar", true)]
        [TestCase("foobar", true)]
        [TestCase("farboo", false)]
        [TestCase("boofar", false)]
        [TestCase("barfoo", true)]
        [TestCase("foofoobarfooX", false)]
        [TestCase("foofoobarfoo", true)]
        public void RepeatedAlternation(string input, bool expected)
        {
            string pattern = "^(foo|bar)*$";
            var target = NFA.Sequence(NFA.ZeroOrMany(NFA.Alternation("foo", "bar")), "");

            Test(input, expected, target, pattern);
        }

        [TestCase("", false)]
        [TestCase("a", true)]
        [TestCase("ab", true)]
        [TestCase("abb", false)]
        [TestCase("b", false)]
        [TestCase("aa", false)]
        [TestCase("aab", false)]
        [TestCase("aabb", false)]
        public void OptionalTest(string input, bool expected)
        {
            string pattern = "^ab?$";
            var target = NFA.Sequence(NFA.Character('a'), NFA.ZeroOrOne(NFA.Character('b')));

            Test(input, expected, target, pattern);
        }

        [TestCase("a", true)]
        [TestCase("b", true)]
        [TestCase("c", true)]
        [TestCase("c", true)]
        public void AnyCharTest(string input, bool expected)
        {
            string pattern = "^.$";
            var target = NFA.AnyCharacter();

            Test(input, expected, target, pattern);
        }

        [TestCase("abc", true)]
        [TestCase("abd", false)]
        [TestCase("adc", true)]
        [TestCase("aZc", true)]
        [TestCase("bbc", false)]
        [TestCase("Z.Z", false)]
        [TestCase("ZZZ", false)]
        public void AnyCharSequenceTest(string input, bool expected)
        {
            string pattern = "^a.c$";
            var target = NFA.Sequence(NFA.Character('a'), NFA.AnyCharacter(), NFA.Character('c'));

            Test(input, expected, target, pattern);
        }

        [TestCase("a", true)]
        [TestCase("ab", false)]
        [TestCase("ba", false)]
        public void StartOfStringAssertionTest(string input, bool expected)
        {
            string pattern = "^a$";
            var target = NFA.Sequence(NFA.StartOfStringAssertion(), NFA.Character('a'));

            Test(input, expected, target, pattern);
        }

        [TestCase("fooa", true)]
        [TestCase("fooas", false)]
        [TestCase("foob", false)]
        [TestCase("aaab", false)]
        public void EndOfStringAssertionTest(string input, bool expected)
        {
            string pattern = "^fooa$";
            var target = NFA.Sequence(NFA.Sequence("fooa"), NFA.EndOfStringAssertion());

            Test(input, expected, target, pattern);
        }

        [TestCase("a", true)]
        [TestCase("bbbb", true)]
        [TestCase("abbbb", false)]
        [TestCase("c", true)]
        public void StartOfStringAssertionMidPatternTest(string input, bool expected)
        {
            string pattern = "^(a|^b*|c)$";
            var target = NFA.Alternation(NFA.Character('a'), NFA.Sequence( NFA.StartOfStringAssertion(), NFA.ZeroOrMany( NFA.Character('b') ) ), NFA.Character('c'));

            Test(input, expected, target, pattern);
        }

        [TestCase("a", true)]
        [TestCase("bbbb", true)]
        [TestCase("abbbb", false)]
        [TestCase("bbbba", false)]
        [TestCase("c", true)]
        public void EndOfStringAssertionMidPatternTest(string input, bool expected)
        {
            string pattern = "^(a|b*$|c)$";
            var target = NFA.Alternation(
                            NFA.Character('a'), 
                            NFA.Sequence(
                                NFA.ZeroOrMany(
                                    NFA.Character('b')
                                ), 
                                NFA.EndOfStringAssertion()
                            ), 
                            NFA.Character('c')
                        );

            Test(input, expected, target, pattern);
        }

        [TestCase("abbbb", true)]
        [TestCase("cdddd", false)]
        [TestCase("effff", false)]

        [TestCase("aabbb", false)]
        [TestCase("ccddd", false)]
        [TestCase("eefff", false)]
        [TestCase("acccc", false)]
        
        [TestCase("-abbbb", false)]
        [TestCase("cdddd-", false)]
        [TestCase("effff-", false)]
        [TestCase("-effff", false)]
        public void StartAndEndOfStringAssertionMidPatternTest(string input, bool expected)
        {
            string pattern = "^ab+$";
            var target = NFA.Sequence(                    
                            NFA.StartOfStringAssertion(),
                            NFA.Character('a'),
                            NFA.OneOrMany(
                                NFA.Character('b')
                            ),                        
                            NFA.EndOfStringAssertion()
                        );

            Test(input, expected, target, pattern);
        }

        [TestCase("AbC", true)]
        [TestCase("AcC", true)]
        [TestCase("AbbbbbbbbbbbC", true)]
        [TestCase("AbcdefghijklC", true)]
        [TestCase("Abcdefghijkl", false)]
        [TestCase("AC", false)]
        public void AnyCharSequenceRepeatedTest(string input, bool expected)
        {
            string pattern = "^A.+C$";
            var target = NFA.Sequence(NFA.Character('A'), NFA.OneOrMany( NFA.AnyCharacter() ), NFA.Character('C'));

            Test(input, expected, target, pattern);
        }

        [TestCase("a", true)]
        [TestCase("b", true)]
        [TestCase("c", true)]
        [TestCase("d", true)]
        [TestCase("e", true)]
        [TestCase("f", true)]
        [TestCase("g", true)]
        [TestCase("h", true)]
        [TestCase("i", true)]
        [TestCase("j", true)]
        [TestCase("k", true)]
        [TestCase("l", true)]
        [TestCase("m", true)]
        [TestCase("n", true)]
        [TestCase("o", true)]
        [TestCase("p", true)]
        [TestCase("q", true)]
        [TestCase("r", true)]
        [TestCase("s", true)]
        [TestCase("t", true)]
        [TestCase("u", true)]
        [TestCase("v", true)]
        [TestCase("w", true)]
        [TestCase("x", true)]
        [TestCase("y", true)]
        [TestCase("z", true)]
        [TestCase("A", false)]
        [TestCase("B", false)]
        [TestCase("C", false)]
        [TestCase("D", false)]
        [TestCase("E", false)]
        [TestCase("F", false)]
        [TestCase("G", false)]
        [TestCase("H", false)]
        [TestCase("I", false)]
        [TestCase("J", false)]
        [TestCase("K", false)]
        [TestCase("L", false)]
        [TestCase("M", false)]
        [TestCase("N", false)]
        [TestCase("O", false)]
        [TestCase("P", false)]
        [TestCase("Q", false)]
        [TestCase("R", false)]
        [TestCase("S", false)]
        [TestCase("T", false)]
        [TestCase("U", false)]
        [TestCase("V", false)]
        [TestCase("W", false)]
        [TestCase("X", false)]
        [TestCase("Y", false)]
        [TestCase("Z", false)]
        [TestCase("0", false)]
        [TestCase("1", false)]
        [TestCase("2", false)]
        [TestCase("3", false)]
        [TestCase("4", false)]
        [TestCase("5", false)]
        [TestCase("6", false)]
        [TestCase("7", false)]
        [TestCase("8", false)]
        [TestCase("9", false)]
        public void CharacterClassTest(string input, bool expected)
        {
            string pattern = "^[a-z]$";
            var target = NFA.CharacterClass('a', 'z');

            Test(input, expected, target, pattern);
        }

        [TestCase("abbaaabba", true)]
        [TestCase("abbaasabba", false)]
        [TestCase("aaaaaaa", true)]
        [TestCase("bbbbbbb", true)]
        public void CharacterClassRepeatedTest(string input, bool expected)
        {
            string pattern = "^[a-b]+$";
            var target = NFA.OneOrMany(NFA.CharacterClass('a', 'b'));

            Test(input, expected, target, pattern);
        }

        [TestCase("a", true)]
        [TestCase("b", true)]
        [TestCase("c", false)]
        [TestCase("d", false)]
        [TestCase("e", false)]
        [TestCase("f", true)]
        [TestCase("g", true)]
        public void CharacterClassInvertedTest(string input, bool expected)
        {
            string pattern = "^[^c-e]$";
            var target = NFA.CharacterClass('c', 'e', included:false);

            Test(input, expected, target, pattern);
        }

        [TestCase("abc", false)]
        [TestCase("abbbbc", false)]
        [TestCase("ac", false)]
        [TestCase("abbbd", false)]
        [TestCase("bbbbc", false)]
        [TestCase("fbc", false)]
        [TestCase("a", true)]
        [TestCase("b", true)]
        [TestCase("c", true)]
        [TestCase("", true)]
        [TestCase("bb", true)]
        [TestCase("bbbbbbbbbbb", true)]
        [TestCase("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", true)]
        public void AlternationAndRepetitionTest(string input, bool expected)
        {
            string pattern = "^(a|b*|c)$";

            var target = NFA.Alternation(NFA.Character('a'), NFA.Alternation( NFA.ZeroOrMany( NFA.Character('b') ), NFA.Character('c') ) );

            Test(input, expected, target, pattern);
        }
    }
}
