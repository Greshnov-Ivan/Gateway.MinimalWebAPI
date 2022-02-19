namespace Gateway.MinimalWebAPI.Models
{
    public class UserWithAccountViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public Guid AccountId { get; set; }
        public int AccountType { get; set; }
    }
}
