namespace Model.DTO
{
    public record CreatorResponseDTO
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public string UserName { get; init; } = string.Empty;
        public int CoursesCount { get; init; }
        public List<string> CourseNames { get; init; } = new();

        public static CreatorResponseDTO FromCreator(Creator creator)
        {
            return new CreatorResponseDTO
            {
                Id = creator.Id,
                UserId = creator.UserId,
                UserName = creator.User?.UserName ?? "", 
                CoursesCount = creator.Courses?.Count ?? 0,
                CourseNames = creator.Courses?.Select(c => c.Name).ToList() ?? new List<string>()
            };
        }
        public Creator ToCreator()
        {
            return new Creator
            {
                Id = this.Id,
                UserId = this.UserId,
                User = new User { Id = this.UserId, UserName = this.UserName },
                Courses = this.CourseNames.Select(name => new Course { Name = name }).ToList()
            };
        }
    }

    public record AppendCreatorDTO
    {
        public required Guid CourseId { get; init; }
        public required Guid UserId { get; init; }
    }
} 