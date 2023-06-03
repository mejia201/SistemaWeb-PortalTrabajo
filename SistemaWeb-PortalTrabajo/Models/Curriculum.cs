using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace SistemaWeb_PortalTrabajo.Models
{
    public class Curriculum
    {
        [Key]
        public int idCurriculum { get; set; }
        public int idUsuario { get; set; }
        public string? titulo_profesional { get; set; }
        public string? experiencia_laboral { get; set; }
        public string? educacion { get; set; }
        public string? habilidades { get; set; }
        public string? certificaciones { get; set; }


		[ForeignKey("idUsuario")]
		public Usuario Usuario { get; set; }

	}
}
