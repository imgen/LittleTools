// See https://aka.ms/new-console-template for more information
using NugetPackagesCleaner;

var dir = args.FirstOrDefault() ?? @"C:\Users\imgen\.nuget\packages";
var subDirs = Directory.GetDirectories(dir, "*.*", SearchOption.TopDirectoryOnly);

foreach (var path in subDirs)
{
    var directoryInfo = new DirectoryInfo(path);
    var topSubDirs = directoryInfo.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly);
    var nugetPackageDirs = topSubDirs
        .Where(x => Version.TryParse(x.Name, out _))
        .Select(x => new NugetPackageDirectory(x.FullName, new Version(x.Name)))
        .ToArray();
    if (nugetPackageDirs.Length <= 1)
    {
        continue;
    }
    var maxVersion = nugetPackageDirs.Max(x => x.Version);
    Console.WriteLine($"The max version of {directoryInfo.Name} is {maxVersion}");
    foreach(var (nugetPackageDirPath, version) in nugetPackageDirs)
    {
        if (version < maxVersion)
        {
            Console.WriteLine($"The version to be deleted is {version}");
            Directory.Delete(nugetPackageDirPath, recursive: true);
        }
    }

    Console.WriteLine();
}    
