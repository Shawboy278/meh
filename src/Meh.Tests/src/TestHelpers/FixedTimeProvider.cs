namespace Meh.Tests.TestHelpers;

/// <summary>
///     A <see cref="TimeProvider" /> that always returns the same time.
/// </summary>
internal sealed class FixedTimeProvider(
        DateTimeOffset utcNow)
    : TimeProvider
{
    private readonly DateTimeOffset _utcNow = utcNow;

    /// <inheritdoc />
    public override DateTimeOffset GetUtcNow() => _utcNow;
}
