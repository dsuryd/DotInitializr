using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DotInitializr
{
   public partial class DotNetTemplateMetadata
   {
      public const string FILE_NAME = "template.json";
      public const string FILE_PATH = "/.template.config";

      /// <summary>
      /// The author of the template
      /// </summary>
      [JsonProperty("author")]
      public string Author { get; set; }

      /// <summary>
      /// Alternate sets of defaults for symbols
      /// </summary>
      [JsonProperty("baselines", NullValueHandling = NullValueHandling.Ignore)]
      public Dictionary<string, Baseline> Baselines { get; set; }

      /// <summary>
      /// Zero or more characteristics of the template that a user might search for it by
      /// </summary>
      [JsonProperty("classifications")]
      [JsonConverter(typeof(DecodeArrayConverter))]
      public string[] Classifications { get; set; }

      /// <summary>
      /// The name to use during creation if no name has been specified and no other opinionation
      /// about naming has been provided from the host
      /// </summary>
      [JsonProperty("defaultName", NullValueHandling = NullValueHandling.Ignore)]
      [JsonConverter(typeof(MinMaxLengthCheckConverter))]
      public string DefaultName { get; set; }

      /// <summary>
      /// A description of the template's purpose or contents for use in help
      /// </summary>
      [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
      public string Description { get; set; }

      /// <summary>
      /// Custom value forms used by the template
      /// </summary>
      [JsonProperty("forms", NullValueHandling = NullValueHandling.Ignore)]
      public Dictionary<string, Form> Forms { get; set; }

      /// <summary>
      /// The semantic version range of the Microsoft.TemplateEngine.Orchestrator.RunnableProjects
      /// package that this template is usable with. In older versions of the engine, the four
      /// octet version string is used but is matched against a constant which was never changed
      /// from 1.0.0.0 - either syntax is now accepted, however the four octet string will not be
      /// changed from 1.0.0.0
      /// </summary>
      [JsonProperty("generatorVersions", NullValueHandling = NullValueHandling.Ignore)]
      public string GeneratorVersions { get; set; }

      /// <summary>
      /// The ID of the group this template belongs to. When combined with the "tags" section, this
      /// allows multiple templates to be displayed as one, with the the decision for which one to
      /// use being presented as a choice in each one of the pivot categories (keys).
      /// </summary>
      [JsonProperty("groupIdentity", NullValueHandling = NullValueHandling.Ignore)]
      [JsonConverter(typeof(MinMaxLengthCheckConverter))]
      public string GroupIdentity { get; set; }

      /// <summary>
      /// A list of guids which appear in the template source and should be replaced in the
      /// template output. For each guid listed, a replacement guid is generated, and replaces all
      /// occurrences of the source guid in the output.
      /// </summary>
      [JsonProperty("guids", NullValueHandling = NullValueHandling.Ignore)]
      public string[] Guids { get; set; }

      /// <summary>
      /// A unique name for this template
      /// </summary>
      [JsonProperty("identity")]
      [JsonConverter(typeof(MinMaxLengthCheckConverter))]
      public string Identity { get; set; }

      /// <summary>
      /// The name for the template that users should see
      /// </summary>
      [JsonProperty("name")]
      [JsonConverter(typeof(MinMaxLengthCheckConverter))]
      public string Name { get; set; }

      /// <summary>
      /// A filename that will be completely ignored except to indicate that its containing
      /// directory should be copied. This allows creation of an empty directory in the created
      /// template, by having a corresponding source directory containing just the placeholder
      /// file. Completely empty directories are ignored.
      /// </summary>
      [JsonProperty("placeholderFilename", NullValueHandling = NullValueHandling.Ignore)]
      public string PlaceholderFilename { get; set; }

      /// <summary>
      /// Defines an ordered list of actions to perform after template generation. The post action
      /// information is provided to the creation broker, to act on as appropriate.
      /// </summary>
      [JsonProperty("postActions", NullValueHandling = NullValueHandling.Ignore)]
      public PostAction[] PostActions { get; set; }

      /// <summary>
      /// A value used to determine how preferred this template is among the other templates with
      /// the same groupIdentity (higher values are more preferred)
      /// </summary>
      [JsonProperty("precedence", NullValueHandling = NullValueHandling.Ignore)]
      public Precedence? Precedence { get; set; }

      /// <summary>
      /// Indicates whether to create a directory for the template if name is specified but an
      /// output directory is not set (instead of creating the content directly in the current
      /// directory)
      /// </summary>
      [JsonProperty("preferNameDirectory", NullValueHandling = NullValueHandling.Ignore)]
      public bool? PreferNameDirectory { get; set; }

      /// <summary>
      /// A list of important output paths created during template generation. These paths need to
      /// be added to the newly created project at the end of template creation.
      /// </summary>
      [JsonProperty("primaryOutputs", NullValueHandling = NullValueHandling.Ignore)]
      public PrimaryOutput[] PrimaryOutputs { get; set; }

      /// <summary>
      /// A shorthand name or a list of names for selecting the template (applies to environments
      /// where the template name is specified by the user - not selected via a GUI). The first
      /// entry is the preferred short name.
      /// </summary>
      [JsonProperty("shortName")]
      public ShortName ShortName { get; set; }

      /// <summary>
      /// The name in the source tree to replace with the name the user specifies
      /// </summary>
      [JsonProperty("sourceName", NullValueHandling = NullValueHandling.Ignore)]
      public string SourceName { get; set; }

      /// <summary>
      /// The set of mappings in the template content to user directories
      /// </summary>
      [JsonProperty("sources", NullValueHandling = NullValueHandling.Ignore)]
      public Source[] Sources { get; set; }

      /// <summary>
      /// The symbols section defines variables and their values, the values may be the defined in
      /// terms of other symbols. When a defined symbol name is encountered anywhere in the
      /// template definition, it is replaced by the value defined in this configuration. The
      /// symbols configuration is a collection of key-value pairs. The keys are the symbol names,
      /// and the value contains key-value-pair configuration information on how to assign the
      /// symbol a value.
      /// </summary>
      [JsonProperty("symbols", NullValueHandling = NullValueHandling.Ignore)]
      public Dictionary<string, Generator> Symbols { get; set; }

      /// <summary>
      /// Common information about templates, these are effectively interchangeable with choice
      /// type parameter symbols
      /// </summary>
      [JsonProperty("tags")]
      public Tags Tags { get; set; }

      /// <summary>
      /// An URL for a document indicating any libraries used by the template that are not
      /// owned/provided by the template author
      /// </summary>
      [JsonProperty("thirdPartyNotices", NullValueHandling = NullValueHandling.Ignore)]
      public string ThirdPartyNotices { get; set; }
   }

   /// <summary>
   /// A named set of alternate defaults
   /// </summary>
   public partial class Baseline
   {
      /// <summary>
      /// A lookup of symbol names to new defaults
      /// </summary>
      [JsonProperty("defaultOverrides", NullValueHandling = NullValueHandling.Ignore)]
      public Dictionary<string, string> DefaultOverrides { get; set; }

      /// <summary>
      /// A string to use to indicate the intent of the baesline
      /// </summary>
      [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
      public string Description { get; set; }
   }

   public partial class Form
   {
      /// <summary>
      /// The identifier for the value form component that will be used to transform the value
      /// </summary>
      [JsonProperty("identifier", NullValueHandling = NullValueHandling.Ignore)]
      public string Identifier { get; set; }
   }

   public partial class PostAction
   {
      /// <summary>
      /// A guid uniquely defining the action. The value must correspond to a post-action known by
      /// the broker.
      /// </summary>
      [JsonProperty("actionId")]
      public string ActionId { get; set; }

      /// <summary>
      /// A list of key-value pairs to use when performing the action. The specific parameters
      /// required / allowed are defined by the action itself.
      /// </summary>
      [JsonProperty("args", NullValueHandling = NullValueHandling.Ignore)]
      public Dictionary<string, string> Args { get; set; }

      /// <summary>
      /// A C++ style expression that, if it evaluates to 'false' causes the post-action to be
      /// skipped. This expression may refer to any symbols that have been defined
      /// </summary>
      [JsonProperty("condition", NullValueHandling = NullValueHandling.Ignore)]
      public string Condition { get; set; }

      /// <summary>
      /// Additional configuration for the associated post action. The structure & content will
      /// vary based on the post action.
      /// </summary>
      [JsonProperty("configFile", NullValueHandling = NullValueHandling.Ignore)]
      public string ConfigFile { get; set; }

      /// <summary>
      /// If this action fails, the value of continueOnError indicates whether to attempt the next
      /// action, or stop processing the post actions. Should be set to true when subsequent
      /// actions rely on the success of the current action.
      /// </summary>
      [JsonProperty("continueOnError", NullValueHandling = NullValueHandling.Ignore)]
      public bool? ContinueOnError { get; set; }

      /// <summary>
      /// A human-readable description of the action.
      /// </summary>
      [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
      public string Description { get; set; }

      /// <summary>
      /// An ordered list of possible instructions to display if the action cannot be performed.
      /// Each element in the list must contain a key named "text", whose value contains the
      /// instructions. Each element may also optionally provide a key named "condition" - a
      /// Boolean evaluate-able string. The first instruction whose condition is false or blank
      /// will be considered valid, all others are ignored.
      /// </summary>
      [JsonProperty("manualInstructions")]
      public ManualInstruction[] ManualInstructions { get; set; }
   }

   public partial class ManualInstruction
   {
      [JsonProperty("condition", NullValueHandling = NullValueHandling.Ignore)]
      public string Condition { get; set; }

      [JsonProperty("text")]
      public string Text { get; set; }
   }

   public partial class PrimaryOutput
   {
      /// <summary>
      /// The condition for including the specified path in the primary outputs set
      /// </summary>
      [JsonProperty("condition", NullValueHandling = NullValueHandling.Ignore)]
      public string Condition { get; set; }

      /// <summary>
      /// The path to the file in the template content whose corresponding output file should be
      /// included as a primary output
      /// </summary>
      [JsonProperty("path")]
      public string Path { get; set; }
   }

   public partial class Source
   {
      /// <summary>
      /// A list of additional source information which gets added to the top-level source
      /// information, based on evaluation the corresponding source.modifiers.condition.
      /// </summary>
      [JsonProperty("modifiers", NullValueHandling = NullValueHandling.Ignore)]
      public Modifier[] Modifiers { get; set; }

      /// <summary>
      /// The path in the template content (relative to the directory containing the
      /// .template.config folder) that should be processed
      /// </summary>
      [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
      public string SourceSource { get; set; }

      /// <summary>
      /// The path (relative to the directory the user has specified) that content should be
      /// written to
      /// </summary>
      [JsonProperty("target", NullValueHandling = NullValueHandling.Ignore)]
      public string Target { get; set; }

      /// <summary>
      /// Boolean-evaluable condition to indicate if the sources configuration should be included
      /// or ignored. If the condition evaluates to true or is not provided, the sources config
      /// will be used for creating the template. If it evaluates to false, the sources config will
      /// be ignored.
      /// </summary>
      [JsonProperty("condition", NullValueHandling = NullValueHandling.Ignore)]
      public string Condition { get; set; }

      /// <summary>
      /// The set of globbing patterns indicating the content that was included by sources.include,
      /// that hasn't been excluded by sources.exclude that should be placed in the user's
      /// directory without modification
      /// </summary>
      [JsonProperty("copyOnly", NullValueHandling = NullValueHandling.Ignore)]
      public Ice? CopyOnly { get; set; }

      /// <summary>
      /// The set of globbing patterns indicating the content that was included by sources.include
      /// that should not be processed
      /// </summary>
      [JsonProperty("exclude", NullValueHandling = NullValueHandling.Ignore)]
      public Ice? Exclude { get; set; }

      /// <summary>
      /// The set of globbing patterns indicating the content to process in the path referred to by
      /// sources.source
      /// </summary>
      [JsonProperty("include", NullValueHandling = NullValueHandling.Ignore)]
      public Ice? Include { get; set; }

      /// <summary>
      /// The set of explicit renames to perform. Each key is a path to a file in the source, each
      /// value is a path to the target location - only the values will be evaluated with the
      /// information the user supplies
      /// </summary>
      [JsonProperty("rename", NullValueHandling = NullValueHandling.Ignore)]
      public Dictionary<string, string> Rename { get; set; }
   }

   public partial class Modifier
   {
      /// <summary>
      /// Boolean-evaluable condition to indicate if the sources configuration should be included
      /// or ignored. If the condition evaluates to true or is not provided, the sources config
      /// will be used for creating the template. If it evaluates to false, the sources config will
      /// be ignored.
      /// </summary>
      [JsonProperty("condition", NullValueHandling = NullValueHandling.Ignore)]
      public string Condition { get; set; }

      /// <summary>
      /// The set of globbing patterns indicating the content that was included by sources.include,
      /// that hasn't been excluded by sources.exclude that should be placed in the user's
      /// directory without modification
      /// </summary>
      [JsonProperty("copyOnly", NullValueHandling = NullValueHandling.Ignore)]
      public Ice? CopyOnly { get; set; }

      /// <summary>
      /// The set of globbing patterns indicating the content that was included by sources.include
      /// that should not be processed
      /// </summary>
      [JsonProperty("exclude", NullValueHandling = NullValueHandling.Ignore)]
      public Ice? Exclude { get; set; }

      /// <summary>
      /// The set of globbing patterns indicating the content to process in the path referred to by
      /// sources.source
      /// </summary>
      [JsonProperty("include", NullValueHandling = NullValueHandling.Ignore)]
      public Ice? Include { get; set; }

      [JsonProperty("modifiers", NullValueHandling = NullValueHandling.Ignore)]
      public Modifier[] Modifiers { get; set; }

      /// <summary>
      /// The set of explicit renames to perform. Each key is a path to a file in the source, each
      /// value is a path to the target location - only the values will be evaluated with the
      /// information the user supplies
      /// </summary>
      [JsonProperty("rename", NullValueHandling = NullValueHandling.Ignore)]
      public Dictionary<string, string> Rename { get; set; }
   }

   public partial class Generator
   {
      [JsonProperty("datatype")]
      public object Datatype { get; set; }

      /// <summary>
      /// The text to replace with the value of this symbol
      /// </summary>
      [JsonProperty("replaces", NullValueHandling = NullValueHandling.Ignore)]
      public string Replaces { get; set; }

      /// <summary>
      /// Defines a symbol that has its value provided by the host
      ///
      /// The value of this symbol is derived from the value of another symbol by the application
      /// of value forms
      ///
      /// Defines the high level configuration of symbol
      /// </summary>
      [JsonProperty("type")]
      public SymbolType Type { get; set; }

      /// <summary>
      /// The name of the host property to take the value from
      /// </summary>
      [JsonProperty("binding", NullValueHandling = NullValueHandling.Ignore)]
      public string Binding { get; set; }

      /// <summary>
      /// The name of the symbol that the value should be derived from
      /// </summary>
      [JsonProperty("valueSource", NullValueHandling = NullValueHandling.Ignore)]
      public string ValueSource { get; set; }

      /// <summary>
      /// The name of the value form that should be applied to the source value to use as the value
      /// of this symbol
      /// </summary>
      [JsonProperty("valueTransform", NullValueHandling = NullValueHandling.Ignore)]
      public string ValueTransform { get; set; }

      [JsonProperty("generator", NullValueHandling = NullValueHandling.Ignore)]
      public GeneratorEnum? GeneratorGenerator { get; set; }

      [JsonProperty("parameters")]
      public ParametersUnion? Parameters { get; set; }

      /// <summary>
      /// An array listing the valid choices for a symbol whose datatype = choice. If not provided,
      /// there are no valid choices for the symbol, so it can never be assigned a value.
      /// </summary>
      [JsonProperty("choices", NullValueHandling = NullValueHandling.Ignore)]
      public ChoiceElement[] Choices { get; set; }

      /// <summary>
      /// The value assigned to the symbol if no value for it is provided by the user or host.
      /// </summary>
      [JsonProperty("defaultValue", NullValueHandling = NullValueHandling.Ignore)]
      public string DefaultValue { get; set; }

      /// <summary>
      /// The description of the parameter
      /// </summary>
      [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
      public string Description { get; set; }

      [JsonProperty("isRequired", NullValueHandling = NullValueHandling.Ignore)]
      public bool? IsRequired { get; set; }

      [JsonProperty("onlyIf")]
      public OnlyIfUnion? OnlyIf { get; set; }

      /// <summary>
      /// An evaluate-able condition whose result defines the value of the symbol.
      /// </summary>
      [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
      public string Value { get; set; }
   }

   public partial class ChoiceClass
   {
      /// <summary>
      /// A valid value for the symbol
      /// </summary>
      [JsonProperty("choice")]
      public string Choice { get; set; }

      /// <summary>
      /// Help text describing the meaning of the corresponding value
      /// </summary>
      [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
      public string Description { get; set; }
   }

   public partial class OnlyIfClass
   {
      /// <summary>
      /// The replacement string occurs after this value
      /// </summary>
      [JsonProperty("after", NullValueHandling = NullValueHandling.Ignore)]
      public string After { get; set; }

      /// <summary>
      /// The replacement string occurs before this value
      /// </summary>
      [JsonProperty("before", NullValueHandling = NullValueHandling.Ignore)]
      public string Before { get; set; }
   }

   public partial class ParametersClass
   {
      /// <summary>
      /// The name of the symbol whose value should have its case changed
      ///
      /// The name of a different parameter in the template configuration. A copy of its value will
      /// be used by this generator's regex to generate the value for this parameter. The value of
      /// the source parameter is not modified
      /// </summary>
      [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
      public string Source { get; set; }

      /// <summary>
      /// Whether the case changed to should be lower case, if false, the value is made to be
      /// uppercase instead
      /// </summary>
      [JsonProperty("toLower", NullValueHandling = NullValueHandling.Ignore)]
      public bool? ToLower { get; set; }

      /// <summary>
      /// The value to consider as being the default - if the value of the symbol referred to by
      /// sourceVariableName is equal to this, the value of the symbol referred to in
      /// fallbackVariableName is used instead
      /// </summary>
      [JsonProperty("defaultValue", NullValueHandling = NullValueHandling.Ignore)]
      public string DefaultValue { get; set; }

      /// <summary>
      /// The name of the symbol to return the value of if the symbol referred to by
      /// sourceVariableName has a value equal to the value of defaultValue
      /// </summary>
      [JsonProperty("fallbackVariableName", NullValueHandling = NullValueHandling.Ignore)]
      public string FallbackVariableName { get; set; }

      /// <summary>
      /// The name of the symbol whose value will be inspected - if the value is effectively equal
      /// to the default value, the value of the symbol referred to by fallbackVariableName is
      /// used, otherwise the value of this symbol
      /// </summary>
      [JsonProperty("sourceVariableName", NullValueHandling = NullValueHandling.Ignore)]
      public string SourceVariableName { get; set; }

      /// <summary>
      /// The value to be assigned to the symbol
      /// </summary>
      [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
      public string Value { get; set; }

      /// <summary>
      /// The Boolean predicate whose evaluation result becomes the symbol value
      ///
      /// Must be the string literal "new".
      ///
      /// The format string to use when converting the date-time to a string representation.
      ///
      /// Must be the string literal "new"
      /// </summary>
      [JsonProperty("action", NullValueHandling = NullValueHandling.Ignore)]
      public string Action { get; set; }

      /// <summary>
      /// A string indicating the predicate evaluator to evaluate the action against.
      ///
      /// The style of evaluator to use
      /// </summary>
      [JsonProperty("evaluator", NullValueHandling = NullValueHandling.Ignore)]
      public string Evaluator { get; set; }

      /// <summary>
      /// When a string representation of the guid is needed, this is used as the format string in
      /// Guid.ToString().
      /// </summary>
      [JsonProperty("format", NullValueHandling = NullValueHandling.Ignore)]
      public string Format { get; set; }

      /// <summary>
      /// If true, use UTC time. If false, use local time.
      /// </summary>
      [JsonProperty("utc", NullValueHandling = NullValueHandling.Ignore)]
      public bool? Utc { get; set; }

      /// <summary>
      /// The port number to use if no free ports could be found
      /// </summary>
      [JsonProperty("fallback", NullValueHandling = NullValueHandling.Ignore)]
      public long? Fallback { get; set; }

      /// <summary>
      /// The upper bound of acceptable port numbers
      ///
      /// An integer value indicating the high-end of the range to generate the random number in.
      /// If not explicitly provided, defaults to int.MaxValue.
      /// </summary>
      [JsonProperty("high", NullValueHandling = NullValueHandling.Ignore)]
      public long? High { get; set; }

      /// <summary>
      /// The lower bound of acceptable port numbers
      ///
      /// An integer value indicating the low-end of the range to generate the random number in.
      /// </summary>
      [JsonProperty("low", NullValueHandling = NullValueHandling.Ignore)]
      public long? Low { get; set; }

      /// <summary>
      /// An ordered list of key-value pairs indicating the regex replacement actions. Each element
      /// of the list must contain exactly the keys 'regex' and 'replacement' - along with their
      /// values. These replacements will be applied to the result of the previous replacement
      /// (except the first, which acts on the original value from the source).
      /// </summary>
      [JsonProperty("steps", NullValueHandling = NullValueHandling.Ignore)]
      public StepElement[] Steps { get; set; }

      /// <summary>
      /// The set of cases to test for. The first one, in document order, to return true's value is
      /// used, if none return true, empty string is returned
      /// </summary>
      [JsonProperty("cases", NullValueHandling = NullValueHandling.Ignore)]
      public Case[] Cases { get; set; }

      [JsonProperty("datatype")]
      public object Datatype { get; set; }
   }

   public partial class Case
   {
      /// <summary>
      /// An expression to be interpreted by the specified evaluator type
      /// </summary>
      [JsonProperty("condition")]
      public string Condition { get; set; }

      /// <summary>
      /// The value to return if the condition evaluates to true
      /// </summary>
      [JsonProperty("value")]
      public string Value { get; set; }
   }

   public partial class StepClass
   {
      /// <summary>
      /// The regular expression to use to locate the sequence to replace
      /// </summary>
      [JsonProperty("regex")]
      public string Regex { get; set; }

      /// <summary>
      /// The replacement for any sequences matched by the supplied regular expression
      /// </summary>
      [JsonProperty("replacement")]
      public string Replacement { get; set; }
   }

   /// <summary>
   /// Common information about templates, these are effectively interchangeable with choice
   /// type parameter symbols
   /// </summary>
   public partial class Tags
   {
      /// <summary>
      /// The programming language the template primarily contains or is intended for use with
      /// </summary>
      [JsonProperty("language", NullValueHandling = NullValueHandling.Ignore)]
      public string Language { get; set; }

      /// <summary>
      /// The type of template: project or item
      /// </summary>
      [JsonProperty("type")]
      public TagsType Type { get; set; }
   }

   public enum GeneratorEnum { Casing, Coalesce, Constant, Evaluate, Guid, Now, Port, Random, Regex, Switch };

   /// <summary>
   /// Defines a symbol that has its value provided by the host
   ///
   /// The value of this symbol is derived from the value of another symbol by the application
   /// of value forms
   ///
   /// Defines the high level configuration of symbol
   /// </summary>
   public enum SymbolType { Bind, Computed, Derived, Generated, Parameter };

   /// <summary>
   /// The type of template: project or item
   /// </summary>
   public enum TagsType { Item, Project };

   /// <summary>
   /// A value used to determine how preferred this template is among the other templates with
   /// the same groupIdentity (higher values are more preferred)
   /// </summary>
   public partial struct Precedence
   {
      public long? Integer;
      public string String;

      public static implicit operator Precedence(long Integer) => new Precedence { Integer = Integer };

      public static implicit operator Precedence(string String) => new Precedence { String = String };
   }

   /// <summary>
   /// A shorthand name or a list of names for selecting the template (applies to environments
   /// where the template name is specified by the user - not selected via a GUI). The first
   /// entry is the preferred short name.
   /// </summary>
   public partial struct ShortName
   {
      public object[] AnythingArray;
      public string String;

      public static implicit operator ShortName(object[] AnythingArray) => new ShortName { AnythingArray = AnythingArray };

      public static implicit operator ShortName(string String) => new ShortName { String = String };
   }

   /// <summary>
   /// The set of globbing patterns indicating the content that was included by sources.include,
   /// that hasn't been excluded by sources.exclude that should be placed in the user's
   /// directory without modification
   ///
   /// The set of globbing patterns indicating the content that was included by sources.include
   /// that should not be processed
   ///
   /// The set of globbing patterns indicating the content to process in the path referred to by
   /// sources.source
   /// </summary>
   public partial struct Ice
   {
      public string String;
      public string[] StringArray;

      public static implicit operator Ice(string String) => new Ice { String = String };

      public static implicit operator Ice(string[] StringArray) => new Ice { StringArray = StringArray };
   }

   public partial struct ChoiceElement
   {
      public object[] AnythingArray;
      public bool? Bool;
      public ChoiceClass ChoiceClass;
      public double? Double;
      public long? Integer;
      public string String;

      public static implicit operator ChoiceElement(object[] AnythingArray) => new ChoiceElement { AnythingArray = AnythingArray };

      public static implicit operator ChoiceElement(bool Bool) => new ChoiceElement { Bool = Bool };

      public static implicit operator ChoiceElement(ChoiceClass ChoiceClass) => new ChoiceElement { ChoiceClass = ChoiceClass };

      public static implicit operator ChoiceElement(double Double) => new ChoiceElement { Double = Double };

      public static implicit operator ChoiceElement(long Integer) => new ChoiceElement { Integer = Integer };

      public static implicit operator ChoiceElement(string String) => new ChoiceElement { String = String };

      public bool IsNull => AnythingArray == null && Bool == null && ChoiceClass == null && Double == null && Integer == null && String == null;
   }

   public partial struct OnlyIfUnion
   {
      public object[] AnythingArray;
      public bool? Bool;
      public double? Double;
      public long? Integer;
      public OnlyIfClass OnlyIfClass;
      public string String;

      public static implicit operator OnlyIfUnion(object[] AnythingArray) => new OnlyIfUnion { AnythingArray = AnythingArray };

      public static implicit operator OnlyIfUnion(bool Bool) => new OnlyIfUnion { Bool = Bool };

      public static implicit operator OnlyIfUnion(double Double) => new OnlyIfUnion { Double = Double };

      public static implicit operator OnlyIfUnion(long Integer) => new OnlyIfUnion { Integer = Integer };

      public static implicit operator OnlyIfUnion(OnlyIfClass OnlyIfClass) => new OnlyIfUnion { OnlyIfClass = OnlyIfClass };

      public static implicit operator OnlyIfUnion(string String) => new OnlyIfUnion { String = String };

      public bool IsNull => AnythingArray == null && Bool == null && OnlyIfClass == null && Double == null && Integer == null && String == null;
   }

   public partial struct StepElement
   {
      public object[] AnythingArray;
      public bool? Bool;
      public double? Double;
      public long? Integer;
      public StepClass StepClass;
      public string String;

      public static implicit operator StepElement(object[] AnythingArray) => new StepElement { AnythingArray = AnythingArray };

      public static implicit operator StepElement(bool Bool) => new StepElement { Bool = Bool };

      public static implicit operator StepElement(double Double) => new StepElement { Double = Double };

      public static implicit operator StepElement(long Integer) => new StepElement { Integer = Integer };

      public static implicit operator StepElement(StepClass StepClass) => new StepElement { StepClass = StepClass };

      public static implicit operator StepElement(string String) => new StepElement { String = String };

      public bool IsNull => AnythingArray == null && Bool == null && StepClass == null && Double == null && Integer == null && String == null;
   }

   public partial struct ParametersUnion
   {
      public object[] AnythingArray;
      public bool? Bool;
      public double? Double;
      public long? Integer;
      public ParametersClass ParametersClass;
      public string String;

      public static implicit operator ParametersUnion(object[] AnythingArray) => new ParametersUnion { AnythingArray = AnythingArray };

      public static implicit operator ParametersUnion(bool Bool) => new ParametersUnion { Bool = Bool };

      public static implicit operator ParametersUnion(double Double) => new ParametersUnion { Double = Double };

      public static implicit operator ParametersUnion(long Integer) => new ParametersUnion { Integer = Integer };

      public static implicit operator ParametersUnion(ParametersClass ParametersClass) => new ParametersUnion { ParametersClass = ParametersClass };

      public static implicit operator ParametersUnion(string String) => new ParametersUnion { String = String };

      public bool IsNull => AnythingArray == null && Bool == null && ParametersClass == null && Double == null && Integer == null && String == null;
   }

   public partial class DotNetTemplateMetadata
   {
      public static DotNetTemplateMetadata FromJson(string json) => JsonConvert.DeserializeObject<DotNetTemplateMetadata>(json, DotInitializr.Converter.Settings);
   }

   public static class Serialize
   {
      public static string ToJson(this DotNetTemplateMetadata self) => JsonConvert.SerializeObject(self, DotInitializr.Converter.Settings);
   }

   internal static class Converter
   {
      public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
      {
         MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
         DateParseHandling = DateParseHandling.None,
         Converters =
            {
                PrecedenceConverter.Singleton,
                ShortNameConverter.Singleton,
                IceConverter.Singleton,
                ChoiceElementConverter.Singleton,
                GeneratorEnumConverter.Singleton,
                OnlyIfUnionConverter.Singleton,
                ParametersUnionConverter.Singleton,
                StepElementConverter.Singleton,
                SymbolTypeConverter.Singleton,
                TagsTypeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
      };
   }

   internal class DecodeArrayConverter : JsonConverter
   {
      public override bool CanConvert(Type t) => t == typeof(string[]);

      public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
      {
         reader.Read();
         var value = new List<string>();
         while (reader.TokenType != JsonToken.EndArray)
         {
            var converter = MinMaxLengthCheckConverter.Singleton;
            var arrayItem = (string) converter.ReadJson(reader, typeof(string), null, serializer);
            value.Add(arrayItem);
            reader.Read();
         }
         return value.ToArray();
      }

      public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
      {
         var value = (string[]) untypedValue;
         writer.WriteStartArray();
         foreach (var arrayItem in value)
         {
            var converter = MinMaxLengthCheckConverter.Singleton;
            converter.WriteJson(writer, arrayItem, serializer);
         }
         writer.WriteEndArray();
         return;
      }

      public static readonly DecodeArrayConverter Singleton = new DecodeArrayConverter();
   }

   internal class MinMaxLengthCheckConverter : JsonConverter
   {
      public override bool CanConvert(Type t) => t == typeof(string);

      public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
      {
         var value = serializer.Deserialize<string>(reader);
         if (value.Length >= 1)
         {
            return value;
         }
         throw new Exception("Cannot unmarshal type string");
      }

      public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
      {
         var value = (string) untypedValue;
         if (value.Length >= 1)
         {
            serializer.Serialize(writer, value);
            return;
         }
         throw new Exception("Cannot marshal type string");
      }

      public static readonly MinMaxLengthCheckConverter Singleton = new MinMaxLengthCheckConverter();
   }

   internal class PrecedenceConverter : JsonConverter
   {
      public override bool CanConvert(Type t) => t == typeof(Precedence) || t == typeof(Precedence?);

      public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
      {
         switch (reader.TokenType)
         {
            case JsonToken.Integer:
               var integerValue = serializer.Deserialize<long>(reader);
               return new Precedence { Integer = integerValue };

            case JsonToken.String:
            case JsonToken.Date:
               var stringValue = serializer.Deserialize<string>(reader);
               return new Precedence { String = stringValue };
         }
         throw new Exception("Cannot unmarshal type Precedence");
      }

      public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
      {
         var value = (Precedence) untypedValue;
         if (value.Integer != null)
         {
            serializer.Serialize(writer, value.Integer.Value);
            return;
         }
         if (value.String != null)
         {
            serializer.Serialize(writer, value.String);
            return;
         }
         throw new Exception("Cannot marshal type Precedence");
      }

      public static readonly PrecedenceConverter Singleton = new PrecedenceConverter();
   }

   internal class ShortNameConverter : JsonConverter
   {
      public override bool CanConvert(Type t) => t == typeof(ShortName) || t == typeof(ShortName?);

      public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
      {
         switch (reader.TokenType)
         {
            case JsonToken.String:
            case JsonToken.Date:
               var stringValue = serializer.Deserialize<string>(reader);
               if (stringValue.Length >= 1)
               {
                  return new ShortName { String = stringValue };
               }
               break;

            case JsonToken.StartArray:
               var arrayValue = serializer.Deserialize<object[]>(reader);
               return new ShortName { AnythingArray = arrayValue };
         }
         throw new Exception("Cannot unmarshal type ShortName");
      }

      public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
      {
         var value = (ShortName) untypedValue;
         if (value.String != null)
         {
            if (value.String.Length >= 1)
            {
               serializer.Serialize(writer, value.String);
               return;
            }
         }
         if (value.AnythingArray != null)
         {
            serializer.Serialize(writer, value.AnythingArray);
            return;
         }
         throw new Exception("Cannot marshal type ShortName");
      }

      public static readonly ShortNameConverter Singleton = new ShortNameConverter();
   }

   internal class IceConverter : JsonConverter
   {
      public override bool CanConvert(Type t) => t == typeof(Ice) || t == typeof(Ice?);

      public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
      {
         switch (reader.TokenType)
         {
            case JsonToken.String:
            case JsonToken.Date:
               var stringValue = serializer.Deserialize<string>(reader);
               return new Ice { String = stringValue };

            case JsonToken.StartArray:
               var arrayValue = serializer.Deserialize<string[]>(reader);
               return new Ice { StringArray = arrayValue };
         }
         throw new Exception("Cannot unmarshal type Ice");
      }

      public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
      {
         var value = (Ice) untypedValue;
         if (value.String != null)
         {
            serializer.Serialize(writer, value.String);
            return;
         }
         if (value.StringArray != null)
         {
            serializer.Serialize(writer, value.StringArray);
            return;
         }
         throw new Exception("Cannot marshal type Ice");
      }

      public static readonly IceConverter Singleton = new IceConverter();
   }

   internal class ChoiceElementConverter : JsonConverter
   {
      public override bool CanConvert(Type t) => t == typeof(ChoiceElement) || t == typeof(ChoiceElement?);

      public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
      {
         switch (reader.TokenType)
         {
            case JsonToken.Null:
               return new ChoiceElement { };

            case JsonToken.Integer:
               var integerValue = serializer.Deserialize<long>(reader);
               return new ChoiceElement { Integer = integerValue };

            case JsonToken.Float:
               var doubleValue = serializer.Deserialize<double>(reader);
               return new ChoiceElement { Double = doubleValue };

            case JsonToken.Boolean:
               var boolValue = serializer.Deserialize<bool>(reader);
               return new ChoiceElement { Bool = boolValue };

            case JsonToken.String:
            case JsonToken.Date:
               var stringValue = serializer.Deserialize<string>(reader);
               return new ChoiceElement { String = stringValue };

            case JsonToken.StartObject:
               var objectValue = serializer.Deserialize<ChoiceClass>(reader);
               return new ChoiceElement { ChoiceClass = objectValue };

            case JsonToken.StartArray:
               var arrayValue = serializer.Deserialize<object[]>(reader);
               return new ChoiceElement { AnythingArray = arrayValue };
         }
         throw new Exception("Cannot unmarshal type ChoiceElement");
      }

      public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
      {
         var value = (ChoiceElement) untypedValue;
         if (value.IsNull)
         {
            serializer.Serialize(writer, null);
            return;
         }
         if (value.Integer != null)
         {
            serializer.Serialize(writer, value.Integer.Value);
            return;
         }
         if (value.Double != null)
         {
            serializer.Serialize(writer, value.Double.Value);
            return;
         }
         if (value.Bool != null)
         {
            serializer.Serialize(writer, value.Bool.Value);
            return;
         }
         if (value.String != null)
         {
            serializer.Serialize(writer, value.String);
            return;
         }
         if (value.AnythingArray != null)
         {
            serializer.Serialize(writer, value.AnythingArray);
            return;
         }
         if (value.ChoiceClass != null)
         {
            serializer.Serialize(writer, value.ChoiceClass);
            return;
         }
         throw new Exception("Cannot marshal type ChoiceElement");
      }

      public static readonly ChoiceElementConverter Singleton = new ChoiceElementConverter();
   }

   internal class GeneratorEnumConverter : JsonConverter
   {
      public override bool CanConvert(Type t) => t == typeof(GeneratorEnum) || t == typeof(GeneratorEnum?);

      public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
      {
         if (reader.TokenType == JsonToken.Null)
            return null;
         var value = serializer.Deserialize<string>(reader);
         switch (value)
         {
            case "casing":
               return GeneratorEnum.Casing;

            case "coalesce":
               return GeneratorEnum.Coalesce;

            case "constant":
               return GeneratorEnum.Constant;

            case "evaluate":
               return GeneratorEnum.Evaluate;

            case "guid":
               return GeneratorEnum.Guid;

            case "now":
               return GeneratorEnum.Now;

            case "port":
               return GeneratorEnum.Port;

            case "random":
               return GeneratorEnum.Random;

            case "regex":
               return GeneratorEnum.Regex;

            case "switch":
               return GeneratorEnum.Switch;
         }
         throw new Exception("Cannot unmarshal type GeneratorEnum");
      }

      public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
      {
         if (untypedValue == null)
         {
            serializer.Serialize(writer, null);
            return;
         }
         var value = (GeneratorEnum) untypedValue;
         switch (value)
         {
            case GeneratorEnum.Casing:
               serializer.Serialize(writer, "casing");
               return;

            case GeneratorEnum.Coalesce:
               serializer.Serialize(writer, "coalesce");
               return;

            case GeneratorEnum.Constant:
               serializer.Serialize(writer, "constant");
               return;

            case GeneratorEnum.Evaluate:
               serializer.Serialize(writer, "evaluate");
               return;

            case GeneratorEnum.Guid:
               serializer.Serialize(writer, "guid");
               return;

            case GeneratorEnum.Now:
               serializer.Serialize(writer, "now");
               return;

            case GeneratorEnum.Port:
               serializer.Serialize(writer, "port");
               return;

            case GeneratorEnum.Random:
               serializer.Serialize(writer, "random");
               return;

            case GeneratorEnum.Regex:
               serializer.Serialize(writer, "regex");
               return;

            case GeneratorEnum.Switch:
               serializer.Serialize(writer, "switch");
               return;
         }
         throw new Exception("Cannot marshal type GeneratorEnum");
      }

      public static readonly GeneratorEnumConverter Singleton = new GeneratorEnumConverter();
   }

   internal class OnlyIfUnionConverter : JsonConverter
   {
      public override bool CanConvert(Type t) => t == typeof(OnlyIfUnion) || t == typeof(OnlyIfUnion?);

      public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
      {
         switch (reader.TokenType)
         {
            case JsonToken.Null:
               return new OnlyIfUnion { };

            case JsonToken.Integer:
               var integerValue = serializer.Deserialize<long>(reader);
               return new OnlyIfUnion { Integer = integerValue };

            case JsonToken.Float:
               var doubleValue = serializer.Deserialize<double>(reader);
               return new OnlyIfUnion { Double = doubleValue };

            case JsonToken.Boolean:
               var boolValue = serializer.Deserialize<bool>(reader);
               return new OnlyIfUnion { Bool = boolValue };

            case JsonToken.String:
            case JsonToken.Date:
               var stringValue = serializer.Deserialize<string>(reader);
               return new OnlyIfUnion { String = stringValue };

            case JsonToken.StartObject:
               var objectValue = serializer.Deserialize<OnlyIfClass>(reader);
               return new OnlyIfUnion { OnlyIfClass = objectValue };

            case JsonToken.StartArray:
               var arrayValue = serializer.Deserialize<object[]>(reader);
               return new OnlyIfUnion { AnythingArray = arrayValue };
         }
         throw new Exception("Cannot unmarshal type OnlyIfUnion");
      }

      public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
      {
         var value = (OnlyIfUnion) untypedValue;
         if (value.IsNull)
         {
            serializer.Serialize(writer, null);
            return;
         }
         if (value.Integer != null)
         {
            serializer.Serialize(writer, value.Integer.Value);
            return;
         }
         if (value.Double != null)
         {
            serializer.Serialize(writer, value.Double.Value);
            return;
         }
         if (value.Bool != null)
         {
            serializer.Serialize(writer, value.Bool.Value);
            return;
         }
         if (value.String != null)
         {
            serializer.Serialize(writer, value.String);
            return;
         }
         if (value.AnythingArray != null)
         {
            serializer.Serialize(writer, value.AnythingArray);
            return;
         }
         if (value.OnlyIfClass != null)
         {
            serializer.Serialize(writer, value.OnlyIfClass);
            return;
         }
         throw new Exception("Cannot marshal type OnlyIfUnion");
      }

      public static readonly OnlyIfUnionConverter Singleton = new OnlyIfUnionConverter();
   }

   internal class ParametersUnionConverter : JsonConverter
   {
      public override bool CanConvert(Type t) => t == typeof(ParametersUnion) || t == typeof(ParametersUnion?);

      public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
      {
         switch (reader.TokenType)
         {
            case JsonToken.Null:
               return new ParametersUnion { };

            case JsonToken.Integer:
               var integerValue = serializer.Deserialize<long>(reader);
               return new ParametersUnion { Integer = integerValue };

            case JsonToken.Float:
               var doubleValue = serializer.Deserialize<double>(reader);
               return new ParametersUnion { Double = doubleValue };

            case JsonToken.Boolean:
               var boolValue = serializer.Deserialize<bool>(reader);
               return new ParametersUnion { Bool = boolValue };

            case JsonToken.String:
            case JsonToken.Date:
               var stringValue = serializer.Deserialize<string>(reader);
               return new ParametersUnion { String = stringValue };

            case JsonToken.StartObject:
               var objectValue = serializer.Deserialize<ParametersClass>(reader);
               return new ParametersUnion { ParametersClass = objectValue };

            case JsonToken.StartArray:
               var arrayValue = serializer.Deserialize<object[]>(reader);
               return new ParametersUnion { AnythingArray = arrayValue };
         }
         throw new Exception("Cannot unmarshal type ParametersUnion");
      }

      public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
      {
         var value = (ParametersUnion) untypedValue;
         if (value.IsNull)
         {
            serializer.Serialize(writer, null);
            return;
         }
         if (value.Integer != null)
         {
            serializer.Serialize(writer, value.Integer.Value);
            return;
         }
         if (value.Double != null)
         {
            serializer.Serialize(writer, value.Double.Value);
            return;
         }
         if (value.Bool != null)
         {
            serializer.Serialize(writer, value.Bool.Value);
            return;
         }
         if (value.String != null)
         {
            serializer.Serialize(writer, value.String);
            return;
         }
         if (value.AnythingArray != null)
         {
            serializer.Serialize(writer, value.AnythingArray);
            return;
         }
         if (value.ParametersClass != null)
         {
            serializer.Serialize(writer, value.ParametersClass);
            return;
         }
         throw new Exception("Cannot marshal type ParametersUnion");
      }

      public static readonly ParametersUnionConverter Singleton = new ParametersUnionConverter();
   }

   internal class StepElementConverter : JsonConverter
   {
      public override bool CanConvert(Type t) => t == typeof(StepElement) || t == typeof(StepElement?);

      public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
      {
         switch (reader.TokenType)
         {
            case JsonToken.Null:
               return new StepElement { };

            case JsonToken.Integer:
               var integerValue = serializer.Deserialize<long>(reader);
               return new StepElement { Integer = integerValue };

            case JsonToken.Float:
               var doubleValue = serializer.Deserialize<double>(reader);
               return new StepElement { Double = doubleValue };

            case JsonToken.Boolean:
               var boolValue = serializer.Deserialize<bool>(reader);
               return new StepElement { Bool = boolValue };

            case JsonToken.String:
            case JsonToken.Date:
               var stringValue = serializer.Deserialize<string>(reader);
               return new StepElement { String = stringValue };

            case JsonToken.StartObject:
               var objectValue = serializer.Deserialize<StepClass>(reader);
               return new StepElement { StepClass = objectValue };

            case JsonToken.StartArray:
               var arrayValue = serializer.Deserialize<object[]>(reader);
               return new StepElement { AnythingArray = arrayValue };
         }
         throw new Exception("Cannot unmarshal type StepElement");
      }

      public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
      {
         var value = (StepElement) untypedValue;
         if (value.IsNull)
         {
            serializer.Serialize(writer, null);
            return;
         }
         if (value.Integer != null)
         {
            serializer.Serialize(writer, value.Integer.Value);
            return;
         }
         if (value.Double != null)
         {
            serializer.Serialize(writer, value.Double.Value);
            return;
         }
         if (value.Bool != null)
         {
            serializer.Serialize(writer, value.Bool.Value);
            return;
         }
         if (value.String != null)
         {
            serializer.Serialize(writer, value.String);
            return;
         }
         if (value.AnythingArray != null)
         {
            serializer.Serialize(writer, value.AnythingArray);
            return;
         }
         if (value.StepClass != null)
         {
            serializer.Serialize(writer, value.StepClass);
            return;
         }
         throw new Exception("Cannot marshal type StepElement");
      }

      public static readonly StepElementConverter Singleton = new StepElementConverter();
   }

   internal class SymbolTypeConverter : JsonConverter
   {
      public override bool CanConvert(Type t) => t == typeof(SymbolType) || t == typeof(SymbolType?);

      public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
      {
         if (reader.TokenType == JsonToken.Null)
            return null;
         var value = serializer.Deserialize<string>(reader);
         switch (value)
         {
            case "bind":
               return SymbolType.Bind;

            case "computed":
               return SymbolType.Computed;

            case "derived":
               return SymbolType.Derived;

            case "generated":
               return SymbolType.Generated;

            case "parameter":
               return SymbolType.Parameter;
         }
         throw new Exception("Cannot unmarshal type SymbolType");
      }

      public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
      {
         if (untypedValue == null)
         {
            serializer.Serialize(writer, null);
            return;
         }
         var value = (SymbolType) untypedValue;
         switch (value)
         {
            case SymbolType.Bind:
               serializer.Serialize(writer, "bind");
               return;

            case SymbolType.Computed:
               serializer.Serialize(writer, "computed");
               return;

            case SymbolType.Derived:
               serializer.Serialize(writer, "derived");
               return;

            case SymbolType.Generated:
               serializer.Serialize(writer, "generated");
               return;

            case SymbolType.Parameter:
               serializer.Serialize(writer, "parameter");
               return;
         }
         throw new Exception("Cannot marshal type SymbolType");
      }

      public static readonly SymbolTypeConverter Singleton = new SymbolTypeConverter();
   }

   internal class TagsTypeConverter : JsonConverter
   {
      public override bool CanConvert(Type t) => t == typeof(TagsType) || t == typeof(TagsType?);

      public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
      {
         if (reader.TokenType == JsonToken.Null)
            return null;
         var value = serializer.Deserialize<string>(reader);
         switch (value)
         {
            case "item":
               return TagsType.Item;

            case "project":
               return TagsType.Project;
         }
         throw new Exception("Cannot unmarshal type TagsType");
      }

      public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
      {
         if (untypedValue == null)
         {
            serializer.Serialize(writer, null);
            return;
         }
         var value = (TagsType) untypedValue;
         switch (value)
         {
            case TagsType.Item:
               serializer.Serialize(writer, "item");
               return;

            case TagsType.Project:
               serializer.Serialize(writer, "project");
               return;
         }
         throw new Exception("Cannot marshal type TagsType");
      }

      public static readonly TagsTypeConverter Singleton = new TagsTypeConverter();
   }
}