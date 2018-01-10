include ../common.make

rules_sources =  \
	CentralizePInvokesIntoNativeMethodsTypeRule.cs \
	DelegatesPassedToNativeCodeMustIncludeExceptionHandlingRule.cs \
	DoNotAssumeIntPtrSizeRuleRule.cs \
	GetLastErrorMustBeCalledRightAfterPInvokeRule.cs \
	MarshalBooleansInPInvokeDeclarationsRule.cs \
	MarshalStringsInPInvokeDeclarationsRule.cs \
	PInvokeShouldNotBeVisibleRule.cs \
	UseManagedAlternativesToPInvokeRule.cs

EXTRA_TESTS_OPTIONS += -unsafe

tests_sources = \
	CentralizePInvokesIntoNativeMethodsTypeTest.cs \
	DoNotAssumeIntPtrSizeRuleTest.cs \
	GetLastErrorMustBeCalledRightAfterPInvokeTest.cs \
	MarshalBooleansInPInvokeDeclarationsTest.cs \
	MarshalStringsInPInvokeDeclarationsTest.cs \
	PInvokeShouldNotBeVisibleTest.cs \
	UseManagedAlternativesToPInvokeTest.cs	\
	DelegatesPassedToNativeCodeMustIncludeExceptionHandlingTest.cs
