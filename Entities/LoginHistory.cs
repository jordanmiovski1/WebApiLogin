namespace WebApi.Entities
{
    public class LoginHistory
    {
        public int id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public DateTime InsertedDate { get; set; }
        public int UserId  { get; set;}
    }

}
