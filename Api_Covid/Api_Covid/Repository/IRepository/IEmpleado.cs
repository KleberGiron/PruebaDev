using Api_Covid.Models;

namespace Api_Covid.Repository.IRepository
{
    public interface IEmpleado : IRepositorio<Empleado> 
    {
        Task<Empleado> Actualizar(Empleado entidad);
    }
}
