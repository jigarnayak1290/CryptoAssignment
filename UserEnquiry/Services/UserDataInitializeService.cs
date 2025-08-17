using UserEnquiry.DBContext;
using UserEnquiry.Repository;

namespace UserEnquiry.Services
{
    public static class UserDataInitializeService
    {
        /// <summary>
        /// Initialize User data and then create Merkle tree from it.
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static bool InitializeDailyData(IServiceProvider provider)
        {
            using var scope = provider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            List<string> userInfoStrings = [.. dbContext.UserInfos.Select(user => $"({user.UserId},{user.Balance})")];
            if (userInfoStrings.Count == 0)
            {
                return false;
            }

            var merkleTreeLibrary = new MerkleTree.MerkleTree(UserEnquiryService.HashTagForLeaf, UserEnquiryService.HashTagForBranch);
            var merklenode = merkleTreeLibrary.CalculateMerkleRoot(userInfoStrings);

            if (merklenode == null)
            {
                return false;
            }

            var merkleTreeRepo = scope.ServiceProvider.GetRequiredService<UserMerkleTreeRepo>();
            merkleTreeRepo.SetMerkleTreeRoot(merklenode);

            return true;
        }

    }
}
