using System.Threading.Tasks;
using HybridEncryptionLogin.Models;
using HybridEncryptionLogin.Services.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace HybridEncryptionLogin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IAESService _aesService;
        private readonly IRSAService _rsaService;

        public AccountController(IMemoryCache cache, IAESService aesService, IRSAService rsaService)
        {
            _aesService = aesService;
            _rsaService = rsaService;
            _cache = cache;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            if (_cache.TryGetValue(userLogin.Email, out string privateKey))
            {
                _cache.Remove(userLogin.Email);

                byte[] symmetricKey = _rsaService.DecryptString(HttpContext.Request.Headers["symmetric-key-encrypted"], privateKey);

                string password = _aesService.DecryptString(userLogin.Password, symmetricKey);

                if (userLogin.Email == "example@test.com" && password == "test")
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return Forbid();
            }


        }
    }
}