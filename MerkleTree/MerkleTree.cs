using System.Text;
using System.Security.Cryptography;

namespace MerkleTree
{
    public class MerkleTree
    {
        //Hash tag (text) 
        private const string DefaultHashTagForLeafNBranch = "Bitcoin_Transaction";
        private readonly string LeafHashTag;
        private readonly string BranchHashTag;

        /// <summary>
        /// Will use default hash tag "Bitcoin_Transaction" for leaf and branch
        /// </summary>
        public MerkleTree() : this(DefaultHashTagForLeafNBranch, DefaultHashTagForLeafNBranch)
        { }

        /// <summary>
        /// will use given single hash tag for leaf and branch both
        /// This is useful when you want to use same tag for both leaf and branch nodes.
        /// </summary>
        /// <param name="HashTagForLeaf"></param>
        public MerkleTree(string HashTagForLeaf) : this(HashTagForLeaf, HashTagForLeaf)
        { }

        /// <summary>
        /// Will use given first hash tags for leaf and second hash tag for branch nodes.
        /// </summary>
        /// <param name="HashTagForLeaf"></param>
        /// <param name="HashTagForBranch"></param>
        public MerkleTree(string HashTagForLeaf, string HashTagForBranch)
        {
            LeafHashTag = HashTagForLeaf;
            BranchHashTag = HashTagForBranch;
        }

        /// <summary>
        /// Create merkle tree from received leaf nodes
        /// </summary>
        /// <param name="_leafNodes">Transactions as Leaf nodes</param>
        /// <returns>Merkle root node</returns>
        public MerkleTreeNode? CalculateMerkleRoot(IEnumerable<string> _leafNodes)
        {
            if (_leafNodes == null || !_leafNodes.Any())
            {
                return null;
            }

            //Calculate hash of individual leaf node
            List<MerkleTreeNode> MerkleLeafNodes = _leafNodes.Select(
                t => new MerkleTreeNode(
                    Hash: ToHexString(HashTransactionWithTag(LeafHashTag, Encoding.UTF8.GetBytes(t)))
                )).ToList();

            //Make Merkle tree from hashed leaf nodes
            return MakeMerkleTree(BranchHashTag, MerkleLeafNodes);
        }

        public List<MerkleNodeTuple>? GetMerklePath(MerkleTreeNode? root, string leafNode)
        {
            if (root == null)
            {
                return null;
            }
            var path = new List<MerkleNodeTuple>();
            var leafHash = ToHexString(HashTransactionWithTag(LeafHashTag, Encoding.UTF8.GetBytes(leafNode)));
            
            if (GetMerklePathRecursive(root, leafHash, path))
            {
                //If need to add merkle root node at the end of the path
                //path.Add(new MerkleNodeTuple(root.Hash, null));
                return path;
            }
            else
            {
                // If the leaf node is not found, return null
                return null;
            }            
        }

        public bool GetMerklePathRecursive(MerkleTreeNode? node, string leafHash, List<MerkleNodeTuple> path)
        {
            if (node == null)
            {
                return false;
            }
            // If we found the leaf node, we can stop searching
            if (node.Hash == leafHash)
            {
                return true;
            }
            // Check if the left child exists and recurse into it
            if (GetMerklePathRecursive(node.Left, leafHash, path))
            {
                path.Add(new MerkleNodeTuple(node.Right.Hash, true));
                return true;
            }
            // Check if the right child exists and recurse into it
            if (GetMerklePathRecursive(node.Right, leafHash, path))
            {
                path.Add(new MerkleNodeTuple(node.Left.Hash, false));
                return true;
            }

            // If neither child contains the leaf node, return false
            return false; 
        }

