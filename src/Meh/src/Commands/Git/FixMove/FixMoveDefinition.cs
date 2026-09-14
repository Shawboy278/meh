namespace Meh.Commands.Git;

/// <summary>
/// Resolves an untracked file move.
/// </summary>
public sealed class FixMoveDefinition(
        Argument<string?> originPathArg,
        Argument<string?> targetPathArg,
        Option<string> reposPathOption)
    : ICommandDefinition, IHasParentDefinition<GitDefinition>
{
    private readonly Argument<string?> _originPathArg = targetPathArg;
    private readonly Argument<string?> _targetPathArg = targetPathArg;
    private readonly Option<string> _reposPathOption = reposPathOption;
    
    /// <inheritdoc />
    public string Name => "fix-mv";

    /// <inheritdoc />
    public string Description => "Corrects file moves performed outside of Git tooling.";

    /// <inheritdoc />
    public string[] Aliases { get; } = [ "fix-move" ];
    
    /// <inheritdoc />
    public Argument[] Arguments { get; } = [ originPathArg, targetPathArg ];
    
    /// <inheritdoc />
    public Option[] Options { get; } = [ reposPathOption ];
    
    /// <inheritdoc />
    public Task<int> ExecuteAsync(
        ParseResult parseResult,
        CancellationToken ct)
    {
        try
        {
            var originFile = parseResult.GetValue(_originPathArg);
            var targetFile = parseResult.GetValue(_targetPathArg);
            var reposRoot = parseResult.GetValue(_reposPathOption)
                            ?? FindReposRoot(originFile ?? targetFile)
                            ?? throw new ArgumentException("Could not determine repository's root path.");
        
            using (var repos = new Repository(reposRoot))
            {
                originFile ??= GetSourceFile(repos);
                targetFile ??= GetTargetFile(repos);

                // Masking the `destFile`
                var maskedTargetFile = MaskTargetFile(targetFile);
                AnsiConsole.WriteLine($"Renamed target file => '{maskedTargetFile}'");

                // Restoring deleted file and moving it to the target location
                repos.RestoreFile(originFile);
                GitCommands.Move(repos, originFile, targetFile);

                // Unmasking original `targetFile` 
                File.Move(maskedTargetFile, targetFile, true);
            }

            return Task.FromResult(0);
        }
        catch (Exception ex)
        {
            // TODO: Handle better
            AnsiConsole.WriteLine(ex.Message);
            if (ex.StackTrace is not null)
            {
                AnsiConsole.WriteLine(ex.StackTrace);                
            }
            throw;
        }
    }

    private static string GetSourceFile(
        Repository repos)
    {
        var missingFiles = repos.GetTrackedDeletedFiles();
        if (missingFiles.Count == 0)
        {
            throw new ArgumentException(
                "Could not determine any files from the current state.");
        }

        var originFile = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .PageSize(10)
                    .Title("Select an origin file")
                    .AddChoices(missingFiles))
            ?? throw new NotImplementedException(
                "The origin file could not be determined.");;

        AnsiConsole.WriteLine($"Origin file => '{originFile}'");

        return originFile;
    }

    private static string GetTargetFile(
        Repository repos)
    {
        var untrackedFiles = repos.GetUntrackFiles();
        if (untrackedFiles.Count == 0)
        {
            throw new ArgumentException(
                "Could not determine any untracked files from the current state.");
        }

        var targetFile = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .PageSize(10)
                .Title("Select the target file")
                .AddChoices(untrackedFiles));
        
        return targetFile
            ?? throw new NotImplementedException("The target file could not be determined.");
    }

    private static string MaskTargetFile(
        string targetFile)
    {
        var maskedName = Path.GetRandomFileName();
        var targetPath = new FileInfo(targetFile)
            .DirectoryName;
        var maskedFile = Path.Combine(targetPath!, maskedName);

        File.Move(targetFile, maskedFile);
        return maskedFile;
    }

    private static bool TryFindWorktreeReposRoot(
        DirectoryInfo dirInfo,
        out DirectoryInfo? worktreeReposRoot)
    {
        worktreeReposRoot = null;

        var dotGit = dirInfo.GetFiles(".git")
            .SingleOrDefault();
        if (dotGit != null)
        {
            using var readStream = dotGit.OpenRead();
            using TextReader textReader = new StreamReader(readStream);

            while (textReader.Peek() != -1)
            {
                var line = textReader.ReadLine();
                if (line?.StartsWith("gitdir:") ?? false)
                {
                    worktreeReposRoot = new(line[8..]);
                    break;
                }
            }
        }
        
        return worktreeReposRoot != null;
    }

    private static string? FindReposRoot(
        string? reposFile = null)
    {
        var dir = reposFile is not null
            ? new FileInfo(reposFile)
                    .Directory
                    ?.FullName
                ?? throw new ArgumentException(
                    $"Could not obtain directory name for '{reposFile}'.")
            : Environment.CurrentDirectory;

        DirectoryInfo? dirInfo = new(dir);
        while (dirInfo is not null)
        {
            if (dirInfo.GetDirectories(".git").Length != 0)
            {
                break;
            }

            if (TryFindWorktreeReposRoot(dirInfo, out var worktreeReposRoot))
            {
                dirInfo = worktreeReposRoot;
                break;
            }

            dirInfo = dirInfo.Parent;
        }

        return dirInfo?.FullName;
    }
}
