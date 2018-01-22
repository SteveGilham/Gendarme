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

namespace Test.Framework.Rocks {

	internal static class TestTypeNames {
		internal static readonly string Namespace = typeof (PublicType).Namespace;
		internal static readonly string PublicType = typeof (PublicType).FullName;
		internal static readonly string InternalType = typeof (InternalType).FullName;
		internal static readonly string NestedPublicType = typeof (PublicType.NestedPublicType).FullName.Replace ('+', '/');
		internal static readonly string NestedNestedPublicType =
			typeof (PublicType.NestedPublicType.NestedNestedPublicType).FullName.Replace ('+', '/');
		internal static readonly string NestedProtectedType = (typeof (PublicType).FullName + "/NestedProtectedType");
		internal static readonly string NestedPrivateType = (typeof (PublicType).FullName + "/NestedPrivateType");
		internal static readonly string NestedInternalType = typeof (PublicType.NestedInternalType).FullName.Replace ('+', '/');
		internal static readonly string InternalNestedPublicType = typeof (InternalType.NestedPublicType).FullName.Replace ('+', '/');
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
}
