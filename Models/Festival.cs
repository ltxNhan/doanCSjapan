namespace JapanApp.Models
{
    public class Festival
    {
        public int FestivalID { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string LocationName { get; set; } = string.Empty;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public int RegionID { get; set; }
        public Region? Region { get; set; }

        public int SeasonID { get; set; }
        public Season? Season { get; set; }

        public List<Review> Reviews { get; set; } = new List<Review>();
        public List<Favorite> Favorites { get; set; } = new List<Favorite>();
    }
}