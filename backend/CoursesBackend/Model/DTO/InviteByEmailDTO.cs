using System;

namespace Model.DTO
{
    public class InviteByEmailDTO
    {
        public string Email { get; set; }
        public Guid CourseId { get; set; }
    }
} 