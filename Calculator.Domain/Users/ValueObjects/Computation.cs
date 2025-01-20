using Ardalis.SmartEnum;
using Calculator.Domain.Users.Errors;
using FluentResults;

namespace Calculator.Domain.Users.ValueObjects;

public class Computation : SmartEnum<Computation>
{

    public static readonly Computation Undefined = new Computation(nameof(Undefined), 0);
    public static readonly Computation Addition = new Computation(nameof(Addition), 1);
    public static readonly Computation Subtraction = new Computation(nameof(Subtraction), 2);
    public static readonly Computation Division = new Computation(nameof(Division), 3);
    public static readonly Computation Multiplication = new Computation(nameof(Multiplication), 4);
    public static readonly Computation Concatenation = new Computation(nameof(Concatenation), 5);
    public static readonly Computation IndexOf = new Computation(nameof(IndexOf), 6);
    public static readonly Computation Custom = new Computation(nameof(Custom), 7);

    private Computation(string name, int value) : base(name, value) { }

    public Result<Operation> Compute(Operation operation)
    {

        switch (Name)
        {
            case nameof(Addition):
                return DoAddition(operation);

            case nameof(Subtraction):
                return DoSubtraction(operation);

            case nameof(Division):
                return DoDivision(operation);

            case nameof(Multiplication):
                return DoMultiplication(operation);

            case nameof(Concatenation):
                return DoConcatenation(operation);

            case nameof(IndexOf):
                return DoIndexOf(operation);

            case nameof(Custom):
                return ErrorFactory.CustomOperationsCannotBeInvokedWithDefaultOperations();

            default:
                return ErrorFactory.UndefinedOperationBehavior();
        }
    }

    private Result<Operation> DoIndexOf(Operation operation)
    {
        var result = operation.Value1.IndexOf(operation.Value2);
        return operation.SetResult(result.ToString());
    }

    private Result<Operation> DoConcatenation(Operation operation)
    {
        var result = operation.Value1.Concat(operation.Value2);
        return operation.SetResult(result.ToString());
    }

    private Result<Operation> DoMultiplication(Operation operation)
    {
        Result areNumbers = AreValuesNumbers(operation, out var value1, out var value2);

        if (areNumbers.IsFailed)
            return Result.Fail(areNumbers.Errors);

        return operation.SetResult((value1 * value2).ToString());
    }

    private Result<Operation> DoDivision(Operation operation)
    {
        Result areNumbers = AreValuesNumbers(operation, out var value1, out var value2);

        if (areNumbers.IsFailed)
            return Result.Fail(areNumbers.Errors);

        if (value2 == 0)
            return ErrorFactory.OperationEncounteredDivisionByZeroError();

        return operation.SetResult((value1 / value2).ToString());
    }

    private Result<Operation> DoSubtraction(Operation operation)
    {
        Result areNumbers = AreValuesNumbers(operation, out var value1, out var value2);

        if (areNumbers.IsFailed)
            return Result.Fail(areNumbers.Errors);

        return operation.SetResult((value1 - value2).ToString());
    }

    private Result<Operation> DoAddition(Operation operation)
    {
        Result areNumbers = AreValuesNumbers(operation, out var value1, out var value2);

        if (areNumbers.IsFailed)
            return Result.Fail(areNumbers.Errors);

        return operation.SetResult((value1 + value2).ToString());
    }

    private Result AreValuesNumbers(Operation operation, out double value1, out double value2)
    {
        value2 = -1; // temp 1 is being initialized by the try parse

        if (!double.TryParse(operation.Value1, out value1) || 
            !double.TryParse(operation.Value2, out value2))
            return ErrorFactory.OneOrMoreOfValuesProvidedAreNotNumbers();

        return Result.Ok();
    }
}
