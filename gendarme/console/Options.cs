//
// Options.cs
//
// Authors:
//  Jonathan Pryor <jpryor@novell.com>
//
// Copyright (C) 2008 Novell (http://www.novell.com)
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

// Compile With:
//   gmcs -debug+ -r:System.Core Options.cs -o:NDesk.Options.dll
//   gmcs -debug+ -d:LINQ -r:System.Core Options.cs -o:NDesk.Options.dll
//
// The LINQ version just changes the implementation of
// OptionSet.Parse(IEnumerable<string>), and confers no semantic changes.

//
// A Getopt::Long-inspired option parsing library for C#.
//
// NDesk.Options.OptionSet is built upon a key/value table, where the
// key is a option format string and the value is a delegate that is
// invoked when the format string is matched.
//
// Option format strings:
//  Regex-like BNF Grammar:
//    name: .+
//    type: [=:]
//    sep: ( [^{}]+ | '{' .+ '}' )?
//    aliases: ( name type sep ) ( '|' name type sep )*
//
// Each '|'-delimited name is an alias for the associated action.  If the
// format string ends in a '=', it has a required value.  If the format
// string ends in a ':', it has an optional value.  If neither '=' or ':'
// is present, no value is supported.  `=' or `:' need only be defined on one
// alias, but if they are provided on more than one they must be consistent.
//
// Each alias portion may also end with a "key/value separator", which is used
// to split option values if the option accepts > 1 value.  If not specified,
// it defaults to '=' and ':'.  If specified, it can be any character except
// '{' and '}' OR the *string* between '{' and '}'.  If no separator should be
// used (i.e. the separate values should be distinct arguments), then "{}"
// should be used as the separator.
//
// Options are extracted either from the current option by looking for
// the option name followed by an '=' or ':', or is taken from the
// following option IFF:
//  - The current option does not contain a '=' or a ':'
//  - The current option requires a value (i.e. not a Option type of ':')
//
// The `name' used in the option format string does NOT include any leading
// option indicator, such as '-', '--', or '/'.  All three of these are
// permitted/required on any named option.
//
// Option bundling is permitted so long as:
//   - '-' is used to start the option group
//   - all of the bundled options are a single character
//   - at most one of the bundled options accepts a value, and the value
//     provided starts from the next character to the end of the string.
//
// This allows specifying '-a -b -c' as '-abc', and specifying '-D name=value'
// as '-Dname=value'.
//
// Option processing is disabled by specifying "--".  All options after "--"
// are returned by OptionSet.Parse() unchanged and unprocessed.
//
// Unprocessed options are returned from OptionSet.Parse().
//
// Examples:
//  int verbose = 0;
//  OptionSet p = new OptionSet ()
//    .Add ("v", v => ++verbose)
//    .Add ("name=|value=", v => Console.WriteLine (v));
//  p.Parse (new string[]{"-v", "--v", "/v", "-name=A", "/name", "B", "extra"});
//
// The above would parse the argument string array, and would invoke the
// lambda expression three times, setting `verbose' to 3 when complete.
// It would also print out "A" and "B" to standard output.
// The returned array would contain the string "extra".
//
// C# 3.0 collection initializers are supported and encouraged:
//  var p = new OptionSet () {
//    { "h|?|help", v => ShowHelp () },
//  };
//
// System.ComponentModel.TypeConverter is also supported, allowing the use of
// custom data types in the callback type; TypeConverter.ConvertFromString()
// is used to convert the value option to an instance of the specified
// type:
//
//  var p = new OptionSet () {
//    { "foo=", (Foo f) => Console.WriteLine (f.ToString ()) },
//  };
//
// Random other tidbits:
//  - Boolean options (those w/o '=' or ':' in the option format string)
//    are explicitly enabled if they are followed with '+', and explicitly
//    disabled if they are followed with '-':
//      string a = null;
//      var p = new OptionSet () {
//        { "a", s => a = s },
//      };
//      p.Parse (new string[]{"-a"});   // sets v != null
//      p.Parse (new string[]{"-a+"});  // sets v != null
//      p.Parse (new string[]{"-a-"});  // sets v == null
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;

#if LINQ
using System.Linq;
#endif

#if TEST
using NDesk.Options;
#endif

namespace NDesk.Options
{
  public sealed class OptionValueCollection : IList, IList<string>
  {
    private readonly List<string> values = new List<string>();
    private readonly OptionContext c;

