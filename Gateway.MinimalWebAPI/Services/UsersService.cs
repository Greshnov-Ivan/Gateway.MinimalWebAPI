using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;

namespace Gateway.MinimalWebAPI.Services
{
    public class UsersService: IUsersService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _baseAddress;
        private readonly string _endPoint;
        public UsersService(IHttpClientFactory httpClientFactory, IOptions<ApisConfigure> options) => 
            (_clientFactory, _baseAddress, _endPoint) = (httpClientFactory, options.Value.UsersAPI.BaseAddress, options.Value.UsersAPI.EndPoints);

        public async Task InitialData(CancellationToken cancellationToken)
        {
            #region Получение списка пользователей из файла для дефолтной инициализации
            string path = @"Files\FIO_1000.txt";
            List<string> names = File.ReadAllLines(path).ToList();
            #endregion

            await StartSendNamesForCreateUsers(names, cancellationToken);
        }
        public async Task ClearUsers(CancellationToken cancellationToken)
        {
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_baseAddress);
            var endPoint = _endPoint;

            var response = await client.DeleteAsync(endPoint, cancellationToken);
        }

        private async Task StartSendNamesForCreateUsers(List<string> names, CancellationToken cancellationToken)
        {
            var block = new ActionBlock<string>(
                i => CreateUser(i, cancellationToken),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 50 });

            foreach(string name in names)
            {
                block.Post(name);
            }

            block.Complete();
            await block.Completion;
        }

        private async Task CreateUser(string name, CancellationToken cancellationToken)
        {
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(_baseAddress);
            var endPoint = _endPoint;

            var json = JsonSerializer.Serialize(new { name = name });
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endPoint, data, cancellationToken);
        }
    }
}
