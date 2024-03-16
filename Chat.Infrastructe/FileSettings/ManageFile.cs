using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
namespace Chat.Infrastructe.FileSettings
{
    public static class ManageFile
    {
        public static string UploadPhoto(this IWebHostEnvironment WebHost,IFormFile file,string PathName)
        {
            string src = "";
            string root = "wwwroot/";
            if (!Directory.Exists(root + $"Images/{PathName}"))
            {
                Directory.CreateDirectory(root + $"Images/{PathName}");
            }
            if(file is not null)
            {
                src = $"Images/{PathName}/" + Guid.NewGuid() + file.FileName;
                string path=Path.Combine(WebHost.ContentRootPath, root,src);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
                return src;
        }
        public static void RemovePhoto(this IWebHostEnvironment webHost,string OldFile) 
        {
            if(!string.IsNullOrEmpty(OldFile))
            {
                string root = "wwwroot/";
                string oldpath = Path.Combine(webHost.ContentRootPath, root,OldFile);
                File.Delete(oldpath);
            }
        }
    }
}
