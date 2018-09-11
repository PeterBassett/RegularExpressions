using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegularExpressions.Visitor;

namespace RegularExpressions.Ast
{
    public class CharacterClass : AstNode
    {
        internal class Range
        {
            public char start;
            public char end;

            public Range(int start, int end) : this((char)start, (char)end) 
            {
            }

            public Range(char start, char end)
            {
                if (start > end)
                {
                    var t = start;
                    start = end;
                    end = t;
                }

                this.start = start;
                this.end = end;
            }

            internal bool Contains(char p)
            {
                return start <= p && end >= p;
            }

            internal bool Overlaps(Range other)
            {
                return this.start <= other.end && other.start <= this.end;
            }

            internal bool Touching(Range other)
            {
                return this.end + 1 == other.start ||
                        this.start - 1 == other.end;
            }

            internal bool IsSingle { get { return this.start == this.end; } }

            internal Range Merge(Range other)
            {
                return new Range((char)Math.Min(this.start, other.start), (char)Math.Max(this.end, other.end));
            }

            internal Tuple<Range, Range> Subtract(Range other)
            {
                // this is horrible, but it passes the tests
                // do we need two output ranges?
                if (this.start < other.start &&
                   this.end > other.end)
                {
                    var a = new Range(this.start, other.start - 1);
                    var b = new Range(other.end + 1, this.end);
                    return new Tuple<Range, Range>(a, b);
                }

                if (this.start < other.start)
                {
                    return new Tuple<Range, Range>(
                        new Range(this.start, other.start - 1), 
                        null);
                }

                if (this.start > other.start)
                {
                    return new Tuple<Range, Range>(
                        new Range(other.end + 1, this.end),
                        null);
                }

                if (this.start == other.start && this.end == other.end)
                    return new Tuple<Range, Range>(null, null);

                if (this.start == other.start)
                {

                    return new Tuple<Range, Range>(
                        new Range(other.start + 1, this.end),
                        null);
                }               

                throw new ApplicationException();
            }

            internal Range Intersection(Range other)
            {
                return new Range((char)Math.Max(this.start, other.start), (char)Math.Min(this.end, other.end));
            }

            #region Equality
            public override bool Equals(object obj)
            {
                // If run-time types are not exactly the same, return false.
                if (this.GetType() != obj.GetType())
                {
                    return false;
                }

                return this.Equals(obj as Range);
            }

            public bool Equals(Range r)
            {
                // If parameter is null, return false.
                if (Object.ReferenceEquals(r, null))
                    return false;

                // Optimization for a common success case.
                if (Object.ReferenceEquals(this, r))
                    return true;
                
                return this.start == r.start && this.end == r.end;
            }
            #endregion
        }

        internal class RangeComparer : IComparer<Range>
        {
            public int Compare(Range a, Range b)
            {
                if (a.start >= b.start)
                {
                    if (a.start <= b.start)
                        return 0;
                    
                    return 1;
                }

                return -1;
            }
        }

        internal List<Range> includedRanges;
        internal List<Range> excludedRanges;
        internal bool negated;
        
        public CharacterClass()
        {
            includedRanges = new List<Range>();
            excludedRanges = new List<Range>();
            negated = false;
        }

        public CharacterClass(char character) : this()
	    {
            var range = new Range(character, character);
            includedRanges.Add(range);
        }

        public CharacterClass(char start, char end) : this()
	    {
            var range = new Range(start, end);
            includedRanges.Add(range);
	    }        

        internal CharacterClass(List<Range> include, List<Range> exclude)
            : this()
        {
            this.includedRanges = include;
            this.excludedRanges = exclude;

            ConsolidateRanges(this.includedRanges);
            ConsolidateRanges(this.excludedRanges);
        }

        public CharacterClass(CharacterClass other)
        {
            this.includedRanges = new List<Range>( other.includedRanges );
            this.excludedRanges = new List<Range>(other.excludedRanges);
            this.negated = other.negated;
        }

        public bool IsNegated
        {
            get
            {
                return this.negated;
            }
            set
            {
                var t = this.includedRanges;
                this.includedRanges = this.excludedRanges;
                this.excludedRanges = t;
                negated = value;
            }
        }

        public override void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        internal bool Contains(char character)
        {
            var included = Contains(this.includedRanges, character);
            var excluded = Contains(this.excludedRanges, character);

            if (IsNegated)
            {
                if (excluded)
                    return false;

                return included || !this.includedRanges.Any();
            }

            return included && !excluded;
        }

        internal bool Contains(IEnumerable<Range> ranges, char character)
        {
            foreach (var range in ranges)
            {
                if (range.Contains(character))
                    return true;
            }

            return false;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("[");

            foreach (var item in this.includedRanges)
	        {
		        builder.Append(item.start);

                if(!item.IsSingle)
                    builder.Append("-" + item.end);
	        }

            if (this.excludedRanges.Any())
            {
                builder.Append("&&[^");
                
                foreach (var item in this.excludedRanges)
                {
                    builder.Append(item.start);

                    if (!item.IsSingle)
                        builder.Append("-" + item.end);
                }

                builder.Append("]");
            }

            builder.Append("]");
            return builder.ToString();
        }

        internal static void ConsolidateRanges(List<Range> ranges)
        {
            MergeOverlappingRanges(ranges);
            MergeAdjacentRanges(ranges);
        }

