using MerkleTree;
using UserEnquiry.DBContext;
using UserEnquiry.Models;
using UserEnquiry.Repository;

namespace UserEnquiry.Services
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserEnquiryService"/> class.
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public class UserEnquiryService(AppDbContext context)
    {
        private readonly UserMerkleTreeRepo _userMerkleTreeRepo = new();
        private readonly AppDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        public const string HashTagForLeaf = "ProofOfReserve_Leaf";
        public const string HashTagForBranch = "ProofOfReserve_Branch";

        /// <summary>
        /// Get Merkle from repository and returns the Merkle root.
        /// </summary>
        /// <returns></returns>
        public MerkleTreeNode? GetMerkleRootOfUsers()
        {
            return _userMerkleTreeRepo.GetMerkleTreeRoot();
        }

        public MerkleProof? GetMerkleProofOfUser(string userId)
        {
            //Fetch user information from a data source
            var integerUserId = int.TryParse(userId, out var parsedUserId) ? parsedUserId : (int?)null;
            if (integerUserId == null)
            {
                return null; // Return empty proof if userId is not valid
            }

            //Check if user exists in the database
            UserInfo? userInfo = _context.UserInfos.FirstOrDefault(u => u.UserId == integerUserId);
            if (userInfo == null)
            {
                return null; // Return empty proof if user is not found
            }

            //Get the Merkle root of users
            MerkleTreeNode? merkleRoot = _userMerkleTreeRepo.GetMerkleTreeRoot();
            if (merkleRoot == null)
            {
                return null; // Return empty proof if Merkle root is not available
            }

            //Initialize the Merkle tree service and Generate the Merkle proof for the users
            var merkleTreeLibrary = new MerkleTree.MerkleTree(HashTagForLeaf, HashTagForBranch);
            List<MerkleNodeTuple>? merkleNodeTuples = merkleTreeLibrary.GetMerklePath(merkleRoot, $"({userInfo.UserId},{userInfo.Balance})");
            if (merkleNodeTuples == null || merkleNodeTuples.Count == 0)
            {
                return null; // Return empty proof if Merkle root is not available
            }

            return new MerkleProof(userInfo, merkleNodeTuples); 
        }
    }
}
