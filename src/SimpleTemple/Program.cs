using CommandLine;
using SimpleTemple;
using System.Text.Json;

string[] ignoreDirs = [".vs", ".idea", "Logs", "bin", "obj", "TestResults"];
string[] ignoreFiles = [".editorconfig", ".env"];
string[] ignoreTextReplaceFiles = ["confy.db"];

var templeJsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "temple.json");
if (File.Exists(templeJsonFilePath))
{
    var stream = File.OpenRead(templeJsonFilePath);
    var jsonOptions = await JsonSerializer.DeserializeAsync(stream, AppJsonSerializerContext.Default.JsonOptions);

    string sourceDir = jsonOptions!.SourceDir;
    string generateToDir = jsonOptions.GenerateToDir;
    // Clean up the GenerateTo dir first
    CleanUp(generateToDir);
    CopyDirectory(sourceDir, generateToDir, recursive: true, ignoreDirs);

    var commonReplacers = jsonOptions.CommonReplacers;
    IEnumerable<Replacer> dirNameReplacers = commonReplacers
            .Concat(jsonOptions.DirNameReplacers);

    ReplaceDirName(generateToDir, ProcessReplacers(dirNameReplacers));

    IEnumerable<Replacer> fileNameReplacers = commonReplacers
            .Concat(jsonOptions.FileNameReplacers)
            .Distinct();

    IEnumerable<Replacer> textReplacers = commonReplacers
                 .Concat(jsonOptions.TextReplacers)
                 .Distinct();

    await ReplaceFileNameAndTextAsync(
        generateToDir,
        ProcessReplacers(fileNameReplacers),
        ProcessReplacers(textReplacers)
    );

    return;
}

await Parser.Default.ParseArguments<CommandLineOptions>(args)
    .WithParsedAsync(async options =>
    {
        string sourceDir = options.SourceDir;
        string generateToDir = options.GenerateToDir;
        // Clean up the GenerateTo dir first
        CleanUp(generateToDir);
        CopyDirectory(sourceDir, generateToDir, recursive: true, ignoreDirs);

        List<string> commonReplacers = options.CommonReplacers;
        IEnumerable<string> dirNameReplacers = commonReplacers
            .Concat(options.DirNameReplacers);
        
        ReplaceDirName(generateToDir, ToReplacerObjects(dirNameReplacers));

        IEnumerable<string> fileNameReplacers = commonReplacers
            .Concat(options.FileNameReplacers)
            .Distinct();
        
        IEnumerable<string> textReplacers = commonReplacers
                     .Concat(options.TextReplacers)
                     .Distinct();

        await ReplaceFileNameAndTextAsync(
            generateToDir,
            ToReplacerObjects(fileNameReplacers),
            ToReplacerObjects(textReplacers)
        );
    });
return;

void ReplaceDirName(string rootDir, Replacer[] replacers)
{
    string[] subDirs = Directory.GetDirectories(rootDir, "*", SearchOption.TopDirectoryOnly);
    List<string> updatedSubDirs = [];
    foreach (string subDir in subDirs)
    {
        DirectoryInfo dirInfo = new (subDir);
        if (ignoreDirs.Contains(dirInfo.Name))
        {
            continue;
        }
        string updatedDir = dirInfo.FullName;
        foreach ((string from, string to) in replacers)
        {
            if (!dirInfo.Name.Contains(from))
            {
                continue;
            }

            string newDirName = dirInfo.Name.Replace(from, to);
            string newDirFullName = Path.Combine(dirInfo.Parent!.FullName, newDirName);
            Directory.Move(dirInfo.FullName, newDirFullName);
            updatedDir = newDirFullName;
            break;
        }
        
        updatedSubDirs.Add(updatedDir);
    }

    foreach (string subDir in updatedSubDirs)
    {
        ReplaceDirName(subDir, replacers);
    }
}

async Task ReplaceFileNameAndTextAsync(string rootDir,
    Replacer[] fileNameReplacers,
    Replacer[] textReplacers)
{
    if (ignoreDirs.Contains(rootDir))
    {
        return;
    }
    
    string[] files = Directory.GetFiles(rootDir, "*", SearchOption.TopDirectoryOnly);

    foreach (string filePath in files)
    {
        string fileName = Path.GetFileName(filePath);
        if (ignoreFiles.Contains(fileName))
        {
            continue;
        }
        
        await ReplaceTextAsync(filePath, textReplacers);

        foreach ((string from, string to) in fileNameReplacers)
        {
            if (!fileName.Contains(from))
            {
                continue;
            }

            string newFileName = fileName.Replace(from, to);
            string newFilePath =  Path.Combine(rootDir, newFileName);
            File.Move(filePath, newFilePath);
            break;
        }
    }
    
    string[] subDirs = Directory.GetDirectories(rootDir, "*", SearchOption.TopDirectoryOnly);

    foreach (string subDir in subDirs)
    {
        await ReplaceFileNameAndTextAsync(subDir, fileNameReplacers, textReplacers);
    }
}

async Task ReplaceTextAsync(string filePath, Replacer[] replacers)
{
    string fileName = Path.GetFileName(filePath);
    if (ignoreTextReplaceFiles.Contains(fileName))
    {
        return;
    }
    
    string allText = await File.ReadAllTextAsync(filePath);
    foreach ((string from, string to) in replacers)
    {
        allText = allText.Replace(from, to);
    }
    
    await File.WriteAllTextAsync(filePath, allText);
}

static Replacer ParseReplacer(string replacerText)
{
    var splitOptions = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;
    string[] parts = replacerText.Split('=', splitOptions);
    return new Replacer(parts[0], parts[1]);
}

static Replacer[] ToReplacerObjects(IEnumerable<string> replacers) =>
    ProcessReplacers(replacers.Select(ParseReplacer));

static Replacer[] ProcessReplacers(IEnumerable<Replacer> replacers) =>
    [..replacers.DistinctBy(x => x.From).OrderByDescending(x => x.From)];

static void CopyDirectory(string sourceDir, string destinationDir, bool recursive, string[]? ignoreDirs = null)
{
    ignoreDirs ??= [];
    // Get information about the source directory
    DirectoryInfo dir = new (sourceDir);

    if (ignoreDirs.Contains(dir.Name))
    {
        return;
    }

    // Check if the source directory exists
    if (!dir.Exists)
        throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

    // Cache directories before we start copying
    DirectoryInfo[] dirs = dir.GetDirectories();

    // Create the destination directory
    Directory.CreateDirectory(destinationDir);

    // Get the files in the source directory and copy to the destination directory
    foreach (FileInfo file in dir.GetFiles())
    {
        string targetFilePath = Path.Combine(destinationDir, file.Name);
        file.CopyTo(targetFilePath, overwrite: true);
    }
    
    if (!recursive)
    {
        return;
    }
    
    // If recursive and copying subdirectories, recursively call this method
    foreach (DirectoryInfo subDir in dirs)
    {
        string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
        CopyDirectory(subDir.FullName, newDestinationDir, recursive, ignoreDirs);
    }
}

static void CleanUp(string generateToDir)
{
    // Clean up the dir first
    if (Directory.Exists(generateToDir))
    {
        Directory.Delete(generateToDir, true);
    }
}