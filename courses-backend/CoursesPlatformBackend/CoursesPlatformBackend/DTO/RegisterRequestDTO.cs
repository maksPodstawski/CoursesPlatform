namespace CoursesPlatformBackend.DTO
{
    public record RegisterRequestDTO
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
    }
}
