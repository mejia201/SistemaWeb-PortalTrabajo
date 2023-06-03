using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace SistemaWeb_PortalTrabajo.Models
{
    public class Candidato
    {
        [Key]
        public int idPostulacion { get; set; }
        public int idUsuario { get; set; }
        public int idTrabajo { get; set; }

        public DateTime? FechaSolicitud { get; set; }

        public string? Estado { get; set; }


		[ForeignKey("idUsuario")]
		public Usuario Usuario { get; set; }

		[ForeignKey("idTrabajo")]
		public Trabajo Trabajo { get; set; }


    }
}
