namespace FormRequest.Models
{
    public class Notifications
    {
        public int Id { get; set; }
        public string UserId { get; set; }  // FK to Users
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
