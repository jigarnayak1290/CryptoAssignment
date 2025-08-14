using MerkleTree;
using Microsoft.EntityFrameworkCore;

namespace UserEnquiry.Repository
{
    public class MerkleTreeRepo
    {
        private static MerkleTreeNode? merkleTreeRoot = null;

        /// <summary>
        /// Get the Merkle tree of users.
        /// </summary>
        /// <returns></returns>
        public MerkleTreeNode? GetMerkleTreeRoot()
        {
            if (merkleTreeRoot == null)
            {
                // This will ensure that the Merkle tree is initialized with the latest user information
                ReinitializeMerkleTreeRepo();
            }
            return merkleTreeRoot;
        }

        /// <summary>
        /// Set the Merkle tree root.
        /// </summary>
        /// <param name="root"></param>
        public void SetMerkleTreeRoot(MerkleTreeNode? root)
        {
            merkleTreeRoot = root;
        }

        public bool ReinitializeMerkleTreeRepo()
        {
            //Reinitialize daily, as we are daily updating user information
            return true;
        }
    }
}
