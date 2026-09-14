namespace Meh;

/// <summary>
///   Contains the entry method for the app.
/// </summary>
public static class Program
{
    /// <summary>
    ///   The initial point of execution for the app.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    public static async Task<int> Main(
        string[] args)
    {   
        var serviceProvider = ServiceProviderFactory.Create();
        var rootCommand = serviceProvider.GetRequiredService<RootCommand>();

        return await rootCommand.Parse(args)
            .InvokeAsync();
    }
}
