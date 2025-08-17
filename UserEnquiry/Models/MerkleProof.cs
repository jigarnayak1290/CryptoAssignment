using MerkleTree;

namespace UserEnquiry.Models
{
    /// <summary>
    /// Merkle proof with userId, Balance & all respected node order from user leaf node to root node
    /// </summary>
    /// <param name="UserInfo">UserId & Balance</param>
    /// <param name="MerkleNodeTuples">Merkle nodes</param>
    public record MerkleProof(
        UserInfo? UserInfo = null,
        List<MerkleNodeTuple>? MerkleNodeTuples = null);
    
}
