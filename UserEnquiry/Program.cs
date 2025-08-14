using Microsoft.EntityFrameworkCore;
using UserEnquiry;
using UserEnquiry.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Register All Services
builder.Services.AddScoped<UserEnquiry.UserEnquiryService>();     //Register User Enquiry Service
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("UserDb"));
// Register the DbContext with an in-memory database for testing purposes

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
