namespace ParameterNamesShouldMatch

type Severity =
  | Info
  | Error
  | Warning

type IVisualizerWindow =
  interface
  abstract member Title : string with get, set
  abstract member ShowMessage : Severity -> string -> unit
  end

type Handler() =
  class
    [<DefaultValue(true)>]
    val mutable mainWindow : string

    member private self.ShowMessage severity message =
      printfn "Message(%A): %s" severity message

    interface IVisualizerWindow with
      member self.Title
       with get() = self.mainWindow
       and set(value) = self.mainWindow <- value
      member self.ShowMessage severity message =
        self.ShowMessage severity message
  end