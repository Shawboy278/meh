namespace Meh.Commands.Git;

/// <summary>
///     The root command for Git actions.
/// </summary>
public sealed class GitDefinition
    : ICommandDefinition
{
    /// <inheritdoc />
    public string Name => "git";

    /// <inheritdoc />
    public string Description => "Custom Git actions.";
}
