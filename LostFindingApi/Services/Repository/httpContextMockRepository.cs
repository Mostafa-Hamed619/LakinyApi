using LostFindingApi.Services.IRepository;
using Microsoft.AspNetCore.Http;

namespace LostFindingApi.Services.Repository
{
    public class httpContextMockRepository : IhttpContextAccessor
    {
        private readonly IHttpContextAccessor context;

        public httpContextMockRepository(IHttpContextAccessor context)
        {
            this.context = context;
        }

        public IHttpContextAccessor getContext()
        {
            return context;
        }
    }
}
