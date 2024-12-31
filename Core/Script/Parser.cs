namespace RingEngine.Core.Script;

using System;
using System.Collections.Generic;
using System.Linq;
using Sprache;
using static RingEngine.Core.Script.Branch;

public class ParserException : Exception
{
    private const int MaxLength = 50;

    public ParserException(string source)
        : base(source[..Math.Min(source.Length, MaxLength)]) { }

    public ParserException(string source, int line)
        : base($"Syntax Error at line {line}: \"{source[..Math.Min(source.Length, MaxLength)]}\"")
    { }

    public ParserException(Exception inner, int line)
        : base($"Syntax Error at line {line}: \"{inner.Message}\"") { }
}

/// <summary>
/// 占位符，用来让Parser跳过空白，直接丢弃即可
/// </summary>
internal class DummyBlock : IScriptBlock, IEquatable<DummyBlock>
{
    public override void Execute(VNRuntime runtime) => throw new NotImplementedException();

    // 一堆方便比较的方法
    public override bool Equals(object obj) => this.Equals(obj as DummyBlock);

    public bool Equals(DummyBlock other) => other is not null;

    public static bool operator ==(DummyBlock left, DummyBlock right) =>
        EqualityComparer<DummyBlock>.Default.Equals(left, right);

    public static bool operator !=(DummyBlock left, DummyBlock right) => !(left == right);

    public override int GetHashCode() => HashCode.Combine(0);
}

public static class Parser
{
    public static readonly Parser<CodeBlock> CodeBlockParser =
        from openTicks in Parse.String("```")
        from ident in Parse.AnyChar.Except(Parse.LineEnd).Many().Text()
        from lineEnd in Parse.LineEnd
        from code in Parse.AnyChar.Until(Parse.String("```")).Text()
        select new CodeBlock(ident.Trim(), code.Trim());

    public static readonly Parser<ShowChapterName> ShowChapterNameParser =
        // Markdown只支持四级标题，超过四个#怎么处理？
        from leading in Parse.Char('#').AtLeastOnce()
        from whitespace in Parse.WhiteSpace.AtLeastOnce()
        // Except(Parse.LineEnd)会吃掉换行符
        from name in Parse.AnyChar.Except(Parse.LineEnd).Many().Text().Token()
        select new ShowChapterName(name);

    public static readonly Parser<Say> SayParser =
        from character in Parse.CharExcept([':', '：']).Except(Parse.LineEnd).Many().Text().Token()
        from colon in Parse.Chars(':', '：').Then(_ => Parse.WhiteSpace.Optional())
        from openQuote in Parse.Char('"')
        from content in Parse.CharExcept('"').Many().Text()
        from closeQuote in Parse.Char('"')
        select new Say(character.Trim(), content);

    public static readonly Parser<PlayAudio> PlayAudioParser =
        from openTag in Parse.String("<audio src=\"")
        from path in Parse.CharExcept('"').Except(Parse.LineEnd).Many().Text()
        from closeTag in Parse.String("\"></audio>")
        select new PlayAudio(path);

    // Label目前只支持字面量
    public static readonly Parser<Label> LabelParser =
        from openStars in Parse.String("**")
        from label in Parse.CharExcept('*').Except(Parse.LineEnd).Many().Text().Token()
        from closeStars in Parse.String("**")
        select new Label(label);

    public static readonly Parser<BranchType> BranchHeadParser =
        from start in Parse.Char('|')
        from type in Parse.CharExcept('|').Many().Text()
        from split in Parse.Char('|')
        from _ in Parse.WhiteSpace.Many()
        from end in Parse.Char('|')
        from __ in Parse.WhiteSpace.Many().Optional()
        select type.Trim().ToLowerInvariant() switch
        {
            "vertical" or "竖排" => BranchType.Vertical,
            "horizontal" or "横排" => BranchType.Horizontal,
            _ => throw new ArgumentException($"Invalid branch type {type}"),
        };

    public static readonly Parser<BranchOption> BranchLineParser =
        from start in Parse.Char('|')
        from text in Parse.CharExcept('|').Except(Parse.LineEnd).Many().Text()
        from split in Parse.Char('|')
        from label in Parse.CharExcept('|').Except(Parse.LineEnd).Many().Text()
        from end in Parse.Char('|')
        from _ in Parse.WhiteSpace.Many().Optional()
        select new BranchOption(text.Trim(), label.Trim());

    public static readonly Parser<Branch> BranchParser =
        from type in BranchHeadParser
        from lines in BranchLineParser.Repeat(2, null)
        select new Branch(type, lines.Skip(1));

    public static readonly Parser<IScriptBlock> ScriptBlockParser = CodeBlockParser
        .Or<IScriptBlock>(ShowChapterNameParser)
        .Or(PlayAudioParser)
        .Or(LabelParser)
        .Or(BranchParser)
        .Or(BuiltInFunctionParser.BuiltInFunction)
        // SayParser一定要放在最后，因为没有固定的prefix
        .Or(SayParser);