    internal OptionValueCollection(OptionContext context)
    {
      this.c = context;
    }

    #region ICollection

    public void CopyTo(Array array, int index)
    { (values as ICollection).CopyTo(array, index); }

    public bool IsSynchronized
    { get { return (values as ICollection).IsSynchronized; } }

    public object SyncRoot
    { get { return (values as ICollection).SyncRoot; } }

    #endregion ICollection

    #region ICollection<T>

    public void Add(string item)
    { values.Add(item); }

    public void Clear()
    { values.Clear(); }

    public bool Contains(string item)
    { return values.Contains(item); }

    public void CopyTo(string[] array, int arrayIndex)
    { values.CopyTo(array, arrayIndex); }

    public bool Remove(string item)
    { return values.Remove(item); }

    public int Count
    { get { return values.Count; } }

    public bool IsReadOnly
    { get { return false; } }

    #endregion ICollection<T>

    #region IEnumerable

    IEnumerator IEnumerable.GetEnumerator()
    {
      return values.GetEnumerator();
    }

    #endregion IEnumerable

    #region IEnumerable<T>

    public IEnumerator<string> GetEnumerator()
    { return values.GetEnumerator(); }

    #endregion IEnumerable<T>

    #region IList

    int IList.Add(object value)
    {
      return (values as IList).Add(value);
    }

    bool IList.Contains(object value)
    {
      return (values as IList).Contains(value);
    }

    int IList.IndexOf(object value)
    {
      return (values as IList).IndexOf(value);
    }

    void IList.Insert(int index, object value)
    {
      (values as IList).Insert(index, value);
    }

    void IList.Remove(object value)
    {
      (values as IList).Remove(value);
    }

    void IList.RemoveAt(int index)
    {
      (values as IList).RemoveAt(index);
    }

    bool IList.IsFixedSize { get { return false; } }
    object IList.this[int index] { get { return this[index]; } set { (values as IList)[index] = value; } }

    #endregion IList

    #region IList<T>

    public int IndexOf(string item)
    { return values.IndexOf(item); }

    public void Insert(int index, string item)
    { values.Insert(index, item); }

