using Calculator.Web.Users.Dtos;

namespace Calculator.Web.Users.Request;

public class AppendOperationRequest
{

    public string? UserId { get; init; }
    public OperationDto? Operation { get; init; }

}
