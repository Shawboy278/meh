namespace Meh.Commands.CleanPath;

/// <summary>
///     Defines the <c>clean-path</c> command.
/// </summary>
internal sealed class CleanPathDefinition(
        Argument<DirectoryInfo> targetPathArg,
        Option<int> ageOption,
        Option<FileProcessorTypes> sourceOption,
        Option<bool> dryRunOption,
        FileProcessorFactory fileProcessorFactory)
    : ICommandDefinition
{
    private readonly Argument<DirectoryInfo> _targetPathArg = targetPathArg;
    private readonly Option<int> _ageOption = ageOption;
    private readonly Option<FileProcessorTypes> _sourceOption = sourceOption;
    private readonly Option<bool> _dryRunOption = dryRunOption;
    private readonly FileProcessorFactory _fileProcessorFactory = fileProcessorFactory;

    /// <inheritdoc />
    public string Name => "clean-path";

    /// <inheritdoc />
    public string Description => "Removes old files and empty folders from a given path.";

    /// <inheritdoc />
    public string[] Aliases => ["clean-dir"];

    /// <inheritdoc />
    public Argument[] Arguments => [_targetPathArg];

    /// <inheritdoc />
    public Option[] Options => [_ageOption, _sourceOption, _dryRunOption];

    /// <inheritdoc />
    public Task<ExitCodes?> ExecuteAsync(
        ParseResult parseResult,
        CancellationToken ct)
    {
        try
        {
            var targetDir = parseResult.GetRequiredValue(_targetPathArg);
            var age = parseResult.GetValue(_ageOption);
            var source = parseResult.GetValue(_sourceOption);
            var isDryRun = parseResult.GetValue(_dryRunOption);

            var dryRunNote = isDryRun ? " (dry run)" : string.Empty;
            AnsiConsole.MarkupLineInterpolated(
                $"Let's clean '{targetDir.FullName}'{dryRunNote}!!!");

            var fileProcessor = _fileProcessorFactory.Create(source, age);
            var cleaner = new PathCleaner(fileProcessor, AnsiConsole.Console, isDryRun);
            cleaner.Clean(targetDir, allowEmptyDelete: false, ct);

            return Task.FromResult<ExitCodes?>(
                cleaner.ErrorCount == 0 ? ExitCodes.Success : ExitCodes.Failure);
        }
        catch (Exception ex)
        {
            // TODO: Handle better?
            AnsiConsole.WriteException(ex);

            return Task.FromResult<ExitCodes?>(ExitCodes.Failure);
        }
    }
}
