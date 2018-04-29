//
// Shared test code / declarations for rocks
//
// Authors:
//	Sebastien Pouliot  <sebastien@ximian.com>
//	Andreas Noever <andreas.noever@gmail.com>
//
// (C) 2008 Andreas Noever
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Test.Framework.Rocks {

	internal static class TestTypeNames {
		internal static readonly string Namespace = typeof (PublicType).Namespace;
		internal static readonly string PublicType = GetType<PublicType>();
		internal static readonly string InternalType = GetType<InternalType>();
		internal static readonly string NestedPublicType = GetType<PublicType.NestedPublicType>();
		internal static readonly string NestedNestedPublicType = GetType<PublicType.NestedPublicType.NestedNestedPublicType>();
		internal static readonly string NestedProtectedType = (PublicType + "/NestedProtectedType");
		internal static readonly string NestedPrivateType = (PublicType + "/NestedPrivateType");
		internal static readonly string NestedInternalType = GetType<PublicType.NestedInternalType>();
		internal static readonly string InternalNestedPublicType = GetType<InternalType.NestedPublicType>();

		internal static readonly string NoEnumerator = GetType<NoEnumerator<int>>();
		internal static readonly string NoStringEnumerator = GetType<NoStringEnumerator>();
		internal static readonly string TwoGenericStringImplementations = GetType<TwoGenericStringImplementations>();
		internal static readonly string TwoGenericStringIntImplementations = GetType<TwoGenericStringIntImplementations>();

		private static string GetType<T>()
		{
			string name = typeof (T).FullName.Replace ('+', '/');
			int pos = name.IndexOf ('[');
			if (pos > 0)
				name = name.Remove (pos);
			return name;
		}
	}

	public abstract class PublicType {
		public int PublicField;
		protected int ProtectedField;
		internal int InternalField;
		private int PrivateField;

		public abstract void PublicMethod ();
		protected abstract void ProtectedMethod ();
		internal abstract void InternalMethod ();
		private void PrivateMethod () { }

		public abstract class NestedPublicType {
			public int PublicField;
			protected int ProtectedField;
			private int PrivateField;

			public abstract void PublicMethod ();
			protected abstract void ProtectedMethod ();
			private void PrivateMethod () { }

			public abstract class NestedNestedPublicType {
			}
		}

		protected abstract class NestedProtectedType {
			public int PublicField;

			public abstract void PublicMethod ();
		}

		private abstract class NestedPrivateType {
			public int PublicField;

			public abstract void PublicMethod ();
		}

		internal abstract class NestedInternalType {
		}
	}

	internal abstract class InternalType {
		public int PublicField;

		public abstract void PublicMethod ();

		public abstract class NestedPublicType {
			public int PublicField;
		}
	}

	internal class MyList : List<string> {
		IEnumerator<string> GetNoEnumerator ()
		{
			return new NoStringEnumerator ();
		}
	}

	internal class NoEnumerator<T> : IEnumerator<T> {
		virtual public T Current
		{
			get {
				return (default (T));
			}
		}

		object IEnumerator.Current
		{
			get {
				return (default (T));
			}
		}

		public void Dispose ()
		{
			// nothing
		}

		public bool MoveNext ()
		{
			return (false);
		}

		public void Reset ()
		{
			// nothing
		}
	}

	internal sealed class NoStringEnumerator : NoEnumerator<string>
	{
		public override string Current
		{
			get
			{
				return string.Empty;
			}
		}
	}

	internal class TwoGenericImplementationsBase<T> : IEnumerable<T>, IEnumerator<T>
	{
		virtual public T Current
		{
			get
			{
				return (default (T));
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return (default (T));
			}
		}

		public void Dispose ()
		{
			// nothing
		}

		public IEnumerator<T> GetEnumerator ()
		{
			return this;
		}

		public bool MoveNext ()
		{
			return (false);
		}

		public void Reset ()
		{
			// nothing
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return this;
		}
	}

	internal sealed class TwoGenericStringImplementations : TwoGenericImplementationsBase<string>
	{
		public override string Current
		{
			get
			{
				return string.Empty;
			}
		}
	}

	internal sealed class TwoGenericStringIntImplementations : IEnumerable<string>, IEnumerator<int>
	{
		public int Current
		{
			get
			{
				return 0;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return (0);
			}
		}

		public void Dispose ()
		{
			// nothing
		}

		public IEnumerator<string> GetEnumerator ()
		{
			return null;
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return this;
		}

		public bool MoveNext ()
		{
			return (false);
		}

		public void Reset ()
		{
			// nothing
		}
	}
}
