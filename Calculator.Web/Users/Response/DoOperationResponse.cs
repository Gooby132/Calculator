using Calculator.Web.Shared;
using Calculator.Web.Users.Dtos;

namespace Calculator.Web.Users.Response;

public class DoOperationResponse
{

    public IEnumerable<ErrorDto>? Errors { get; set; }
    public OperationDto? Operation { get; init; }

}
