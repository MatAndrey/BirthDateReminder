namespace BirthDateReminder.Server.Services
{
    public class ImageService
    {
        private readonly string _imagePath;

        public ImageService(IConfiguration config)
        {
            _imagePath = Path.Combine(Directory.GetCurrentDirectory(),
                                    config["FileStorage:ImagePath"]);

            if (!Directory.Exists(_imagePath))
                Directory.CreateDirectory(_imagePath);
        }

        public async Task<string> SaveImage(IFormFile file)
        {
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_imagePath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/images/{fileName}";
        }

        public void DeleteImage(string imageUrl)
        {
            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_imagePath, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
