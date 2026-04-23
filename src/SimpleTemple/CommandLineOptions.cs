using CommandLine;

namespace SimpleTemple;

public class CommandLineOptions
{
    public const string DefaultTemplateSourceDir = @"C:\Create\confy\api";

    [Option('s', "source", Required = false, HelpText = "The source directory")]
    public string SourceDir { get; set; } = DefaultTemplateSourceDir;

    [Option('g', "generate-to", Required = false, HelpText = "The source directory")]
    public string GenerateToDir { get; set; } = Directory.GetCurrentDirectory();

    [Option('c', "common-replacements", Required = false, HelpText = "The common replacements")]
    public List<string> CommonReplacements { get; set; } = [];

    [Option('t', "text-replacements", Required = false, HelpText = "The text replacements")]
    public List<string> TextReplacements { get; set; } = [];

    [Option('f', "file-name-replacements", Required = false, HelpText = "The file replacements")]
    public List<string> FileNameReplacements { get; set; } = [];

    [Option('d', "dir-name-replacements", Required = false, HelpText = "The dir name replacements")]
    public List<string> DirNameReplacements { get; set; } = [];
}