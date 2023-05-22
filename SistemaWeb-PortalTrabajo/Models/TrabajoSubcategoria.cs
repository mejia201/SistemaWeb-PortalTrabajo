using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace SistemaWeb_PortalTrabajo.Models
{
    public class TrabajoSubcategoria
    {
        [Key]
        public int id { get; set; }
        public int idTrabajo { get; set; }
        public int idSubcategoria { get; set; }

    }
}
