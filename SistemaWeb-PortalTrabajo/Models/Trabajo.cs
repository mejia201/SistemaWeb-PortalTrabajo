using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace SistemaWeb_PortalTrabajo.Models
{
    public class Trabajo
    {
        [Key]
        public int idTrabajo  { get; set; }
        public string? titulo { get; set; }
        public string? descripcion { get; set; }
        public string? requisitos { get; set; } 
        public string? ubicacion { get; set; }  
        public decimal salario { get; set; } 

        public string? tipoContrato { get; set; }  

        public DateTime fechaPublicacion { get; set; }
        public DateTime? fechaVencimiento { get; set; }

        [Column("idEmpresa")]
        public int idEmpresa { get; set; }

        public string? estado { get; set; }


        [ForeignKey("idEmpresa")]
        public Empresa Empresa { get; set; }


    }
}
