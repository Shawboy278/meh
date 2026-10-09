namespace Meh.Commands;

/// <summary>
///     Implements the root meh command.
/// </summary>
internal sealed class MehCommandFactory(
    IServiceProvider serviceProvider)
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    
    /// <summary>
    ///     Creates a new instance of <see cref="RootCommand"/> configured as the base meh command.
    /// </summary>
    /// <returns>
    ///     The root command.
    /// </returns>
    internal RootCommand Create()
    {
        var commandTypeMapping = _serviceProvider.GetServices<ICommandDefinition>()
            .ToDictionary(
                keySelector: s => s.GetType(),
                elementSelector: s => s.ToCommand());
        var subCommandCollection = commandTypeMapping.Keys
            .Where(IsSubCommand);
        foreach (var subCommand in subCommandCollection)
        {
            var interfaceCollection = subCommand.GetInterfaces()
                .Where(i => i.IsGenericType
                            && i.GetGenericTypeDefinition() == typeof(IHasParentDefinition<>));
            foreach (var @interface in interfaceCollection)
            {
                var parentDefinition = @interface.GetGenericArguments()[0];
                commandTypeMapping[parentDefinition].Subcommands
                    .Add(commandTypeMapping[subCommand]);
            }
        }
        
        var rootCommand = new RootCommand(
            "My Extendable Hack (meh)");
        var baseCommandCollection = commandTypeMapping.Keys
            .Where(t => !IsSubCommand(t));
        foreach (var baseCommand in baseCommandCollection)
        {
            rootCommand.Subcommands.Add(commandTypeMapping[baseCommand]);
        }

        return rootCommand;
    }

    private static bool IsSubCommand(
        Type t)
    {
        return t.GetInterfaces()
            .Any(i => i.IsGenericType
                      && i.GetGenericTypeDefinition() == typeof(IHasParentDefinition<>)); 
    }
}
