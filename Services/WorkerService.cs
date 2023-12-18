namespace WebApi.Services;

using AutoMapper;
using BCrypt.Net;
using WebApi.Authorization;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Pagination;
using WebApi.Models.Users;

public interface IUserService
{
    AuthenticateResponse Authenticate(AuthenticateRequest model);
    IEnumerable<User> GetAll();

    List<UserResponse> GetFilter(PaginationFilter filter);
    User GetById(int id);
    void Register(RegisterRequest model);
    void Update(int id, UpdateRequest model);
    void Delete(int id);
    public IEnumerable<LoginHistory> GetByLogin(int id);
}

public class WorkerService : IUserService
{
    private DataContext _context;
    private IJwtUtils _jwtUtils;
    private readonly IMapper _mapper;

    public WorkerService(
        DataContext context,
        IJwtUtils jwtUtils,
        IMapper mapper)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _mapper = mapper;
    }

    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {
        var user = _context.Users.SingleOrDefault(x => x.UserName == model.Username);

        if (user == null || !BCrypt.Verify(model.Password, user.PasswordHash))
            throw new AppException("Username or password is incorrect");

        var response = _mapper.Map<AuthenticateResponse>(user);
        LogLoginHistory(user);

        response.Token = _jwtUtils.GenerateToken(user);

        return response;
    }

    private async Task LogLoginHistory(User user)
    {
        var loginHistory = new LoginHistory
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            UserName = user.UserName,
            InsertedDate = DateTime.UtcNow,
            UserId = user.id
        };
        _context.LoginHistories.Add(loginHistory);
        await _context.SaveChangesAsync();
    }

    public IEnumerable<User> GetAll()
    {
        return _context.Users;
    }

    public List<UserResponse> GetFilter(PaginationFilter filter)
    {
        var users=  _context.Users
        .Skip((filter.PageNumber - 1) * filter.PageSize)
        .Take(filter.PageSize)
        .ToList();

        var userResponses=_mapper.Map<List<UserResponse>>(users);
        return userResponses;
    }

    public User GetById(int id)
    {
        return getUser(id);
    }

    public void Register(RegisterRequest model)
    {
         if (_context.Users.Any(x => x.UserName == model.Username))
            throw new AppException("Username '" + model.Username + "' is already taken");

        var user = _mapper.Map<User>(model);

        user.PasswordHash = BCrypt.HashPassword(model.Password);

        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void Update(int id, UpdateRequest model)
    {
        var user = getUser(id);

        if (model.Username != user.UserName && _context.Users.Any(x => x.UserName == model.Username))
            throw new AppException("Username '" + model.Username + "' is already taken");

        if (!string.IsNullOrEmpty(model.Password))
            user.PasswordHash = BCrypt.HashPassword(model.Password);

        _mapper.Map(model, user);
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var user = getUser(id);
        _context.Users.Remove(user);
        _context.SaveChanges();
    }

    private User getUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }

    public IEnumerable<LoginHistory> GetByLogin(int id)
    {
         var user = _context.Users.FirstOrDefault(u => u.id == id);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with id {id} not found.");
        }

        var loginHistories = _context.LoginHistories
                                     .Where(lh => lh.UserId == user.id)
                                     .OrderBy(lh => lh.InsertedDate)
                                     .ToList();

        return loginHistories;
    }


    public LoginHistory GetByLogin(string email)
    {
        throw new NotImplementedException();
    }

}