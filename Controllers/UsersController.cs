namespace WebApi.Controllers;

using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WebApi.Authorization;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Pagination;
using WebApi.Models.Users;
using WebApi.Services;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private IUserService _userService;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;
    private readonly IValidator<RegisterRequest> _registerValidator;

    public UsersController(
        IUserService userService,
        IMapper mapper,
        IOptions<AppSettings> appSettings,
        IValidator<RegisterRequest> registerValidator)
    {
        _userService = userService;
        _mapper = mapper;
        _appSettings = appSettings.Value;
        _registerValidator = registerValidator;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterRequest model)
    {
        var validation = await _registerValidator.ValidateAsync(model);

        if (!validation.IsValid)
        {
            return StatusCode(StatusCodes.Status400BadRequest, validation.Errors);
        }
        _userService.Register(model);
        return Ok(new { message = "Registration successful" });
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    public IActionResult Authenticate(AuthenticateRequest model)
    {
        var response = _userService.Authenticate(model);
        return Ok(response);
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var users = _userService.GetAll();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var user = _userService.GetById(id);
        return Ok(user);
    }

    [HttpGet("byLogin/{id}")]
    public IEnumerable<LoginHistory> GetByLogin(int id)
    {
       return _userService.GetByLogin(id); 
    }


    [HttpGet("filter")]
    public IActionResult Filter([FromQuery] PaginationFilter filter)
    {
        var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
        var pagedData = _userService.GetFilter(validFilter);
        var totalRecords = pagedData.Count;
        return Ok(new PagedResponse<List<UserResponse>>(pagedData, validFilter.PageNumber, validFilter.PageSize, totalRecords));
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, UpdateRequest model)
    {
        _userService.Update(id, model);
        return Ok(new { message = "User updated successfully" });
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _userService.Delete(id);
        return Ok(new { message = "User deleted successfully" });
    }
}