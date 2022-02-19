using Microsoft.Extensions.Options;

namespace Gateway.MinimalWebAPI.Services
{
    public class AccountsService: IAccountsService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _baseAddress;
        private readonly string _endPoint;
        public AccountsService(IHttpClientFactory httpClientFactory, IOptions<ApisConfigure> options) =>
            (_clientFactory, _baseAddress, _endPoint) = (httpClientFactory, options.Value.AccountsAPI.BaseAddress, options.Value.AccountsAPI.EndPoints);

        public async Task ClearAccounts(CancellationToken cancellationToken)
        {
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_baseAddress);
            var endPoint = _endPoint;

            var response = await client.DeleteAsync(endPoint, cancellationToken);
        }
    }
}
