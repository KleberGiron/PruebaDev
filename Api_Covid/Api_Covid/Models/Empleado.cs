using System.ComponentModel.DataAnnotations;

namespace Api_Covid.Models
{
    public class Empleado
    {
        [Key]
        public  int Id { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public string PuestoLaboral { get; set; }

        public string Vacuna { get; set; }

        public DateTime? FechaDosisVacuna { get; set; }

    }
}
