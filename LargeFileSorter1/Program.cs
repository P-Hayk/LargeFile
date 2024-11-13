using LargeFileSorter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

class Program
{

    static async Task Main()
    {
        var sw = new Stopwatch();
        sw.Start();

        using var s = new StreamReader("C:/Users/hayk.poghosyan/Desktop/Files/file_random.txt");

        var fileNames = await SplitFile(s.BaseStream);

        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);

        MergeSortedChunks(fileNames, "C:/Users/hayk.poghosyan/Desktop/Files/file_Sorted.txt");

        sw.Restart();
        Console.WriteLine(sw.ElapsedMilliseconds);


        Console.ReadKey();


    }

    public static async Task<List<string>> SplitFile(Stream sourceStream)
    {
        var files = new List<string>();
        var _options = new ExternalMergeSorterOptions();

        var fileSize = _options.Split.FileSize;

        await using (sourceStream)
        {
            var currentFile = 0L;
            while (sourceStream.Position < sourceStream.Length)
            {
                int numberOfBytes = 0;
                int totalBytes = 0;
                byte[] bufferToRead = new byte[256 * 1024];

                var filename = $"{++currentFile}.unsorted.txt";

                await using FileStream destinationStream = new FileStream(Path.Combine(_options.FileLocation, filename),
                                                           FileMode.Create, FileAccess.Write);

                while (totalBytes < fileSize && (numberOfBytes = await sourceStream.ReadAsync(bufferToRead, 0, bufferToRead.Length)) > 0)
                {
                    await destinationStream.WriteAsync(bufferToRead, 0, numberOfBytes);
                    totalBytes += numberOfBytes;
                }

                var bytes = new List<byte>();

                while (true)
                {
                    var @byte = sourceStream.ReadByte();

                    if (@byte == -1 || @byte == _options.Split.NewLineSeparator)
                        break;

                    bytes.Add((byte)@byte);
                }

                if (bytes.Count > 0)
                {
                    await destinationStream.WriteAsync(bytes.ToArray());
                }

                files.Add(destinationStream.Name);
            }

        }
        return files;
    }

    private static void MergeSortedChunks(List<string> chunkFiles, string outputFilePath)
    {
        var readers = chunkFiles.Select(file => new StreamReader(file)).ToList();

        var comparer = Comparer<(int, string, int)>.Create((a, b) =>
        {
            int cmp = string.Compare(a.Item2, b.Item2, StringComparison.Ordinal);
            return cmp != 0 ? cmp : a.Item1.CompareTo(b.Item1);
        });

        var sortedLines = new SortedSet<(int number, string text, int fileIndex)>(comparer);

        // Initialize sorted set with the first line from each file
        for (int i = 0; i < readers.Count; i++)
        {
            if (TryParseLine(readers[i].ReadLine(), out var line))
            {
                sortedLines.Add((line.Item1, line.Item2, i));
            }
        }

        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            while (sortedLines.Any())
            {
                // Get the smallest line
                var smallest = sortedLines.Min;
                sortedLines.Remove(smallest);

                writer.WriteLine($"{smallest.number}. {smallest.text}");

                // Add the next line from the same file, if any
                var nextLine = readers[smallest.fileIndex].ReadLine();
                if (nextLine != null && TryParseLine(nextLine, out var line))
                {
                    sortedLines.Add((line.Item1, line.Item2, smallest.fileIndex));
                }
            }
        }

        // Close readers
        foreach (var reader in readers)
        {
            reader.Dispose();
        }
    }

    private static bool TryParseLine(string line, out (int, string) result)
    {
        int dotIndex = line.IndexOf('.');
        if (dotIndex > 0 && int.TryParse(line.Substring(0, dotIndex), out int number))
        {
            result = (number, line.Substring(dotIndex + 2)); // +2 to skip ". "
            return true;
        }

        result = (0, string.Empty);
        return false;
    }
}