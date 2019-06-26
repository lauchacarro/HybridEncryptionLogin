using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using HybridEncryptionLogin.Models;
using HybridEncryptionLogin.Utils;
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

        public AccountController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            if (_cache.TryGetValue(userLogin.Email, out string privateKey))
            {
                _cache.Remove(userLogin.Email);

                byte[] symmetricKey = RSAUtils.DecryptStringRSA(HttpContext.Request.Headers["symmetric-key-encrypted"], privateKey);

                string password = AESUtils.DecryptStringAES(userLogin.Password, symmetricKey);

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