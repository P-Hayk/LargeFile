using Common;
using FileGenerator;
using FileSorter;

internal class Program
{
    private static async Task Main(string[] args)
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Generate File");
            Console.WriteLine("2. Sort Existing File");
            Console.WriteLine("3. Exit");

            string filePath = null;

            var fileConfigs = new FileConfigurations();

            var sorter = new Sorter(fileConfigs);
            var generator = new Generator(fileConfigs);

            switch (Console.ReadLine())
            {
                case "1":
                    filePath = await generator.GenerateAsync();

                    Console.WriteLine($"{filePath} file successfully generated.");
                    Console.WriteLine("Do you want to sort the file? (y/n)");

                    string sortChoice = Console.ReadLine().ToLower();

                    if (sortChoice == "y")
                    {
                        await sorter.SortAsync(filePath, fileConfigs.OutputFileName);

                        Console.WriteLine($"{fileConfigs.OutputFileName} file successfully generated.");

                    }
                    else
                    {
                        Console.WriteLine("Exiting.");
                    }
                    break;

                case "2":
                    Console.WriteLine("Do you want to provide a path for the generated file? (y/n)");
                    filePath = Path.Combine(fileConfigs.Directory, fileConfigs.FileName);
                    Console.WriteLine($"Default path is {filePath}");
                    string pathChoice = Console.ReadLine().ToLower();

                    if (pathChoice == "y")
                    {
                        Console.WriteLine("Enter the file path(e.g., C:/myfolder/):");
                        filePath = Console.ReadLine();
                    }

                    await sorter.SortAsync(filePath, fileConfigs.OutputFileName);

                    Console.WriteLine($"{fileConfigs.OutputFileName} file successfully sorted.");

                    break;

                case "3":
                    exit = true;
                    break;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
}