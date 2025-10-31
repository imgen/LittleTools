using CommandLine;

namespace GetxPageTemplator;

class Options
{
    [Option('n', "name", 
        Required = true,
        HelpText = "The name of your page, in the form of snake case such as user_posts")]
    public string PageName { get; set; }
    
    [Option('t', "title",
        Required = true,
        HelpText = "The title of your page, such as Your Posts, etc")]
    public string PageTitle { get; set; }
}