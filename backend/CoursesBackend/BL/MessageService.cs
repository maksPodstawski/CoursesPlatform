using DAL;
using IBL;
using IDAL;
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
        public IQueryable<Message> GetMessagesByChatIdAsync(Guid chatId)
        {
            var messages =  _messageRepository.GetMessages();
            return messages.Where(message => message.ChatId == chatId && !message.IsDeleted);
        }

        public async Task<Message?> GetMessageByIdAsync(Guid messageId)
        {
            var message = await _messageRepository.GetMessageByIdAsync(messageId);
            return message?.IsDeleted == true ? null : message;
        }

        public async Task AddMessageAsync(Message message)
        {
            message.Id = Guid.NewGuid();
            message.CreatedAt = DateTime.UtcNow;
            message.IsDeleted = false;
            await _messageRepository.AddMessageAsync(message);
        }

        public async Task EditMessageAsync(Guid messageId, string newContent)
        {
            var message = await _messageRepository.GetMessageByIdAsync(messageId);
            if (message != null && !message.IsDeleted)
            {
                message.Content = newContent;
                message.EditedAt = DateTime.UtcNow;
                await _messageRepository.UpdateMessageAsync(message);
            }
        }

        public async Task DeleteMessageAsync(Guid messageId)
        {
            var message = await _messageRepository.GetMessageByIdAsync(messageId);
            if (message != null && !message.IsDeleted)
            {
                message.IsDeleted = true;
                await _messageRepository.UpdateMessageAsync(message);
            }
        }
        public async Task<bool> MessageExistsAsync(Guid messageId)
        {
            var message = await _messageRepository.GetMessageByIdAsync(messageId);
            return message != null && !message.IsDeleted;
        }
    }
}
