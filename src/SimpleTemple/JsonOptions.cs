namespace SimpleTemple;

public class JsonOptions
{
    public string SourceDir { get; set; } = CommandLineOptions.DefaultTemplateSourceDir;

    public string GenerateToDir { get; set; } = Directory.GetCurrentDirectory();

    public List<Replacer> CommonReplacers { get; set; } = [];

    public List<Replacer> TextReplacers { get; set; } = [];

    public List<Replacer> FileNameReplacers { get; set; } = [];

    public List<Replacer> DirNameReplacers { get; set; } = [];
}
