namespace Meh.Tests.Commands.CleanPath;

public sealed class PathCleanerTests
    : IDisposable
{
    private const int MaxAgeInDays = 15;

    private static readonly DateTime Now = new(2026, 1, 31, 12, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime Expired = Now.AddDays(-MaxAgeInDays - 1);
    private static readonly DateTime Fresh = Now.AddDays(-1);

    private readonly TempDirectory _temp = new();
    private readonly StringWriter _output = new();

    [Fact]
    public void Clean_DeletesExpiredFiles_KeepsFreshFiles()
    {
        var expired = _temp.CreateFile("expired.txt", Expired);
        var fresh = _temp.CreateFile("fresh.txt", Fresh);

        var cleaner = CreateCleaner();
        cleaner.Clean(_temp.Info, allowEmptyDelete: false, CancellationToken.None);

        Assert.False(File.Exists(expired.FullName));
        Assert.True(File.Exists(fresh.FullName));
        Assert.Equal(0, cleaner.ErrorCount);
    }

    [Fact]
    public void Clean_DeletesNestedDirectories_EmptiedByClean()
    {
        _temp.CreateFile("a/b/expired.txt", Expired);
        _temp.CreateDirectory("a/empty");

        CreateCleaner().Clean(_temp.Info, allowEmptyDelete: false, CancellationToken.None);

        Assert.False(Directory.Exists(Path.Combine(_temp.Info.FullName, "a")));
    }

    [Fact]
    public void Clean_KeepsDirectories_WithFreshFiles()
    {
        _temp.CreateFile("a/expired.txt", Expired);
        var fresh = _temp.CreateFile("a/b/fresh.txt", Fresh);

        CreateCleaner().Clean(_temp.Info, allowEmptyDelete: false, CancellationToken.None);

        Assert.True(File.Exists(fresh.FullName));
        Assert.False(File.Exists(Path.Combine(_temp.Info.FullName, "a", "expired.txt")));
    }

    [Fact]
    public void Clean_NeverDeletesRoot_WhenNotAllowed()
    {
        _temp.CreateFile("expired.txt", Expired);

        var isDeleted = CreateCleaner()
            .Clean(_temp.Info, allowEmptyDelete: false, CancellationToken.None);

        Assert.False(isDeleted);
        Assert.True(Directory.Exists(_temp.Info.FullName));
    }

    [Fact]
    public void Clean_DryRun_DeletesNothing_ButReportsDeletes()
    {
        var expired = _temp.CreateFile("a/expired.txt", Expired);

        var isDeleted = CreateCleaner(isDryRun: true)
            .Clean(_temp.Info, allowEmptyDelete: true, CancellationToken.None);

        Assert.True(isDeleted);
        Assert.True(File.Exists(expired.FullName));
        Assert.Contains("Would delete", _output.ToString());
        Assert.DoesNotContain("Deleting", _output.ToString());
    }

    [Fact]
    public void Clean_DoesNotFollowLinkedDirectories()
    {
        using var outside = new TempDirectory();
        var outsideFile = outside.CreateFile("expired.txt", Expired);
        Directory.CreateSymbolicLink(
            Path.Combine(_temp.Info.FullName, "link"),
            outside.Info.FullName);

        CreateCleaner().Clean(_temp.Info, allowEmptyDelete: false, CancellationToken.None);

        Assert.True(File.Exists(outsideFile.FullName));
        Assert.True(Directory.Exists(Path.Combine(_temp.Info.FullName, "link")));
    }

    [Fact]
    public void Clean_Cancelled_Throws()
    {
        _temp.CreateFile("expired.txt", Expired);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Assert.Throws<OperationCanceledException>(
            () => CreateCleaner().Clean(_temp.Info, allowEmptyDelete: false, cts.Token));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _temp.Dispose();
        _output.Dispose();
    }

    private PathCleaner CreateCleaner(
        bool isDryRun = false)
    {
        var processor = new FileProcessorFactory(new FixedTimeProvider(Now))
            .Create(FileProcessorTypes.LastModifiedTime, MaxAgeInDays);
        var console = AnsiConsole.Create(new AnsiConsoleSettings
        {
            Ansi = AnsiSupport.No,
            ColorSystem = ColorSystemSupport.NoColors,
            Out = new AnsiConsoleOutput(_output),
        });

        return new PathCleaner(processor, console, isDryRun);
    }
}
