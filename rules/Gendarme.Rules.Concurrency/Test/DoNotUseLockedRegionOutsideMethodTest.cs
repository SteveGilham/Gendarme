//
// Unit tests for DoNotUseLockedRegionOutsideMethodRule
//
// Authors:
//	Andres G. Aragoneses <aaragoneses@novell.com>
//
// Copyright (C) 2008 Andres G. Aragoneses
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Threading;
using Gendarme.Rules.Concurrency;

using NUnit.Framework;
using Test.Rules.Definitions;
using Test.Rules.Fixtures;

namespace Test.Rules.Concurrency {

	[TestFixture]
	public class DoNotUseLockedRegionOutsideMethodTest : MethodRuleTestFixture<DoNotUseLockedRegionOutsideMethodRule> {

		[Test]
		public void DoesNotApply ()
		{
			// no IL for p/invokes
			AssertRuleDoesNotApply (SimpleMethods.ExternalMethod);
			// no calls[virt]
			AssertRuleDoesNotApply (SimpleMethods.EmptyMethod);
		}

		public class Monitors {

			private Monitors ()
			{
			}

			public static void WithLockStatement () {
				lock ( new object () )
				{
					// do something...
					WithLockStatement ();
				}
			}

			public static void WithoutConcurrency () {
				// do something...
				WithoutConcurrency ();
			}

			public static void WithoutThreadExit () {
				// do something...
				WithoutThreadExit ();
				
				System.Threading.Monitor.Enter ( new object () );
				
				// do something...
				WithoutThreadExit ();
			}

			public static void TwoEnterOneExit ()
			{
				lock (new object ()) {
					System.Threading.Monitor.Enter (new object ());
				}
			}
		}

		public sealed class LockClass
		{
			readonly object @lock = new object();
			readonly object mutex = new object();

			public void Lock()
			{
				Monitor.Enter(@lock);
			}

			public void TryLock()
			{
				Monitor.TryEnter(mutex);
			}

			public void MultiLock()
			{
				Monitor.Enter(@lock);
				Monitor.TryEnter(mutex);
			}

			public void Exit()
			{
				Monitor.Exit(@lock);
			}

			public void ExitLock()
			{
				Monitor.Exit(@lock);
				Monitor.TryEnter(mutex);
			}

			public void LockExitDifferent()
			{
				Monitor.TryEnter(mutex);
				Monitor.Exit(@lock);
			}

			public void LockExitOK1()
			{
				Monitor.TryEnter(mutex);
				Monitor.Exit(mutex);
			}

			public void LockExitOK2()
			{
				Monitor.TryEnter(@lock);
				Monitor.Exit(@lock);
			}

			public void MultipleLockExit()
			{
				Monitor.TryEnter(mutex);
				Monitor.Exit(mutex);
				Monitor.TryEnter(@lock);
				Monitor.Exit(@lock);
			}

			public void LockExitExitLock()
			{
				Monitor.TryEnter(mutex);
				Monitor.Exit(mutex);
				Monitor.Exit(@lock);
				Monitor.TryEnter(@lock);
			}

			public void NestedLock()
			{
				lock (@lock)
				{
					lock (mutex)
						Console.WriteLine();
				}
			}
		}
	
		[Test]
		public void Check ()
		{
			AssertRuleSuccess<Monitors> ("WithLockStatement");
			AssertRuleSuccess<Monitors> ("WithoutConcurrency");
			AssertRuleFailure<Monitors> ("WithoutThreadExit");
			AssertRuleFailure<Monitors> ("TwoEnterOneExit");
		}

		[Test]
		public void UseLockCorrectly ()
		{
			AssertRuleSuccess<LockClass> ("LockExitOK1");
			AssertRuleSuccess<LockClass> ("LockExitOK2");
		}

		[Test]
		public void IncorrectUseOfLock ()
		{
			AssertRuleFailure<LockClass> ("Lock");
			AssertRuleFailure<LockClass> ("TryLock");
			AssertRuleFailure<LockClass> ("MultiLock");
			AssertRuleFailure<LockClass> ("Exit");
			AssertRuleFailure<LockClass> ("ExitLock");
			AssertRuleFailure<LockClass> ("MultipleLockExit");
			AssertRuleFailure<LockClass> ("LockExit" + "ExitLock"); // spellcheck bypass
			AssertRuleFailure<LockClass> ("NestedLock");
		}

		[Test, Ignore ("Not yet supported to test lock on different objects.")]
		public void LockExitDifferent ()
		{
			AssertRuleFailure<LockClass> ("LockExitDifferent");
		}
	}
}
