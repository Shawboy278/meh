namespace Meh.Commands.Git;

/// <summary>
///     Common options for Git commands.
/// </summary>
internal static class CommonGitOptions
{
    /// <summary>
    ///     The repository root option.
    /// </summary>
    internal static Option<string> RepositoryRootOption { get; }
        = new(
            "--repository-root",
            "--repos-root",
            "-r");
}
