// See https://aka.ms/new-console-template for more information
using WpsPoolCleaner;

var dir = args.FirstOrDefault() ?? @"C:\Users\imgen\AppData\Roaming\kingsoft\wps\addons\pool\win-i386";
var subDirs = Directory.GetDirectories(dir, "*.*", SearchOption.TopDirectoryOnly);

var wpsPoolDirectoryGroups = subDirs.Select(path =>
{
    var directoryInfo = new DirectoryInfo(path);
    var directoryName = directoryInfo.Name;
    var parts = directoryName.Split('_');
    if (parts.Length == 2)
    {
        var wpsPoolDirectoryName = parts[0];
        var version = parts[1];
        return new WpsPoolDirectory(path, wpsPoolDirectoryName, new Version(version));
    }

    return new WpsPoolDirectory(path, "", null);
}).Where(x => x.Name != "")
.GroupBy(x => x.Name)
.Where(x => x.Count() > 1);

foreach (var group in wpsPoolDirectoryGroups)
{
    var maxVersion = group.Max(x => x.Version);
    Console.WriteLine($"The max version of {group.Key} is {maxVersion}");
    foreach (var wpsPoolDirectory in group)
    {
        if (wpsPoolDirectory.Version < maxVersion)
        {
            Console.WriteLine($"The directory to be deleted is {wpsPoolDirectory.Path}");
            Directory.Delete(wpsPoolDirectory.Path, true );
        }
    }

    Console.WriteLine();
}