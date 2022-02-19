using Accounts.MinimalWebAPI;
using Gateway.MinimalWebAPI.Models;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Users.MinimalWebAPI;

namespace Gateway.MinimalWebAPI.Services
{
    public class GrpcService: IGrpcService
    {
        private readonly ILogger<GrpcService> _logger;
        private readonly BaseApisConfigure _usersConf;
        private readonly BaseApisConfigure _accountsConf;
        public GrpcService(IOptions<ApisConfigure> options, ILogger<GrpcService> logger) => (_logger, _usersConf, _accountsConf) = (logger, options.Value.UsersAPI, options.Value.AccountsAPI);
        public async Task<List<UserWithAccountViewModel>> GetUsersWithAccounts()
        {
            List<UserWithAccountViewModel> result;

            Task<List<User>> getUsers = Task.Run(() => GetUsersStream());
            Task<List<Account>> getAccounts = Task.Run(() => GetAccountsStream());

            var allTask = Task.WhenAll(getUsers, getAccounts);
            try
            {
                await allTask;
                List<User> listUsers = getUsers.Result;
                List<Account> listAccounts = getAccounts.Result;

                result = listAccounts.Join(listUsers, // второй набор
                    a => a.UserId, // свойство-селектор объекта из первого набора
                    u => u.Id, // свойство-селектор объекта из второго набора
                    (a, u) => new UserWithAccountViewModel() { UserId = u.Id, FullName = u.Name, AccountType = a.AccountType, AccountId = a.Id }).ToList();
            }
            catch
            {
                foreach (Exception ex in allTask.Exception.InnerExceptions)
                {
                    _logger.LogError($"Oops, something went wrong: {ex.Message}");
                }

                throw new Exception();
            }

            return result;
        }
        private async Task<List<User>> GetUsersStream()
        {
            List<User> result = new List<User>();

            using var channel = GrpcChannel.ForAddress(_usersConf.BaseAddressGrpc);

            var client = new UsersGrpc.UsersGrpcClient(channel);

            using (var call = client.GetListUsers(new GetUsersRequest()))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    UserResponse userResponse = call.ResponseStream.Current;
                    result.Add(new User() { Id = userResponse.Id, Name = userResponse.Name });
                }
            }
            channel.ShutdownAsync().Wait();

            return result;
        }
        private async Task<List<Account>> GetAccountsStream()
        {
            List<Account> result = new List<Account>();

            using var channel = GrpcChannel.ForAddress(_accountsConf.BaseAddressGrpc);

            var client = new AccountsGrpc.AccountsGrpcClient(channel);

            using (var call = client.GetListAccounts(new GetAccountsRequest()))
            {
                while (await call.ResponseStream.MoveNext())
                {
                    AccountResponse accountResponse = call.ResponseStream.Current;
                    result.Add(new Account() { Id = new Guid(accountResponse.Id), AccountType = accountResponse.AccountType, UserId = accountResponse.UserId });
                }
            }
            channel.ShutdownAsync().Wait();

            return result;
        }
    }
}
