using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace SistemaWeb_PortalTrabajo.Models
{
    public class Subcategoria
    {
        [Key]
        public int idSubcategoria { get; set; }
        public string? nombre { get; set; }
        public string? descripcion { get; set; }
        public int idCategoria  { get; set; }

    }
}
