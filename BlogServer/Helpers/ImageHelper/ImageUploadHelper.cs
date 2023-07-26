using BlogServer.Consts.UploadPaths;
using BlogServer.Helpers.FileNameHelper;


namespace BlogServer.Helpers.ImageHelper
{
    public class ImageUploadHelper : FileNameHelper.FileOptions
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ImageUploadHelper(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<(string fileName, string pathName)> UploadImageAsync(IFormFile file)
        {
            string pathName = BlogImageUploadPath.BlogImagePath;
            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, pathName);

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            (string fileName, string path) values = new();

            string newFileName = await FileRenameAsync(pathName, file.FileName);

            await using FileStream fileStream = new($"{uploadPath}\\{newFileName}", 
                                                FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: false);
            await file.CopyToAsync(fileStream);
            await fileStream.FlushAsync();
            values.fileName = newFileName;
            values.path = $"{pathName}\\{newFileName}";
            return values;
        }
    }
}
