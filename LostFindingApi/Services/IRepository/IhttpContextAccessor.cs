using Microsoft.AspNetCore.Http;

namespace LostFindingApi.Services.IRepository
{
    public interface IhttpContextAccessor
    {
        public IHttpContextAccessor getContext();
    }
}
