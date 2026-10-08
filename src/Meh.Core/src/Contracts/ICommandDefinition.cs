namespace Meh.Core;

/// <summary>
///   Represents a command.
/// </summary>
public interface ICommandDefinition
{   
    /// <summary>
    ///   The command's name.
    /// </summary>
    string Name { get; }
    
    /// <summary>
    ///   The command's description.
    /// </summary>
    string Description { get; }

    /// <summary>
    ///   The command's aliases.
    /// </summary>
    string[] Aliases => [];

    /// <summary>
    ///   The arguments.
    /// </summary>
    /// <remarks>
    ///   The order is significant.   
    /// </remarks>
    Argument[] Arguments => [];

    /// <summary>
    ///   The options.
    /// </summary>
    Option[] Options => [];
    
    /// <summary>
    ///   Indicates if the command will be hidden.
    /// </summary>
    bool IsHidden => false;

    /// <summary>
    ///   Executes the command.
    /// </summary>
    /// <param name="parseResult">
    ///   The parsed command line arguments/options results.
    /// </param>
    /// <param name="ct">
    ///   The <see cref="CancellationToken"/>.
    /// </param>
    /// <returns>
    ///   The exit code indicating if the command ran successfully or not.
    /// </returns>
    /// <remarks>
    ///   Returning a <c>null</c> triggers the help command.
    /// </remarks>
    Task<int?> ExecuteAsync(
            ParseResult parseResult,
            CancellationToken ct)
        => Task.FromResult((int?)null);
}
