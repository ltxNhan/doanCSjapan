using JapanApp.Data;
using JapanApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace JapanApp.Controllers
{
    public class QuizController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public QuizController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await SeedQuizIfEmpty();

            var questions = await _context.QuizQuestions
                .Include(q => q.Answers)
                .ToListAsync();

            return View(questions);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(List<int> answerIds)
        {
            if (answerIds == null || answerIds.Count == 0)
            {
                TempData["Error"] = "Vui lòng chọn câu trả lời.";
                return RedirectToAction("Index");
            }

            var selectedAnswers = await _context.QuizAnswers
                .Where(a => answerIds.Contains(a.AnswerID))
                .ToListAsync();

            if (selectedAnswers.Count == 0)
            {
                TempData["Error"] = "Không tìm thấy câu trả lời hợp lệ.";
                return RedirectToAction("Index");
            }

            int totalScore = selectedAnswers.Sum(a => a.Points);
            double avgScore = selectedAnswers.Average(a => a.Points);

            int maxScore = selectedAnswers.Count * 40;
            int matchPercent = (int)Math.Round((double)totalScore / maxScore * 100);

            string suggestedSeason = GetSeasonByAverageScore(avgScore);

            var festivals = await _context.Festivals
                .Include(f => f.Season)
                .Where(f => f.Season != null && f.Season.SeasonName.Contains(suggestedSeason))
                .Take(6)
                .ToListAsync();

            if (festivals.Count == 0)
            {
                festivals = await _context.Festivals
                    .Include(f => f.Season)
                    .Take(6)
                    .ToListAsync();
            }

            string prompt = BuildPrompt(selectedAnswers, suggestedSeason, totalScore, matchPercent, festivals);
            string aiResult = await CallGeminiAI(prompt);

            ViewBag.TotalScore = totalScore;
            ViewBag.MatchPercent = matchPercent;
            ViewBag.SuggestedSeason = suggestedSeason;
            ViewBag.AIResult = aiResult;

            return View("Result", festivals);
        }

        private string GetSeasonByAverageScore(double avgScore)
        {
            if (avgScore <= 15) return "Xuân";
            if (avgScore <= 25) return "Hạ";
            if (avgScore <= 35) return "Thu";
            return "Đông";
        }

        private string BuildPrompt(List<QuizAnswer> answers, string season, int totalScore, int matchPercent, List<Festival> festivals)
        {
            string choices = string.Join(", ", answers.Select(a => a.AnswerText));
            string festivalNames = festivals.Count > 0
                ? string.Join(", ", festivals.Select(f => f.Name))
                : "Không có lễ hội cụ thể trong database";

            return $@"
Bạn là chuyên gia tư vấn du lịch và văn hóa Nhật Bản.

Người dùng vừa làm quiz về sở thích lễ hội Nhật Bản.
Các lựa chọn của người dùng: {choices}
Tổng điểm nội bộ: {totalScore}
Độ phù hợp hiển thị: {matchPercent}%
Mùa phù hợp theo hệ thống: {season}
Danh sách lễ hội hệ thống đang gợi ý: {festivalNames}

Hãy viết phần phân tích cá nhân hóa bằng tiếng Việt.
Yêu cầu:
- Viết tự nhiên, thân thiện.
- Khoảng 4 đến 6 câu.
- Giải thích vì sao người dùng phù hợp với mùa {season}.
- Nhắc đến một vài lễ hội phù hợp nếu có.
- Không dùng markdown.
";
        }

        private async Task<string> CallGeminiAI(string prompt)
        {
            string? apiKey = _configuration["Gemini:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return "AI chưa được cấu hình API key. Hệ thống vẫn gợi ý lễ hội dựa trên điểm số và sở thích bạn đã chọn.";
            }

            try
            {
                using var client = new HttpClient();

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                string json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";

                var response = await client.PostAsync(url, content);
                string responseText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return "AI hiện chưa phản hồi được. Hệ thống vẫn hiển thị kết quả gợi ý dựa trên điểm số quiz của bạn.";
                }

                dynamic? result = JsonConvert.DeserializeObject(responseText);
                string? aiText = result?.candidates?[0]?.content?.parts?[0]?.text;

                if (string.IsNullOrWhiteSpace(aiText))
                {
                    return "AI chưa tạo được nội dung phân tích. Hệ thống vẫn gợi ý lễ hội dựa trên sở thích của bạn.";
                }

                return aiText;
            }
            catch
            {
                return "Có lỗi khi kết nối AI. Hệ thống vẫn gợi ý lễ hội dựa trên điểm số quiz của bạn.";
            }
        }

        private async Task SeedQuizIfEmpty()
        {
            bool hasAnswers = await _context.QuizQuestions
                .Include(q => q.Answers)
                .AnyAsync(q => q.Answers.Any());

            if (hasAnswers) return;

            _context.QuizAnswers.RemoveRange(_context.QuizAnswers);
            _context.QuizQuestions.RemoveRange(_context.QuizQuestions);
            await _context.SaveChangesAsync();

            var questions = new List<QuizQuestion>
{
    new QuizQuestion
    {
        QuestionText = "Bạn thích mùa nào nhất?",
        Answers = new List<QuizAnswer>
        {
            new QuizAnswer { AnswerText = "Mùa xuân", Points = 10 },
            new QuizAnswer { AnswerText = "Mùa hè", Points = 20 },
            new QuizAnswer { AnswerText = "Mùa thu", Points = 30 },
            new QuizAnswer { AnswerText = "Mùa đông", Points = 40 }
        }
    },

    new QuizQuestion
    {
        QuestionText = "Bạn thích không khí lễ hội như thế nào?",
        Answers = new List<QuizAnswer>
        {
            new QuizAnswer { AnswerText = "Nhẹ nhàng", Points = 10 },
            new QuizAnswer { AnswerText = "Sôi động", Points = 20 },
            new QuizAnswer { AnswerText = "Truyền thống", Points = 30 },
            new QuizAnswer { AnswerText = "Độc đáo", Points = 40 }
        }
    },

    new QuizQuestion
    {
        QuestionText = "Bạn thích hoạt động nào nhất?",
        Answers = new List<QuizAnswer>
        {
            new QuizAnswer { AnswerText = "Ngắm hoa", Points = 10 },
            new QuizAnswer { AnswerText = "Xem pháo hoa", Points = 20 },
            new QuizAnswer { AnswerText = "Tham quan đền chùa", Points = 30 },
            new QuizAnswer { AnswerText = "Ngắm tuyết", Points = 40 }
        }
    },

    new QuizQuestion
    {
        QuestionText = "Bạn thích đi du lịch cùng ai?",
        Answers = new List<QuizAnswer>
        {
            new QuizAnswer { AnswerText = "Một mình", Points = 10 },
            new QuizAnswer { AnswerText = "Bạn bè", Points = 20 },
            new QuizAnswer { AnswerText = "Gia đình", Points = 30 },
            new QuizAnswer { AnswerText = "Người yêu", Points = 40 }
        }
    },

    new QuizQuestion
    {
        QuestionText = "Bạn thích món ăn lễ hội nào?",
        Answers = new List<QuizAnswer>
        {
            new QuizAnswer { AnswerText = "Takoyaki", Points = 10 },
            new QuizAnswer { AnswerText = "Yakitori", Points = 20 },
            new QuizAnswer { AnswerText = "Sushi", Points = 30 },
            new QuizAnswer { AnswerText = "Ramen", Points = 40 }
        }
    },

    new QuizQuestion
    {
        QuestionText = "Bạn thích địa điểm nào ở Nhật?",
        Answers = new List<QuizAnswer>
        {
            new QuizAnswer { AnswerText = "Tokyo", Points = 10 },
            new QuizAnswer { AnswerText = "Osaka", Points = 20 },
            new QuizAnswer { AnswerText = "Kyoto", Points = 30 },
            new QuizAnswer { AnswerText = "Hokkaido", Points = 40 }
        }
    },

    new QuizQuestion
    {
        QuestionText = "Bạn thích màu sắc nào nhất?",
        Answers = new List<QuizAnswer>
        {
            new QuizAnswer { AnswerText = "Hồng", Points = 10 },
            new QuizAnswer { AnswerText = "Đỏ", Points = 20 },
            new QuizAnswer { AnswerText = "Cam lá", Points = 30 },
            new QuizAnswer { AnswerText = "Trắng", Points = 40 }
        }
    },

    new QuizQuestion
    {
        QuestionText = "Bạn thích kiểu trải nghiệm nào?",
        Answers = new List<QuizAnswer>
        {
            new QuizAnswer { AnswerText = "Chụp ảnh sống ảo", Points = 10 },
            new QuizAnswer { AnswerText = "Vui chơi náo nhiệt", Points = 20 },
            new QuizAnswer { AnswerText = "Tìm hiểu văn hóa", Points = 30 },
            new QuizAnswer { AnswerText = "Khám phá thiên nhiên", Points = 40 }
        }
    },

    new QuizQuestion
    {
        QuestionText = "Bạn thích thời tiết nào?",
        Answers = new List<QuizAnswer>
        {
            new QuizAnswer { AnswerText = "Mát mẻ", Points = 10 },
            new QuizAnswer { AnswerText = "Nắng nóng", Points = 20 },
            new QuizAnswer { AnswerText = "Se lạnh", Points = 30 },
            new QuizAnswer { AnswerText = "Lạnh có tuyết", Points = 40 }
        }
    },

    new QuizQuestion
    {
        QuestionText = "Bạn muốn lễ hội mang lại cảm giác gì?",
        Answers = new List<QuizAnswer>
        {
            new QuizAnswer { AnswerText = "Thư giãn", Points = 10 },
            new QuizAnswer { AnswerText = "Hào hứng", Points = 20 },
            new QuizAnswer { AnswerText = "Hoài cổ", Points = 30 },
            new QuizAnswer { AnswerText = "Ấn tượng mạnh", Points = 40 }
        }
    }
};
            _context.QuizQuestions.AddRange(questions);
            await _context.SaveChangesAsync();
        }
    }
}