using IBL;
using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<List<Chat>> GetAllChatsAsync()
        {
            return await _chatRepository.GetChats().ToListAsync();
        }

        public async Task<Chat?> GetChatByIdAsync(Guid chatId)
        {
            return await Task.FromResult(_chatRepository.GetChatById(chatId));
        }

        public async Task<Chat> AddChatAsync(Chat chat)
        {
            return await Task.FromResult(_chatRepository.AddChat(chat));
        }
        public async Task<Chat?> DeleteChatAsync(Guid chatId)
        {
            var chat = await Task.FromResult(_chatRepository.GetChatById(chatId));
            if(chat == null)
            {
                return null;
            }
            return await Task.FromResult(_chatRepository.DeleteChat(chatId));
        }

        public async Task<IEnumerable<User>> GetUsersInChatAsync(Guid chatId)
        {
            var chat = await Task.FromResult(_chatRepository.GetChatById(chatId));

            if (chat == null || chat.Users == null)
            {
                return Enumerable.Empty<User>();
            }
            return await Task.FromResult(chat.Users.Select(chatUser => chatUser.User));
        }


        public async Task RenameChatAsync(Guid chatId, string chatName)
        {
            var chat = await Task.FromResult(_chatRepository.GetChatById(chatId));
            if (chat != null)
            {
                chat.Name = chatName;
                await Task.FromResult(_chatRepository.UpdateChat(chat));
            }
        }

        public async Task<bool> ChatExistsAsync(Guid chatId)
        {
            var chat = await Task.FromResult(_chatRepository.GetChatById(chatId));
            return chat != null;
        }
    }
}
