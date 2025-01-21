using Calculator.Web.Users.Dtos;

namespace Calculator.Web.Users.Request;

public class DoOperationRequest
{

    public required int UserId { get; init; }
    public required OperationDto Operation { get; init; }

}
