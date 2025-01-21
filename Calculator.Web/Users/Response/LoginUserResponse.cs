using Calculator.Web.Shared;
using Calculator.Web.Users.Dtos;

namespace Calculator.Web.Users.Response
{
    public class LoginUserResponse
    {
        public IEnumerable<ErrorDto>? Errors { get; set; }
        public UserDto? User { get; init; }
    }
}
