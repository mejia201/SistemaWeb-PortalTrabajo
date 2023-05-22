using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace SistemaWeb_PortalTrabajo.Models
{
    public class Usuario
    {
        [Key]
        public int idUsuario { get; set; }
        public string? nombre { get; set; }
        public string? apellido  { get; set; }
        public string? correo  { get; set; }
        public string? clave  { get; set; }
        public string? direccion  { get; set; }
        public string? contacto  { get; set; }
        public byte[]? foto { get; set; }
        public DateOnly? fecha_nacimiento  { get; set; }

        public DateTime? fechaCreacion { get; set; }

    }
}
