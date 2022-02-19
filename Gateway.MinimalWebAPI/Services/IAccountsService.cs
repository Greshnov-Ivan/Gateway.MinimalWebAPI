namespace Gateway.MinimalWebAPI.Services
{
    public interface IAccountsService
    {
        public Task ClearAccounts(CancellationToken cancellationToken);
    }
}
