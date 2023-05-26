using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace SistemaWeb_PortalTrabajo.Models
{
    public class Empresa
    {

        [Key]
        public int idEmpresa { get; set; }
        public string? nombre_empresa { get; set; }
        public string? correo_empresa { get; set; }
        public string? clave_empresa { get; set; }
        public string? descripcion_empresa { get; set; }
        public string? ubicacion_empresa { get; set; }
        public string? sector { get; set; }
        public string? sitio_web { get; set; }


        public virtual ICollection<Trabajo> Trabajo { get; set; }
    }
}
