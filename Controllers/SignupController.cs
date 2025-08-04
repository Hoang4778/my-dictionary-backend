using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.Text.Json;
using backend.Context;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace backend.Controllers
{
    [ApiController]
    public class SignupController : ControllerBase
    {
        private readonly AppDBContext _appDBContext;
        public SignupController(AppDBContext appDBContext)
        {
            _appDBContext = appDBContext;
        }

        [HttpPost]
        [Route("api/signup")]
        public async Task<IActionResult> Signup(SignupInfo signupInfo)
        {
            try
            {
                if (signupInfo != null)
                {
                    string emailValue = signupInfo.email.Trim();
                    string passwordValue = signupInfo.password.Trim();
                    string userNameValue = signupInfo.userName.Trim();

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

                    var user = await _appDBContext.UserAccount.Where(user => user.email == signupInfo.email).FirstOrDefaultAsync();

                    if (user != null)
                    {
                        return BadRequest(new ServerResponse { code = 400, message = "Account already existed. Please sign up under a different email address.", data = null });
                    }
                    else
                    {
                        Random random = new Random();

                        UserAccount newUser = new UserAccount
                        {
                            email = emailValue,
                            name = string.IsNullOrEmpty(userNameValue) ? $"User {random.Next()}" : userNameValue,
                            password = passwordValue,
                        };
                        _appDBContext.Add(newUser);
                        await _appDBContext.SaveChangesAsync();

                        return Ok(new ServerResponse { code = 200, message = "Account created successfully.", data = new { userId = newUser.id, userName = newUser.name } });
                    }
                }
                else
                {
                    return BadRequest(new ServerResponse { code = 400, message = "Bad user signup input. Please try again.", data = null });
                }
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ServerResponse { code = 500, message = error.Message, data = null });
            }
        }
    }
}
