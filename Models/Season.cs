namespace JapanApp.Models
{
    public class Season
    {
        public int SeasonID { get; set; }
        public string SeasonName { get; set; }

        public List<Festival> Festivals { get; set; }
    }
}