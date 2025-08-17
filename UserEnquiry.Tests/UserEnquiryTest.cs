using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UserEnquiry.DBContext;
using UserEnquiry.Models;
using UserEnquiry.Repository;
using UserEnquiry.Services;

namespace UserEnquiry.Tests
{
    public class UserEnquiryTest
    {
        private readonly IServiceProvider? _provider;

        public UserEnquiryTest()
        {
            // Initialize the service provider
            var services = new ServiceCollection();
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("UserDb"));
            services.AddScoped<UserMerkleTreeRepo>();
            _provider = services.BuildServiceProvider();            
        }

        [Fact]
        public void Test_UerIdWithBalance()
        {
            // Arrange test data (Example transactions to be hashed)
            // In a real-world scenario, these would be actual transaction data.            
            addTestUserData(_provider);

            // Build the Merkle tree from user info strings and set it in the repository
            UserDataInitializeService.InitializeDailyData(_provider);

            // Act (calculate the Merkle root by passing Hash tags)
            var userMerkleRoot = new UserEnquiryService(_provider.GetRequiredService<AppDbContext>());
            var merkleRoot = userMerkleRoot.GetMerkleRootOfUsers();

            // Assert (Checking the root hash with result)
            var rootHash = merkleRoot.Hash;
            Assert.Equal("b1231de33da17c23cebd80c104b88198e0914b0463d0e14db163605b904a7ba3", rootHash);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        private void addTestUserData(IServiceProvider provider)
        {
            var inMemoryUserDB = provider.GetRequiredService<AppDbContext>();

            // Seed the database with sample user data
            inMemoryUserDB.UserInfos.AddRange(
                new UserInfo { UserId = 1, Balance = 1111 },
                new UserInfo { UserId = 2, Balance = 2222 },
                new UserInfo { UserId = 3, Balance = 3333 },
                new UserInfo { UserId = 4, Balance = 4444 },
                new UserInfo { UserId = 5, Balance = 5555 },
                new UserInfo { UserId = 6, Balance = 6666 },
                new UserInfo { UserId = 7, Balance = 7777 },
                new UserInfo { UserId = 8, Balance = 8888 }
            );
            inMemoryUserDB.SaveChanges();
        }
    }
}