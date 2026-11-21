namespace Meh.Tests.Commands.CleanPath;

public sealed class FileTimeAttributeProcessorTests
{
    private static readonly DateTime Now = new(2026, 1, 31, 12, 0, 0, DateTimeKind.Utc);

    [Theory]
    [InlineData(14, false)]
    [InlineData(15, true)]
    [InlineData(16, true)]
    public void IsExpired_ComparesAgeAgainstMaxAge(
        int fileAgeInDays,
        bool expected)
    {
        var fileTime = Now.AddDays(-fileAgeInDays);
        var processor = new FileTimeAttributeProcessor(
            TimeSpan.FromDays(15),
            new FixedTimeProvider(Now),
            _ => fileTime);

        var isExpired = processor.IsExpired(new FileInfo("unused"));

        Assert.Equal(expected, isExpired);
    }
}
