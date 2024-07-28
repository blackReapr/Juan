namespace Juan.Helpers;

public class DeleteFile
{
    public static void Delete(string? fileName, string rootFolder)
    {
        if (fileName != null)
        {
            string oldFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "img", rootFolder, fileName);
            if (File.Exists(oldFile)) File.Delete(oldFile);
        }
    }
}
