using backend.Context;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly AppDBContext _appDBContext;

        public AccountController(AppDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetAccountInfo(string accountId)
        {
            if (accountId == null)
            {
                return BadRequest(new ServerResponse { code = 400, message = "No account ID found. Please try again.", data = null });
            }
            else if (!int.TryParse(accountId, out var id))
            {
                return BadRequest(new ServerResponse { code = 400, message = "Account ID is not a number. Please try again.", data = null });
            }
            else
            {
                int realId = int.Parse(accountId);

                try
                {
                    var accountInfo = await _appDBContext.UserAccount.Where(account => account.id == realId).FirstOrDefaultAsync();

                    if (accountInfo != null)
                    {
                        accountInfo.password = "";

                        return Ok(new ServerResponse { code = 200, message = "Account info found.", data = accountInfo });
                    }
                    else
                    {
                        return NotFound(new ServerResponse { code = 404, message = "Account not existed. Please sign up before logging in.", data = null });
                    }
                }
                catch (Exception error)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ServerResponse { code = 500, message = error.Message, data = null });
                }
            }
        }
    }
}
