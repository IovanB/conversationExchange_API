using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class MessagesController : BaseAPIController
    {
        private readonly IUserRepository userRepository;
        private readonly IMessageRepository messageRepository;
        private readonly IMapper mapper;

        public MessagesController(IUserRepository userRepository,
            IMessageRepository messageRepository,
            IMapper mapper)
        {
            this.userRepository = userRepository;
            this.messageRepository = messageRepository;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var UserName = User.GetUserName();
            if (UserName == createMessageDto.RecipientUserName.ToLower())
                return BadRequest("Not allowed to send yourself a message");
            var sender = await userRepository.GetUserByUserNameAsync(UserName);
            var recipient = await userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUserName);

            if (recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = createMessageDto.Content
            };

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync()) return Ok(mapper.Map<MessageDto>(message));

            return BadRequest("Message not sent");
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForuser([FromQuery] MessageParams messageParams)
        {
            messageParams.UserName = User.GetUserName();
            var messages = await messageRepository.GetMessageForUser(messageParams);

            Response.AddPageHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return messages;

        }

        [HttpGet("thread/{UserName}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string UserName)
        {
            var currentUserName = User.GetUserName();

            return Ok(await messageRepository.GetMessageThread(currentUserName, UserName));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessagE(int id)
        {
            var UserName = User.GetUserName();
            var message = await messageRepository.GetMessage(id);

            if(message.Sender.UserName != UserName && message.Recipient.UserName != UserName)
            {
                return Unauthorized();
            }

            if(message.Sender.UserName == UserName) message.SenderDeleted = true;
            
            if(message.Recipient.UserName == UserName ) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted) messageRepository.DeleteMessage(message);

            if (await messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Message could not be deleted");
        }

    }
}
