namespace JapanApp.Models
{
    public class User
    {
        public int UserID { get; set; }

        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "User";

        public List<Review> Reviews { get; set; } = new();

        public List<Favorite> Favorites { get; set; } = new();
    }
}