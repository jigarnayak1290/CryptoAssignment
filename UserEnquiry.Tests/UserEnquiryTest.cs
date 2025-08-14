using Microsoft.EntityFrameworkCore;
using UserEnquiry.Models;

namespace UserEnquiry.Tests
{
    public class UserEnquiryTest
    {
        [Fact]
        public void Test_UerIdWithBalance()
        {
            // Arrange test data (Example transactions to be hashed)
            // In a real-world scenario, these would be actual transaction data.
            var context = GetInMemoryDbContext();
            SeedDatabase(context);            

            // Act (calculate the Merkle root by passing Hash tags)
            var service = new UserEnquiryService(context);
            var result = service.GetMerkleRootOfUsers();

            // Assert (Checking the root hash with result)
            var rootHash = result.Hash;
            Assert.Equal("b1231de33da17c23cebd80c104b88198e0914b0463d0e14db163605b904a7ba3", rootHash);
        }

        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "UserDb")
                .Options;

            return new AppDbContext(options);
        }
        private void SeedDatabase(AppDbContext context)
        {
            // Seed the database with sample user data
            context.UserInfos.AddRange(
                new UserInfo { UserId = 1, Balance = 1111 },
                new UserInfo { UserId = 2, Balance = 2222 },
                new UserInfo { UserId = 3, Balance = 3333 },
                new UserInfo { UserId = 4, Balance = 4444 },
                new UserInfo { UserId = 5, Balance = 5555 },
                new UserInfo { UserId = 6, Balance = 6666 },
                new UserInfo { UserId = 7, Balance = 7777 },
                new UserInfo { UserId = 8, Balance = 8888 }
            );
            context.SaveChanges();
        }
    }
}