using Calculator.Web.Shared;
using Calculator.Web.Users.Dtos;

namespace Calculator.Web.Users.Response;

public class AvailableOperationsResponse
{
    public IEnumerable<ComputationDto>? Computations { get; set; }
    public IEnumerable<ErrorDto>? Errors { get; set; }
}
