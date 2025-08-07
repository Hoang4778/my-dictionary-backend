using backend.Context;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/favorite-words")]
    public class FavoriteWordController : Controller
    {
        private readonly AppDBContext _appDBContext;

        public FavoriteWordController(AppDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetFavoriteWords(int userId)
        {
            if (userId == null || userId.GetType() != typeof(int))
            {
                return BadRequest(new ServerResponse { code = 400, message = "Bad user input. Please try again.", data = null });
            }

            try
            {
                var favoriteWords = await _appDBContext.FavoriteWord.Where(word => word.userId == userId).ToListAsync();

                return Ok(new ServerResponse { code = 200, message = $"Favorite words found for user ID #{userId}", data = favoriteWords });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ServerResponse { code = 500, message = error.Message, data = null });
            }
        }

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddFavoriteWords(FavoriteWordsInfo wordInput)
        {
            if (wordInput == null)
            {
                return BadRequest(new ServerResponse { code = 400, message = "Bad user input. Please try again.", data = null });
            }

            if (wordInput.userId == null || wordInput.userId.GetType() != typeof(int))
            {
                return BadRequest(new ServerResponse { code = 400, message = "No bad user ID input. Please try again.", data = null });
            }

            if (wordInput.favoriteWords.Count == 0)
            {
                return BadRequest(new ServerResponse { code = 400, message = "No favorite word submitted. Please try again.", data = null });
            }

            foreach (var word in wordInput.favoriteWords)
            {
                if (word is string)
                {
                    string wordValue = word.Trim();

                    if (string.IsNullOrEmpty(wordValue))
                    {
                        return BadRequest(new ServerResponse { code = 400, message = "One of the words is an empty string. Please try again.", data = null });
                    }
                }
                else
                {
                    return BadRequest(new ServerResponse { code = 400, message = "One of the words is not text. Please try again.", data = null });
                }
            }

            try
            {
                List<FavoriteWord> wordList = new List<FavoriteWord>();
                foreach (var word in wordInput.favoriteWords)
                {
                    string wordValue = word.Trim();
                    FavoriteWord favoriteWord = new FavoriteWord
                    {
                        entry = wordValue,
                        userId = wordInput.userId,
                    };
                    wordList.Add(favoriteWord);
                }

                _appDBContext.FavoriteWord.AddRange(wordList);
                await _appDBContext.SaveChangesAsync();

                return Ok(new ServerResponse { code = 200, message = "Favorite words added successfully.", data = new { userId = wordInput.userId } });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ServerResponse { code = 500, message = error.Message, data = null });
            }
        }

        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> RemoveFavoriteWords(FavoriteWordsInfo wordInput)
        {
            if (wordInput == null)
            {
                return BadRequest(new ServerResponse { code = 400, message = "Bad user input. Please try again.", data = null });
            }

            if (wordInput.userId == null || wordInput.userId.GetType() != typeof(int))
            {
                return BadRequest(new ServerResponse { code = 400, message = "No bad user ID input. Please try again.", data = null });
            }

            if (wordInput.favoriteWords.Count == 0)
            {
                return BadRequest(new ServerResponse { code = 400, message = "No favorite word submitted. Please try again.", data = null });
            }

            foreach (var word in wordInput.favoriteWords)
            {
                if (word is string)
                {
                    string wordValue = word.Trim();

                    if (string.IsNullOrEmpty(wordValue))
                    {
                        return BadRequest(new ServerResponse { code = 400, message = "One of the words is an empty string. Please try again.", data = null });
                    }
                }
                else
                {
                    return BadRequest(new ServerResponse { code = 400, message = "One of the words is not text. Please try again.", data = null });
                }
            }

            try
            {
                var favoriteWords = await _appDBContext.FavoriteWord.Where(word => wordInput.favoriteWords.Contains(word.entry) && word.userId == wordInput.userId).ToListAsync();
                _appDBContext.FavoriteWord.RemoveRange(favoriteWords);
                await _appDBContext.SaveChangesAsync();

                return Ok(new ServerResponse { code = 200, message = "Favorite words removed successfully.", data = new { userId = wordInput.userId } });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ServerResponse { code = 500, message = error.Message, data = null });
            }
        }
    }
}
