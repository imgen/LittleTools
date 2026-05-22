// See https://aka.ms/new-console-template for more information
using DartPackagesCleaner;

var dir = args.FirstOrDefault() ?? @"C:\Users\imgen\AppData\Local\Pub\Cache\hosted\pub.dev";
var subDirs = Directory.GetDirectories(dir, "*.*", SearchOption.TopDirectoryOnly);

var dartPackageDirectoryGroups = subDirs.Select(path =>
{
    var directoryInfo = new DirectoryInfo(path);
    var directoryName = directoryInfo.Name;
    var parts = directoryName.Split('-');
    if (parts.Length == 2)
    {
        var dartPackageDirectoryName = parts[0];
        var version = parts[1].Replace("+", ".")
            .Replace("hotfix", "1");
        return new DartPackageDirectory(path, dartPackageDirectoryName, new Version(version));
    }

    return new DartPackageDirectory(path, "", null);
}).Where(x => x.Name != "")
.GroupBy(x => x.Name)
.Where(x => x.Count() > 1);

foreach (var group in dartPackageDirectoryGroups)
{
    var maxVersion = group.Max(x => x.Version);
    Console.WriteLine($"The max version of {group.Key} is {maxVersion}");
    foreach (var dartPackageDirectory in group)
    {
        if (dartPackageDirectory.Version < maxVersion)
        {
            Console.WriteLine($"The directory to be deleted is {dartPackageDirectory.Path}");
            Directory.Delete(dartPackageDirectory.Path, true );
        }
    }

    Console.WriteLine();
}