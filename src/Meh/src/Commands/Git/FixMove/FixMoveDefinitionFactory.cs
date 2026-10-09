namespace Meh.Commands.Git;

using static Meh.Commands.Git.CommonGitOptions;

/// <summary>
///     Creates new <see cref="FixMoveDefinition" /> instances.
/// </summary>
internal sealed class FixMoveDefinitionFactory
    : ICommandDefinitionFactory<FixMoveDefinition>
{
    /// <inheritdoc />
    public FixMoveDefinition Create()
    {
        var originPathArg = new Argument<string?>("origin-path")
        {
            Description = "Where the file originated.",
            DefaultValueFactory = _ => null,
        };
        var targetPathArg = new Argument<string?>("target-path")
        {
            Description = "Where the file resides.",
            DefaultValueFactory = _ => null,
        };
        
        return new(originPathArg, targetPathArg, RepositoryRootOption);
    }
}
