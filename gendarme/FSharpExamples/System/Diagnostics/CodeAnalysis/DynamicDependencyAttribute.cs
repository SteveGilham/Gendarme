using System.Runtime.CompilerServices;

namespace System.Diagnostics.CodeAnalysis
{
	internal class DynamicDependencyAttribute : Attribute
	{
		[CompilerGenerated]
		[DebuggerNonUserCode]
		private DynamicallyAccessedMemberTypes MemberType_0040;

		[CompilerGenerated]
		[DebuggerNonUserCode]
		private Type Type_0040;

		[CompilerGenerated]
		[DebuggerNonUserCode]
		public DynamicallyAccessedMemberTypes MemberType
		{
			[CompilerGenerated]
			[DebuggerNonUserCode]
			get
			{
				return MemberType_0040;
			}
		}

		[CompilerGenerated]
		[DebuggerNonUserCode]
		public Type Type
		{
			[CompilerGenerated]
			[DebuggerNonUserCode]
			get
			{
				return Type_0040;
			}
		}

		[CompilerGenerated]
		[DebuggerNonUserCode]
		public DynamicDependencyAttribute(DynamicallyAccessedMemberTypes MemberType, Type Type)
		{
			MemberType_0040 = MemberType;
			Type_0040 = Type;
		}
	}
}
