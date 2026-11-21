namespace Meh.Commands.CleanPath;

/// <summary>
///     Removes expired files and empty directories from a directory tree.
/// </summary>
/// <remarks>
///     Directories are processed depth first so a directory emptied by the
///     clean can be removed as well. Symbolic links and junctions to
///     directories are never followed.
/// </remarks>
internal sealed class PathCleaner(
    IFileProcessor fileProcessor,
    IAnsiConsole console,
    bool isDryRun = false)
{
    private readonly IFileProcessor _fileProcessor = fileProcessor;
    private readonly IAnsiConsole _console = console;
    private readonly bool _isDryRun = isDryRun;

    /// <summary>
    ///     The number of files and directories that could not be processed.
    /// </summary>
    public int ErrorCount { get; private set; }

    /// <summary>
    ///     Cleans the directory and its subdirectories.
    /// </summary>
    /// <param name="dir">
    ///     The directory to clean.
    /// </param>
    /// <param name="allowEmptyDelete">
    ///     Indicates if <paramref name="dir" /> may be removed once empty.
    /// </param>
    /// <param name="ct">
    ///     The <see cref="CancellationToken" />.
    /// </param>
    /// <returns>
    ///     <c>true</c> if <paramref name="dir" /> was (or, for a dry run, would
    ///     be) removed; otherwise <c>false</c>.
    /// </returns>
    public bool Clean(
        DirectoryInfo dir,
        bool allowEmptyDelete,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        DirectoryInfo[] subDirs;
        FileInfo[] files;
        try
        {
            subDirs = dir.GetDirectories();
            files = dir.GetFiles();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            ReportError(dir, ex);
            return false;
        }

        var remaining = 0;
        foreach (var subDir in subDirs)
        {
            if (subDir.Attributes.HasFlag(FileAttributes.ReparsePoint))
            {
                _console.MarkupLineInterpolated(
                    $"[grey]Skipping linked dir '{subDir.FullName}'[/]");
                remaining++;
                continue;
            }

            if (!Clean(subDir, true, ct))
            {
                remaining++;
            }
        }

        _console.MarkupLineInterpolated($"Examining files of '{dir.FullName}'");
        foreach (var file in files)
        {
            ct.ThrowIfCancellationRequested();

            if (!_fileProcessor.IsExpired(file)
                || !TryDelete(file, $"  {DeleteVerb} file"))
            {
                remaining++;
            }
        }

        return allowEmptyDelete
               && remaining == 0
               && TryDelete(dir, DeleteVerb + " dir");
    }

    private string DeleteVerb => _isDryRun ? "Would delete" : "Deleting";

    private bool TryDelete(
        FileSystemInfo info,
        string message)
    {
        _console.MarkupLineInterpolated($"[yellow]{message}[/] '{info.FullName}'");
        if (_isDryRun)
        {
            return true;
        }

        try
        {
            info.Delete();
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            ReportError(info, ex);
            return false;
        }
    }

    private void ReportError(
        FileSystemInfo info,
        Exception ex)
    {
        ErrorCount++;
        _console.MarkupLineInterpolated(
            $"[red]Failed to process '{info.FullName}': {ex.Message}[/]");
    }
}