    public void RemoveAt(int index)
    { values.RemoveAt(index); }

#pragma warning disable IDE0079 // Remove unnecessary suppression

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly",
      Justification = "OptionContext names a type")]
    private void AssertValid(int index)
    {
      if (c.Option == null)
        throw new InvalidOperationException("OptionContext.Option is null.");
      if (index >= c.Option.MaxValueCount)
        throw new ArgumentOutOfRangeException(nameof(index));
      if (c.Option.OptionValueType == OptionValueType.Required &&
          index >= values.Count)
        throw new OptionException(string.Format(CultureInfo.InvariantCulture,
              c.OptionSet.MessageLocalizer("Missing required value for option '{0}'."), c.OptionName),
            c.OptionName);
    }

    public string this[int index]
    {
      get
      {
        AssertValid(index);
        return index >= values.Count ? null : values[index];
      }
      set
      {
        values[index] = value;
      }
    }

    #endregion IList<T>

    //public List<string> ToList()
    //{
    //  return new List<string>(values);
    //}

    public string[] ToArray()
    {
      return values.ToArray();
    }

    public override string ToString()
    {
      return string.Join(", ", values.ToArray());
    }
  }

  public class OptionContext
  {
    public OptionContext(OptionCollection set)
    {
      this.OptionSet = set;
      this.OptionValues = new OptionValueCollection(this);
    }

    public Option Option { get; set; }

    public string OptionName { get; set; }

    public int OptionIndex { get; set; }

    public OptionCollection OptionSet { get; private set; }

    public OptionValueCollection OptionValues { get; private set; }
  }

  [Serializable]
  public enum OptionValueType
  {
    None,
    Optional,
    Required,
  }

  [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords",
    Justification = "'Option' has been reserved long enough")]
  public abstract class Option
  {
    private readonly string[] names;
    private string[] separators;

    protected Option(string prototype, string description)
      : this(prototype, description, 1)
    {
    }

    protected Option(string prototype, string description, int maxValueCount)
    {
      if (prototype == null)
        throw new ArgumentNullException(nameof(prototype));
      if (prototype.Length == 0)
        throw new ArgumentException("Cannot be the empty string.", nameof(prototype));
      if (maxValueCount < 0)
        throw new ArgumentOutOfRangeException(nameof(maxValueCount));

      this.Prototype = prototype;
      this.names = prototype.Split('|');
      this.Description = description;
      this.MaxValueCount = maxValueCount;
      this.OptionValueType = ParsePrototype();

      if (this.MaxValueCount == 0 && OptionValueType != OptionValueType.None)
        throw new ArgumentException(
            "Cannot provide maxValueCount of 0 for OptionValueType.Required or " +
              "OptionValueType.Optional.",
            nameof(maxValueCount));
      if (this.OptionValueType == OptionValueType.None && maxValueCount > 1)
        throw new ArgumentException(
            string.Format(CultureInfo.InvariantCulture, "Cannot provide maxValueCount of {0} for OptionValueType.None.", maxValueCount),
            nameof(maxValueCount));
      if (Array.IndexOf(names, "<>") >= 0 &&
          ((names.Length == 1 && this.OptionValueType != OptionValueType.None) ||
           (names.Length > 1 && this.MaxValueCount > 1)))
        throw new ArgumentException(
            "The default option handler '<>' cannot require values.",
            nameof(prototype));
    }

    public string Prototype { get; private set; }
    public string Description { get; private set; }

    public OptionValueType OptionValueType { get; private set; }

    public int MaxValueCount { get; private set; }

    public string[] GetNames()
    {
      return (string[])names.Clone();
    }

    public string[] GetValueSeparators()
    {
      if (separators == null)
        return Array.Empty<string>();
      return (string[])separators.Clone();
    }

    [SuppressMessage("Gendarme.Rules.Design.Generic",
                     "AvoidMethodWithUnusedGenericTypeRule",
                     Justification = "Converts to type")]
    [SuppressMessage("Gendarme.Rules.Exceptions",
                 "DoNotSwallowErrorsCatchingNonSpecificExceptionsRule",
                 Justification = "Raises with inner exception rethrow")]
    protected static T Parse<T>(string value, OptionContext context)
    {
      var c = context ?? throw new ArgumentNullException(nameof(context));
      TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
      T t = default;
      try
      {
        if (value != null)
          t = (T)conv.ConvertFromString(value);
      }
      catch (Exception e)
      {
        throw new OptionException(
            string.Format(CultureInfo.InvariantCulture,
              c.OptionSet.MessageLocalizer("Could not convert string `{0}' to type {1} for option `{2}'."),
              value, typeof(T).Name, c.OptionName),
            c.OptionName, e);
      }
      return t;
    }

    internal ReadOnlyCollection<string> Names
    { get { return new ReadOnlyCollection<string>(names ?? Array.Empty<string>()); } }

    internal ReadOnlyCollection<string> ValueSeparators
    { get { return new ReadOnlyCollection<string>(separators ?? Array.Empty<string>()); } }

    private static readonly char[] NameTerminator = new char[] { '=', ':' };

    [SuppressMessage("Gendarme.Rules.Exceptions",
                     "InstantiateArgumentExceptionCorrectlyRule",
                     Justification = "Inherited Exception Abuse")]
    private OptionValueType ParsePrototype()
    {
      char type = '\0';
      List<string> seps = new List<string>();
      for (int i = 0; i < names.Length; ++i)
      {
        string name = names[i];
        if (name.Length == 0)
          throw new ArgumentException("Empty option names are not supported.", "prototype");

        int end = name.IndexOfAny(NameTerminator);
        if (end == -1)
          continue;
        names[i] = name.Substring(0, end);
        if (type == '\0' || type == name[end])
          type = name[end];
        else
          throw new ArgumentException(
              string.Format(CultureInfo.InvariantCulture,
                "Conflicting option types: '{0}' vs. '{1}'.", type, name[end]),
              "prototype");
        AddSeparators(name, end, seps);
      }

      if (type == '\0')
        return OptionValueType.None;

      if (MaxValueCount <= 1 && seps.Count != 0)
        throw new ArgumentException(
            string.Format(CultureInfo.InvariantCulture,
              "Cannot provide key/value separators for Options taking {0} value(s).", MaxValueCount),
            "prototype");
      if (MaxValueCount > 1)
      {
        if (seps.Count == 0)
          this.separators = new string[] { ":", "=" };
        else if (seps.Count == 1 && seps[0].Length == 0)
          this.separators = null;
        else
          this.separators = seps.ToArray();
      }

      return type == '=' ? OptionValueType.Required : OptionValueType.Optional;
    }

    [SuppressMessage("Gendarme.Rules.Exceptions",
                     "InstantiateArgumentExceptionCorrectlyRule",
                     Justification = "User level name")]
    private static void AddSeparators(string name, int end, ICollection<string> seps)
    {
      int start = -1;
      for (int i = end + 1; i < name.Length; ++i)
      {
        switch (name[i])
        {
          case '{':
            if (start != -1)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
              throw new ArgumentException(
                  string.Format(CultureInfo.InvariantCulture, "Ill-formed name/value separator found in \"{0}\".", name),
                  "prototype");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
            start = i + 1;
            break;

          case '}':
            if (start == -1)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
              throw new ArgumentException(
                  string.Format(CultureInfo.InvariantCulture, "Ill-formed name/value separator found in \"{0}\".", name),
                  "prototype");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
            seps.Add(name.Substring(start, i - start));
            start = -1;
            break;

          default:
            if (start == -1)
              seps.Add(name[i].ToString(CultureInfo.InvariantCulture));
            break;
        }
      }
      if (start != -1)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
        throw new ArgumentException(
            string.Format(CultureInfo.InvariantCulture, "Ill-formed name/value separator found in \"{0}\".", name),
            "prototype");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
    }

    public void Invoke(OptionContext context)
    {
      var c = context ?? throw new ArgumentNullException(nameof(context));
      OnParseComplete(context);
      c.OptionName = null;
      c.Option = null;
      c.OptionValues.Clear();
    }

    protected abstract void OnParseComplete(OptionContext context);

    public override string ToString()
    {
      return Prototype;
    }
  }

  [Serializable]