        /// <summary>
        /// Generate merkletree from given leaf nodes
        /// </summary>
        /// <param name="hashTag">Hash tag to add in hash</param>
        /// <param name="transactions">Transactions as Leaf nodes</param>
        /// <returns>Merkle root node</returns>
        private MerkleTreeNode MakeMerkleTree(string hashTag, IEnumerable<MerkleTreeNode> transactions)
        {
            //Check for single transaction, if so return as root.
            if (transactions.Count() == 1)
            {
                return transactions.Single();
            }

            var nextLevelNodes = new List<MerkleTreeNode>();

            for (int i = 0; i < transactions.Count(); i += 2)
            {
                var left = transactions.ElementAt(i);
                var right = i == transactions.Count() - 1 ? transactions.ElementAt(i) : transactions.ElementAt(i + 1);
                var leftBytes = FromHexString(left.Hash);
                var rightBytes = FromHexString(right.Hash);
                var mergedBytes = leftBytes.Concat(rightBytes).ToArray();

                var hashTransactionWithTag = HashTransactionWithTag(hashTag, mergedBytes);
                var hexString = ToHexString(hashTransactionWithTag);

                var node = new MerkleTreeNode(hexString, left, right);

                nextLevelNodes.Add(node);
            }

            //Recursively calling function with merged transaction list
            return MakeMerkleTree(hashTag, nextLevelNodes);


            //// Below code is commented as it is not used in current implementation, but can be used for reference (It is similar to above code)
            ////if transactions are odd than duplicate last transcation to make it even (As per Merkle tree rule)
            //if (transactions.Count() % 2 != 0)
            //{
            //    transactions = balancedMerkleTree(transactions);
            //}

            //var mergedTransaction = transactions
            //    .Chunk(2)
            //    .Select(pair =>
            //    {
            //        var left = pair.First();
            //        var right = pair.Last();
            //        var leftBytes = FromHexString(left.Hash);
            //        var rightBytes = FromHexString(right.Hash);
            //        var mergedBytes = leftBytes.Concat(rightBytes).ToArray();

            //        var hashTransactionWithTag = HashTransactionWithTag(hashTag, mergedBytes);

            //        return new MerkleTreeNode(ToHexString(hashTransactionWithTag), left, right);
            //    }).ToList();

            ////Recursively calling function with merged transaction list
            //return MakeMerkleTree(hashTag, mergedTransaction);
        }

        /// <summary>
        /// Convert byte array to hex string
        /// </summary>
        /// <param name="cryptoTransactions"></param>
        /// <returns>Hex strings</returns>
        private string ToHexString(byte[] cryptoTransactions) =>
            BitConverter.ToString(cryptoTransactions).Replace("-", "").ToLowerInvariant();

        /// <summary>
        /// Convert hex string to byte array
        /// </summary>
        /// <param name="hex"></param>
        /// <returns>byte array of Hex string</returns>
        private byte[] FromHexString(string hex) => Enumerable.Range(0, hex.Length)
            .Where(x => x % 2 == 0)
            .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
            .ToArray();

        /// <summary>
        /// Hash transaction data in format of BIP340 with SHA256 (Hash(tag) + Hash(tag) + transaction)
        /// </summary>
        /// <param name="cryptoTransactions"></param>
        /// <returns>Hash of hash & transaction</returns>
        private byte[] HashTransactionWithTag(string hashTag, byte[] cryptoTransaction)
        {
            var tagBytes = Encoding.UTF8.GetBytes(hashTag);
            var tagHash = SHA256.HashData(tagBytes);

            //var concateTagHashWithTransaction = tagHash.Concat(tagHash).Concat(cryptoTransaction).ToArray();
            //Improved version using ConcatAll method
            var concateTagHashWithTransaction = ConcatAll(tagHash, tagHash, cryptoTransaction);
            return SHA256.HashData(concateTagHashWithTransaction);
        }

        /// <summary>
        /// Fast, efficient method to concatenate multiple byte arrays into a single byte array. (Single allocation, no iterator overload)
        /// </summary>
        /// <param name="arrays">Items to be concat</param>
        /// <returns></returns>
        public static byte[] ConcatAll(params byte[][] arrays)
        {
            var totalLength = arrays.Sum(a => a.Length);
            var result = new byte[totalLength];
            int offset = 0;

            foreach (var array in arrays)
            {
                Buffer.BlockCopy(array, 0, result, offset, array.Length);
                offset += array.Length;
            }

            return result;
        }

        ///// <summary>
        /////  Balance merkle tree by duplicate last transcation to make it even (If its odd)
        ///// </summary>
        ///// <param name="cryptoTransactions"></param>
        ///// <returns>Even merkle leaf nodes</returns>
        //private IEnumerable<MerkleTreeNode> balancedMerkleTree(IEnumerable<MerkleTreeNode> cryptoTransactions)
        //{
        //    if (cryptoTransactions.Count() % 2 != 0)
        //    {
        //        var listMerkleTreeLeafs = new List<MerkleTreeNode>();

        //        listMerkleTreeLeafs = cryptoTransactions.ToList();
        //        listMerkleTreeLeafs.Add(cryptoTransactions.Last());
        //        return listMerkleTreeLeafs;
        //    }

        //    return cryptoTransactions;
        //}

    }

    public record MerkleTreeNode(
        string Hash,
        MerkleTreeNode? Left = null,
        MerkleTreeNode? Right = null);

    /// <summary>
    /// Single node of user merkle tree
    /// </summary>
    /// <param name="HashTupleString">Merkle node</param>
    /// <param name="IsRightNode">whether this node is right node or left (if true than node is right else left)</param>
    public record MerkleNodeTuple(
        string? HashTupleString = null,
        Boolean? IsRightNode = null);
}
