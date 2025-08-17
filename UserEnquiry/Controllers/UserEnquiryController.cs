using Microsoft.AspNetCore.Mvc;
using UserEnquiry.Services;

namespace UserEnquiry.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserEnquiryController(UserEnquiryService userEnquiryService) : ControllerBase
    {
        private readonly UserEnquiryService _userEnquiryService = userEnquiryService;

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

        [HttpGet("getmerkleproofofuser")]
        public IActionResult GetMerkleProofOfUser(string UserId)
        {
            var userMerkleProof = _userEnquiryService.GetMerkleProofOfUser(UserId);
            if (userMerkleProof == null)
            {
                return BadRequest("Failed to find merkle root for given user");
            }
            return Ok(userMerkleProof);
        }
    }
}
