namespace Meh.Commands;

/// <summary>
///     Extension methods for <see cref="ICommandDefinition" />.
/// </summary>
public static class CommandDefinitionExtensions
{
    /// <summary>
    ///     Converts a <see cref="ICommandDefinition" /> to a <see cref="Command" />.
    /// </summary>
    /// <param name="definition">
    ///     The command definition.
    /// </param>
    /// <returns>
    ///     The converted command.
    /// </returns>
    public static Command ToCommand(
        this ICommandDefinition definition)
    {
        Command command = new(
            definition.Name,
            definition.Description);

        foreach (var alias in definition.Aliases)
        {
            command.Aliases.Add(alias);
        }

        foreach (var argument in definition.Arguments)
        {
            command.Arguments.Add(argument);
        }

        foreach (var option in definition.Options)
        {
            command.Options.Add(option);
        }

        command.SetAction(
            async (parseResult, ct) =>
            {
                var result = await definition.ExecuteAsync(parseResult, ct);
                return result
                    ?? await command.Parse("--help")
                        .InvokeAsync(cancellationToken: ct);
            });
        command.Hidden = definition.IsHidden;
        
        return command;
    }
}
