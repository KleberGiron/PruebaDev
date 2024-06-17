using Api_Covid.Datos;
using Api_Covid.Models;
using Api_Covid.Repository.IRepository;

namespace Api_Covid.Repository
{
    public class EmpleadoRepositorio : Repositorio<Empleado>, IEmpleado
    {
        private readonly ApplicationDbContext _db;
        public EmpleadoRepositorio(ApplicationDbContext db) :base(db) 
        {
            _db = db;
        }
        public async Task<Empleado> Actualizar(Empleado entidad)
        {
            entidad.FechaDosisVacuna = DateTime.Now;
            _db.empleados.Update(entidad);
            await _db.SaveChangesAsync();
            return entidad;
        }
    }
}
