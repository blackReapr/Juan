using Juan.Models;

namespace Juan.Extensions;

public static partial class Extension
{
    public static bool IsImage(this IFormFile file) => file.ContentType.Contains("image");
    public static bool DoesExceed(this IFormFile file, int size) => file.Length / 1024 > size;
    public static async Task<string> SaveAsync(this IFormFile file)
    {
        var filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        using FileStream fileStream = new(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images", filename), FileMode.Create);
        await file.CopyToAsync(fileStream);
        return filename;
    }
}
