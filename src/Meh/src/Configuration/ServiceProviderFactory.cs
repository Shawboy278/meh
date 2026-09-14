using Meh;

/// <summary>
///   Builds the application's <see cref="IServiceProvider"/>.
/// </summary>
internal static class ServiceProviderFactory
{
    private static readonly Type CommandDefinitionType = typeof(ICommandDefinition);
    
    /// <summary>
    ///   Creates a configured <see cref="IServiceProvider"/> for the application.
    /// </summary>
    public static IServiceProvider Create()
    {
        var services = new ServiceCollection();

        services.AddSingleton(typeof(MehCommandFactory));
        services.AddSingleton(
            typeof(RootCommand),
            sp => sp.GetRequiredService<MehCommandFactory>()
                .Create());
        RegisterCommandDefinitions(services, Assembly.GetExecutingAssembly());

        return services.BuildServiceProvider();
    }

    private static void RegisterCommandDefinitions(
        IServiceCollection services,
        Assembly assembly)
    {
        var allTypes = assembly.GetTypes();

        var definitionTypes = allTypes
            .Where(t => t is { IsAbstract: false, IsInterface: false }
                        && t.IsAssignableTo(CommandDefinitionType));

        foreach (var definitionType in definitionTypes)
        {
            var factoryInterfaceType = typeof(ICommandDefinitionFactory<>)
                .MakeGenericType(definitionType);
            var factoryType = allTypes.FirstOrDefault(
                t => t is { IsAbstract: false, IsInterface: false }
                     && t.IsAssignableTo(factoryInterfaceType));
            if (factoryType is null)
            {
                services.AddSingleton(definitionType);
                services.AddSingleton(
                    CommandDefinitionType,
                    sp => sp.GetRequiredService(definitionType));
                continue;   
            }

            // ICommandDefinitionFactory<TDef> → ConcreteFactory
            services.AddSingleton(factoryInterfaceType, factoryType);

            // TDef → resolved via the factory's Create()
            var createMethod = factoryInterfaceType
                .GetMethod(nameof(ICommandDefinitionFactory<ICommandDefinition>.Create))!;
            services.AddSingleton(definitionType, sp =>
            {
                var factory = sp.GetRequiredService(factoryInterfaceType);
                return createMethod.Invoke(factory, null)!;
            });

            // ICommandDefinition → TDef (supports GetServices<ICommandDefinition>() enumeration)
            services.AddSingleton(
                CommandDefinitionType,
                sp => sp.GetRequiredService(definitionType));
        }
    }
}
