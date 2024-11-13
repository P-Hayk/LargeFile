namespace Common;
public class Helper
{
    public static string GenerateRandomWord(int length)
    {
        if (length < 1)
        {
            throw new ArgumentException("Length must be greater than zero.");
        }

        var random = new Random();

        const string CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        char[] stringChars = new char[length];

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = CHARS[random.Next(CHARS.Length)];
        }

        return new string(stringChars);
    }

    public static bool TryParseLine(string line, out (int, string) result)
    {
        var parts = line.Split(new[] { "." }, StringSplitOptions.None);

        if (parts.Length == 2 && int.TryParse(parts[0], out int id))
        {
            result = (id, parts[1]);
            return true;
        }

        result = (0, string.Empty);
        return false;
    }

    public static bool IsSpaceEnough(string directory, long fileSize)
    {
        string driveLetter = Path.GetPathRoot(directory);
        DriveInfo driveInfo = new DriveInfo(driveLetter);

        long availableSpace = driveInfo.AvailableFreeSpace;

        return fileSize <= availableSpace;
    }

    public static long GigabyteToByte(long gb)
    {
        return gb * 1024 * 1024 * 1024;
    }
}
