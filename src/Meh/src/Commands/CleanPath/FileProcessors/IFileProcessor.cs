namespace Meh.Commands.CleanPath;

/// <summary>
///     Determines if a file has expired and should be removed.
/// </summary>
internal interface IFileProcessor
{
    /// <summary>
    ///     Indicates if the file has expired.
    /// </summary>
    /// <param name="file">
    ///     The file to examine.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the file has expired; otherwise <c>false</c>.
    /// </returns>
    bool IsExpired(
        FileInfo file);
}
