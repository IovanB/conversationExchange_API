using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Primitives;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext context;
        private readonly IMapper mapper;

        public MessageRepository(DataContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await context.Messages.FindAsync(id);
        }

        public async Task<PageList<MessageDto>> GetMessageForUser(MessageParams messageParams)
        {
            var query = context.Messages
                .OrderByDescending(m => m.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.Recipient.Username == messageParams.Username),
                "Outbox" => query.Where(u => u.Sender.Username == messageParams.Username),
                _ => query.Where(u => u.Recipient.Username == messageParams.Username && u.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDto>(mapper.ConfigurationProvider);

            return await PageList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int recipientId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
