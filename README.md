# Regular Expressions

A regular expression parser and execution engine.

Functionality implemented so far.

* Primitives	"a"
* Sequences	"abc"
* Alternation	"a|b|c"
* Kleene star repetition	"a*"
* Plus repetition		"a+"
* Question repetition "a?"
* Start Of Input Assertion "a$" "a|$b|c"
* End Of Input Assertion "a$" "a|b$|c"
* Character Classes "[a-z]"
* Negated Character Classes "[^aeiuo]"
	- Negated classes are still unicode compliant with low memory usage. Not bitmaps involved.
* Subtraction Of Character Classes "[a-z-[aeiuo]]"
* Intersection Of Character Classes "[a-z&&[^aeiuo]]"
* Character Class unicode support