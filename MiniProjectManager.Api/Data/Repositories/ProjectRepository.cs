using Microsoft.EntityFrameworkCore;
using MiniProjectManager.Domain.Entities;

namespace MiniProjectManager.Data.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;

    public ProjectRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Project>> GetAllByUserIdAsync(int userId)
    {
        return _context.Projects.Where(p => p.UserId == userId).ToListAsync();
    }

    public Task<Project> GetByIdAsync(int id)
    {
        return _context.Projects.Include(p => p.Tasks).FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Project project)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Project project)
    {
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
    }
}