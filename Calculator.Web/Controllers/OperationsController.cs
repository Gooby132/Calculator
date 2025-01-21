using Calculator.Domain.OperationsService;
using Calculator.Domain.Shared;
using Calculator.Domain.Users;
using Calculator.Domain.Users.Errors;
using Calculator.Domain.Users.ValueObjects;
using Calculator.Persistence.Context;
using Calculator.Web.Helpers;
using Calculator.Web.Users.Request;
using Calculator.Web.Users.Response;
using Microsoft.AspNetCore.Mvc;

namespace Calculator.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsController : ControllerBase
    {

        private readonly IUserRepository _userRepository;
        private readonly IOperationService _operationService;
        private readonly IUnitOfWork _uow;

        public OperationsController(IUserRepository userRepository, IOperationService operationService, IUnitOfWork uow)
        {
            _userRepository = userRepository;
            _operationService = operationService;
            _uow = uow;
        }

        [HttpGet("available-operations")]
        public IActionResult AvailableOperations(CancellationToken token = default)
        {
            return Ok(new AvailableOperationsResponse
            {
                Computations = Computation.List.Select(c => new Users.Dtos.ComputationDto(
                    c.Value,
                    c.Name
                ))
            });
        }

        [HttpPost("append-operation")]
        public async Task<IActionResult> AppendOperation(AppendOperationRequest request, CancellationToken token = default)
        {

            if (request.Operation is null)
                return BadRequest(new AppendOperationResponse
                {
                    Errors = [ErrorFactory.OperationProvidedIsNull().ToDto()]
                });

            // validate parameters

            var operation = Operation.Create(request.Operation.Value1, request.Operation.Value2, request.Operation.Operation, request.Operation.Custom);

            if (operation.IsFailed)
                return BadRequest(new AppendOperationResponse
                {
                    Errors = operation.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
                });

            // fetch user

            var user = await _userRepository.GetById(request.UserId, token);

            if (user.IsFailed)
                return BadRequest(new AppendOperationResponse
                {
                    Errors = user.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
                });

            if (user.Value is null)
                return NotFound(request.UserId);

            // append user operations to later job queue

            var res = user.Value.AppendOperation(operation.Value);

            if (res.IsFailed)
                return BadRequest(new AppendOperationResponse
                {
                    Errors = res.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
                });

            // persist changes

            var update = _userRepository.Update(user.Value);

            if (update.IsFailed)
                return BadRequest(new AppendOperationResponse
                {
                    Errors = update.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
                });

            // complete transaction

            var transaction = await _uow.Commit(token);

            if (transaction.IsFailed)
                return BadRequest(new AppendOperationResponse
                {
                    Errors = transaction.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
                });

            return Ok(new AppendOperationResponse { });
        }

        [HttpPost("do-operation")]
        public async Task<IActionResult> DoOperation(DoOperationRequest request, CancellationToken token = default)
        {
            // try get user id

            if (request.Operation is null)
                return BadRequest(new DoOperationResponse
                {
                    Errors = [ErrorFactory.OperationProvidedIsNull().ToDto()]
                });

            // validate parameters

            var operation = Operation.Create(request.Operation.Value1, request.Operation.Value2, request.Operation.Operation, request.Operation.Custom);

            if (operation.IsFailed)
                return BadRequest(new DoOperationResponse
                {
                    Errors = operation.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
                });

            // fetch userHistory

            var user = await _userRepository.GetById(request.UserId, token);

            if (user.IsFailed)
                return BadRequest(new DoOperationResponse
                {
                    Errors = user.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
                });

            if (user.Value is null)
                return NotFound(request.UserId);

            // append user operations to later job queue

            var res = await user.Value.ComputeAndAppendOperation(operation.Value, _operationService, token);

            if (res.IsFailed)
                return BadRequest(new DoOperationResponse
                {
                    Errors = res.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
                });

            // persist changes

            var update = _userRepository.Update(user.Value);

            if (update.IsFailed)
                return BadRequest(new DoOperationResponse
                {
                    Errors = update.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
                });

            // complete transaction

            var transaction = await _uow.Commit(token);

            if (transaction.IsFailed)
                return BadRequest(new DoOperationResponse
                {
                    Errors = transaction.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
                });

            return Ok(new DoOperationResponse
            {
                Operation = res.Value.ToDto()
            });
        }

        [HttpGet("fetch-history")]
        public async Task<IActionResult> FetchHistory([FromQuery] FetchHistoryRequest request, CancellationToken token = default)
        {
            var user = await _userRepository.GetOperations(request.UserId, request.Size, request.Page, token: token);

            if (user.IsFailed) return BadRequest(new FetchHistoryResponse
            {
                Errors = user.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
            });

            if (user.Value is null)
                return NotFound(request.UserId);

            return Ok(new FetchHistoryResponse
            {
                Operations = user.Value.Operations.Select(o => o.ToDto())
            });
        }
    }
}
