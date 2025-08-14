namespace MerkleTree.Tests
{
    public class MerkleTreeTest
    {
        [Fact]
        public void Test_ABCDE_transaction_sample()
        {
            // Arrange test data (Example transactions to be hashed)
            // In a real-world scenario, these would be actual transaction data.
            List<string> transactions = new List<string>
            { "aaa", "bbb", "ccc", "ddd", "eee" };

            // Act (calculate the Merkle root)
            var service = new MerkleTree(); //Not passing any argument, So it will use default constructor with default hash tag values
            var result = service.CalculateMerkleRoot(transactions);

            // Assert (Checking the root hash with result)
            var rootHash = result.Hash;
            Assert.Equal("4aa906745f72053498ecc74f79813370a4fe04f85e09421df2d5ef760dfa94b5", rootHash);
        }
    }
}