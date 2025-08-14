using MerkleTree;
using UserEnquiry.Models;
using UserEnquiry.Repository;

namespace UserEnquiry
{
    public class UserEnquiryService
    {
        private readonly AppDbContext _context;
        MerkleTree.MerkleTree merkleTreeService;
        private const string HashTagForLeaf = "ProofOfReserve_Leaf";
        private const string HashTagForBranch = "ProofOfReserve_Branch";

        /// <summary>
        /// Initializes a new instance of the <see cref="UserEnquiryService"/> class.
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UserEnquiryService(AppDbContext context)
        {
            // Check if the context is null and throw an exception
            _context = context ?? throw new ArgumentNullException(nameof(context));

            //Initialize the Merkle tree service
            merkleTreeService = new MerkleTree.MerkleTree(HashTagForLeaf, HashTagForBranch);
        }

        /// <summary>
        /// Initializes the Merkle tree with user information and returns the Merkle root.
        /// </summary>
        /// <returns></returns>
        public MerkleTreeNode? GetMerkleRootOfUsers()
        {
            var merkleTreeRepo = new MerkleTreeRepo();

            if (merkleTreeRepo.GetMerkleTreeRoot() == null)
            {
                try
                {
                    //Fetch user information from a data source if no user available then return null
                    List<UserInfo> userInfos = fetchUserInfos();
                    if (userInfos == null || !userInfos.Any())
                    {
                        return null; // No user information available
                    }

                    //Convert user info to string format for hashing
                    List<string> userInfoStrings = userInfoToString(userInfos);

                    //Build the Merkle tree from user info strings
                    merkleTreeRepo.SetMerkleTreeRoot(merkleTreeService.CalculateMerkleRoot(userInfoStrings));
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return merkleTreeRepo.GetMerkleTreeRoot();
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
            MerkleTreeNode? merkleRoot = GetMerkleRootOfUsers();
            if (merkleRoot == null)
            {
                return null; // Return empty proof if Merkle root is not available
            }

            //Generate the Merkle proof for the user
            List<MerkleNodeTuple>? merkleNodeTuples = merkleTreeService.GetMerklePath(merkleRoot, $"({userInfo.UserId},{userInfo.Balance})");
            if (merkleNodeTuples == null || merkleNodeTuples.Count == 0)
            {
                return new MerkleProof(userInfo, merkleNodeTuples); // Return empty proof if no Merkle path is found
            }

            return (new MerkleProof(userInfo, merkleNodeTuples)); 
        }

        /// <summary>
        /// Generally this method will be called by a scheduled job or manually when needed just after user info updates.
        /// When user information is updated, this method can be called to re-calculate the Merkle root.
        /// As per instruction, user information will be updated once in a day.
        /// (As of now, this method is not used in the code but can be used in future if needed.)
        /// </summary>
        /// <returns></returns>
        private MerkleTreeNode? ReCalculateMerkleRootOfUsers()
        {
            var merkleTreeRepo = new MerkleTreeRepo();

            try
            {

                //Initialize the Merkle tree service
                MerkleTree.MerkleTree merkleTreeService = new MerkleTree.MerkleTree(HashTagForLeaf, HashTagForBranch);

                //Fetch user information from a data source if no user available then return null
                List<UserInfo> userInfos = fetchUserInfos();
                if (userInfos == null || !userInfos.Any())
                {
                    return null; // No user information available
                }

                //Convert user info to string format for hashing
                List<string> userInfoStrings = userInfoToString(userInfos);

                //Build the Merkle tree from user info strings
                merkleTreeRepo.SetMerkleTreeRoot(merkleTreeService.CalculateMerkleRoot(userInfoStrings));
            }
            catch (Exception)
            {
                return null;
            }
            return merkleTreeRepo.GetMerkleTreeRoot();
        }

        private List<string> userInfoToString(List<UserInfo> userInfos)
        {
            //Convert user info to string format for hashing
            return userInfos.Select(user => $"({user.UserId},{user.Balance})").ToList();
        }

        private List<UserInfo> fetchUserInfos()
        {
            return [.. _context.UserInfos];
        }
    }
}
