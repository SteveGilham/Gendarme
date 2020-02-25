namespace BadRecursiveInvocation

type Handler() =
  class
    [<DefaultValue(true); System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Gendarme.Rules.Design",
                                                                                   "AvoidVisibleFieldsRule")>]
    val mutable activeRow : int
  end