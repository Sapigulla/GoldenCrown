using GoldenCrown.Database;
using GoldenCrown.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAccountAsync(string login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login);
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to find a user with login: {login}");
            }
            //Создать новый счет для пользователя с балансом 0
            var account = new Account
            {
                UserId = user.Id,
                Balance = 0,
            };

            //Сохранить в базу данных
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
        }
    }
}
