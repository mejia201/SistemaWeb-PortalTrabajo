using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace SistemaWeb_PortalTrabajo.Models
{
    public class Empresa
    {

        [Key]
        public int idEmpresa { get; set; }
        public string? nombre_empresa { get; set; }
        public string? descripción { get; set; }
        public string? ubicación { get; set; }
        public string? sector { get; set; }
        public string? sitio_web { get; set; }

   

    }
}
