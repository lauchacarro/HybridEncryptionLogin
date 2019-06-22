using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HybridEncryptionLogin.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HybridEncryptionLogin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            if(userLogin.Email == "example@test.com" && userLogin.Password == "test")
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