    public static readonly Parser<IEnumerable<IScriptBlock>> ScriptParser = ScriptBlockParser
        .Or(Parse.WhiteSpace.AtLeastOnce().Return(new DummyBlock()))
        .Or(Parse.LineEnd.Return(new DummyBlock()))
        .Many();

    public static List<IScriptBlock> _Parse(string source)
    {
        try
        {
            var blocks = ScriptParser.Parse(source).Where(item => item is not DummyBlock).ToList();
            //var labels = new Dictionary<string, int>();

            //for (var i = 0; i < blocks.Count; i++)
            //{
            //    if (blocks[i] is Label label)
            //    {
            //        labels[label.Name] = i;
            //    }
            //}

            return blocks;
        }
        catch (ParseException ex)
        {
            throw new ParserException(ex.Message);
        }
    }
}

public static class BuiltInFunctionParser
{
    public static readonly Parser<string> InlineCodeBlockParser =
        from openTicks in Parse.String("`")
        from code in Parse.CharExcept('`').Except(Parse.LineEnd).Many().Text()
        from closeTicks in Parse.String("`")
        select code;

    public static readonly Parser<string> IdentifierParser = Parse
        .AnyChar.Except(Parse.WhiteSpace)
        .Many()
        .Text()
        .Token();

    public static readonly Parser<Show> ShowParser =
        from id in Parse.String("show").Then(_ => Parse.WhiteSpace.Many())
        from imgOpen in Parse.String("<img src=\"")
        from path in Parse.CharExcept('"').Many().Text()
        from imgClose in Parse
            .String("\"")
            .Then(_ => Parse.AnyChar.Except(Parse.String("/>")).Many())
            .Then(_ => Parse.String("/>"))
        from _ in Parse.String("as").Token()
        from name in IdentifierParser
        from __ in Parse.String("at").Token()
        from position in InlineCodeBlockParser.XOr(IdentifierParser)
        from withEffect in (
            from _ in Parse.String("with").Token()
            from effect in InlineCodeBlockParser.XOr(IdentifierParser)
            select effect
        ).Optional()
        select new Show(path, position, withEffect.GetOrDefault(), name);

    public static readonly Parser<Hide> HideParser =
        from _ in Parse.String("hide").Token()
        from name in IdentifierParser
        from withEffect in (
            from _ in Parse.String("with").Token()
            from effect in InlineCodeBlockParser.XOr(IdentifierParser)
            select effect
        ).Optional()
        select new Hide(name, withEffect.GetOrDefault());

    public static readonly Parser<ChangeBG> ChangeBGParser =
        from _ in Parse.String("changeBG").Token()
        from imgOpen in Parse.String("<img src=\"")
        from path in Parse.CharExcept('"').Many().Text()
        from imgClose in Parse.Char('"').Token().Then(_ => Parse.AnyChar.Until(Parse.String("/>")))
        from withEffect in (
            from _ in Parse.String("with").Token()
            from effect in InlineCodeBlockParser.XOr(IdentifierParser)
            select effect
        ).Optional()
        select new ChangeBG(path, withEffect.GetOrDefault());

    public static readonly Parser<ChangeScene> ChangeSceneParser =
        from _ in Parse.String("changeScene").Token()
        from imgOpen in Parse.String("<img src=\"")
        from path in Parse.CharExcept('"').Many().Text()
        from imgClose in Parse.AnyChar.Until(Parse.String("/>"))
        from withEffect in (
            from _ in Parse.String("with").Token()
            from effect in InlineCodeBlockParser.XOr(IdentifierParser)
            select effect
        ).Optional()
        select new ChangeScene(path, withEffect.GetOrDefault());

    public static readonly Parser<JumpToLabel> JumpToLabelParser =
        from _ in Parse.String("goto").Token()
        from label in InlineCodeBlockParser.XOr(IdentifierParser)
        select new JumpToLabel(label.Trim());

    public static readonly Parser<UIAnim> UIAnimParser =
        from _ in Parse.String("UIAnim").Token()
        from effect in InlineCodeBlockParser.XOr(IdentifierParser)
        select new UIAnim(effect);

    public static readonly Parser<StopAudio> StopAudioParser =
        from _ in Parse.String("stopAudio").Token()
        select new StopAudio();

    public static readonly Parser<ChangeScript> ChangeScriptParser =
        from _ in Parse.String("changeScript").Token()
        from displayName in Parse.Char('[').Then(_ => Parse.AnyChar.Until(Parse.Char(']')).Text())
        from scriptPath in Parse.Char('(').Then(_ => Parse.AnyChar.Until(Parse.Char(')')).Text())
        select new ChangeScript(scriptPath);

    public static readonly Parser<IScriptBlock> BuiltInFunction = ShowParser
        .Or<IScriptBlock>(HideParser)
        .Or(ChangeBGParser)
        .Or(ChangeSceneParser)
        .Or(JumpToLabelParser)
        .Or(UIAnimParser)
        .Or(StopAudioParser)
        .Or(ChangeScriptParser);
}
