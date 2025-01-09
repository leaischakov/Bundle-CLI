
using System.CommandLine;

var languageOption = new Option<string[]>("--language", description: "רשימת שפות תכנות (או 'all')") {
    IsRequired = true,
};
languageOption.AddAlias("-l");


var outputOption = new Option<FileInfo>("--output",description: "שם קובץ ה-bundle המיוצא", getDefaultValue: () => new FileInfo("bundled_code.txt"));
outputOption.AddAlias("-o");

var noteOption = new Option<bool>("--note",description: "האם לרשום את מקור הקוד כהערה בקובץ");
noteOption.AddAlias("-n");

var sortOption = new Option<string>("--sort",() => "name","סדר העתקת קבצי הקוד (name או type)");
sortOption.AddAlias("-s");

var removeEmptyLinesOption = new Option<bool>("--remove-empty-lines",description: "האם למחוק שורות ריקות");
removeEmptyLinesOption.AddAlias("-r");

var authorOption = new Option<string>("--author",description: "שם יוצר הקובץ",getDefaultValue: () => "Unknown Author");
authorOption.AddAlias("-a");

// יצירת פקודת bundle
var bundleCommand = new Command("bundle", "אריזת קבצי קוד לקובץ אחד")
{
    languageOption,
    outputOption,
    noteOption,
    sortOption,
    removeEmptyLinesOption,
    authorOption
};

bundleCommand.SetHandler(
    (string[] languages, FileInfo output, bool note, string sort, bool removeEmptyLines, string author) =>
    {
        try
        {
            // קריאת קבצים בהתאם לשפות
            var files = GetFilesToBundle(languages);

            // מיון הקבצים
            files = SortFiles(files, sort);

            //  output  מסוג FileInfo
            using var writer = new StreamWriter(output.FullName);

            // כתיבת שם היוצר
            writer.WriteLine($"// Author: {author}");
            writer.WriteLine("// Bundled Code Starts Here");
            writer.WriteLine();

            foreach (var file in files)
            {
                if (note)
                {
                    writer.WriteLine($"// File: {Path.GetRelativePath(Directory.GetCurrentDirectory(), file)}");
                }

                var fileLines = File.ReadAllLines(file);  // שינוי שם המשתנה

                if (removeEmptyLines)
                {
                    fileLines = fileLines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
                }

                foreach (var line in fileLines)  // שימוש ב-fileLines
                {
                    writer.WriteLine(line);
                }

                writer.WriteLine(); // הפרדה בין קבצים
            }

            writer.WriteLine("// Bundled Code Ends Here");

            Console.WriteLine("bundle command executed successfully!");
            Console.WriteLine($"The output file is created in: {output.FullName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    },
    languageOption,
    outputOption,
    noteOption,
    sortOption,
    removeEmptyLinesOption,
    authorOption);


// יצירת פקודת create-rsp
var createRspCommand = new Command("create-rsp", "יוצר קובץ תגובה להרצת פקודת bundle")
{
    new Argument<FileInfo?>("file", "שם קובץ ה-rsp שיווצר")
};

createRspCommand.SetHandler(async (FileInfo? file) =>
{
    // אם לא נשלח שם קובץ, השתמש בקובץ ברירת מחדל
    file ??= new FileInfo("default.rsp");

    try
    {
        Console.WriteLine($"Creating response file: {file.FullName}");

        // קליטת ערכים מהמשתמש
        Console.WriteLine("Type the languages (e.g., csharp, javascript, html or 'all'):");
        var languages = Console.ReadLine() ?? "all";

        Console.WriteLine("Enter the name of the output file:");
        var output = Console.ReadLine() ?? "bundled_code.txt";

        Console.WriteLine("Do you want to add comments with the file name? (true/false):");
        var noteInput = Console.ReadLine();
        var note = bool.TryParse(noteInput, out var noteResult) ? noteResult : false;

        Console.WriteLine("Sort type (name or type):");
        var sort = Console.ReadLine() ?? "name";

        Console.WriteLine("Do you want to delete empty lines? (true/false):");
        var removeEmptyLinesInput = Console.ReadLine();
        var removeEmptyLines = bool.TryParse(removeEmptyLinesInput, out var removeEmptyLinesResult) ? removeEmptyLinesResult : false;

        Console.WriteLine("The name of the creator:");
        var author = Console.ReadLine() ?? "Unknown Author";

        // יצירת הפקודה המלאה
        var bundleCommand = $"bundle --language {languages} --output {output} --note {note} --sort {sort} --remove-empty-lines {removeEmptyLines} --author \"{author}\"";

        // שמירת הפקודה בקובץ
        await File.WriteAllTextAsync(file.FullName, bundleCommand);

        // הודעה למשתמש
        Console.WriteLine($"The response file was created successfully: {file.FullName}!");
        Console.WriteLine($"To run the command: dotnet @{file.FullName}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}, (System.CommandLine.Binding.IValueDescriptor<FileInfo>)createRspCommand.Arguments[0]);




// יצירת rootCommand והוספת פקודות
var rootCommand = new RootCommand("כלי CLI לאריזת קבצי קוד")
{
    bundleCommand, createRspCommand
};

// הרצת הפקודה
return await rootCommand.InvokeAsync(args);
    

    // פונקציה לאיסוף קבצים לפי שפות
static string[] GetFilesToBundle(string[] languages)
{
    var excludedDirectories = new[] { "bin", "obj", ".git", "node_modules" };
    var currentDir = Directory.GetCurrentDirectory();

    var allFiles = Directory.GetFiles(currentDir, "*.*", SearchOption.AllDirectories)
                            .Where(file => !excludedDirectories.Any(dir => file.Contains(Path.DirectorySeparatorChar + dir + Path.DirectorySeparatorChar)))
                            .ToList();

    if (languages.Contains("all", StringComparer.OrdinalIgnoreCase))
    {
        return allFiles.ToArray();
    }

    // קבלת הרחבות הקבצים מהשפות
    var extensions = languages.Select(lang => lang.ToLower()) // כאן אפשר להוסיף מפה של שפות לרחבות
                              .Select(lang => lang switch
                              {
                                  "csharp" => ".cs",
                                  "python" => ".py",
                                  "javascript" => ".js",
                                  "java" => ".java",
                                  "cpp" => ".cpp",
                                  "html" => ".html",
                                  "txt" => ".txt",
                                  "word"=>".docs",
                                  _ => "." + lang
                              }).ToList();

    Console.WriteLine("Filtered files for bundling:");
    foreach (var file in allFiles)
    {
        Console.WriteLine(file);
    }

    return allFiles.Where(file => extensions.Contains(Path.GetExtension(file).ToLower()))
                   .ToArray();
}

// פונקציה למיון קבצים
static string[] SortFiles(string[] files, string sortOption)
{
    return sortOption.ToLower() switch
    {
        "type" => files.OrderBy(f => Path.GetExtension(f)).ToArray(),
        _ => files.OrderBy(f => f).ToArray(), // ברירת מחדל לפי שם
    };
}