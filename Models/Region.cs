namespace JapanApp.Models
{
    public class Region
    {
        public int RegionID { get; set; }
        public string RegionName { get; set; }

        public List<Festival> Festivals { get; set; }
    }
}