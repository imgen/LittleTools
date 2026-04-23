using CommandLine;

namespace SimpleTemple;

public class CommandLineOptions
{
    public const string DefaultTemplateSourceDir = @"C:\Create\confy\api";

    [Option('s', "source", Required = false, HelpText = "The source directory")]
    public string SourceDir { get; set; } = DefaultTemplateSourceDir;

    [Option('g', "generate-to", Required = false, HelpText = "The source directory")]
    public string GenerateToDir { get; set; } = Directory.GetCurrentDirectory();

    [Option('c', "common-replacers", Required = false, HelpText = "The common replacers")]
    public List<string> CommonReplacers { get; set; } = [];

    [Option('t', "text-replacers", Required = false, HelpText = "The text replacers")]
    public List<string> TextReplacers { get; set; } = [];

    [Option('f', "file-name-replacers", Required = false, HelpText = "The file replacers")]
    public List<string> FileNameReplacers { get; set; } = [];

    [Option('d', "dir-name-replacers", Required = false, HelpText = "The dir name replacers")]
    public List<string> DirNameReplacers { get; set; } = [];
}