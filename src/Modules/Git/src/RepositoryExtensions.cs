namespace Meh.Modules.Git;

/// <summary>
///     Contains extension methods for the <see cref="Repository"/> class.
/// </summary>
public static class RepositoryExtensions
{
    /// <summary>
    ///     List of files in the working directory that are not indexed.
    /// </summary>
    /// <param name="repos">
    ///     The repository.
    /// </param>
    /// <returns>
    ///     The untracked files.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     The repos value is null.
    /// </exception>
    public static IReadOnlyCollection<string> GetUntrackFiles(
            this Repository repos)
        => repos?.RetrieveStatus()
            .Untracked
            .Select(se => se.FilePath)
            .ToArray()
            ?? throw new ArgumentNullException(nameof(repos));

    /// <summary>
    ///     Indicates if the file is tracked or not.
    /// </summary>
    /// <param name="repos">
    ///     The repository.
    /// </param>
    /// <param name="file">
    ///     The file in question.
    /// </param>
    /// <returns>
    ///     True if the file is tracked, false otherwise.
    /// </returns>
    public static bool IsFileTracked(
            this Repository repos,
            string file)
        => repos.Head.Tip.Tree[file] != null;

    /// <summary>
    ///     Restores a tracked file.
    /// </summary>
    /// <param name="repos">
    ///     The repository.
    /// </param>
    /// <param name="file">
    ///     The file to restore.
    /// </param>
    /// <returns>
    ///     The repository.
    /// </returns>
    /// <exception cref="FileNotFoundException">
    ///     The file was not tracked.
    /// </exception>
    public static Repository RestoreFile(
        this Repository repos,
        string file)
    {
        if (!repos.IsFileTracked(file))
        {
            throw new FileNotFoundException(
                $"The file '{file}' is not being tracked.");
        }

        var options = new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force };
        repos.CheckoutPaths(repos.Head.FriendlyName, new[] { file }, options);

        return repos;
    }

    /// <summary>
    ///     List of files in the working directory that are tracked and deleted.
    /// </summary>
    /// <param name="repos">
    ///     The repository.
    /// </param>
    /// <returns>
    ///     The deleted files.
    /// </returns>
    public static IReadOnlyCollection<string> GetTrackedDeletedFiles(
        this Repository repos)
    {
        var status = repos.RetrieveStatus(
            new StatusOptions
            {
                IncludeUntracked = false,
            });

        // Should handle deleted staged and unstaged (tracked!) files
        return status
            .Where(e =>
                (e.State & FileStatus.DeletedFromWorkdir) != 0
                || (e.State & FileStatus.DeletedFromIndex) != 0)
            .Select(e => e.FilePath)
            .ToArray();
    }
}
