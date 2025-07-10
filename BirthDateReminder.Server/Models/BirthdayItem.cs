namespace BirthDateReminder.Server.Models
{
    public class BirthdayItem
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public string Name { get; set; }
        public DateOnly BirthDate { get; set; }
        public string? ImagePath { get; set; }
    }
}
