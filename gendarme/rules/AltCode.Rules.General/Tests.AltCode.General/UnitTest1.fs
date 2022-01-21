module Tests.AltCode.General

open NUnit.Framework
open AltCode.Rules.General

[<SetUp>]
let Setup () =
    ()

[<Test>]
let Test1 () =
    let v = Say.hello "General"
    Assert.That(v, Is.EqualTo "Ave General")