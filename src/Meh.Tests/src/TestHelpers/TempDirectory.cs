namespace Meh.Tests.TestHelpers;

/// <summary>
///     A uniquely named directory that's removed when disposed.
/// </summary>
internal sealed class TempDirectory
    : IDisposable
{
    public TempDirectory()
    {
        Info = Directory.CreateDirectory(
            Path.Combine(Path.GetTempPath(), "meh-tests", Path.GetRandomFileName()));
    }

    /// <summary>
    ///     The temporary directory.
    /// </summary>
    public DirectoryInfo Info { get; }

    /// <summary>
    ///     Creates a file relative to the temporary directory.
    /// </summary>
    /// <param name="relativePath">
    ///     The file's path relative to the temporary directory.
    /// </param>
    /// <param name="lastWriteTimeUtc">
    ///     The file's last write time.
    /// </param>
    /// <returns>
    ///     The created file.
    /// </returns>
    public FileInfo CreateFile(
        string relativePath,
        DateTime lastWriteTimeUtc)
    {
        var file = new FileInfo(Path.Combine(Info.FullName, relativePath));
        file.Directory!.Create();
        File.WriteAllText(file.FullName, string.Empty);
        File.SetLastWriteTimeUtc(file.FullName, lastWriteTimeUtc);

        return file;
    }

    /// <summary>
    ///     Creates a directory relative to the temporary directory.
    /// </summary>
    /// <param name="relativePath">
    ///     The directory's path relative to the temporary directory.
    /// </param>
    /// <returns>
    ///     The created directory.
    /// </returns>
    public DirectoryInfo CreateDirectory(
        string relativePath)
    {
        return Directory.CreateDirectory(Path.Combine(Info.FullName, relativePath));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (Info.Exists)
        {
            Info.Delete(recursive: true);
        }
    }
}
