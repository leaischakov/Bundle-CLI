


//using System.CommandLine;

//// יצירת פקודת bundle
//var defaultValueFile = new FileInfo("bundled_code.txt");
//var outputOption = new Option<FileInfo>(
//            "--output",
//            description: "שם קובץ ה-bundle המיוצא",
//            getDefaultValue: () => defaultValueFile
//        );

//var languageOption = new Option<string[]>(
//           "--language",
//           description: "רשימת שפות תכנות (או 'all')"
//       )
//{
//    IsRequired = false,
//};

//var sortOption = new Option<string>(
//           "--sort",
//           () => "name",
//           "סדר העתקת קבצי הקוד (name או type)"
//       );
//var removeEmptyLinesOption = new Option<bool>(
//          "--remove-empty-lines",
//          description: "האם למחוק שורות ריקות"
//      );

//var authorOption = new Option<string>(
//    "--author",
//    description: "שם יוצר הקובץ",
//    getDefaultValue: () => "Unknown Author"
//);
//var noteOption = new Option<bool>(
//            "--note",
//            description: "האם לרשום את מקור הקוד כהערה בקובץ"
//        );

//var bundleCommand = new Command("bundle", "אריזת קבצי קוד לקובץ אחד")
//        {
//            languageOption,
//            //outputOption,
//            noteOption,
//            sortOption,
//            removeEmptyLinesOption,
//            authorOption
//        };

//bundleCommand.AddOption(outputOption);
//bundleCommand.SetHandler((FileInfo output, string[] languages, string sort, string author, bool note, bool removeEmptyLines) =>
//{
//    try
//    {

//        // מחיקת הקובץ אם הוא כבר קיים
//        if (File.Exists(output.FullName))
//        {
//            File.Delete(output.FullName);
//        }
//        //    File.Create(output.FullName);

//        // using var writer = new StreamWriter(output.FullName);
//        using var writer = new StreamWriter(new FileStream(output.FullName, FileMode.Create, FileAccess.Write, FileShare.Read));

//        Console.WriteLine("file was created");

//        // קריאת קבצים בהתאם לשפות
//        var files = GetFilesToBundle(languages);

//        // מיון הקבצים
//        files = SortFiles(files, sort);



//        // כתיבת שם היוצר
//        writer.WriteLine($"// Author: {author}");
//        writer.WriteLine("// Bundled Code Starts Here");
//        writer.WriteLine();

//        foreach (var file in files)
//        {
//            if (note)
//            {
//                writer.WriteLine($"// File: {Path.GetRelativePath(Directory.GetCurrentDirectory(), file)}");
//            }

//            var fileLines = File.ReadAllLines(file);  // שינוי שם המשתנה

//            if (removeEmptyLines)
//            {
//                fileLines = fileLines.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
//            }

//            foreach (var line in fileLines)  // שימוש ב-fileLines
//            {
//                writer.WriteLine(line);
//            }

//            writer.WriteLine(); // הפרדה בין קבצים
//        }

//        writer.WriteLine("// Bundled Code Ends Here");

//        Console.WriteLine("פקודת bundle בוצעה בהצלחה!");
//        //Console.WriteLine($"קובץ הפלט נוצר ב: {Path.GetFullPath(output)}");
//    }
//    catch(DirectoryNotFoundException ex)
//    {
//        Console.WriteLine("Error: File path is invalid");
//    }



//    // יצירת הקובץ היעד
//   //

//}, outputOption, languageOption, sortOption, authorOption, noteOption, removeEmptyLinesOption);




//// פונקציה לאיסוף קבצים לפי שפות

//static string[] GetFilesToBundle(string[] languages)
//{
//    var excludedDirectories = new[] { "bin", "obj", ".git", "node_modules" };
//    var currentDir = Directory.GetCurrentDirectory();

//    var allFiles = Directory.GetFiles(currentDir, "*.*", SearchOption.AllDirectories)
//                            .Where(file => !excludedDirectories.Any(dir => file.Contains(Path.DirectorySeparatorChar + dir + Path.DirectorySeparatorChar)))
//                            .ToList();

//    if (languages.Contains("all", StringComparer.OrdinalIgnoreCase))
//    {
//        return allFiles.ToArray();
//    }

//    // קבלת הרחבות הקבצים מהשפות
//    var extensions = languages.Select(lang => lang.ToLower()) // כאן אפשר להוסיף מפה של שפות לרחבות
//                              .Select(lang => lang switch
//                              {
//                                  "csharp" => ".cs",
//                                  "python" => ".py",
//                                  "javascript" => ".js",
//                                  "java" => ".java",
//                                  "cpp" => ".cpp",
//                                  "html" => ".html",
//                                  "txt" => ".txt",
//                                  _ => "." + lang
//                              }).ToList();

//    Console.WriteLine("Filtered files for bundling:");
//    foreach (var file in allFiles)
//    {
//        Console.WriteLine(file);
//    }

//    return allFiles.Where(file => extensions.Contains(Path.GetExtension(file).ToLower()))
//                   .ToArray();
//}


//// פונקציה למיון קבצים
//static string[] SortFiles(string[] files, string sortOption)
//{
//    return sortOption.ToLower() switch
//    {
//        "type" => files.OrderBy(f => Path.GetExtension(f)).ToArray(),
//        _ => files.OrderBy(f => f).ToArray(), // ברירת מחדל לפי שם
//    };
//}




//var rootCommand = new RootCommand("Root command for File Bundle Cli");
//rootCommand.AddCommand(bundleCommand);
//rootCommand.InvokeAsync(args);




// הגדרת אפשרויות
using System.CommandLine;

var languageOption = new Option<string[]>(
    "--language",
    description: "רשימת שפות תכנות (או 'all')"
)
{
    IsRequired = true
};

var outputOption = new Option<string>(
    "--output",
    description: "שם קובץ ה-bundle המיוצא",
    getDefaultValue: () => "bundled_code.txt"
);

var noteOption = new Option<bool>(
    "--note",
    description: "האם לרשום את מקור הקוד כהערה בקובץ"
);

var sortOption = new Option<string>(
    "--sort",
    () => "name",
    "סדר העתקת קבצי הקוד (name או type)"
);

var removeEmptyLinesOption = new Option<bool>(
    "--remove-empty-lines",
    description: "האם למחוק שורות ריקות"
);

var authorOption = new Option<string>(
    "--author",
    description: "שם יוצר הקובץ",
    getDefaultValue: () => "Unknown Author"
);

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
    (string[] languages, string output, bool note, string sort, bool removeEmptyLines, string author) =>
    {
        try
        {
            // קריאת קבצים בהתאם לשפות
            var files = GetFilesToBundle(languages);

            // מיון הקבצים
            files = SortFiles(files, sort);

            // יצירת הקובץ היעד
            using var writer = new StreamWriter(output);

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

            Console.WriteLine("פקודת bundle בוצעה בהצלחה!");
            Console.WriteLine($"קובץ הפלט נוצר ב: {Path.GetFullPath(output)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"אירעה שגיאה: {ex.Message}");
        }
    },
    languageOption,
    outputOption,
    noteOption,
    sortOption,
    removeEmptyLinesOption,
    authorOption);

// יצירת rootCommand והוספת פקודות
var rootCommand = new RootCommand("כלי CLI לאריזת קבצי קוד")
        {
            bundleCommand
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