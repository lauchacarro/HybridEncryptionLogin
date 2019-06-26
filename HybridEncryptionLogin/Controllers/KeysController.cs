using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using HybridEncryptionLogin.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace HybridEncryptionLogin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeysController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        public KeysController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPublicKey(string email)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            if (_cache.TryGetValue(email, out string pem))
            {
                _cache.Remove(email);
            }

            string publickey = PemKeyUtils.GetPublicPEM(rsa);

            string privatekey = PemKeyUtils.GetPrivatePEM(rsa);

            _cache.Set(email, privatekey, new TimeSpan(1,0,0));
            
            return Ok( new { Key = publickey });
        }
    }
}