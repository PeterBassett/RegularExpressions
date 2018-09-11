using System;
using NUnit.Framework;
using RegularExpressions.Ast;
using RegularExpressions.Automata;

namespace RegularExpressions.Tests
{
    [TestFixture]
    public class CharacterClassTests
    {
        [TestCase('a', false)]
        [TestCase('b', true)]
        [TestCase('c', false)]
        public void SimpleContainsTest(char c, bool expected)
        {
            var target = new CharacterClass('b');

            Assert.AreEqual(expected, target.Contains(c));
        }

        [TestCase('a', false)]
        [TestCase('b', true)]
        [TestCase('c', true)]
        [TestCase('d', true)]
        [TestCase('e', false)]
        [TestCase('1', false)]
        public void SimpleRangeContainsTest(char c, bool expected)
        {
            var target = new CharacterClass('b', 'd');

            Assert.AreEqual(expected, target.Contains(c));
        }

        [TestCase('a', false)]
        [TestCase('b', true)]
        [TestCase('c', true)]
        [TestCase('d', true)]
        [TestCase('e', false)]
        [TestCase('1', false)]
        public void SwappedRangeContainsTest(char c, bool expected)
        {
            var target = new CharacterClass('d', 'b');

            Assert.AreEqual(expected, target.Contains(c));
        }

        [TestCase('a', false)]
        [TestCase('b', true)]
        [TestCase('c', true)]
        [TestCase('d', true)]
        [TestCase('e', true)]
        [TestCase('f', true)]
        [TestCase('g', false)]
        public void UnionRangeContainsTest(char c, bool expected)
        {
            var target = CharacterClass.Union(new CharacterClass('b', 'd'), new CharacterClass('c', 'f'));

            Assert.AreEqual(expected, target.Contains(c));
        }

        [TestCase('a', true)]
        [TestCase('b', false)]
        [TestCase('c', false)]
        [TestCase('d', false)]
        [TestCase('e', true)]
        [TestCase('f', false)]
        [TestCase('g', false)]
        public void IntersectionRangeContainsTest(char c, bool expected)
        {
            var target = CharacterClass.Intersect(new CharacterClass('a', 'f'), 
                CharacterClass.Union(
                        new CharacterClass('a'),
                        new CharacterClass('e'),
                        new CharacterClass('i'),
                        new CharacterClass('o'),
                        new CharacterClass('u')
                    ));

            Assert.AreEqual(expected, target.Contains(c));
        }
        
        [Test]
        public void MultipleIntersectionsRangeContainsTest()
        {
            //[0-9&&[0-6&&[4-9]]] is the same as [4-6]

            var target = CharacterClass.Intersect(new CharacterClass('0', '9'), 
                            CharacterClass.Intersect(new CharacterClass('0', '6'), new CharacterClass('4', '9')));

            var included = "456";
            var excluded = "123789";

            foreach (var character in included)
                Assert.AreEqual(true, target.Contains(character));

            foreach (var character in excluded)
                Assert.AreEqual(false, target.Contains(character));
            
        }

        [TestCase('a', false)]
        [TestCase('b', true)]
        [TestCase('c', false)]
        [TestCase('d', false)]
        [TestCase('e', false)]
        [TestCase('f', false)]
        [TestCase('g', false)]
        public void SubtractionRangeContainsTest(char c, bool expected)
        {
            var target = CharacterClass.Subtract(new CharacterClass('b', 'd'), new CharacterClass('c', 'f'));

            Assert.AreEqual(expected, target.Contains(c));
        }

        [TestCase('a', false)]
        [TestCase('b', false)]
        [TestCase('c', false)]
        [TestCase('d', false)]
        [TestCase('e', false)]
        [TestCase('f', false)]
        [TestCase('g', false)]
        public void SubtractAllRangeContainsTest(char c, bool expected)
        {
            var target = CharacterClass.Subtract(new CharacterClass('b', 'd'), new CharacterClass('a', 'h'));

            Assert.AreEqual(expected, target.Contains(c));
        }

        [TestCase('a', false)]
        [TestCase('b', false)]
        [TestCase('c', false)]
        [TestCase('d', false)]
        [TestCase('e', false)]
        [TestCase('f', false)]
        [TestCase('g', false)]
        public void SubtractAllRangesContainsTest(char c, bool expected)
        {
            var target = CharacterClass.Subtract(new CharacterClass('a'), new CharacterClass('a'));

            Assert.AreEqual(expected, target.Contains(c));
        }

        [TestCase('a', true)]
        [TestCase('b', true)]
        [TestCase('c', false)]
        [TestCase('d', false)]
        [TestCase('e', false)]
        [TestCase('f', true)]
        [TestCase('g', true)]
        public void SubtractFromMiddleOfRangeContainsTest(char c, bool expected)
        {
            var target = CharacterClass.Subtract(new CharacterClass('a', 'g'), new CharacterClass('c', 'e'));

            Assert.AreEqual(expected, target.Contains(c));
        }

