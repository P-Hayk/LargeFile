using Common;

namespace FileSorter;
internal class Splitter
{
    private readonly FileConfigurations _configs;

    public Splitter(FileConfigurations configs)
    {
        _configs = configs ?? throw new ArgumentNullException(nameof(configs));
    }

    public async Task<List<string>> SplitFileAsync(string sourceFilePath)
    {
        var files = new List<string>();

        await using (Stream sourceStream = new StreamReader(sourceFilePath).BaseStream)
        {
            var currentFile = 0L;
            while (sourceStream.Position < sourceStream.Length)
            {
                int numberOfBytes = 0;
                int totalBytes = 0;
                byte[] bufferToRead = new byte[256 * 1024];

                var filename = $"{++currentFile}.unsorted.txt";

                await using FileStream destinationStream = new FileStream(Path.Combine(_configs.Directory, filename),
                                                           FileMode.Create, FileAccess.Write);

                while (totalBytes < _configs.ChunkSize && (numberOfBytes = await sourceStream.ReadAsync(bufferToRead)) > 0)
                {
                    await destinationStream.WriteAsync(bufferToRead, 0, numberOfBytes);
                    totalBytes += numberOfBytes;
                }

                var bytes = new List<byte>();

                while (true)
                {
                    var @byte = sourceStream.ReadByte();

                    if (@byte == -1 || @byte == _configs.NewLineSeparator)
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
}
