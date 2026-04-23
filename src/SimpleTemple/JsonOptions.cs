namespace SimpleTemple;

public class JsonOptions
{
    public string SourceDir { get; set; } = CommandLineOptions.DefaultTemplateSourceDir;

    public string GenerateToDir { get; set; } = Directory.GetCurrentDirectory();

    public List<Replacement> CommonReplacements { get; set; } = [];

    public List<Replacement> TextReplacements { get; set; } = [];

    public List<Replacement> FileNameReplacements { get; set; } = [];

    public List<Replacement> DirNameReplacements { get; set; } = [];
}
