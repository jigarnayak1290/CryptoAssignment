using MerkleTree;
using UserEnquiry.Models;

namespace UserEnquiry
{
    public class UserEnquiryService
    {
        private readonly AppDbContext _context;
        private static MerkleTreeNode? userMerkleTree = null;
        private const string HashTagForLeaf = "ProofOfReserve_Leaf";
        private const string HashTagForBranch = "ProofOfReserve_Branch";

        /// <summary>
        /// Initializes a new instance of the <see cref="UserEnquiryService"/> class.
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UserEnquiryService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }       

        /// <summary>
        /// Initializes the Merkle tree with user information and returns the Merkle root.
        /// </summary>
        /// <returns></returns>
        public MerkleTreeNode? GetMerkleRootOfUsers()
        {
            if (userMerkleTree == null)
            {
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
                    userMerkleTree = merkleTreeService.CalculateMerkleRoot(userInfoStrings);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return userMerkleTree;
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
                userMerkleTree = merkleTreeService.CalculateMerkleRoot(userInfoStrings);
            }
            catch (Exception)
            {
                return null;
            }
            return userMerkleTree;
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
