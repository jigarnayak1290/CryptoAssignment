using MerkleTree;
using static UserEnquiry.Models.MerkleProof;

namespace UserEnquiry.Models
{
    /// <summary>
    /// Merkle proof with userId, Balance & all respected node order from user leaf node to root node
    /// </summary>
    /// <param name="UserInfo">UserId & Balance</param>
    /// <param name="merkleNodeTuples">Merkle nodes</param>
    public record MerkleProof(
        UserInfo? UserInfo = null,
        List<MerkleNodeTuple>? merkleNodeTuples = null)
    {
        /// <summary>
        /// Single node of user merkle tree
        /// </summary>
        /// <param name="Tuple">Merkle node</param>
        /// <param name="IsRightNode">whether this node is right node or left (if true than node is right else left)</param>
        public record MerkleNodeTuple(
        MerkleTreeNode? Tuple = null,
        Boolean? IsRightNode = null);
    }
}
