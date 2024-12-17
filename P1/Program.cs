using System;
using System.IO;
using System.CommandLine;
using System.Threading.Tasks;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var bundleCommand = new Command("bundle", "Pack source files into a single file");

        var languageOption = new Option<string>(
            "--language",
            "Specify programming languages to include (or 'all').")
        {
            IsRequired = true
        };
        languageOption.AddAlias("-l");

        var outputOption = new Option<string>(
            "--output",
            "Specify the output bundle file name.")
        {
            IsRequired = true
        };
        outputOption.AddAlias("-o");

        var noteOption = new Option<bool>(
            "--note",
            "Include source file paths as comments in the bundle.");
        noteOption.AddAlias("-n");

        var sortOption = new Option<string>(
            "--sort",
            () => "name", // default value
            "Sort order: 'name' or 'type' (default is 'name').");
        sortOption.AddAlias("-s");

        var removeEmptyLinesOption = new Option<bool>(
            "--remove-empty-lines",
            "Remove empty lines from source files.");
        removeEmptyLinesOption.AddAlias("-r");

        var authorOption = new Option<string>(
            "--author",
            "Specify the author's name.");
        authorOption.AddAlias("-a");

        // הוספת האופציות לפקודה
        bundleCommand.AddOption(languageOption);
        bundleCommand.AddOption(outputOption);
        bundleCommand.AddOption(noteOption);
        bundleCommand.AddOption(sortOption);
        bundleCommand.AddOption(removeEmptyLinesOption);
        bundleCommand.AddOption(authorOption);

        // הגדרת הפעולה לביצוע לפקודת bundle
        bundleCommand.SetHandler(
            (string language, string output, bool note, string sort, bool removeEmptyLines, string author) =>
            {
                BundleFiles(language, output, note, sort, removeEmptyLines, author);
            },
            languageOption, outputOption, noteOption, sortOption, removeEmptyLinesOption, authorOption
        );

        // פקודה נוספת: create-rsp
        var createRspCommand = new Command("create-rsp", "Create a response file with the bundled command parameters");

        createRspCommand.SetHandler(() =>
        {
            CreateRspFile();
        });

        var rootCommand = new RootCommand { bundleCommand, createRspCommand };

        return await rootCommand.InvokeAsync(args);
    }

    static void BundleFiles(string language, string output, bool note, string sort, bool removeEmptyLines, string author)
    {
        Console.WriteLine($"Bundling files with parameters:\n" +
                          $"Language: {language}\n" +
                          $"Output: {output}\n" +
                          $"Include Note: {note}\n" +
                          $"Sort: {sort ?? "name"}\n" +
                          $"Remove Empty Lines: {removeEmptyLines}\n" +
                          $"Author: {author}");
    }

    static void CreateRspFile()
    {
        Console.Write("Enter languages (or 'all'): ");
        var language = Console.ReadLine();
        Console.Write("Enter output file name: ");
        var output = Console.ReadLine();
        Console.Write("Include note? (true/false): ");
        var note = bool.Parse(Console.ReadLine() ?? "false");
        Console.Write("Sort order (name/type): ");
        var sort = Console.ReadLine();
        Console.Write("Remove empty lines? (true/false): ");
        var removeEmptyLines = bool.Parse(Console.ReadLine() ?? "false");
        Console.Write("Enter author's name: ");
        var author = Console.ReadLine();

        var rspContent = $"dotnet bundle --language {language} --output {output} --note {note} " +
                         $"--sort {sort} --remove-empty-lines {removeEmptyLines} --author \"{author}\"";

        var rspFileName = $"{Path.GetFileNameWithoutExtension(output)}.rsp";
        File.WriteAllText(rspFileName, rspContent);
        Console.WriteLine($"Response file created: {rspFileName}");
    }
}
