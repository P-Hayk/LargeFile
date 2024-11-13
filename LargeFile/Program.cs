using System.Diagnostics;
using System.Text;
class Program
{
    static async Task Main()
    {
        var sw = new Stopwatch();

        sw.Start();
        //await Files();
        sw.Stop();

        //Console.WriteLine(sw.ElapsedMilliseconds);

        sw.Restart();
        await SingleFile();
        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);

        Console.ReadKey();

    }

    private async static Task SingleFile()
    {
        long availableMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
        int bufferSize;

        if (availableMemory > 16L * 1024 * 1024 * 1024) // Greater than 8 GB
        {
            bufferSize = 1024 * 1024; // 512 KB for high-memory systems
            await NewMethod(bufferSize);

        }
        //if (availableMemory > 8L * 1024 * 1024 * 1024) // Greater than 8 GB
        //{
        //    bufferSize = 512 * 1024; // 512 KB for high-memory systems
        //    await NewMethod(bufferSize);

        //}
        //if (availableMemory > 4L * 1024 * 1024 * 1024) // Between 4 GB and 8 GB
        //{
        //    bufferSize = 256 * 1024; // 256 KB for moderate memory
        //    await NewMethod(bufferSize);

        //}
        ////else
        ////{
        //bufferSize = 128 * 1024; // 128 KB for low-memory systems
        //await NewMethod(bufferSize);

        //bufferSize = 4 * 1024; // 128 KB for low-memory systems
        //await NewMethod(bufferSize);

        //bufferSize = 8 * 1024; // 128 KB for low-memory systems
        //await NewMethod(bufferSize);

    }

    private static async Task NewMethod(int bufferSize)
    {
        var sw = new Stopwatch();
        sw.Start();
        long totalTargetSize = 1L * 1024 * 1024 * 1024;
        int x = 1;
        string outputDirectory = "C:/Users/hayk.poghosyan/desktop/Files";
        Directory.CreateDirectory(outputDirectory);

        string filePath = Path.Combine(outputDirectory, $"file_random.txt");

        long bytesWritten = 0;

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            while (bytesWritten < totalTargetSize)
            {
                string buffer = $"{new Random().Next(1, 999999)}. {GenerateRandomWord(new Random().Next(1, 5))}" + Environment.NewLine;
                await writer.WriteAsync(buffer);
                bytesWritten += buffer.Length;
            }
        }

        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);
    }


    public static string GenerateRandomWord(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var random = new Random();
        var result = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }

        return result.ToString();
    }

    //private static async Task Files()
    //{
    //    long availableMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
    //    int bufferSize;

    //    // Adjust buffer size based on available memory

    //    if (availableMemory > 8L * 1024 * 1024 * 1024) // Greater than 8 GB
    //    {
    //        bufferSize = 512 * 1024; // 512 KB for high-memory systems
    //    }
    //    else if (availableMemory > 4L * 1024 * 1024 * 1024) // Between 4 GB and 8 GB
    //    {
    //        bufferSize = 256 * 1024; // 256 KB for moderate memory
    //    }
    //    else
    //    {
    //        bufferSize = 128 * 1024; // 128 KB for low-memory systems
    //    }

    //    Console.WriteLine(availableMemory);

    //    string outputDirectory = "files";
    //    Directory.CreateDirectory(outputDirectory);
    //    long totalTargetSize = 100L * 1024 * 1024 * 1024; // 100 GB total
    //    int fileCount = 1;
    //    long fileSizeInBytes = totalTargetSize / fileCount; // 1 GB per file
    //    int maxParallelCount = 8; // Limit to 10 files at a time
    //    string buffer = new string('A', 512 * 1024); // 4 KB buffer
    //    // List to hold tasks
    //    List<Task> tasks = new List<Task>();
    //    List<string> filePaths = new List<string>();
    //    using (SemaphoreSlim semaphore = new SemaphoreSlim(maxParallelCount))
    //    {
    //        for (int i = 1; i <= fileCount; i++)
    //        {
    //            int fileIndex = i; // Capture the loop variable
    //            await semaphore.WaitAsync(); // Wait for an available slot
    //            tasks.Add(Task.Run(async () =>
    //            {
    //                try
    //                {
    //                    string filePath = Path.Combine(outputDirectory, $"part_{fileIndex}.txt");
    //                    filePaths.Add(filePath);
    //                    long bytesWritten = 0;
    //                    using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8, buffer.Length))
    //                    {
    //                        while (bytesWritten < fileSizeInBytes)
    //                        {
    //                            long bytesToWrite = Math.Min(buffer.Length, fileSizeInBytes - bytesWritten);
    //                            await writer.WriteAsync(buffer);
    //                            bytesWritten += bytesToWrite;
    //                        }
    //                    }
    //                    Console.WriteLine($"Created {filePath}");
    //                }
    //                catch (Exception ex)
    //                {
    //                    Console.WriteLine($"Error writing file {fileIndex}: {ex.Message}");
    //                }
    //                finally
    //                {
    //                    semaphore.Release(); // Release the semaphore slot
    //                }
    //            }));
    //        }
    //        await Task.WhenAll(tasks);
    //    }

    //string newFilePath = Path.Combine(outputDirectory, $"combined.txt");

    //using (var destination = new FileStream(newFilePath, FileMode.OpenOrCreate))
    //{

    //    foreach (string fp in filePaths)
    //    {
    //        using (var inputStream = new FileStream(fp, FileMode.Open))
    //        {
    //            inputStream.CopyTo(destination);
    //        }
    //    }

    //    Console.WriteLine("All files created successfully.");
    //}

    // Create a memory-mapped file with the total size
    //using (var mmf = MemoryMappedFile.CreateFromFile(newFilePath, FileMode.Create, null, totalTargetSize))
    //{
    //    long offset = 0;
    //    foreach (string inputFile in filePaths)
    //    {
    //        long fileSize = new FileInfo(inputFile).Length;

    //        // Map a view of the output file section
    //        using (var accessor = mmf.CreateViewAccessor(offset, fileSize, MemoryMappedFileAccess.Write))
    //        using (var inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
    //        {
    //            byte[] bufferToHold = new byte[81920]; // Buffer to hold chunks
    //            int bytesRead;
    //            long position = 0;

    //            // Read the input file and write it to the mapped view
    //            while ((bytesRead = await inputStream.ReadAsync(bufferToHold, 0, bufferToHold.Length)) > 0)
    //            {
    //                accessor.WriteArray(position, bufferToHold, 0, bytesRead);
    //                position += bytesRead;
    //            }
    //        }
    //        offset += fileSize; // Move offset for the next file
    //    }
    //}
}
