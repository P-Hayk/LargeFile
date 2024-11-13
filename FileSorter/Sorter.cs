using Common;
using System.IO;

namespace FileSorter;

public class Sorter
{
    private readonly FileConfigurations _configs;
    private readonly Logger _logger;

    public Sorter(FileConfigurations configs)
    {
        _configs = configs ?? throw new ArgumentNullException(nameof(configs));
        _logger = new Logger();
    }

    public async Task<bool> SortAsync(string inputFilePath, string outputFilePath)
    {
        if (!File.Exists(inputFilePath))
        {
            _logger.LogError("File does not exist.");
            return false;
        }

        if (!Helper.IsSpaceEnough(_configs.Directory, _configs.FileSize * 2))
        {
            _logger.LogError("There is no enough space.");
            return false;
        }

        _logger.LogInfo("Started sorting the file. It may take several minutes. Please wait.");

        var splitter = new Splitter(_configs);

        var chunks = await splitter.SplitFileAsync(inputFilePath);

        if (chunks.Count == 0)
        {
            return false;
        }

        await Sort(chunks, outputFilePath);

        return true;
    }

    private async Task Sort(List<string> filesPaths, string outputFilePath)
    {
        // Create a list of tasks that create StreamReader objects
        var tasks = filesPaths.Select(path =>
            Task.Run(() =>
                new StreamReader(Path.Combine(_configs.Directory, path)))).ToList();

        // Wait for all tasks to complete
        var readers = await Task.WhenAll(tasks);

        // The readers will now be an array of StreamReader objects
        List<StreamReader> readerList = readers.ToList();

        var comparer = Comparer<(int, string, int)>.Create((a, b) =>
        {
            int cmp = string.Compare(a.Item2, b.Item2, StringComparison.Ordinal);
            return cmp != 0 ? cmp : a.Item1.CompareTo(b.Item1);
        });

        var sortedLines = new SortedSet<(int id, string name, int fileIndex)>(comparer);

        // Initialize sorted set with the first line from each file
        for (int i = 0; i < readerList.Count; i++)
        {
            if (Helper.TryParseLine(readers[i].ReadLine(), out (int, string) line))
            {
                sortedLines.Add((line.Item1, line.Item2, i));
            }
        }

        using (StreamWriter writer = new StreamWriter(Path.Combine(_configs.Directory, outputFilePath)))
        {
            while (sortedLines.Any())
            {
                // Get the smallest line
                var smallest = sortedLines.Min;
                sortedLines.Remove(smallest);

                writer.WriteLine($"{smallest.id}. {smallest.name}");

                // Add the next line from the same file, if any
                var nextLine = readers[smallest.fileIndex].ReadLine();
                if (nextLine != null && Helper.TryParseLine(nextLine, out var line))
                {
                    sortedLines.Add((line.Item1, line.Item2, smallest.fileIndex));
                }
            }
        }

        foreach (var reader in readers)
        {
            reader.Dispose();
        }
    }
}
