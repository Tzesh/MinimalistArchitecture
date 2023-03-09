using MinimalistArchitecture.Abstract;

namespace MinimalistArchitecture.User
{
    public class UserDTO : DTO
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public UserDTO() { }
        public UserDTO(User user) =>
        (Id, Name, Email) = (user.Id, user.Name, user.Email);
    }
}