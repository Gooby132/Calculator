using Calculator.Domain.OperationsService;
using Calculator.Domain.Shared;
using Calculator.Domain.Users;
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
    public class UsersController : ControllerBase
    {

        private readonly IUserRepository _userRepository;
        private readonly IOperationService _operationService;
        private readonly IUnitOfWork _uow;

        public UsersController(IUserRepository userRepository, IOperationService operationService, IUnitOfWork uow)
        {
            _userRepository = userRepository;
            _operationService = operationService;
            _uow = uow;
        }

        [HttpPost("login-user")]
        public async Task<IActionResult> LoginUser(LoginUserRequest request, CancellationToken token = default)
        {

            var remoteIpAddress = request.InternetAddress ?? HttpContext.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(remoteIpAddress))
                return BadRequest(new AppendOperationResponse
                {
                    Errors = [new ErrorDto { Error = 999, Message = "user id not provided" }]
                });

            var user = Domain.Users.User.Create(remoteIpAddress);

            if (user.IsFailed)
                return BadRequest(new AppendOperationResponse
                {
                    Errors = user.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
                });

            var persist = await _userRepository.Persist(user.Value, token);

            if (persist.IsFailed)
                return BadRequest(new AppendOperationResponse
                {
                    Errors = user.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
                });

            var uow = await _uow.Commit(token);

            if (uow.IsFailed)
                return BadRequest(new AppendOperationResponse
                {
                    Errors = user.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
                });

            return Ok(new LoginUserResponse
            {
                User = user.Value.ToDto()
            });
        }


        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUsersRequest request, CancellationToken token = default)
        {
            var users = await _userRepository.GetAll(token);

            if (users.IsFailed)
                return BadRequest(new GetAllUsersResponse
                {
                    Errors = users.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
                });

            return Ok(users.Value.Select(user => user.ToDto()));
        }

        [HttpGet("get-user-by-internet-address")]
        public async Task<IActionResult> GetUserByInternetAddress([FromQuery] GetUserByInternetAddressRequest request, CancellationToken token = default)
        {
            var remoteIpAddress = request.UserId ?? HttpContext.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(remoteIpAddress))
                return BadRequest(new AppendOperationResponse

                {
                    Errors = [new ErrorDto { Error = 999, Message = "user id not provided" }]
                });

            var user = await _userRepository.GetByInternetAddress(remoteIpAddress, token);

            if (user.IsFailed)
                return BadRequest(new GetUserByInternetAddressResponse
                {
                    Errors = user.Errors.Where(e => e is ErrorBase).Select(e => (e as ErrorBase)!.ToDto())
                });

            if (user.Value is null)
                return NotFound(remoteIpAddress);

            return Ok(user.Value.ToDto());
        }

    }
}
