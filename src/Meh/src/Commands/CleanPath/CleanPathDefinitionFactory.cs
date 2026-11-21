namespace Meh.Commands.CleanPath;

/// <summary>
///     Creates new instances of <see cref="CleanPathDefinition" />.
/// </summary>
internal sealed class CleanPathDefinitionFactory
    : ICommandDefinitionFactory<CleanPathDefinition>
{
    private const int DefaultAgeInDays = 15;

    /// <inheritdoc />
    public CleanPathDefinition Create()
    {
        var targetPathArg = new Argument<DirectoryInfo>("path")
        {
            Description = "The path to clean.",
        }.AcceptExistingOnly();

        var ageOption = new Option<int>("--age", "-a")
        {
            Description = "The maximum age of a file in days.",
            DefaultValueFactory = _ => DefaultAgeInDays,
        };
        ageOption.Validators.Add(result =>
        {
            var age = result.GetValueOrDefault<int>();
            if (age <= 0)
            {
                result.AddError($"The age '{age}' must be greater than 0.");
            }
        });

        var sourceOption = new Option<FileProcessorTypes>("--source", "-s")
        {
            Description = "Where a file's age is obtained from.",
            DefaultValueFactory = _ => FileProcessorTypes.LastModifiedTime,
        };

        var dryRunOption = new Option<bool>("--dry-run", "-n")
        {
            Description = "Lists what would be deleted without deleting anything.",
        };

        return new(
            targetPathArg,
            ageOption,
            sourceOption,
            dryRunOption,
            new FileProcessorFactory(TimeProvider.System));
    }
}
