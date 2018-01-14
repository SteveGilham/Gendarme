To do list
----------

* Fix rules:

  - AvoidCodeDuplicatedInSameClassRule, 

    Wrong assumptions, too many false positives,

    Fix the locator, to not search in generated _(partial)_ code, e. g. when using Linq or `dynamic` _(type)_ in function, and to report only when the similarity can really be extracted/merged _(not when the functions look similar but have different values in nested calls, or they call a different function with same checks)_. Similarity can be caused even by the check of input values _(not null, out of range e.t.c.)_ in public functions.

  - Implement alternative compare method; overloaded operators are not supported in VB *rule*

    _need to find the sample code that caused this message and make a minimal implementation with this warning_

  - AvoidLargeClassesRule

    Wrong assumptions, too many false positives; rule is fired on two members with the same *prefix*, e. g. GUI form with label `userNameLabel` and text box `userNameTextBox`, that simply can not be extracted to two different classes and to create custom user control for all these *little things* is not the best idea either.

  - Mono compatibility rule

    Remove downloading; the source of current state in no longer updated

* Test Linux/Mono support of changes

* Update from obsolete *Cecil* functions

* Update to latest *Cecil*

* Add rules and updates from other forks

* Reformat code to standard C# standards (?)

* Remove dead (commented) code

* Fix grammar in comments, examples and texts *(way too many typos and other mistakes)*

* Remove/fix code analyzer warnings _(way too many, over 4000 warnings and over 1000 other problems in **Cecil** and **Gendarme**)_

* Fix code samples for rules:

  * code formatting,
  * *(sometimes)* missing lines *(e. g. `ImplementIComparableCorrectlyRule` in **Design** package)*

* Add comments *(documentation)* for all public types

