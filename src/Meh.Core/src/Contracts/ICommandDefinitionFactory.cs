namespace Meh.Core;

/// <summary>
///     Abstraction for a factory that creates <see cref="ICommandDefinition"/>
///     instances. 
/// </summary>
/// <typeparam name="TDefinition">
///     A <see cref="ICommandDefinition"/> implementation the factory creates.
/// </typeparam>
public interface ICommandDefinitionFactory<TDefinition>
    where TDefinition : ICommandDefinition
{
    /// <summary>
    ///   Creates a new <see cref="ICommandDefinition"/> instance.
    /// </summary>
    TDefinition Create();
}
