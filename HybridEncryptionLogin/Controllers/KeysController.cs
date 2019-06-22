using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
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
        public async Task<IActionResult> GetPublicKey()
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            _cache.Set(rsa.ExportCspBlob(false), rsa.ExportCspBlob(true));
            return Ok(rsa.ExportCspBlob(false));
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetPrivateKey(string publickey)
        {

            byte[] privateKey = _cache.Get<byte[]>(Convert.FromBase64String(publickey));
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportCspBlob(privateKey);

           
            return Ok(rsa.ExportCspBlob(true));
        }
    }
}