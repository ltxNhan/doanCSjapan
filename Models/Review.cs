namespace JapanApp.Models
{
    public class Review
    {
        public int ReviewID { get; set; }

        public int UserID { get; set; }
        public User User { get; set; }

        public int FestivalID { get; set; }
        public Festival Festival { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}