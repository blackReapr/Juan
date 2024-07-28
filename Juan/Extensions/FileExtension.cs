using Juan.Models;

namespace Juan.Extensions;

public static partial class Extension
{
    public static bool IsImage(this IFormFile formFile) => formFile.ContentType.Contains("image");
    public static bool DoesSizeExceed(this IFormFile formFile, int size) => formFile.Length / 1024 > size;
    public static async Task<string> SaveFileAsync(this IFormFile formFile, string? rootFolder = null)
    {
        string filename = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);
        string filePath;
        if (rootFolder == null) filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "img", filename);

        else filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "img", rootFolder, filename);

        using FileStream fileStream = new(filePath, FileMode.Create);
        await formFile.CopyToAsync(fileStream);
        return filename;
    }

    public static IEnumerable<string> VerifyFile(this IFormFile? formFile, int size = 100)
    {
        List<string> errors = new();
        if (formFile == null)
        {
            errors.Add("Image is required");
            return errors;
        }
        if (!formFile.IsImage()) errors.Add("Invalid file type");
        if (!formFile.DoesSizeExceed(size)) errors.Add("File size is too large");
        return errors;
    }
}
