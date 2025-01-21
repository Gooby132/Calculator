using Calculator.Web.Shared;

namespace Calculator.Web.Users.Response;

public class GetAllUsersResponse
{

    public IEnumerable<ErrorDto>? Errors { get; init; }

}
