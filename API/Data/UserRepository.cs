﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
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

        public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
        {
            return await context.Users
                .ProjectTo<MemberDTO>(mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}