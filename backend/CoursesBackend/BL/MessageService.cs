using DAL;
using IBL;
using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        public async Task<List<Message>> GetMessagesByChatIdAsync(Guid chatId)
        {
            return await _messageRepository
                 .GetMessages()
                 .Where(m => m.ChatId == chatId && !m.IsDeleted)
                 .ToListAsync();
        }

        public async Task<Message?> GetMessageByIdAsync(Guid messageId)
        {
            return await Task.FromResult(_messageRepository.GetMessageById(messageId));
        }

        public async Task<Message> AddMessageAsync(Message message)
        {
            message.Id = Guid.NewGuid();
            message.CreatedAt = DateTime.UtcNow;
            message.IsDeleted = false;
            return await Task.FromResult(_messageRepository.AddMessage(message));
        }

        public async Task<Message?> EditMessageAsync(Guid messageId, string newContent)
        {
            var message = await Task.FromResult(_messageRepository.GetMessageById(messageId));
            if (message != null && !message.IsDeleted)
            {
                message.Content = newContent;
                message.EditedAt = DateTime.UtcNow;
                return await Task.FromResult(_messageRepository.AddMessage(message));
            }
            return null;
        }

        public async Task<Message?> DeleteMessageAsync(Guid messageId)
        {
            var message = await Task.FromResult(_messageRepository.GetMessageById(messageId));
            if (message != null && !message.IsDeleted)
            {
                message.IsDeleted = true;
                return await Task.FromResult(_messageRepository.AddMessage(message));
            }
            return null;
        }
        public async Task<bool> MessageExistsAsync(Guid messageId)
        {
            var message = await Task.FromResult(_messageRepository.GetMessageById(messageId));
            return message != null && !message.IsDeleted;
        }
    }
}
