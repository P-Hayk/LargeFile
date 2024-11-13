using FileGenerator;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var configs = new FileConfigs();

        var logger = new Logger();

        logger.LogInfo("Started Generating a file. Please wait...");

        var generator = new Generator(configs);

        var path = await generator.GenerateAsync();

        logger.LogInfo($"File {path} successfully generated.");
    }
}