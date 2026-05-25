using Identity.Application.Common.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _context;
    public UserRepository(IdentityDbContext context) =>
        _context = context;

    public async Task AddAsync(User user, CancellationToken cancellationToken) => 
        await _context.Users.AddAsync(user, cancellationToken);

    public async Task AddRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken)=>
        await _context.RefreshTokens.AddAsync(token, cancellationToken);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken) => 
        await _context.Users.AnyAsync(u => u.Email == email, cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) => 
        await _context.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);        

    public async Task SaveChangesAsync(CancellationToken cancellationToken) => 
        await _context.SaveChangesAsync(cancellationToken);
}