        [TestCase('a', false)]
        [TestCase('b', false)]
        [TestCase('c', false)]
        [TestCase('d', true)]
        [TestCase('e', true)]
        [TestCase('f', true)]
        [TestCase('g', true)]
        public void SubtractFromStartOfRangeContainsTest(char c, bool expected)
        {
            var target = CharacterClass.Subtract(new CharacterClass('a', 'g'), new CharacterClass('a', 'c'));

            Assert.AreEqual(expected, target.Contains(c));
        }

        [TestCase('a', true)]
        [TestCase('b', false)]
        [TestCase('c', false)]
        [TestCase('d', false)]
        [TestCase('e', true)]
        [TestCase('f', true)]
        [TestCase('g', true)]
        public void NegateRangeContainsTest(char c, bool expected)
        {
            var target = CharacterClass.Negate(new CharacterClass('b', 'd'));

            Assert.AreEqual(expected, target.Contains(c));
        }

        [Test]
        public void IntersectANegatedSetTest()
        {
            // gives just the consonants
            //[a-z&&[^aeiou]]

            var target = CharacterClass.Intersect(
                new CharacterClass('a', 'z'),
                CharacterClass.Negate(
                    CharacterClass.Union(
                        new CharacterClass('a'),
                        new CharacterClass('e'),
                        new CharacterClass('i'),
                        new CharacterClass('o'),
                        new CharacterClass('u')
                    )
                )
            );

            var vowels = "aeiou";
            var consonants = "bcdfghjklmnpqrstvwxyz";

            foreach (var character in vowels)
                Assert.AreEqual(false, target.Contains(character));

            foreach (var character in consonants)
                Assert.AreEqual(true, target.Contains(character));

        }

        [Test]
        public void WideUnicodeContainsTest()
        {
            var target = new CharacterClass('\xFF', '\xFFFE');

            Assert.AreEqual(false, target.Contains((char)254));
            Assert.AreEqual(true, target.Contains((char)255));

            Assert.AreEqual(true, target.Contains((char)65534));
            Assert.AreEqual(false, target.Contains((char)65535));
        }

        [Test]
        public void CharacterClassMergingAdjacentTest()
        {
            var target = CharacterClass.Union(new CharacterClass('a'), new CharacterClass('b'));

            Assert.AreEqual(1, target.includedRanges.Count);
        }

        [Test]
        public void CharacterClassMergingAdjacentTest2()
        {
            var target = CharacterClass.Union(new CharacterClass('a'), new CharacterClass('b'), new CharacterClass('c'), new CharacterClass('d'), new CharacterClass('e'));

            Assert.AreEqual(1, target.includedRanges.Count);
            Assert.AreEqual(0, target.excludedRanges.Count);
        }

        [Test]
        public void CharacterClassMergingAdjacentNegatedTest()
        {
            var target = CharacterClass.Negate(CharacterClass.Union(new CharacterClass('a'), new CharacterClass('b'), new CharacterClass('c'), new CharacterClass('d'), new CharacterClass('e')));

            Assert.AreEqual(1, target.excludedRanges.Count);
            Assert.AreEqual(0, target.includedRanges.Count);
        }

        [Test]
        public void CharacterClassMergingAdjacentNegatedTest2()
        {
            var target = CharacterClass.Union(CharacterClass.Negate(new CharacterClass('a')), CharacterClass.Negate(new CharacterClass('b')), 
                CharacterClass.Negate(new CharacterClass('c')), CharacterClass.Negate(new CharacterClass('d')), CharacterClass.Negate(new CharacterClass('e')));

            Assert.AreEqual(1, target.excludedRanges.Count);
            Assert.AreEqual(0, target.includedRanges.Count);
        }

        [Test]
        public void CharacterClassMergingIdenticalRangesTest()
        {
            var target = CharacterClass.Union(
                new CharacterClass('a'),new CharacterClass('a'),new CharacterClass('a'),new CharacterClass('a')
            );

            Assert.AreEqual(0, target.excludedRanges.Count);
            Assert.AreEqual(1, target.includedRanges.Count);
        }

        [Test]
        public void CharacterClassMergingIdenticalSetsOfRangesTest()
        {
            var target = CharacterClass.Union(
                new CharacterClass('a'), new CharacterClass('b'), new CharacterClass('d'), new CharacterClass('e')
            );

            Assert.AreEqual(0, target.excludedRanges.Count);
            Assert.AreEqual(2, target.includedRanges.Count);
        }

        [Test]
        public void CharacterClassToStringTest1()
        {
            var target = CharacterClass.Union(
                new CharacterClass('a'), new CharacterClass('b'), new CharacterClass('d'), new CharacterClass('e')
            );

            Assert.AreEqual("[a-bd-e]", target.ToString());
        }

        [Test]
        public void CharacterClassToStringTest()
        {
            // gives just the consonants
            //[a-z&&[^aeiuo]]

            var target = CharacterClass.Intersect(
                new CharacterClass('a', 'z'),
                CharacterClass.Negate(
                    CharacterClass.Union(
                        new CharacterClass('a'),
                        new CharacterClass('e'),
                        new CharacterClass('i'),
                        new CharacterClass('o'),
                        new CharacterClass('u')
                    )
                )
            );

            Assert.AreEqual("[a-z&&[^aeiou]]", target.ToString());
        }
    }
}
