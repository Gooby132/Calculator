using Calculator.Domain.Shared;
using Calculator.Domain.Users;
using Calculator.Domain.Users.ValueObjects;
using Calculator.Web.Shared;
using Calculator.Web.Users.Dtos;

namespace Calculator.Web.Helpers;

public static class UserExtensions
{

    public static ErrorDto ToDto(this ErrorBase error) => new ErrorDto
    {
        Error = error.Error,
        Message = error.Message,
    };

    public static OperationDto ToDto(this Operation operation) => new OperationDto(
        operation.Value1,
        operation.Value2,
        operation.Computation.Value,
        operation.Custom,
        operation.Result
    );

    public static UserDto ToDto(this User user) => new UserDto(
        user.Id,
        user.InternetAddress,
        user.Operations.Select(operation => operation.ToDto())
        );

}
