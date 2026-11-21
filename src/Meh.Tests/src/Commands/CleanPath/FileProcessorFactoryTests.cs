namespace Meh.Tests.Commands.CleanPath;

public sealed class FileProcessorFactoryTests
{
    private static readonly DateTime Now = new(2026, 1, 31, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_LastModifiedTime_UsesLastWriteTime()
    {
        using var temp = new TempDirectory();
        var oldFile = temp.CreateFile("old.txt", Now.AddDays(-2));
        var newFile = temp.CreateFile("new.txt", Now);
        var factory = new FileProcessorFactory(new FixedTimeProvider(Now));

        var processor = factory.Create(FileProcessorTypes.LastModifiedTime, 1);

        Assert.True(processor.IsExpired(oldFile));
        Assert.False(processor.IsExpired(newFile));
    }

    [Fact]
    public void Create_UnknownType_Throws()
    {
        var factory = new FileProcessorFactory(new FixedTimeProvider(Now));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => factory.Create((FileProcessorTypes)int.MaxValue, 1));
    }
}
