using System.Runtime.CompilerServices;

namespace System.Diagnostics.CodeAnalysis
{
	[Serializable]
	[Flags]
	[CompilerGenerated]
	[DebuggerNonUserCode]
	internal enum DynamicallyAccessedMemberTypes
	{
		All = -1,
		None = 0x0,
		PublicParameterlessConstructor = 0x1,
		PublicConstructors = 0x3,
		NonPublicConstructors = 0x4,
		PublicMethods = 0x8,
		NonPublicMethods = 0x10,
		PublicFields = 0x20,
		NonPublicFields = 0x40,
		PublicNestedTypes = 0x80,
		NonPublicNestedTypes = 0x100,
		PublicProperties = 0x200,
		NonPublicProperties = 0x400,
		PublicEvents = 0x800,
		NonPublicEvents = 0x1000,
		Interfaces = 0x2000
	}
}
