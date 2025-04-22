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
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public IQueryable<Chat> GetAllChatsAsync()
        {
            return _chatRepository.GetChats();
        }

        public async Task<Chat?> GetChatByIdAsync(Guid chatId)
        {
            return await _chatRepository.GetChatByIdAsync(chatId);
        }

        public async Task AddChatAsync(Chat chat)
        {
            await _chatRepository.AddChatAsync(chat);
        }
        public async Task<Chat?> DeleteChatAsync(Guid chatId)
        {
            var chat = await _chatRepository.GetChatByIdAsync(chatId);
            if(chat == null)
            {
                return null;
            }
            await _chatRepository.DeleteChatAsync(chatId);
            return chat;
        }

        public async Task<IEnumerable<User>> GetUsersInChatAsync(Guid chatId)
        {
            var chat = await _chatRepository.GetChatByIdAsync(chatId);


            if (chat == null || chat.Users == null)
            {
                return Enumerable.Empty<User>();
            }
            return chat.Users.Select(chatUser => chatUser.User);
        }


        public async Task RenameChatAsync(Guid chatId, string chatName)
        {
            var chat = await _chatRepository.GetChatByIdAsync(chatId);
            if (chat != null)
            {
                chat.Name = chatName;
                await _chatRepository.UpdateChatAsync(chat);
            }
        }

        public async Task<bool> ChatExistsAsync(Guid chatId)
        {
            var chat = await _chatRepository.GetChatByIdAsync(chatId);
            return chat != null;
        }
    }
}
