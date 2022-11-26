using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi50.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DbUsersContext>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/users", async (DbUsersContext db) =>
{
    var users = await db.Users.ToListAsync();
    return Results.Ok(users);
})
.WithName("GetUsers")
.WithOpenApi();

app.MapPost("/users", async (User user, DbUsersContext db) =>
{
    var u = db.Users.Add(user);

    await db.SaveChangesAsync();
    return Results.Ok(u);
})
.WithName("AddUser")
.WithOpenApi();

app.MapPost("/Login", async (User user, DbUsersContext db) =>
{
    var u = db.Users.Where(u => u.Email == user.Email && u.Password == user.Password).FirstOrDefault();

    if (u == null)
        return Results.Empty;
    else
    {
        //Generate Token
        var token = CreateAccessToken(u);
        return Results.Ok(token);
    }
})
.WithName("Login")
.WithOpenApi();


app.Run();

String CreateAccessToken(User user)
{
    //suppose a public key can read from appsetting
    String K = "12345678901234567890123456789012345678901234567890123456789012345678901234567890";
    //Convert to bytes
    var key = Encoding.UTF8.GetBytes(K);
    //convert to symmetric Security key
    var skey = new SymmetricSecurityKey(key);
    //SignCredential the symmetric key
    var SignedCredential = new SigningCredentials(skey, SecurityAlgorithms.HmacSha256Signature);
    //Add some Claims
    var uClaims = new ClaimsIdentity(new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub,user.Name),
        new Claim(JwtRegisteredClaimNames.Email,user.Email)
    });
    var expires = DateTime.UtcNow.AddDays(1);

    //Create the token Descriptor
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = uClaims,
        Expires = expires,
        Issuer = "MasterBlazor",
        SigningCredentials = SignedCredential,
    };
    //initiate the token handler
    var tokenHandler = new JwtSecurityTokenHandler();
    var tokenJwt = tokenHandler.CreateToken(tokenDescriptor);
    var token = tokenHandler.WriteToken(tokenJwt);

    return token;
}

