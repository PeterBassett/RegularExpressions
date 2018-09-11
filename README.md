# Regular Expressions

A regular expression parser and execution engine.

Functionality implemented so far.

* Primitives	"a"
* \d \w \s character class short names
* \\\\ backslash escape sequence
* Unicode escape sequences \u{41}
* Primitive sequences	"abc"
* Alternation	"a|b|c"
* Kleene star repetition	"a*"
* Plus repetition		"a+"
* Question repetition "a?"
* Start Of Input Assertion "a$" "a|$b|c"
* End Of Input Assertion "a$" "a|b$|c"
* Character Classes "[a-z]"
* Negated Character Classes "[^aeiuo]"
	- Negated classes are still unicode compliant with low memory usage. No bitmaps involved.
* Subtraction Of Character Classes "[a-z-[aeiou]]"
* Intersection Of Character Classes "[a-z&&[^aeiou]]"
* Character Class unicode support