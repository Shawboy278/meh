namespace Meh.Commands.CleanPath;

/// <summary>
///     Expires files based on a time attribute of the file.
/// </summary>
internal sealed class FileTimeAttributeProcessor(
        TimeSpan maxAge,
        TimeProvider timeProvider,
        Func<FileInfo, DateTime> getUtcTime)
    : IFileProcessor
{
    private readonly TimeSpan _maxAge = maxAge;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly Func<FileInfo, DateTime> _getUtcTime = getUtcTime;

    /// <inheritdoc />
    public bool IsExpired(
        FileInfo file)
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var age = now - _getUtcTime(file);

        return age >= _maxAge;
    }
}
