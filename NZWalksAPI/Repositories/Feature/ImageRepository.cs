using NZWalksAPI.Data;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Repositories.Base;

namespace NZWalksAPI.Repositories.Feature;

public class ImageRepository : IImageRepository
{
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly NZWalksDbContext dbContext;

    public ImageRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, NZWalksDbContext dbContext)
    {
        this.webHostEnvironment = webHostEnvironment;
        this.httpContextAccessor = httpContextAccessor;
        this.dbContext = dbContext;
    }
    
    public async Task<Image> UploadImage(Image image)
    {
        var localFilePath = Path.Combine(webHostEnvironment.ContentRootPath, "Images", 
            $"{image.FileName}{image.FileExtension}");  

        // Upload Image to Local Path
        using var stream = new FileStream(localFilePath, FileMode.Create);
        await image.File.CopyToAsync(stream);

        // https://localhost:1234/images/image.jpg

        var urlFilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/Image/{image.FileName}{image.FileExtension}";

        image.FilePath = urlFilePath;


        // Add Image to the Images table
        await dbContext.Images.AddAsync(image);
        await dbContext.SaveChangesAsync();

        return image;
    }
}