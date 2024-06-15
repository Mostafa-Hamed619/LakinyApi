using Microsoft.AspNetCore.Http;
using System;

namespace LostFindingApi.Services.IRepository
{
    public interface IFileRepository
    {
        public Tuple<int, string> SaveImage(IFormFile file);

        public bool DeleteImage(string fileName);

        public Tuple<int, string> SaveAccountImage(IFormFile file);

        public bool DeleteAccountImage(string fileName);

        public Tuple<int, string> SearchImage(IFormFile file);

        public bool DeleteImageSearch(string fileName);

        public Tuple<int, string> SaveChatFile(IFormFile file);

        public bool DeleteChatFile(string fileName);
    }
}
