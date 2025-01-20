using Calculator.Domain.Shared;
using Calculator.Web.Shared;

namespace Calculator.Web.Helpers;

public static class UserExtensions
{

    public static ErrorDto ToDto(this ErrorBase error) => new ErrorDto
    {
        Error = error.Error,
        Message = error.Message,
    };

}
