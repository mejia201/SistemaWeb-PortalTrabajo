using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace SistemaWeb_PortalTrabajo.Models
{
    public class Categoria
    {
        [Key]
        public int idCategoria { get; set; }
        public string? nombre { get; set; }
        public string? descripcion { get; set; }

    }
}
