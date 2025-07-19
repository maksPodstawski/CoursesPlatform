using IDAL;
using Microsoft.EntityFrameworkCore;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class InvitationRepository : IInvitationRepository
    {
        private readonly CoursesPlatformContext _context;
        public InvitationRepository(CoursesPlatformContext context)
        {
            _context = context;
        }
        public Invitation AddInvitation(Invitation invitation)
        {
            _context.Invitations.Add(invitation);
            _context.SaveChanges();
            return invitation;
        }
        public Invitation? GetInvitationById(Guid invitationId)
        {
            return _context.Invitations.Include(i => i.Course).FirstOrDefault(i => i.Id == invitationId);
        }
        public Invitation? GetPendingInvitation(string email, Guid courseId)
        {
            return _context.Invitations.FirstOrDefault(i => i.Email == email && i.CourseId == courseId && i.Status == InvitationStatus.Pending);
        }
        public List<Invitation> GetInvitationsByCourse(Guid courseId)
        {
            return _context.Invitations.Where(i => i.CourseId == courseId).ToList();
        }
        public List<Invitation> GetInvitationsByEmail(string email)
        {
            return _context.Invitations.Where(i => i.Email == email).ToList();
        }
        public void UpdateInvitation(Invitation invitation)
        {
            _context.Invitations.Update(invitation);
            _context.SaveChanges();
        }
    }
} 