#pragma warning disable IDE0079 // Remove unnecessary suppression
  [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors",
    Justification = "No use case for them")]
  public class OptionException : Exception
  {
    private readonly string option;

    public OptionException()
    {
    }

    public OptionException(string message, string optionName)
      : base(message)
    {
      this.option = optionName;
    }

    public OptionException(string message, string optionName, Exception innerException)
      : base(message, innerException)
    {
      this.option = optionName;
    }

    [SuppressMessage("Gendarme.Rules.Maintainability",
                     "RemoveDependenceOnObsoleteCodeRule",
                     Justification = "Not all builds affected")]
    protected OptionException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.option = info.GetString("OptionName");
    }

    public string OptionName
    {
      get { return this.option; }
    }

    [SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
    [SuppressMessage("Gendarme.Rules.Maintainability",
                     "RemoveDependenceOnObsoleteCodeRule",
                     Justification = "Not all builds affected")]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("OptionName", option);
    }
  }

  public class OptionCollection : KeyedCollection<string, Option>
  {
    public OptionCollection()
      : this(delegate (string f) { return f; })
    {
    }

    public OptionCollection(Converter<string, string> localizer)
    {
      this.MessageLocalizer = localizer;
    }

    public Converter<string, string> MessageLocalizer { get; private set; }

    [SuppressMessage("Gendarme.Rules.Exceptions",
                     "InstantiateArgumentExceptionCorrectlyRule",
                     Justification = "User level name")]
    protected override string GetKeyForItem(Option item)
    {
      if (item == null)
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
        throw new ArgumentNullException("option");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
      var names = item.Names;
      if (names != null && names.Count > 0)
        return names[0];
      // This should never happen, as it's invalid for Option to be
      // constructed w/o any names.
      throw new InvalidOperationException("Option has no names!");
    }

    [Obsolete("Use KeyedCollection.this[string]")]
    protected Option GetOptionForName(string option)
    {
      if (option == null)
        throw new ArgumentNullException(nameof(option));
      try
      {
        return base[option];
      }
      catch (KeyNotFoundException)
      {
        return null;
      }
    }

    protected override void InsertItem(int index, Option item)
    {
      base.InsertItem(index, item);
      AddImpl(item);
    }

    protected override void RemoveItem(int index)
    {
      base.RemoveItem(index);
      Option p = Items[index];
      var names = p.Names;
      // KeyedCollection.RemoveItem() handles the 0th item
      for (int i = 1; i < names.Count; ++i)
      {
        Dictionary.Remove(names[i]);
      }
    }

    protected override void SetItem(int index, Option item)
    {
      base.SetItem(index, item);
      RemoveItem(index);
      AddImpl(item);
    }

    private void AddImpl(Option option)
    {
      if (option == null)
        throw new ArgumentNullException(nameof(option));
      var names = option.Names;
      List<string> added = new List<string>(names.Count);
      try
      {
        // KeyedCollection.InsertItem/SetItem handle the 0th name.
        for (int i = 1; i < names.Count; ++i)
        {
          Dictionary.Add(names[i], option);
          added.Add(names[i]);
        }
      }
      catch (Exception)
      {
        foreach (string name in added)
          Dictionary.Remove(name);
        throw;
      }
    }

    public new OptionCollection Add(Option option)
    {
      base.Add(option);
      return this;
    }

    private sealed class ActionOption : Option
    {
      private readonly Action<OptionValueCollection> optionAction;

      public ActionOption(string prototype, string description, int count, Action<OptionValueCollection> action)
        : base(prototype, description, count)
      {
        this.optionAction = action ?? throw new ArgumentNullException(nameof(action));
      }

      protected override void OnParseComplete(OptionContext context)
      {
        optionAction(context?.OptionValues ?? throw new ArgumentNullException(nameof(context)));
      }
    }

    public OptionCollection Add(string prototype, Action<string> action)
    {
      return Add(prototype, null, action);
    }

    public OptionCollection Add(string prototype, string description, Action<string> action)
    {
      if (action == null)
        throw new ArgumentNullException(nameof(action));
      Option p = new ActionOption(prototype, description, 1,
          delegate (OptionValueCollection v) { action(v[0]); });
      base.Add(p);
      return this;
    }

    public OptionCollection Add(string prototype, Action<string, string> action)
    {
      return Add(prototype, null, action);
    }

    public OptionCollection Add(string prototype, string description, Action<string, string> action)
    {
      if (action == null)
        throw new ArgumentNullException(nameof(action));
      Option p = new ActionOption(prototype, description, 2,
          delegate (OptionValueCollection v) { action(v[0], v[1]); });
      base.Add(p);
      return this;
    }

    private sealed class ActionOption<T> : Option
    {
      private readonly Action<T> optionAction;

      public ActionOption(string prototype, string description, Action<T> action)
        : base(prototype, description, 1)
      {
        this.optionAction = action ?? throw new ArgumentNullException(nameof(action));
      }

      protected override void OnParseComplete(OptionContext context)
      {
        var c = context ?? throw new ArgumentNullException(nameof(context));
        optionAction(Parse<T>(c.OptionValues[0], c));
      }
    }

    private sealed class ActionOption<TKey, TValue> : Option
    {
      private readonly Action<TKey, TValue> optionAction;

      public ActionOption(string prototype, string description, Action<TKey, TValue> action)
        : base(prototype, description, 2)
      {
        this.optionAction = action ?? throw new ArgumentNullException(nameof(action));
      }

      protected override void OnParseComplete(OptionContext context)
      {
        var c = context ?? throw new ArgumentNullException(nameof(context));
        optionAction(
            Parse<TKey>(c.OptionValues[0], c),
            Parse<TValue>(c.OptionValues[1], c));
      }
    }

    public OptionCollection Add<T>(string prototype, Action<T> action)
    {
      return Add(prototype, null, action);
    }

    public OptionCollection Add<T>(string prototype, string description, Action<T> action)
    {
      return Add(new ActionOption<T>(prototype, description, action));
    }

    public OptionCollection Add<TKey, TValue>(string prototype, Action<TKey, TValue> action)
    {
      return Add(prototype, null, action);
    }

    public OptionCollection Add<TKey, TValue>(string prototype, string description, Action<TKey, TValue> action)
    {
      return Add(new ActionOption<TKey, TValue>(prototype, description, action));
    }

    protected virtual OptionContext CreateOptionContext()
    {
      return new OptionContext(this);
    }

#if LINQ
    public List<string> Parse (IEnumerable<string> arguments)
    {
      bool process = true;
      OptionContext c = CreateOptionContext ();
      c.OptionIndex = -1;
      var def = GetOptionForName ("<>");
      var unprocessed =
        from argument in arguments
        where ++c.OptionIndex >= 0 && (process || def != null)
          ? process
            ? argument == "--"
              ? (process = false)
              : !Parse (argument, c)
                ? def != null
                  ? Unprocessed (null, def, c, argument)
                  : true
                : false
            : def != null
              ? Unprocessed (null, def, c, argument)
              : true
          : true
        select argument;
      List<string> r = unprocessed.ToList ();
      if (c.Option != null)
        c.Option.Invoke (c);
      return r;
    }
#else

    public Collection<string> Parse(IEnumerable<string> arguments)
    {
      arguments = arguments ?? Array.Empty<string>();
      OptionContext c = CreateOptionContext();
      c.OptionIndex = -1;
      bool process = true;
      List<string> unprocessed = new List<string>();
      Option def = Contains("<>") ? this["<>"] : null;
      foreach (string argument in arguments)
      {
        ++c.OptionIndex;
        if (argument == "--")
        {
          process = false;
          continue;
        }
        if (!process)
        {
          Unprocessed(unprocessed, def, c, argument);
          continue;
        }
        if (!Parse(argument, c))
          Unprocessed(unprocessed, def, c, argument);
      }
      if (c.Option != null)
        c.Option.Invoke(c);
      return new Collection<string>(unprocessed);
    }

#endif

    private static bool Unprocessed(ICollection<string> extra, Option def, OptionContext c, string argument)
    {
      if (def == null)
      {
        extra.Add(argument);
        return false;
      }
      c.OptionValues.Add(argument);
      c.Option = def;
      c.Option.Invoke(c);
      return false;
    }

    private readonly Regex ValueOption = new Regex(
      @"^(?<flag>--|-|/)(?<name>[^:=]+)((?<sep>[:=])(?<value>.*))?$");

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters",
      Justification = "multi-return; F# would be simpler")]
    [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms",
      Justification = "It *is* a flag")]
    [SuppressMessage("Gendarme.Rules.Performance",
                 "AvoidRepetitiveCallsToPropertiesRule",
                 Justification = "value of different groups")]
    protected (string, string, string, string)[] GetOptionParts(string argument)//, out string flag, out string name, out string sep, out string value)
    {
      if (argument == null)
        throw new ArgumentNullException(nameof(argument));

      Match m = ValueOption.Match(argument);
      if (!m.Success)
      {
        return Array.Empty<(string, string, string, string)>();
      }
      var groups = m.Groups;
      var flag = groups["flag"].Value;
      var name = groups["name"].Value;
      string sep = null;
      string value = null;
      if (groups["sep"].Success && groups["value"].Success)
      {
        sep = groups["sep"].Value;
        value = groups["value"].Value;
      }
      return new[] { (flag, name, sep, value) };
    }

    protected virtual bool Parse(string argument, OptionContext context)
    {
      var c = context ?? throw new ArgumentNullException(nameof(context));
      if (c.Option != null)
      {
        ParseValue(argument, c);
        return true;
      }

      var parts = GetOptionParts(argument);
      if (!parts.Any())
        return false;
      (string f, string n, string s, string v) = parts[0];

      Option p;
      if (Contains(n))
      {
        p = this[n];
        c.OptionName = f + n;
        c.Option = p;
        switch (p.OptionValueType)
        {
          case OptionValueType.None:
            c.OptionValues.Add(n);
            c.Option.Invoke(c);
            break;

          case OptionValueType.Optional:
          case OptionValueType.Required:
            ParseValue(v, c);
            break;
        }
        return true;
      }
      // no match; is it a bool option?
      if (ParseBool(argument, n, c))
        return true;
      // is it a bundled option?
      if (ParseBundledValue(f, string.Concat(n + s + v), c))
        return true;

      return false;
    }

    private void ParseValue(string option, OptionContext c)
    {
      if (option != null)
      {
        var s = c.Option.ValueSeparators;
        var separators = !s.Any()
            ? option.Split(s.ToArray(), StringSplitOptions.None)
            : new string[] { option };

        foreach (string o in separators)
        {
          c.OptionValues.Add(o);
        }
      }

      if (c.OptionValues.Count == c.Option.MaxValueCount ||
          c.Option.OptionValueType == OptionValueType.Optional)
        c.Option.Invoke(c);
      else if (c.OptionValues.Count > c.Option.MaxValueCount)
      {
        throw new OptionException(MessageLocalizer(string.Format(
          CultureInfo.InvariantCulture,
                "Error: Found {0} option values when expecting {1}.",
                c.OptionValues.Count, c.Option.MaxValueCount)),
            c.OptionName);
      }
    }

    private bool ParseBool(string option, string n, OptionContext c)
    {
      Option p;
      string rn;
      if (n.Length >= 1 && (n[n.Length - 1] == '+' || n[n.Length - 1] == '-') &&
          Contains((rn = n.Substring(0, n.Length - 1))))
      {
        p = this[rn];
        string v = n[n.Length - 1] == '+' ? option : null;
        c.OptionName = option;
        c.Option = p;
        c.OptionValues.Add(v);
        p.Invoke(c);
        return true;
      }
      return false;
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly",
      Justification = "OptionValueType names a type")]
    private bool ParseBundledValue(string f, string n, OptionContext c)
    {
      if (f != "-")
        return false;
      for (int i = 0; i < n.Length; ++i)
      {
        Option p;
        string opt = f + n[i].ToString(CultureInfo.InvariantCulture);
        string rn = n[i].ToString(CultureInfo.InvariantCulture);
        if (!Contains(rn))
        {
          if (i == 0)
            return false;
          throw new OptionException(string.Format(
            CultureInfo.InvariantCulture,
            MessageLocalizer(
                  "Cannot bundle unregistered option '{0}'."), opt), opt);
        }
        p = this[rn];
        switch (p.OptionValueType)
        {
          case OptionValueType.None:
            Invoke(c, opt, n, p);
            break;

          case OptionValueType.Optional:
          case OptionValueType.Required:
            {
              string v = n.Substring(i + 1);
              c.Option = p;
              c.OptionName = opt;
              ParseValue(v.Length != 0 ? v : null, c);
              return true;
            }
          default:
            throw new InvalidOperationException("Unknown OptionValueType: " + p.OptionValueType);
        }
      }
      return true;
    }

    private static void Invoke(OptionContext c, string name, string value, Option option)
    {
      c.OptionName = name;
      c.Option = option;
      c.OptionValues.Add(value);
      option.Invoke(c);
    }

    private const int OptionWidth = 29;

    public void WriteOptionDescriptions(TextWriter writer)
    {
      var w = writer ?? throw new ArgumentNullException(nameof(writer));
      foreach (Option p in this)
      {
        int written = 0;
        if (!WriteOptionPrototype(w, p, ref written))
          continue;

        if (written < OptionWidth)
          w.Write(new string(' ', OptionWidth - written));
        else
        {
          w.WriteLine();
          w.Write(new string(' ', OptionWidth));
        }

        List<string> lines = GetLines(MessageLocalizer(GetDescription(p.Description)));
        w.WriteLine(lines[0]);
        string prefix = new string(' ', OptionWidth + 2);
        for (int i = 1; i < lines.Count; ++i)
        {
          w.Write(prefix);
          w.WriteLine(lines[i]);
        }
      }
    }

    private bool WriteOptionPrototype(TextWriter o, Option p, ref int written)
    {
      var names = p.Names;

      int i = GetNextOptionIndex(names, 0);
      if (i == names.Count)
        return false;

      if (names[i].Length == 1)
      {
        Write(o, ref written, "  -");
        Write(o, ref written, names[0]);
      }
      else
      {
        Write(o, ref written, "      --");
        Write(o, ref written, names[0]);
      }

      for (i = GetNextOptionIndex(names, i + 1);
          i < names.Count; i = GetNextOptionIndex(names, i + 1))
      {
        Write(o, ref written, ", ");
        Write(o, ref written, names[i].Length == 1 ? "-" : "--");
        Write(o, ref written, names[i]);
      }

      if (p.OptionValueType == OptionValueType.Optional ||
          p.OptionValueType == OptionValueType.Required)
      {
        if (p.OptionValueType == OptionValueType.Optional)
        {
          Write(o, ref written, MessageLocalizer("["));
        }
        Write(o, ref written, MessageLocalizer("=" + GetArgumentName(0, p.MaxValueCount, p.Description)));
        var s = p.ValueSeparators;
        string sep = s != null && s.Any()
          ? s[0]
          : " ";
        for (int c = 1; c < p.MaxValueCount; ++c)
        {
          Write(o, ref written, MessageLocalizer(sep + GetArgumentName(c, p.MaxValueCount, p.Description)));
        }
        if (p.OptionValueType == OptionValueType.Optional)
        {
          Write(o, ref written, MessageLocalizer("]"));
        }
      }
      return true;
    }

    private static int GetNextOptionIndex(ReadOnlyCollection<string> names, int i)
    {
      while (i < names.Count && names[i] == "<>")
      {
        ++i;
      }
      return i;
    }

    private static void Write(TextWriter o, ref int n, string s)
    {
      n += s.Length;
      o.Write(s);
    }

