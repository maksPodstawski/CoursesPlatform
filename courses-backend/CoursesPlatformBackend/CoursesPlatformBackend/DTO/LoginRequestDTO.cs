namespace CoursesPlatformBackend.DTO
{
    public record LoginRequestDTO
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}
