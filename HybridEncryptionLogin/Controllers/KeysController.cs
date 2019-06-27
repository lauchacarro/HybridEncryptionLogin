using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using HybridEncryptionLogin.Services.Abstracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace HybridEncryptionLogin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeysController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IRSAService _rsaService;

        public KeysController(IMemoryCache cache, IRSAService rsaService)
        {
            _rsaService = rsaService;
            _cache = cache;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetPublicKey(string email)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            if (_cache.TryGetValue(email, out _))
            {
                _cache.Remove(email);
            }

            string publickey = _rsaService.GetPublicPEM(rsa);

            string privatekey = _rsaService.GetPrivatePEM(rsa);

            _cache.Set(email, privatekey, new TimeSpan(1, 0, 0));

            return Ok(new { Key = publickey });
        }
    }
}