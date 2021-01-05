using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseAPIController
    {
        private readonly DataContext dataContext;
        private readonly ITokenService tokenService;
        private readonly IMapper mapper;

        public AccountController(DataContext dataContext, ITokenService tokenService, IMapper mapper)
        {
            this.dataContext = dataContext;
            this.tokenService = tokenService;
            this.mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDTO registerDTO)
        {
            if(await UserExists(registerDTO.Username)) return BadRequest();

            var user = mapper.Map<AppUser>(registerDTO);
        
            using var hmac = new HMACSHA512();

            user.Username = registerDTO.Username;
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
            user.PasswordSalt = hmac.Key;

            dataContext.Users.Add(user);
            await dataContext.SaveChangesAsync();
            return new UserDto
            {
                Username = user.Username,
                Token = tokenService.CreateToken(user),
                KnownAs = user.KnownAs
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await dataContext.Users
                .Include(x => x.Photos)
                .SingleOrDefaultAsync(x => x.Username == loginDto.Username);

            if (user == null) return Unauthorized();

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized();
            }

            return new UserDto
            {
                Username = user.Username,
                Token = tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs
            };
        }

        private async Task<bool> UserExists (string username)
        {
            return await dataContext.Users.AnyAsync(user => user.Username == username.ToLower());
        }
    }
}
