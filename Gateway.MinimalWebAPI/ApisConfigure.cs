namespace Gateway.MinimalWebAPI
{
    public class ApisConfigure
    {
        public BaseApisConfigure? UsersAPI { get; set; }
        public BaseApisConfigure? AccountsAPI { get; set; }
    }
    public class BaseApisConfigure
    {
        public string BaseAddress { get; set; } = string.Empty;
        public string EndPoints { get; set; } = string.Empty;
    }
}
