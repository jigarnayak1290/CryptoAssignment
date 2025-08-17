using MerkleTree;

namespace UserEnquiry.Repository
{
    /// <summary>
    /// Repository to hold Merkle tree of users, Singleton in nature, which means it will hold the Merkle tree in memory.
    /// </summary>
    public class UserMerkleTreeRepo
    {
        private static MerkleTreeNode? merkleTreeRoot = null;
        private readonly ReaderWriterLockSlim _lock = new();

        /// <summary>
        /// Get the Merkle tree of users.
        /// </summary>
        /// <returns>Merkle tree root of user</returns>
        public MerkleTreeNode? GetMerkleTreeRoot()
        {
            _lock.EnterReadLock();
            try
            {
                return merkleTreeRoot;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Set the Merkle tree root, Generally this will be called after the Merkle tree is built or updated.
        /// </summary>
        /// <param name="root">Merkle tree root of user</param>
        public void SetMerkleTreeRoot(MerkleTreeNode? root)
        {
            _lock.EnterWriteLock();
            try
            {    
                merkleTreeRoot = root;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}
