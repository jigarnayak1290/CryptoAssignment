using MerkleTree;
using Microsoft.EntityFrameworkCore;
using UserEnquiry;
using UserEnquiry.Models;
using UserEnquiry.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Register All Services
//Register Merkle Tree Repository as singleton(To keep the Merkle tree state across requests)
builder.Services.AddSingleton<MerkleTreeRepo>();

// Register the DbContext with an in-memory database for testing purposes
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("UserDb"));

//Register User Enquiry Service as scoped
builder.Services.AddScoped<UserEnquiry.UserEnquiryService>();

var app = builder.Build();

//initialize the database with sample data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.UserInfos.AddRange(
        new UserInfo { UserId = 1, Balance = 1111 },
        new UserInfo { UserId = 2, Balance = 2222 },
        new UserInfo { UserId = 3, Balance = 3333 },
        new UserInfo { UserId = 4, Balance = 4444 },
        new UserInfo { UserId = 5, Balance = 5555 },
        new UserInfo { UserId = 6, Balance = 6666 },
        new UserInfo { UserId = 7, Balance = 7777 },
        new UserInfo { UserId = 8, Balance = 8888 }
    );
    db.SaveChanges();

    // Initialize the Merkle tree with user information
    var merkleTreeRepo = scope.ServiceProvider.GetRequiredService<MerkleTreeRepo>();
    var userEnquiryService = scope.ServiceProvider.GetRequiredService<UserEnquiryService>();

    MerkleTreeNode? merkleTreeRoot = userEnquiryService.GetMerkleRootOfUsers();
    merkleTreeRepo.SetMerkleTreeRoot(merkleTreeRoot); 
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
