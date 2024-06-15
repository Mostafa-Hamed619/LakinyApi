using LostFindingApi.Services.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;

namespace LostFindingApi.Services.Repository
{
    public class FileMockRepository : IFileRepository
    {
        private IWebHostEnvironment environment;

        public FileMockRepository(IWebHostEnvironment env)
        {
            this.environment = env;
        }  
        
        public Tuple<int, string> SaveImage(IFormFile file)
        {
            try
            {
                var contentPath = this.environment.ContentRootPath;
                var path = Path.Combine(contentPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory("Uploads");
                }

                var ext = Path.GetExtension(file.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };

                if (!allowedExtensions.Contains(ext))
                {
                    string Message = string.Format("Only {0} extensions are allowed", string.Join(",", allowedExtensions));
                    return new Tuple<int, string>(0, Message);
                }

                string uniqueString = Guid.NewGuid().ToString();
                var newFileName = uniqueString + ext;
                var fileWithPath = Path.Combine(path, newFileName);
                var stream = new FileStream(fileWithPath, FileMode.Create);
                file.CopyTo(stream);
                stream.Close();
                return new Tuple<int, string>(1, newFileName);
            }catch (Exception ex)
            {
                return new Tuple<int, string>(0, "Error has occured");
            }
        }

        public Tuple<int, string> SearchImage(IFormFile file)
        {
            try
            {
                var contentPath = this.environment.ContentRootPath;
                var path = Path.Combine(contentPath, "Search");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory("Search");
                }

                var ext = Path.GetExtension(file.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };

                if (!allowedExtensions.Contains(ext))
                {
                    string Message = string.Format("Only {0} extensions are allowed", string.Join(",", allowedExtensions));
                    return new Tuple<int, string>(0, Message);
                }

                string uniqueString = Path.GetFileNameWithoutExtension(file.FileName);

                var newFileName = uniqueString + ext;
                var fileWithPath = Path.Combine(path, newFileName);
                var stream = new FileStream(fileWithPath, FileMode.Create);
                file.CopyTo(stream);
                stream.Close();
                return new Tuple<int, string>(1, newFileName);
            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(0, "Error has occured");
            }
        }
        
        public Tuple<int, string> SaveAccountImage(IFormFile file)
        {
            try
            {
                var contentPath = this.environment.ContentRootPath;
                var path = Path.Combine(contentPath, "Accounts");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory("Accounts");
                }

                var ext = Path.GetExtension(file.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };

                if (!allowedExtensions.Contains(ext))
                {
                    string Message = string.Format("Only {0} extensions are allowed", string.Join(",", allowedExtensions));
                    return new Tuple<int, string>(0, Message);
                }

                string uniqueString = Guid.NewGuid().ToString();
                var newFileName = uniqueString + ext;
                var fileWithPath = Path.Combine(path, newFileName);
                var stream = new FileStream(fileWithPath, FileMode.Create);
                file.CopyTo(stream);
                stream.Close();
                return new Tuple<int, string>(1, newFileName);
            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(0, "Error has occured");
            }
        }

        public bool DeleteImage(string fileName)
        {
            try
            {
                var wwwPath = this.environment.ContentRootPath;
                var path = Path.Combine(wwwPath, "Uploads\\", fileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DeleteImageSearch(string fileName)
        {
            try
            {
                var wwwPath = this.environment.ContentRootPath;
                var path = Path.Combine(wwwPath, "Search\\", fileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool DeleteAccountImage(string fileName)
        {
            try
            {
                var wwwPath = this.environment.ContentRootPath;
                var path = Path.Combine(wwwPath, "Accounts\\", fileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Tuple<int, string> SaveChatFile(IFormFile file)
        {
            try
            {
                var contentPath = this.environment.ContentRootPath;
                var path = Path.Combine(contentPath, "chatFiles");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory("chatFiles");
                }

                var ext = Path.GetExtension(file.FileName);
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };

                if (!allowedExtensions.Contains(ext))
                {
                    string Message = string.Format("Only {0} extensions are allowed", string.Join(",", allowedExtensions));
                    return new Tuple<int, string>(0, Message);
                }

                string uniqueString = Guid.NewGuid().ToString();
                var newFileName = uniqueString + ext;
                var fileWithPath = Path.Combine(path, newFileName);
                var stream = new FileStream(fileWithPath, FileMode.Create);
                file.CopyTo(stream);
                stream.Close();
                return new Tuple<int, string>(1, newFileName);
            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(0, "Error has occured");
            }
        }

        public bool DeleteChatFile(string fileName)
        {
            try
            {
                var wwwPath = this.environment.ContentRootPath;
                var path = Path.Combine(wwwPath, "chatFiles\\", fileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
