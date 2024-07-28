namespace Juan.Helpers;

public class DeleteFile
{
    public static void Delete(string? fileName)
    {
        if (fileName != null)
        {
            string oldFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "img", "slider", fileName);
            if (File.Exists(oldFile)) File.Delete(oldFile);
        }
    }
}
