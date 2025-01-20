using Calculator.Domain.OperationsService;
using Calculator.Domain.Shared;
using Calculator.Domain.Users;
using Calculator.Domain.Users.Errors;
using Calculator.Domain.Users.ValueObjects;
using Calculator.Persistence.Context;
using Calculator.Web.Helpers;
using Calculator.Web.Shared;
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

        public async Task<IActionResult> AppendOperation(AppendOperationRequest request, CancellationToken token = default)
        {

            // try get user id

            var remoteIpAddress = request.UserId ?? HttpContext.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(remoteIpAddress))
                return BadRequest(new AppendOperationResponse

                {
                    Errors = [new ErrorDto { Error = 999, Message = "user id not provided" }]
                });

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

            var user = await _userRepository.GetById(remoteIpAddress, token);

            if (user.IsFailed)
                return BadRequest(new AppendOperationResponse
                {
                    Errors = user.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
                });

            if (user.Value is null)
                return NotFound(remoteIpAddress);

            // append user operations to later job queue

            var res = user.Value.AppendOperation(operation.Value);

            if (res.IsFailed)
                return BadRequest(new AppendOperationResponse
                {
                    Errors = res.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
                });

            // persist changes

            var update = await _userRepository.Update(user.Value, token);

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

        public async Task<IActionResult> DoOperation(DoOperationRequest request,CancellationToken token = default)
        {
            // try get user id

            var remoteIpAddress = request.UserId ?? HttpContext.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(remoteIpAddress))
                return BadRequest(new DoOperationResponse
                {
                    Errors = [new ErrorDto { Error = 999, Message = "user id not provided" }]
                });

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

            // fetch user

            var user = await _userRepository.GetById(remoteIpAddress, token);

            if (user.IsFailed)
                return BadRequest(new DoOperationResponse
                {
                    Errors = user.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
                });

            if (user.Value is null)
                return NotFound(remoteIpAddress);

            // append user operations to later job queue

            var res = await user.Value.ComputeAndAppendOperation(operation.Value, _operationService, token);

            if (res.IsFailed)
                return BadRequest(new DoOperationResponse
                {
                    Errors = res.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
                });

            // persist changes

            var update = await _userRepository.Update(user.Value, token);

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

            return Ok(new DoOperationResponse { });
        }

    }
}