        internal static void MergeOverlappingRanges(List<Range> ranges)
        {            
            IterateRangeCombinations(ranges, (List<Range> r, Range a, Range b) =>
            {
                if (a.Overlaps(b))
                {
                    var merged = a.Merge(b);

                    r.Remove(a);
                    r.Remove(b);
                    r.Add(merged);

                    return true;
                }

                return false;
            });
        }

        internal static void MergeAdjacentRanges(List<Range> ranges)
        {
            IterateRangeCombinations(ranges, (List<Range> r, Range a, Range b) =>
            {
                if (a.Touching(b))
                {
                    var merged = a.Merge(b);

                    r.Remove(a);
                    r.Remove(b);
                    r.Add(merged);

                    return true;
                }

                return false;
            });
        }

        internal static void IterateRangeCombinations(List<Range> ranges, Func<List<Range>, Range, Range, bool> process)
        {
            ranges.Sort(0, ranges.Count, new RangeComparer());

            bool workDone = false;
            do
            {
                workDone = false;

                for (int a = 0; a < ranges.Count; a++)
                {
                    for (int b = 1; b < ranges.Count; b++)
                    {
                        if (a == b)
                            continue;

                        var rangeA = ranges[a];
                        var rangeB = ranges[b];

                        workDone |= process(ranges, rangeA, rangeB);
                    }
                }
            }
            while (workDone);
        }


        public static CharacterClass Intersect(CharacterClass a, CharacterClass b)
	    {
            List<Range> include, exclude;

            if (a.IsNegated == b.IsNegated)
            {
                include = Intersect(a.includedRanges, b.includedRanges);
                exclude = Intersect(a.excludedRanges, b.excludedRanges);
            }
            else
            {
                include = a.includedRanges;
                exclude = b.excludedRanges;
            }

            return new CharacterClass(include, exclude);
	    }

        internal static List<Range> Intersect(List<Range> a, List<Range> b)
        {
            var output = new List<Range>();

            ConsolidateRanges(a);
            ConsolidateRanges(b);

            var aranges = new List<Range>(a);

            bool actionPerformed = false;
            do
            {
                actionPerformed = false;

                var newRanges = new List<Range>();
                for (int ai = 0; ai < aranges.Count; ai++)
                {
                    for (int bi = 0; bi < b.Count; bi++)
                    {
                        var rangeA = aranges[ai];
                        var rangeB = b[bi];

                        if (rangeA.Overlaps(rangeB))
                        {
                            var intersected = rangeA.Intersection(rangeB);

                            newRanges.Add(intersected);

                            actionPerformed = true;
                        }
                    }
                }

                if (aranges.Zip(newRanges, (r1, r2) => new { A = r1, B = r2 }).All(item => item.A.Equals(item.B)))
                    actionPerformed = false;

                aranges = newRanges;
            }
            while (actionPerformed);

            ConsolidateRanges(aranges);

            return aranges;
        }

        public static CharacterClass Union(CharacterClass a, CharacterClass b)
        {
            List<Range> include, exclude;

            bool isNegated = false;
            if (a.IsNegated == b.IsNegated)
            {
                include = Union(a.includedRanges, b.includedRanges);
                exclude = Union(a.excludedRanges, b.excludedRanges);
            }
            else
            {
                isNegated = true;
                include = a.includedRanges;
                exclude = b.excludedRanges;
            }

            var charMap = new CharacterClass(include, exclude);
            if (isNegated)
            {
                charMap = new CharacterClass(exclude, include);
                charMap.IsNegated = true;
            }

            return charMap;
        }

        internal static List<Range> Union(List<Range> a, List<Range> b)
        {
            ConsolidateRanges(a);
            ConsolidateRanges(b);

            var unioned = a.Concat(b).ToList();
            ConsolidateRanges(unioned);

            return unioned;
        }

        public static CharacterClass Union(params CharacterClass [] others)
        {
            var output = new CharacterClass();

            foreach (var item in others)
            {
                output = Union(output, item);
            }

            ConsolidateRanges(output.includedRanges);

            return output;
        }

        public static CharacterClass Subtract(CharacterClass a, CharacterClass b)
        {
            List<Range> include, exclude;

            if (a.IsNegated == b.IsNegated)
            {
                include = Subtract(a.includedRanges, b.includedRanges);
                exclude = Subtract(a.excludedRanges, b.excludedRanges);
            }
            else
            {
                include = a.includedRanges;
                exclude = b.excludedRanges;
            }

            return new CharacterClass(include, exclude);
        }

        internal static List<Range> Subtract(List<Range> a, List<Range> b)
        {
            var output = new List<Range>();

            ConsolidateRanges(a);
            ConsolidateRanges(b);

            var aranges = new List<Range>(a);

            bool actionPerformed = false;
            do
            {
                actionPerformed = false;

                for (int ai = 0; ai < aranges.Count; ai++)
                {
                    for (int bi = 0; bi < b.Count; bi++)
                    {
                        var rangeA = aranges[ai];
                        var rangeB = b[bi];

                        if (rangeA.Overlaps(rangeB))
                        {
                            var intersected = rangeA.Subtract(rangeB);

                            aranges.Remove(rangeA);

                            if (intersected.Item1 != null)
                                aranges.Add(intersected.Item1);

                            if(intersected.Item2 != null)
                                aranges.Add(intersected.Item2);

                            actionPerformed = true;
                        }
                    }
                }
            }
            while (actionPerformed);

            ConsolidateRanges(aranges);

            return aranges;
        }

        public static CharacterClass Negate(CharacterClass other)
        {
            var output = new CharacterClass(other);
            output.IsNegated = !other.IsNegated;
            return output;
        }
    }
}