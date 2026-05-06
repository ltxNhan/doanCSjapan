namespace JapanApp.Models
{
    public class Favorite
    {
        public int UserID { get; set; }
        public int FestivalID { get; set; }

        public User? User { get; set; }
        public Festival? Festival { get; set; }

        public DateTime SavedAt { get; set; } = DateTime.Now;
    }
}