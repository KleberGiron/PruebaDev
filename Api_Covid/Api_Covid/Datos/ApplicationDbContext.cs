using Api_Covid.Models;
using Microsoft.EntityFrameworkCore;

namespace Api_Covid.Datos
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {

        }
        public DbSet<Empleado> empleados {  get; set; }
    }
}