#pragma warning disable IDE0079 // Remove unnecessary suppression

    [SuppressMessage("Gendarme.Rules.Globalization",
                    "PreferStringComparisonOverrideRule",
                    Justification = "IndexOf overrides not available")]
    private static string GetArgumentName(int index, int maxIndex, string description)
    {
      if (description == null)
        return maxIndex == 1 ? "VALUE" : "VALUE" + (index + 1).ToString(CultureInfo.InvariantCulture);
      string[] nameStart;
      if (maxIndex == 1)
        nameStart = new string[] { "{0:", "{" };
      else
        nameStart = new string[] { "{" + index.ToString(CultureInfo.InvariantCulture) + ":" };
      for (int i = 0; i < nameStart.Length; ++i)
      {
        int start, j = 0;
        do
        {
          start = description.IndexOf(nameStart[i], j);
        } while (start >= 0 && j != 0 && description[j++ - 1] == '{');
        if (start == -1)
          continue;
        int end = description.IndexOf('}', start);
        if (end == -1)
          continue;
        return description.Substring(start + nameStart[i].Length, end - start - nameStart[i].Length);
      }
      return maxIndex == 1 ? "VALUE" : "VALUE" + (index + 1).ToString(CultureInfo.InvariantCulture); ;
    }

    private static string GetDescription(string description)
    {
      if (description == null)
        return string.Empty;
      StringBuilder sb = new StringBuilder(description.Length);
      int start = -1;
      for (int i = 0; i < description.Length; ++i)
      {
        switch (description[i])
        {
          case '{':
            if (i == start)
            {
              sb.Append('{');
              start = -1;
            }
            else if (start < 0)
              start = i + 1;
            break;

          case '}':
            if (start < 0)
            {
              if ((i + 1) == description.Length || description[i + 1] != '}')
                throw new InvalidOperationException("Invalid option description: " + description);
              ++i;
              sb.Append('}');
            }
            else
            {
#pragma warning disable CA1846 // Prefer 'AsSpan' over 'Substring'
              sb.Append(description.Substring(start, i - start));
#pragma warning restore CA1846 // Prefer 'AsSpan' over 'Substring'
              start = -1;
            }
            break;

          case ':':
            if (start < 0)
              goto default;
            start = i + 1;
            break;

          default:
            if (start < 0)
              sb.Append(description[i]);
            break;
        }
      }
      return sb.ToString();
    }

    private static List<string> GetLines(string description)
    {
      List<string> lines = new List<string>();
      if (string.IsNullOrEmpty(description))
      {
        lines.Add(string.Empty);
        return lines;
      }
      int length = 80 - OptionWidth - 2;
      int start = 0, end;
      do
      {
        end = GetLineEnd(start, length, description);
        bool cont = false;
        if (end < description.Length)
        {
          char c = description[end];
          if (c == '-' || (char.IsWhiteSpace(c) && c != '\n'))
            ++end;
          else if (c != '\n')
          {
            cont = true;
            --end;
          }
        }
        lines.Add(description.Substring(start, end - start));
        if (cont)
        {
          lines[lines.Count - 1] += "-";
        }
        start = end;
        if (start < description.Length && description[start] == '\n')
          ++start;
      } while (end < description.Length);
      return lines;
    }

#pragma warning disable IDE0079 // Remove unnecessary suppression

    [SuppressMessage("Gendarme.Rules.Smells",
                      "AvoidSwitchStatementsRule",
                      Justification = "chars are not types")]
    private static int GetLineEnd(int start, int length, string description)
    {
      int end = Math.Min(start + length, description.Length);
      int sep = -1;
      for (int i = start; i < end; ++i)
      {
        switch (description[i])
        {
          case ' ':
          case '\t':
          case '\v':
          case '-':
          case ',':
          case '.':
          case ';':
            sep = i;
            break;

          case '\n':
            return i;
        }
      }
      if (sep == -1 || end == description.Length)
        return end;
      return sep;
    }
  }
}