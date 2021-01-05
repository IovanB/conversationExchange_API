using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext dataContext)
        {
            if (await dataContext.Users.AnyAsync()) return;
            var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<AppUser>>(userData);
            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();
                user.Username = user.Username.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password"));
                user.PasswordSalt = hmac.Key;

                dataContext.Users.Add(user);
            }

            await dataContext.SaveChangesAsync();
        }
    }
}
