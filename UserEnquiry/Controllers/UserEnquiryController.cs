using Microsoft.AspNetCore.Mvc;

namespace UserEnquiry.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserEnquiryController : ControllerBase
    {
        private readonly UserEnquiryService _userEnquiryService;

        public UserEnquiryController(UserEnquiryService userEnquiryService)
        {
            _userEnquiryService = userEnquiryService;
        }

        [HttpGet("getmerklerootofusers")]
        public IActionResult GetMerkleRootOfUsers()
        {
            var userMerkleTree = _userEnquiryService.GetMerkleRootOfUsers();
            if (userMerkleTree == null)
            {
                return BadRequest("Failed to initialize user Merkle tree.");
            }
            return Ok(userMerkleTree);
        }
    }
}
