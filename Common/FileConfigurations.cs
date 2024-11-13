namespace Common;

public class FileConfigurations
{
    public string Directory => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    public string FileName => "unsorted.txt";
    public string OutputFileName => "sorted.txt";
    public long FileSize => 4L * 1024 * 1024 * 1024;
    public long ChunkSize => 1L * 1024 * 1024;
    public int MinId => 100;
    public int MaxId => 1000000;
    public int StringMinLength => 5;
    public int StringMaxLength => 15;
    public string LinePattern => "{0}. {1}" + Environment.NewLine;
    public int NewLineSeparator => '\n';
}
