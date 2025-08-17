using Microsoft.AspNetCore.Mvc;
using UserEnquiry.Services;

namespace UserEnquiry.Controllers
{
    /// <summary>
    /// Web API controller to Re-initialize user data
    /// (Created just for reference, if we need to use endpoint to initialize user data)
    /// </summary>
    /// <param name="provider"></param>
    [ApiController]
    [Route("[controller]")]
    public class UserDataInitializeController(IServiceProvider provider) : ControllerBase
    {
        private readonly IServiceProvider _provider = provider;

        [HttpGet("updateuserdata")]
        public IActionResult GetMerkleRootOfUsers()
        {   
            bool isInitialize = UserDataInitializeService.InitializeDailyData(_provider);

            if (isInitialize == false)
            {
                return BadRequest("Failed to update user data.");
            }

            return Ok("User data updated successfully.");
        }
    }
}
