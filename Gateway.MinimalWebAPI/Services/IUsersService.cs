namespace Gateway.MinimalWebAPI.Services
{
    public interface IUsersService
    {
        public Task InitialData(CancellationToken cancellationToken);
        public Task ClearUsers(CancellationToken cancellationToken);
    }
}
