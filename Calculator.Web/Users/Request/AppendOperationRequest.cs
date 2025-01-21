using Calculator.Web.Users.Dtos;

namespace Calculator.Web.Users.Request;

public class AppendOperationRequest
{

    public required int UserId { get; init; }
    public OperationDto? Operation { get; init; }

}
