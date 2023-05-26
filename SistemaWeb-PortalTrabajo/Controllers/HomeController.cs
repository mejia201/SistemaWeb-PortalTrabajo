using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SistemaWeb_PortalTrabajo.Conexion;
using SistemaWeb_PortalTrabajo.Context;
using SistemaWeb_PortalTrabajo.Models;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web;

namespace SistemaWeb_PortalTrabajo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly portalTrabajoDbContext _portalContext;


        public HomeController(ILogger<HomeController> logger, portalTrabajoDbContext dbContext)
        {
            _logger = logger;
            _portalContext = dbContext;


        }

        public IActionResult Index()
        {
            //extraer nombre de la tabla Empresa
            var empresa = _portalContext.Model.FindEntityType(typeof(Empresa)).GetTableName();

            ViewBag.TableEmpresa = empresa;


            //conteo de tablas
            int totalUsuarios = _portalContext.Usuario.Count();
            ViewBag.TotalUsuarios = totalUsuarios;

            int totalEmpresas = _portalContext.Empresa.Count();
            ViewBag.TotalEmpresas = totalEmpresas;

            
            int totalCategorias = _portalContext.Categoria.Count();
            ViewBag.TotalCategorias = totalCategorias;

            int totalTrabajos = _portalContext.Trabajo.Count();
            ViewBag.TotalTrabajos = totalTrabajos;


            //Llamar al metodo ListarEmpleo en una lista
            List<Trabajo> datos = ListarEmpleo();
            return View(datos);
        }



        public List<Trabajo> ListarEmpleo()
        {
            List<Trabajo> datos = _portalContext.Trabajo
                                 .Include(t => t.Empresa)
                                 .Where(t => t.estado == "Disponible")
                                 .OrderByDescending(t => t.fechaPublicacion)
                                 .Take(4)
                                 .ToList();



            //Listar Empresa y cantidad de empleos
            var empresasConCantidad = _portalContext.Empresa
            .Select(e => new
            {
               Empresa = e,
               CantidadTrabajos = e.Trabajo.Count()
            })
            .ToList();

            ViewBag.EmpresasConCantidad = empresasConCantidad;



            //Obtener mis categorias
            var categorias = _portalContext.Categoria
            .Select(c => new
            {
               Categoria = c
            })
               .ToList();

            ViewBag.Categorias = categorias;


            return datos;
        }

        public IActionResult Privacy()
        {
			ViewBag.nombre = HttpContext.Session.GetString("Usuario");
			ViewBag.nombreEmpresa = HttpContext.Session.GetString("Empresa");


            Empresa oEmpresa = new Empresa();

            oEmpresa.correo_empresa = (string)TempData["correoEmpresa"];


            Usuario oUsuario = new Usuario();

            oUsuario.correo = (string)TempData["correoUsuario"];

            return View();
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(Usuario usuario)
        {
                using (SqlConnection oConexion = new SqlConnection(conexion.CN))
                {
                    SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", oConexion);
                    cmd.Parameters.AddWithValue("correo", usuario.correo);
                    cmd.Parameters.AddWithValue("clave", usuario.clave);

                    cmd.CommandType = CommandType.StoredProcedure;

                    oConexion.Open();

                    usuario.idUsuario = Convert.ToInt32(cmd.ExecuteScalar());

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {

                        if (usuario.idUsuario != 0)
                        {

                            int id = (int)reader["idUsuario"];
                            string nombre = (string)reader["nombre"];
                            string apellido = (string)reader["apellido"];
                            string correo = (string)reader["correo"];

                            usuario.idUsuario = id;
                            usuario.nombre = nombre;
                            usuario.apellido = apellido;
                            usuario.correo = correo;


                        }
                        else
                        {
                            ViewData["Mensaje"] = "Usuario no encontrado!";
                            return View();
                        }


                    }

                    reader.Close();
                    oConexion.Close();


                }

                if (usuario.idUsuario != 0)
                {
                    HttpContext.Session.SetString("Usuario", usuario.nombre + " " + usuario.apellido);
                    TempData["correoUsuario"] = usuario.correo;
                    
                return RedirectToAction("Dashboard", "Home", new { mostrarSegundoAcordeon = false });

                }

                else
                {
                    
                    ViewData["Mensaje"] = "Usuario no encontrado!";
                    
                return View();

                }
                
          
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(Usuario usuario)
        {
			bool registrado;
			string mensaje;

			var fotoFile = HttpContext.Request.Form.Files["fotoInput"];

			string fechaNacimientoString = Request.Form["fechaNacimiento"];

			//foto

			if (fotoFile != null && fotoFile.Length > 0)
			{

				byte[] fotoBytes;
				using (var memoryStream = new MemoryStream())
				{
					fotoFile.CopyTo(memoryStream);
					fotoBytes = memoryStream.ToArray();
				}


				usuario.foto = fotoBytes;
			}

			//fecha

			DateTime fechaNacimiento;
			DateTime.TryParse(fechaNacimientoString, out fechaNacimiento);

			usuario.fecha_nacimiento = fechaNacimiento;

			using (SqlConnection oConexion = new SqlConnection(conexion.CN))
			{
				SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", oConexion);
				cmd.Parameters.AddWithValue("nombre", usuario.nombre);
				cmd.Parameters.AddWithValue("apellido", usuario.apellido);
				cmd.Parameters.AddWithValue("correo", usuario.correo);
				cmd.Parameters.AddWithValue("clave", usuario.clave);
				cmd.Parameters.AddWithValue("direccion", usuario.direccion);
				cmd.Parameters.AddWithValue("contacto", usuario.contacto);
				cmd.Parameters.AddWithValue("foto", usuario.foto);
				cmd.Parameters.AddWithValue("fecha_nacimiento", usuario.fecha_nacimiento);


				cmd.Parameters.Add("registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

				cmd.CommandType = CommandType.StoredProcedure;

				oConexion.Open();

				cmd.ExecuteNonQuery();

				registrado = Convert.ToBoolean(cmd.Parameters["registrado"].Value);
				mensaje = cmd.Parameters["mensaje"].Value.ToString();
			}

			ViewData["Mensaje"] = mensaje;

			if (registrado)
			{
				return RedirectToAction("Login");
			}
			else
			{
				return View();
			}
		}


		public IActionResult LoginEmpresa()
		{
			return View();
		}


		[HttpPost]
		public IActionResult LoginEmpresa(Empresa empresa)
		{
			using (SqlConnection oConexion = new SqlConnection(conexion.CN))
			{
				SqlCommand cmd = new SqlCommand("sp_ValidarEmpresa", oConexion);
				cmd.Parameters.AddWithValue("correo_empresa", empresa.correo_empresa);
				cmd.Parameters.AddWithValue("clave_empresa", empresa.clave_empresa);

				cmd.CommandType = CommandType.StoredProcedure;

				oConexion.Open();

				empresa.idEmpresa = Convert.ToInt32(cmd.ExecuteScalar());

				SqlDataReader reader = cmd.ExecuteReader();

				while (reader.Read())
				{

					if (empresa.idEmpresa != 0)
					{

						int id = (int)reader["idEmpresa"];
						string nombre = (string)reader["nombre_empresa"];
						string correo = (string)reader["correo_empresa"];

						empresa.idEmpresa = id;
						empresa.nombre_empresa = nombre;
						empresa.correo_empresa = correo;


					}
					else
					{
						ViewData["Mensaje"] = "Empresa no encontrada!";
						return View();
					}


				}

				reader.Close();
				oConexion.Close();


			}

			if (empresa.idEmpresa != 0)
			{
				HttpContext.Session.SetString("Empresa", empresa.nombre_empresa);
				TempData["correoEmpresa"] = empresa.correo_empresa;

				return RedirectToAction("Privacy", "Home");

			}

			else
			{

				ViewData["Mensaje"] = "Empresa no encontrada!";

				return View();

			}


		}




		public IActionResult SignUpEmpresa()
        {
            return View();
        }


        [HttpPost]
        public IActionResult SignUpEmpresa(Empresa empresa)
        {
            bool registrado;
            string mensaje;

            using (SqlConnection oConexion = new SqlConnection(conexion.CN))
            {
                SqlCommand cmd = new SqlCommand("sp_RegistrarEmpresa", oConexion);
                cmd.Parameters.AddWithValue("nombre_empresa", empresa.nombre_empresa);
                cmd.Parameters.AddWithValue("correo_empresa ", empresa.correo_empresa);
                cmd.Parameters.AddWithValue("clave_empresa ", empresa.clave_empresa);
                cmd.Parameters.AddWithValue("descripcion_empresa  ", empresa.descripcion_empresa);
                cmd.Parameters.AddWithValue("ubicacion_empresa ", empresa.ubicacion_empresa);
                cmd.Parameters.AddWithValue("sector ", empresa.sector);
                cmd.Parameters.AddWithValue("sitio_web  ", empresa.sitio_web);

                cmd.Parameters.Add("registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                cmd.CommandType = CommandType.StoredProcedure;

                oConexion.Open();

                cmd.ExecuteNonQuery();

                registrado = Convert.ToBoolean(cmd.Parameters["registrado"].Value);
                mensaje = cmd.Parameters["mensaje"].Value.ToString();
            }

            ViewData["Mensaje"] = mensaje;

            if (registrado)
            {
                return RedirectToAction("LoginEmpresa");
            }
            else
            {
                return View();
            }
        }






        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}