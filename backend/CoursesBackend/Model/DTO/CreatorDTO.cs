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
                UserName = creator.User.ToString(),
                CoursesCount = creator.Courses?.Count ?? 0,
                CourseNames = creator.Courses?.Select(c => c.Name).ToList() ?? new List<string>()
            };
        }
    }

    public record BecomeCreatorDTO
    {
        public required Guid CourseId { get; init; }
    }
} 