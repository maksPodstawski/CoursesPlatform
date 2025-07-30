using Model;
using System;
using System.Collections.Generic;

namespace IDAL
{
    public interface IInvitationRepository
    {
        Invitation AddInvitation(Invitation invitation);
        Invitation? GetInvitationById(Guid invitationId);
        Invitation? GetPendingInvitation(string email, Guid courseId);
        List<Invitation> GetInvitationsByCourse(Guid courseId);
        List<Invitation> GetInvitationsByEmail(string email);
        void UpdateInvitation(Invitation invitation);
        void DeleteInvitation(Invitation invitation);
    }
} 