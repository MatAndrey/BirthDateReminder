using System.Text.Json.Serialization;

namespace BirthDateReminder.Server.Models
{
    public enum UnitTypes { Day, Week, Month };
    public class Reminder
    {
        public int Id { get; set; }

        public UnitTypes UnitsType { get; set; } // тип единицы

        public int UnitsCount { get; set; } // За сколько единиц (дней, недель, месяцев) до ДР нужно напоминание

        public int BirthdayId { get; set; }

        [JsonIgnore]
        public BirthdayItem Birthday { get; set; }
    }
}
