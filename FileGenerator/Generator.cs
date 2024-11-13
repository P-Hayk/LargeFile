using Common;

namespace FileGenerator;

public class Generator
{
    private FileConfigurations _configs;
    private readonly Logger _logger;

    public Generator(FileConfigurations configs)
    {
        _configs = configs ?? throw new ArgumentNullException(nameof(configs));
        _logger = new Logger();
    }

    public async Task<string> GenerateAsync()
    {
        if (_configs.FileSize < Helper.GigabyteToByte(1) || _configs.FileSize > Helper.GigabyteToByte(200))
        {
            _logger.LogError("File size must be between 1 to 200 GB.");
            throw new Exception();
        }

        if (!Helper.IsSpaceEnough(_configs.Directory, _configs.FileSize))
        {
            _logger.LogError("There is no enough space.");
            throw new Exception();
        }

        _logger.LogInfo("Started generating the file. It may take several minutes. Please wait.");

        Directory.CreateDirectory(_configs.Directory);

        string filePath = Path.Combine(_configs.Directory, _configs.FileName);

        long bytesWritten = 0;
        var random = new Random();

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            while (bytesWritten < _configs.FileSize)
            {
                int randomId = random.Next(_configs.MinId, _configs.MaxId);
                int stringLength = random.Next(_configs.StringMinLength, _configs.StringMaxLength);
                string randomString = Helper.GenerateRandomWord(stringLength);

                string line = string.Format(_configs.LinePattern, randomId, randomString);

                await writer.WriteAsync(line);
                bytesWritten += line.Length;
            }
        }

        return filePath;
    }


}
