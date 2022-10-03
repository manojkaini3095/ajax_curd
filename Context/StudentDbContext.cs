using ajax_curd.Models;
using Microsoft.EntityFrameworkCore;

namespace ajax_curd.Context
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options)
        { }

        public DbSet<StudentModel> Student { get; set; }
    }
}
