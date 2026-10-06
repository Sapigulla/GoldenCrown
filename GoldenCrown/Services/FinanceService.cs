using GoldenCrown.Database;
using GoldenCrown.Models;
using Microsoft.EntityFrameworkCore;

namespace GoldenCrown.Services
{
    public class FinanceService : IFinanceService
    {
        private readonly ApplicationDbContext _dbcontext;
        public FinanceService(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<Result<decimal>> GetBalanceAsync(string token)
        {
            var session = await _dbcontext.Sessions.FirstOrDefaultAsync(s => s.Token == token);
            if (session == null)
            {
                return Result<decimal>.Failure("Пользователь не авторизован.");
            }

            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Id == session.UserId);
            var account = await _dbcontext.Accounts.FirstOrDefaultAsync(a => a.UserId == user!.Id);
            return Result<decimal>.Success(account!.Balance);
        }

        public async Task<Result> DepositAsync(string token, decimal amount)
        {
            var session = await _dbcontext.Sessions.FirstOrDefaultAsync(s => s.Token == token);
            if (session == null)
            {
                return Result.Failure("Пользователь не авторизован.");
            }

            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Id == session.UserId);
            var account = await _dbcontext.Accounts.FirstOrDefaultAsync(a => a.UserId == user!.Id);
          
            account!.Balance += amount;
            await _dbcontext.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> TransferAsync(string fromToken, string toLogin, decimal amount)
        {
            var fromSession = await _dbcontext.Sessions.FirstOrDefaultAsync(s => s.Token == fromToken);
            if (fromSession == null)
            {
                return Result.Failure("Пользователь не авторизован.");
            }

            var fromUser = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Id == fromSession.UserId);
            var fromAccount = await _dbcontext.Accounts.FirstOrDefaultAsync(a => a.UserId == fromUser!.Id);
            var toUser = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Login == toLogin);
            if (toUser == null)
            {
                return Result.Failure("Пользователь не найден.");
            }
            var toAccount = await _dbcontext.Accounts.FirstOrDefaultAsync(a => a.UserId == toUser.Id);
            if (fromAccount!.Balance < amount)
            {
                return Result.Failure("Недостаточно средств.");
            }
            fromAccount.Balance -= amount;
            toAccount!.Balance += amount;

            var transaction = new Transaction
            {
                ReceiverAccountId = toAccount.Id,
                SenderAccountId = fromAccount.Id,
                Amount = amount,
                CreatedAt = DateTime.UtcNow
            };

            _dbcontext.Transactions.Add(transaction);

            await _dbcontext.SaveChangesAsync();
            return Result.Success();
        }
    }
}
