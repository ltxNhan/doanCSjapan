using JapanApp.Data;
using JapanApp.Models;
using Microsoft.EntityFrameworkCore;

namespace JapanApp.Services
{
    public class FestivalService
    {
        private readonly AppDbContext _context;

        public FestivalService(AppDbContext context)
        {
            _context = context;
        }

        // 🔍 SEARCH + FILTER
        // 🔍 SEARCH + FILTER + REMOVE DUPLICATES
        public List<Festival> Search(string keyword, int? seasonId, int? regionId)
        {
            var query = _context.Festivals
                .Include(f => f.Region)
                .Include(f => f.Season)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(f => f.Name.Contains(keyword));
            }

            if (seasonId.HasValue && seasonId.Value > 0)
            {
                query = query.Where(f => f.SeasonID == seasonId.Value);
            }

            if (regionId.HasValue && regionId.Value > 0)
            {
                query = query.Where(f => f.RegionID == regionId.Value);
            }

            var festivals = query.ToList();

            // Xóa trùng khi hiển thị: cùng tên + cùng địa điểm thì chỉ lấy 1 cái
            var uniqueFestivals = festivals
                .GroupBy(f => new
                {
                    Name = (f.Name ?? "").Trim().ToLower(),
                    Location = (f.LocationName ?? "").Trim().ToLower()
                })
                .Select(g => g.First())
                .OrderBy(f => f.StartDate.Month)
                .ThenBy(f => f.StartDate.Day)
                .ToList();

            return uniqueFestivals;
        }

        // ❤️ FAVORITE
        public void AddFavorite(int userId, int festivalId)
        {
            var exists = _context.Favorites
                .Any(f => f.UserID == userId && f.FestivalID == festivalId);

            if (!exists)
            {
                _context.Favorites.Add(new Favorite
                {
                    UserID = userId,
                    FestivalID = festivalId,
                    SavedAt = DateTime.Now
                });

                _context.SaveChanges();
            }
        }

        // ⭐ REVIEW
        public void AddReview(int userId, int festivalId, int rating, string comment)
        {
            var exists = _context.Reviews
                .Any(r => r.UserID == userId && r.FestivalID == festivalId);

            if (exists)
            {
                return;
            }

            var review = new Review
            {
                UserID = userId,
                FestivalID = festivalId,
                Rating = rating,
                Comment = comment,
                ReviewDate = DateTime.Now
            };

            _context.Reviews.Add(review);
            _context.SaveChanges();
        }

        // 🧠 QUIZ
        public int GetSuggestedSeason(List<int> answerIds)
        {
            var result = _context.QuizAnswers
                .Where(a => answerIds.Contains(a.AnswerID))
                .GroupBy(a => a.SuggestSeasonID)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();

            return result?.Key ?? 1;
        }

        public List<Festival> GetBySeason(int seasonId)
        {
            return _context.Festivals
                .Include(f => f.Region)
                .Include(f => f.Season)
                .Where(f => f.SeasonID == seasonId)
                .ToList();
        }

        // 🔎 GET BY ID
        public Festival? GetById(int id)
        {
            return _context.Festivals
                .Include(f => f.Region)
                .Include(f => f.Season)
                .Include(f => f.Reviews)
                .ThenInclude(r => r.User)
                .FirstOrDefault(f => f.FestivalID == id);
        }

        // ➕ CREATE
        public void CreateFestival(Festival festival)
        {
            _context.Festivals.Add(festival);
            _context.SaveChanges();
        }

        // ✏️ UPDATE
        public void UpdateFestival(Festival festival)
        {
            var existing = _context.Festivals.Find(festival.FestivalID);

            if (existing != null)
            {
                existing.Name = festival.Name;
                existing.Content = festival.Content;
                existing.StartDate = festival.StartDate;
                existing.EndDate = festival.EndDate;
                existing.LocationName = festival.LocationName;
                existing.RegionID = festival.RegionID;
                existing.SeasonID = festival.SeasonID;
                existing.Latitude = festival.Latitude;
                existing.Longitude = festival.Longitude;

                _context.SaveChanges();
            }
        }

        // ❌ DELETE
        public void DeleteFestival(int id)
        {
            var festival = _context.Festivals.Find(id);

            if (festival != null)
            {
                _context.Festivals.Remove(festival);
                _context.SaveChanges();
            }
        }
    }
}