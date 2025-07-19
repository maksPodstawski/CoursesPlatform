using IBL;
using IDAL;
using Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace BL.Services
{
    public class InvitationService : IInvitationService
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IUserService _userService;
        private readonly ICreatorService _creatorService;

        public InvitationService(IInvitationRepository invitationRepository, IUserService userService, ICreatorService creatorService)
        {
            _invitationRepository = invitationRepository;
            _userService = userService;
            _creatorService = creatorService;
        }

        public async Task<Invitation> InviteCoAuthorByEmailAsync(string email, Guid courseId)
        {
            var existing = _invitationRepository.GetPendingInvitation(email, courseId);
            if (existing != null)
                throw new Exception("Zaproszenie już wysłane.");

            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                Email = email,
                CourseId = courseId,
                Status = InvitationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            _invitationRepository.AddInvitation(invitation);
            return invitation;
        }

        public async Task AcceptInvitationAsync(Guid invitationId, Guid userId)
        {
            var invitation = _invitationRepository.GetInvitationById(invitationId);
            if (invitation == null || invitation.Status != InvitationStatus.Pending)
                throw new Exception("Zaproszenie nie istnieje lub zostało już obsłużone.");

            await _creatorService.AddCreatorFromUserAsync(userId, invitation.CourseId);

            invitation.Status = InvitationStatus.Accepted;
            invitation.RespondedAt = DateTime.UtcNow;
            _invitationRepository.UpdateInvitation(invitation);
        }

        public async Task DeclineInvitationAsync(Guid invitationId, Guid userId)
        {
            var invitation = _invitationRepository.GetInvitationById(invitationId);
            if (invitation == null || invitation.Status != InvitationStatus.Pending)
                throw new Exception("Zaproszenie nie istnieje lub zostało już obsłużone.");
            invitation.Status = InvitationStatus.Declined;
            invitation.RespondedAt = DateTime.UtcNow;
            _invitationRepository.UpdateInvitation(invitation);
        }

        public async Task<List<Invitation>> GetInvitationsByCourseAsync(Guid courseId)
        {
            return _invitationRepository.GetInvitationsByCourse(courseId);
        }

        public async Task<List<Invitation>> GetInvitationsByEmailAsync(string email)
        {
            return _invitationRepository.GetInvitationsByEmail(email);
        }
    }
} 