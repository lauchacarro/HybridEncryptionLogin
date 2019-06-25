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
            string password = string.Empty;
            if (_cache.TryGetValue(userLogin.Email, out string privateKey))
            {
                _cache.Remove(userLogin.Email);

                RSACryptoServiceProvider provider =  PemKeyUtils.GetRSAProviderFromPEM(privateKey);

                byte[] CiphertextData = Convert.FromBase64String(userLogin.Password);

                password = Encoding.UTF8.GetString(provider.Decrypt(CiphertextData, false));

            }
            if (userLogin.Email == "example@test.com" && password == "test")
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}