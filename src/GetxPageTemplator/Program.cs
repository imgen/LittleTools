using CommandLine;
using GetxPageTemplator;

await Parser.Default.ParseArguments<Options>(args)
    .WithParsedAsync(async options =>
    {
        var pageName = options.PageName;
        var pageTitle = options.PageTitle;

        var pageNameWords = pageName.Split('_');
        var pascalCasePageName = pageNameWords.Aggregate("", (current, pageNameWord) => 
            current + pageNameWord[0].ToString().ToUpper() + pageNameWord[1..]);

        var fileNameMap = new Dictionary<string, string>
        {
            ["page.dart"] = $"{pageName}_page.dart",
            ["controller.dart"] = $"{pageName}_controller.dart",
            ["binding.dart"] = $"{pageName}_binging.dart",
            ["provider.dart"] = $"{pageName}_provider.dart",
        };
        var appBaseDir = AppContext.BaseDirectory;
        var currentDirectory = Directory.GetCurrentDirectory();
        foreach (var templateFileName in fileNameMap.Keys)
        {
            var templateFilePath = Path.Combine(appBaseDir, templateFileName);
            var outputFilePath = Path.Combine(currentDirectory, fileNameMap[templateFileName]);
            var template = await File.ReadAllTextAsync(templateFilePath);
            var output = template.Replace("{page_name}", pageName)
                .Replace("{PageName}", pascalCasePageName)
                .Replace("{PageTitle}", pageTitle);
            await File.WriteAllTextAsync(outputFilePath, output);
        }
    });


