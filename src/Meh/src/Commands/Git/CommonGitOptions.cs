namespace Meh.Commands.Git;

/// <summary>
/// 
/// </summary>
internal static class CommonGitOptions
{
    //
    internal static Option<string> RepositoryRootOption { get; }
        = new(
            "--repository-root",
            "--repos-root",
            "-r");
}
