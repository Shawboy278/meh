namespace Meh.Tests.Commands.CleanPath;

public sealed class CleanPathDefinitionTests
    : IDisposable
{
    private readonly TempDirectory _temp = new();
    private readonly Command _command = new CleanPathDefinitionFactory()
        .Create()
        .ToCommand();

    [Fact]
    public void Parse_Defaults()
    {
        var result = _command.Parse([_temp.Info.FullName]);

        Assert.Empty(result.Errors);
        Assert.Equal(15, result.GetValue<int>("--age"));
        Assert.Equal(
            FileProcessorTypes.LastModifiedTime,
            result.GetValue<FileProcessorTypes>("--source"));
        Assert.False(result.GetValue<bool>("--dry-run"));
    }

    [Fact]
    public void Parse_AllOptions()
    {
        var result = _command.Parse(
            [_temp.Info.FullName, "-a", "90", "-s", "lastmodifiedtime", "-n"]);

        Assert.Empty(result.Errors);
        Assert.Equal(90, result.GetValue<int>("--age"));
        Assert.True(result.GetValue<bool>("--dry-run"));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    public void Parse_NonPositiveAge_Errors(
        string age)
    {
        var result = _command.Parse([_temp.Info.FullName, "--age", age]);

        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Parse_MissingDirectory_Errors()
    {
        var missing = Path.Combine(_temp.Info.FullName, "missing");

        var result = _command.Parse([missing]);

        Assert.NotEmpty(result.Errors);
    }

    [Fact]
    public void Parse_UnknownSource_Errors()
    {
        var result = _command.Parse([_temp.Info.FullName, "-s", "nope"]);

        Assert.NotEmpty(result.Errors);
    }

    /// <inheritdoc />
    public void Dispose() => _temp.Dispose();
}
