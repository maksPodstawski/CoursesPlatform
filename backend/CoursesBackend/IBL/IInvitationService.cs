using Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IBL
{
    public interface IInvitationService
    {
        Task<Invitation> InviteCoAuthorByEmailAsync(string email, Guid courseId, Guid invitedById);
        Task AcceptInvitationAsync(Guid invitationId, Guid userId);
        Task<List<Invitation>> GetInvitationsByCourseAsync(Guid courseId);
        Task<List<Invitation>> GetInvitationsByEmailAsync(string email);
        Task DeclineInvitationAsync(Guid invitationId, Guid userId);
        Task DeleteInvitationAsync(Guid invitationId);
    }
} 