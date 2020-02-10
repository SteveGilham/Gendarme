# 2020.2.10.xxx-pre-release
* Build the code as of commit b45416d516b18e77c1e73be31649bb298b1c6652 (24-Oct-2016) against Cecil 0.11.1 and NUnit 2.7.1 using VS2013 (pre-Roslyn compiler)
* Unit test failures for rules
  * Gendarme.Rules.Performance\Test\AvoidUnnecessaryOverridesTest
  * Gendarme.Rules.Design\Test\ProvideTryParseAlternativeTest
  * Gendarme.Rules.Serialization\Test\CallBaseMethodsOnISerializableTypesTest
  * Gendarme.Rules.Correctness\Test\ProvideCorrectRegexPatternTest
  * Gendarme.Rules.Concurrency\Test\ProtectCallToEventDelegatesTest
  * Gendarme.Rules.Exceptions\Test\UseObjectDisposedExceptionTest
* Unit test failure for Gendarme.Framework.Helpers\StackEntryAnalysisTest
* Do not rely on the failed rules.
* Should work against .netstandard/.netcore binaries (you will need to `dotnet publish` to put any relevant dependencies in the same folder for Gendarme to inspect for inheritance -- especially so for F# code)
