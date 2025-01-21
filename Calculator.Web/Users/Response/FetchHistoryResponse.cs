using Calculator.Web.Shared;
using Calculator.Web.Users.Dtos;

namespace Calculator.Web.Users.Response;

public class FetchHistoryResponse
{

    public IEnumerable<OperationDto>? Operations { get; set; }
    public IEnumerable<ErrorDto>? Errors { get; set; }

}
