namespace Calculator.Web.Users.Dtos;

public record OperationDto(string Value1, string Value2, int Operation, string? Custom, string? Result);

public record UserDto(int Id, string InternetAddress, IEnumerable<OperationDto> Operations);
