namespace Meh.Core;

/// <summary>
///     Common exit codes.
/// </summary>
[Flags]
public enum ExitCodes
{
    /// <summary>
    ///     The command completed successfully.
    /// </summary>
    Success = 0,
    
    /// <summary>
    ///     The command failed.
    /// </summary>
    Failure = 1,
}
