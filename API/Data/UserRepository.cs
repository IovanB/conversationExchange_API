using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public UserRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public async Task<IEnumerable<AppUser>> GetUserAsync()
        {
            return await context.Users.Include(p => p.Photos).ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await context.Users.FindAsync(id);
        } 

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await context.Users.Include(p=>p.Photos).SingleOrDefaultAsync(user =>
            user.Username == username);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            context.Entry(user).State = EntityState.Modified;
        }

        public async Task<MemberDTO> GetMemberAsync(string username)
        {
            return await context.Users
                .Where(x => x.Username == username)
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider).SingleOrDefaultAsync(); 
            //o configurationprovier pega as configuracoes que usamos no automapper, na classe AutoMapperProfiles

                
                
        }

        public async Task<PageList<MemberDTO>> GetMembersAsync(UserParams userParams)
        {
            //var query = context.Users
            //    .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
            //    .AsNoTracking()
            //    .AsQueryable();

            var query = context.Users.AsQueryable();
        
            query = query.Where(q => q.Username != userParams.CurrentUsername);

            query = query.Where(q => q.Gender == userParams.Gender);

            // choose by user age
            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(d => d.DateOfBirth >= minDob && d.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)
            };

            //choose by user's native language
            //query = query.Where(l => l.NativeLanguage == userParams.NativeLanguage); 
            
            //choose by user's target language
            //query = query.Where(t => t.TargetLanguage == userParams.TargetLanguage);


            return await PageList<MemberDTO>.CreateAsync(query.ProjectTo<MemberDTO>(mapper.ConfigurationProvider).AsNoTracking(), userParams.PageNumber, userParams.PageSize);

        }
    }
}
