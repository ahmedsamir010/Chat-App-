using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Chat.Application.Helpers.FileSettings
{
    public static class ManageFile
    {
        public static string UploadPhoto(IWebHostEnvironment webHost, IFormFile file, string PathName)
        {
            string src = "";
            string root = "wwwroot/";
            if (!Directory.Exists(Path.Combine(root, $"Images/{PathName}")))
            {
                Directory.CreateDirectory(Path.Combine(root, $"Images/{PathName}"));
            }
            if (file is not null)
            {
                src = $"Images/{PathName}/" + Guid.NewGuid() + file.FileName;
                string path = Path.Combine(webHost.ContentRootPath, root, src);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }
            return src;
        }

        public static void RemovePhoto(IWebHostEnvironment webHost, string OldFile)
        {
            if (!string.IsNullOrEmpty(OldFile))
            {
                string root = "wwwroot/";
                string oldpath = Path.Combine(webHost.ContentRootPath, root, OldFile);
                File.Delete(oldpath);
            }
        }
    }
}
