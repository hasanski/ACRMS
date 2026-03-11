using ACRMS.Data;
using ACRMS.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ACRMS.Repository
{
    public class DepartmentRepository: IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;
        public DepartmentRepository(ApplicationDbContext context) => _context = context;
        public async Task<List<Department>> ListAsync()
        {
            return await _context.Departments
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            name = name.Trim();

            return await _context.Departments.AnyAsync(x =>
                x.Name == name &&
                (!excludeId.HasValue || x.Id != excludeId.Value));
        }

        public async Task CreateAsync(Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Department department)
        {
            var existingDepartment = await _context.Departments.FindAsync(department.Id);

            if (existingDepartment is not null)
            {
                existingDepartment.Name = department.Name;
                existingDepartment.FacultyName = department.FacultyName;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department is not null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
        }

    }
}
