using CreoFileCleaner;

var currentDirectory = Directory.GetCurrentDirectory();
var files = Directory.GetFiles(currentDirectory, "*.*", SearchOption.TopDirectoryOnly);
const char separator = '.';
var creoFileGroups = files.Select(filePath =>
{
    var parts = filePath.Split(separator);
    var lastPart = parts[^1];
    var secondLastPart = parts[^2];
    var isPartOfAssembly = secondLastPart == "prt" || secondLastPart == "asm";
    if (isPartOfAssembly && int.TryParse(lastPart, out int revisionId))
    {
        var partsWithoutRevisionId = parts.Take(parts.Length - 1);
        var fileNameWithoutRevisionId = string.Join(separator, partsWithoutRevisionId);
        return new CreoFileEntry(fileNameWithoutRevisionId, filePath, revisionId);
    }
    // When it's not .exe (.exe might be useful), delete it
    if (!lastPart.Equals("exe", StringComparison.InvariantCultureIgnoreCase))
    {
        File.Delete(filePath);
    }
    return new (filePath, filePath, -1);
}).Where(x => x.RevisionId >= 0)
.GroupBy(x => x.FileName);

foreach(var creoFileGroup in creoFileGroups)
{
    var maxRevisionId = creoFileGroup.Max(x => x.RevisionId);
    foreach(var creoFileEntry in creoFileGroup)
    {
        if (creoFileEntry.RevisionId < maxRevisionId)
        {
            File.Delete(creoFileEntry.FilePath);
        }
    }
}
