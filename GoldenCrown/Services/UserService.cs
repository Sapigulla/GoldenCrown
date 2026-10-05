using GoldenCrown.Database;
using GoldenCrown.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountService _accountService;

        public UserService(ApplicationDbContext context, IAccountService accountService)
        {
            _context = context;
            _accountService = accountService;
        }

        public async Task<Result> RegisterAsync(string login, string name, string password)
        {
            //Проверить, существует ли пользователь с таким логином
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (existing != null)
            {
                return Result.Failure("User already exists");
            }

            //Создать нового пользователя
            var user = new User
            {
                Login = login,
                Name = name,
                Password = password
            };

            //Сохранить в базу данных
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _accountService.CreateAccountAsync(login);

            //Вернуть результат(успех / ошибка)
            return Result.Success();
        }

        public async Task<Result<string>> LoginAsync(string login, string password)
        {
            //Проверить, существует ли пользователь с таким логином
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Login == login && x.Password == password);
            if (user == null)
            {
                return Result<string>.Failure("Invalid login or password");
            }

            var session = new Session
            {
                UserId = user.Id,
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            var existingSession = await _context.Sessions.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (existingSession != null)
            {
                existingSession.Token = session.Token;
                existingSession.ExpiresAt = session.ExpiresAt;
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.Sessions.Add(session);
                await _context.SaveChangesAsync();
            }

            return Result<string>.Success(session.Token);
        }
    }
}
