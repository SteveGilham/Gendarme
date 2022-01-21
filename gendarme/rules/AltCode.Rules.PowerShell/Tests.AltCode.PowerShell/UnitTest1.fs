namespace Tests.AltCode.PowerShell

open NUnit.Framework
open AltCode.Rules.PowerShell

module Tests =
  [<SetUp>]
  let Setup () =
      ()

  [<Test>]
  let Test1 () =
      let v = Say.hello "PowerShell"
      Assert.That(v, Is.EqualTo "Hello PowerShell")