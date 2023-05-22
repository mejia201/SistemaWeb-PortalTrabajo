using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace SistemaWeb_PortalTrabajo.Models
{
    public class Valoracion
    {
        [Key]
        public int idValoracion { get; set; }
        public int idEmpresa { get; set; }
        public int idUsuario { get; set; }
        public string? comentario { get; set; }
        public int? calificacion { get; set; }

    }
}
