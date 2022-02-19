using Gateway.MinimalWebAPI.Models;

namespace Gateway.MinimalWebAPI.Services
{
    public interface IGrpcService
    {
        public Task<List<UserWithAccountViewModel>> GetUsersWithAccounts();
    }
}
