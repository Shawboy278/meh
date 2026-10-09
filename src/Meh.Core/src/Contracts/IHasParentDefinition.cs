namespace Meh.Core;

/// <summary>
///     Indicates the parent definition.
/// </summary>
/// <typeparam name="TParentDefinition">
///     The parent <see cref="ICommandDefinition"/> implementation.
/// </typeparam>
public interface IHasParentDefinition<TParentDefinition>
    where TParentDefinition : ICommandDefinition;

