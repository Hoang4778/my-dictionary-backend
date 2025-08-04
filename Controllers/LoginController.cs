using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using backend.Context;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    public class LoginController : Controller
    {
        private readonly AppDBContext _appDBContext;

        public LoginController(AppDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        [HttpPost]
        [Route("api/login")]
        public async Task<ActionResult> Login(LoginInfo loginInfo)
        {
            try
            {
                if (loginInfo != null)
                {
                    string emailValue = loginInfo.email.Trim();
                    string passwordValue = loginInfo.password.Trim();

                    if (string.IsNullOrEmpty(emailValue))
                    {
                        return BadRequest(new ServerResponse { code = 400, message = "Email value is empty. Please try again.", data = null });
                    }

                    if (string.IsNullOrEmpty(passwordValue))
                    {
                        return BadRequest(new ServerResponse { code = 400, message = "Password value is empty. Please try again.", data = null });
                    }

                    if (!Regex.IsMatch(emailValue, "^[^\\s@]+@[^\\s@]+\\.[^\\s@]+$"))
                    {
                        return BadRequest(new ServerResponse { code = 400, message = "Email value is not in the right format. Please try again.", data = null });
                    }

                    if (emailValue.Length > 30)
                    {
                        return BadRequest(new ServerResponse { code = 400, message = "Email value is longer than 30 characters. Please try again.", data = null });
                    }

                    if (passwordValue.Length > 20)
                    {
                        return BadRequest(new ServerResponse { code = 400, message = "Password value is longer than 20 characters. Please try again.", data = null });
                    }

                    var user = await _appDBContext.UserAccount.Where(user => user.email == loginInfo.email).FirstOrDefaultAsync();

                    if (user == null)
                    {
                        return NotFound(new ServerResponse { code = 404, message = "Account not existed. Please sign up before logging in.", data = null });
                    }
                    else
                    {
                        var user2 = await _appDBContext.UserAccount.Where(user => user.email == loginInfo.email && user.password == loginInfo.password).FirstOrDefaultAsync();

                        if (user2 == null)
                        {
                            return BadRequest(new ServerResponse { code = 400, message = "Wrong password. Please try again.", data = null });
                        }
                        else
                        {
                            return Ok(new ServerResponse { code = 200, message = "Logged in successfully.", data = new { userId = user2.id } });
                        }
                    }
                }
                else
                {
                    return BadRequest(new ServerResponse { code = 400, message = "Bad user login input. Please try again.", data = null });
                }
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ServerResponse { code = 500, message = error.Message, data = null });
            }
        }
    }
}
