using CommandLine;
using SimpleTemple;

string[] ignoreDirs = [".vs", ".idea", "Logs", "bin", "obj", "TestResults"];
string[] ignoreFiles = [".editorconfig", ".env"];
string[] ignoreTextReplaceFiles = ["confy.db"];
await Parser.Default.ParseArguments<Options>(args)
    .WithParsedAsync(async options =>
    {
        IEnumerable<string> commonReplacers = options.CommonReplacers;
        string sourceDir = options.SourceDir;
        string generateToDir = options.GenerateToDir;
        // Clean up the dir first
        if (Directory.Exists(generateToDir))
        {
            Directory.Delete(generateToDir, true);
        }
        CopyDirectory(sourceDir, generateToDir, recursive: true, ignoreDirs);

        IEnumerable<string> dirNameReplacers = commonReplacers
            .Concat(options.DirNameReplacers);
        
        ReplaceDirName(generateToDir, ToFromToPairs(dirNameReplacers));

        IEnumerable<string> fileNameReplacers = commonReplacers
            .Concat(options.FileNameReplacers)
            .Distinct();
        
        IEnumerable<string> textReplacers = commonReplacers
                     .Concat(options.TextReplacers)
                     .Distinct();

        await ReplaceFileNameAndTextAsync(
            generateToDir,
            ToFromToPairs(fileNameReplacers),
            ToFromToPairs(textReplacers)
        );
    });
return;

void ReplaceDirName(string rootDir, FromToPair[] fromToPairs)
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
        foreach ((string from, string to) in fromToPairs)
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
        ReplaceDirName(subDir, fromToPairs);
    }
}

async Task ReplaceFileNameAndTextAsync(string rootDir,
    FromToPair[] fileNameReplacerFromToPairs,
    FromToPair[] textReplacerFromToPairs)
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
        
        await ReplaceTextAsync(filePath, textReplacerFromToPairs);

        foreach ((string from, string to) in fileNameReplacerFromToPairs)
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
        await ReplaceFileNameAndTextAsync(subDir, fileNameReplacerFromToPairs, textReplacerFromToPairs);
    }
}

async Task ReplaceTextAsync(string filePath, FromToPair[] fromToPairs)
{
    string fileName = Path.GetFileName(filePath);
    if (ignoreTextReplaceFiles.Contains(fileName))
    {
        return;
    }
    
    string allText = await File.ReadAllTextAsync(filePath);
    foreach ((string from, string to) in fromToPairs)
    {
        allText = allText.Replace(from, to);
    }
    
    await File.WriteAllTextAsync(filePath, allText);
}

static FromToPair ParseReplacer(string replacer)
{
    string[] parts = replacer.Split('=', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    return new FromToPair(parts[0], parts[1]);
}

static FromToPair[] ToFromToPairs(IEnumerable<string> replacers) =>
    replacers.Select(ParseReplacer)
        .DistinctBy(x => x.From)
        .OrderByDescending(x => x.From)
        .ToArray();

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

internal record FromToPair(string From, string To);