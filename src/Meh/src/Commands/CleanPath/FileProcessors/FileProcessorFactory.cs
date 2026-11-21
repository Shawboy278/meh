namespace Meh.Commands.CleanPath;

/// <summary>
///     Creates <see cref="IFileProcessor" /> instances.
/// </summary>
internal sealed class FileProcessorFactory(
    TimeProvider timeProvider)
{
    private readonly TimeProvider _timeProvider = timeProvider;

    /// <summary>
    ///     Creates a new <see cref="IFileProcessor" />.
    /// </summary>
    /// <param name="processorType">
    ///     Where the file's age is obtained from.
    /// </param>
    /// <param name="ageInDays">
    ///     The maximum age of a file in days.
    /// </param>
    /// <returns>
    ///     The file processor.
    /// </returns>
    public IFileProcessor Create(
        FileProcessorTypes processorType,
        int ageInDays)
    {
        var maxAge = TimeSpan.FromDays(ageInDays);

        return processorType switch
        {
            FileProcessorTypes.LastModifiedTime => new FileTimeAttributeProcessor(
                maxAge,
                _timeProvider,
                f => f.LastWriteTimeUtc),
            _ => throw new ArgumentOutOfRangeException(
                nameof(processorType),
                processorType,
                $"Support for '{processorType}' has not been configured."),
        };
    }
}
