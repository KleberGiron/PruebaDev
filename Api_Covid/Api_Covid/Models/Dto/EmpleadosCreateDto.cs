namespace Api_Covid.Models.Dto
{
    public class EmpleadosCreateDto
    {
        
        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public string PuestoLaboral { get; set; }

        public string Vacuna { get; set; }

        public DateTime? FechaDosisVacuna { get; set; }

    }
}
