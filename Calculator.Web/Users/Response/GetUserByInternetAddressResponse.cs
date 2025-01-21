using Calculator.Web.Shared;
using Calculator.Web.Users.Dtos;

namespace Calculator.Web.Users.Response;

public class GetUserByInternetAddressResponse
{

    public IEnumerable<ErrorDto>? Errors { get; init; }

    public UserDto? User { get; init; }

}
