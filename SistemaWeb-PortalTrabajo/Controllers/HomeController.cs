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
using System.Security.Claims;

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

            ViewBag.EmpresasConCantidad = empresasConCantidad.Take(4);



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

            ViewBag.nombreEmpresa = HttpContext.Session.GetString("Empresa");


            Empresa oEmpresa = new Empresa();

            oEmpresa.correo_empresa = (string)TempData["correoEmpresa"];


            Usuario oUsuario = new Usuario();

            oUsuario.correo = (string)TempData["correoUsuario"];


            int idEmpresa = (int)TempData.Peek("idEmpresa");
            ViewBag.idEmpresa = idEmpresa;


            List<Empresa> perfil = ObtenerPerfilPorIdEmpresa(idEmpresa);

            ViewBag.PerfilEmpresa = perfil;


            int conteoPostulados = ObtenerConteoPostulados(idEmpresa);

            ViewBag.conteo = conteoPostulados;

            return View();
        }

        public IActionResult TrabajosEmpresa()
        {

			ViewBag.nombreEmpresa = HttpContext.Session.GetString("Empresa");

			int idEmpresa = (int)TempData.Peek("idEmpresa");

            List<Trabajo> datos = ListarEmpleosPorEmpresa(idEmpresa);

            List<Empresa> perfil = ObtenerPerfilPorIdEmpresa(idEmpresa);

            ViewBag.PerfilEmpresa = perfil;

			int conteoPostulados = ObtenerConteoPostulados(idEmpresa);

			ViewBag.conteo = conteoPostulados;

			return View(datos);
        }


        public IActionResult PublicarEmpleo()
        {

			ViewBag.nombreEmpresa = HttpContext.Session.GetString("Empresa");

			int idEmpresa = (int)TempData.Peek("idEmpresa");

            List<Empresa> perfil = ObtenerPerfilPorIdEmpresa(idEmpresa);

            ViewBag.PerfilEmpresa = perfil;

            int conteoPostulados = ObtenerConteoPostulados(idEmpresa);

            ViewBag.conteo = conteoPostulados;


			var trabajoGuardadoExitoso = Request.Cookies["TrabajoGuardadoExitoso"];
			if (trabajoGuardadoExitoso == "true")
			{
				ViewBag.TrabajoGuardadoExitoso = true;
				Response.Cookies.Delete("TrabajoGuardadoExitoso");
			}

			

			var categorias = _portalContext.Categoria.ToList();


			return View(categorias);
        }


        [HttpPost]
        public IActionResult PublicarEmpleo(Trabajo trabajo)
        {
            //idEmpresa Logeada
			int idEmpresa = (int)TempData.Peek("idEmpresa");

			try
			{

				var nuevoTrabajo = new Trabajo
				{
					titulo = trabajo.titulo,
					descripcion = trabajo.descripcion,
					requisitos = trabajo.requisitos,
					ubicacion = trabajo.ubicacion,
					salario = trabajo.salario,
					tipoContrato = trabajo.tipoContrato,
					fechaPublicacion = DateTime.Now,
					fechaVencimiento = DateTime.Now.AddDays(30),
					idEmpresa = idEmpresa,
					estado = "Disponible",
					idCategoria = trabajo.idCategoria
				};


				_portalContext.Trabajo.Add(nuevoTrabajo);


				_portalContext.SaveChanges();

				Response.Cookies.Append("TrabajoGuardadoExitoso", "true");
				return RedirectToAction("PublicarEmpleo", "Home");

			}
			catch (Exception ex)
			{
				ViewBag.TrabajoGuardadoExitoso = false;
				return RedirectToAction("Error", "Home");
			}


		}


        public IActionResult VerPostulantes(int idTrabajo)
        {
            int idEmpresa = (int)TempData.Peek("idEmpresa");

            List<Empresa> perfil = ObtenerPerfilPorIdEmpresa(idEmpresa);
            ViewBag.PerfilEmpresa = perfil;

            int conteoPostulados = ObtenerConteoPostulados(idEmpresa);
            ViewBag.conteo = conteoPostulados;

            List<DetalleTrabajoViewModel> detallesTrabajo = new List<DetalleTrabajoViewModel>();

            using (SqlConnection connection = new SqlConnection(conexion.CN))
            {
                connection.Open();

                // Crea la consulta SQL
                string sqlQuery = @"SELECT c.idPostulacion, u.nombre, u.apellido, u.correo, u.contacto, 
                            m.titulo_profesional AS curriculum_titulo_profesional,
                            m.habilidades AS curriculum_habilidades,
                            m.educacion AS curriculum_educacion,
                            m.certificaciones AS curriculum_certificaciones,
                            m.experiencia_laboral AS curriculum_experiencia_laboral,
                            c.FechaSolicitud, c.Estado 
                            FROM Candidato c
                            JOIN Usuario u ON c.idUsuario = u.idUsuario
                            JOIN Curriculum m ON u.idUsuario = m.idUsuario
                            JOIN Trabajo t ON c.idTrabajo = t.idTrabajo
                            WHERE t.idTrabajo = @IdTrabajo";

                // Crea el comando y establece los parámetros
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    command.Parameters.AddWithValue("@IdTrabajo", idTrabajo);

                    // Ejecuta la consulta y lee los resultados
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DetalleTrabajoViewModel detalle = new DetalleTrabajoViewModel
                            {
                                IdPostulacion = reader.GetInt32(0),
                                Nombre = reader.GetString(1),
                                Apellido = reader.GetString(2),
                                Correo = reader.GetString(3),
                                Contacto = reader.GetString(4),
                                TituloProfesional = reader.GetString(5),
                                Habilidades = reader.GetString(6),
                                Educacion = reader.GetString(7),
                                Certificaciones = reader.GetString(8),
                                ExperienciaLaboral = reader.GetString(9),
                                FechaSolicitud = reader.GetDateTime(10),
                                Estado = reader.GetString(11)
                            };

                            detallesTrabajo.Add(detalle);
                        }
                    }
                }
            }

            return View(detallesTrabajo);
        }


        




        public List<Trabajo> ListarEmpleosPorEmpresa(int idEmpresa)
        {
            List<Trabajo> datos = _portalContext.Trabajo
                                 .Include(t => t.Categoria)
                                 .Include(t => t.Empresa)
                                 .Where(t => t.idEmpresa == idEmpresa)
                                 .ToList();
            return datos;
        }


            public List<Empresa> ObtenerPerfilPorIdEmpresa(int idEmpresa)
        {
            var perfil = _portalContext.Empresa
                .Where(e => e.idEmpresa == idEmpresa)
                .ToList();

            return perfil;
        }



        public int ObtenerConteoPostulados(int idEmpresa)
        {
        
              string connectionString = conexion.CN;

       
              string query = "SELECT COUNT(t.idTrabajo) " +
                       "FROM Candidato c " +
                       "JOIN Trabajo t ON c.idTrabajo = t.idTrabajo " +
                       "JOIN Empresa e ON t.idEmpresa = e.idEmpresa " +
                       "WHERE e.idEmpresa = @idEmpresa";

        
             int conteo = 0;

       
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
          
            connection.Open();

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@idEmpresa", idEmpresa);

                conteo = (int)command.ExecuteScalar();
            }
        }

             return conteo;
        }


		public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(Usuario usuario)
        {
            Curriculum curriculum = new Curriculum();

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
                        int idCurriculum = (int)reader["idCurriculum"];

                        usuario.idUsuario = id;
                        usuario.nombre = nombre;
                        usuario.apellido = apellido;
                        usuario.correo = correo;
                        curriculum.idCurriculum = idCurriculum;


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

                TempData["idUsuario"] = usuario.idUsuario;

				if (curriculum.idCurriculum == 0)
                {
                    return RedirectToAction("Curriculum", "Home");
                }
                else
                {
                    return RedirectToAction("BuscarEmpleo", "Home");
                }

            }

            else
            {

                ViewData["Mensaje"] = "Usuario no encontrado!";

                return View();

            }

        }





        public IActionResult Curriculum()
        {

			ViewData["AlertaMensaje"] = "Logeado Correctamente! ";

			ViewBag.nombre = HttpContext.Session.GetString("Usuario");


            return View();
        }

        [HttpPost]
		public IActionResult Curriculum(Curriculum curriculum)
		{
            
            int idUsuario = (int)TempData.Peek("idUsuario");
            ViewBag.idUsuario = idUsuario;

            try
			{
               
                    var nuevoCurriculum = new Curriculum
					{
						idUsuario = idUsuario,
                        titulo_profesional = curriculum.titulo_profesional,
                        experiencia_laboral = curriculum.experiencia_laboral,
                        educacion = curriculum.educacion,
                        habilidades = curriculum.habilidades,
                        certificaciones = curriculum.certificaciones
                    };

					
					_portalContext.Curriculum.Add(nuevoCurriculum);

					
					_portalContext.SaveChanges();

                ViewBag.MensajeCurriculum = "¡El currículum se creó correctamente!";

                return RedirectToAction("BuscarEmpleo");		
			}
			catch (Exception ex)
			{
                return RedirectToAction("Error", "Home");
			}
		}




		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
        {

            return View();
        }



		public IActionResult BuscarEmpleo()
        {

            ViewBag.nombre = HttpContext.Session.GetString("Usuario");


            var postulacionExitosa = Request.Cookies["PostulacionExitosa"];
            if (postulacionExitosa == "true")
            {
                ViewBag.PostulacionExitosa = true;
                Response.Cookies.Delete("PostulacionExitosa");
            }

            int idUsuario = (int)TempData.Peek("idUsuario");
            ViewBag.idUsuario = idUsuario;

            List<Candidato> postulaciones = ObtenerPostulacionesPorIdUsuario(idUsuario);

            ViewBag.Postulaciones = postulaciones;


            List<Usuario> perfil = ObtenerPerfilPorIdUsuario(idUsuario);

            ViewBag.Perfil = perfil;


            var categorias = _portalContext.Categoria.ToList();


            return View(categorias);

		}


        public List<Candidato> ObtenerPostulacionesPorIdUsuario(int idUsuario)
        {
            var postulaciones = _portalContext.Candidato
                .Where(c => c.idUsuario == idUsuario)
                .Include(c => c.Trabajo)
                .ThenInclude(t => t.Empresa)
                .ToList();

            return postulaciones;
        }

        public List<Usuario> ObtenerPerfilPorIdUsuario(int idUsuario)
        {
            var perfil = _portalContext.Usuario
                .Where(u => u.idUsuario == idUsuario)
                .ToList();

            return perfil;
        }


        public ActionResult MostrarCategoria(string nombreCategoria)
        {
            
            ViewBag.NombreCategoria = nombreCategoria;

            
            return View();
        }


        public IActionResult Resultados(int idCategoria)
        {


            var trabajos = _portalContext.Trabajo
            .Where(t => t.idCategoria == idCategoria)
            .Include(t => t.Empresa)
            .Include(t => t.Categoria)
            .ToList();


            var nombreCategoria = _portalContext.Categoria.FirstOrDefault(c => c.idCategoria == idCategoria)?.nombre;

            ViewBag.idCategoria = idCategoria;
            ViewBag.NombreCategoria = nombreCategoria;

            int idUsuario = (int)TempData.Peek("idUsuario");
            ViewBag.idUsuario = idUsuario;

            return View(trabajos);
        }

        [HttpPost]
        public IActionResult GuardarPostulacion(int idTrabajo, int idUsuario)
        {
            try
            {
                var nuevaPostulacion = new Candidato
                {
                    idUsuario = idUsuario,
                    idTrabajo = idTrabajo,
                    FechaSolicitud = DateTime.Now,
                    Estado = "Pendiente"
                };

                _portalContext.Candidato.Add(nuevaPostulacion);
                _portalContext.SaveChanges();


                return RedirectToAction("Confirmacion");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult Confirmacion()
        {

            ViewBag.MensajeConfirmacion = "¡Se ha postulado al empleo correctamente!";

            return View();
        }


        public IActionResult MostrarFoto()
		{
			int idUsuario = (int)TempData.Peek("idUsuario");
			Usuario usuario = ObtenerUsuario(idUsuario);

			if (usuario == null || usuario.foto == null)
			{
				return NotFound();
			}

			return File(usuario.foto, "image/png");
		}


        private Usuario ObtenerUsuario(int id)
		{
			Usuario usuario = _portalContext.Usuario.FirstOrDefault(u => u.idUsuario == id);

			return usuario;
		}



   

		public IActionResult Logout()
		{
			HttpContext.Session.Remove("Usuario");
			return RedirectToAction("Index");
		}

        [HttpPost]
        public IActionResult EliminarCuenta()
        {
            int idUsuario = (int)TempData.Peek("idUsuario");

			var usuarioAEliminar = _portalContext.Usuario.FirstOrDefault(u => u.idUsuario == idUsuario);


			if (usuarioAEliminar != null)
			{

				_portalContext.Usuario.Remove(usuarioAEliminar);
				_portalContext.SaveChanges();
			}

			return RedirectToAction("Index");
        }

   
        [HttpPost]
        public IActionResult EditarPerfil(Usuario usuario, string nombre = null, string apellido = null, string correo = null, string clave = null, string direccion = null, string contacto = null, IFormFile foto = null)
        {

            int idUsuario = (int)TempData.Peek("idUsuario");


            var fotoFile = HttpContext.Request.Form.Files["foto"];

            string fechaNacimientoString = Request.Form["fechaNacimiento"];


    

            var usuarioExistente = _portalContext.Usuario.FirstOrDefault(u => u.idUsuario == idUsuario);
            if (usuarioExistente != null)
            {
                if (!string.IsNullOrEmpty(nombre))
                    usuarioExistente.nombre = nombre;

                if (!string.IsNullOrEmpty(apellido))
                    usuarioExistente.apellido = apellido;

                if (!string.IsNullOrEmpty(correo))
                    usuarioExistente.correo = correo;

                if (!string.IsNullOrEmpty(clave))
                    usuarioExistente.clave = clave;

                if (!string.IsNullOrEmpty(direccion))
                    usuarioExistente.direccion = direccion;

                if (!string.IsNullOrEmpty(contacto))
                    usuarioExistente.contacto = contacto;

                if (foto != null && foto.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        foto.CopyTo(memoryStream);
                        usuarioExistente.foto = memoryStream.ToArray();
                    }
                }

                _portalContext.SaveChanges();

                TempData["SuccessMessage"] = "Perfil actualizado exitosamente";
                return RedirectToAction("Index");
            }

            else
                {
                    TempData["ErrorMessage"] = "No se encontró el usuario";
                    return View();
                }

        }





		public IActionResult LogoutEmpresa()
		{
			HttpContext.Session.Remove("Empresa");
			return RedirectToAction("Index");
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

			var fotoFile = HttpContext.Request.Form.Files["foto"];

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
                @ViewBag.Foto = fotoBytes;

            }

			//fecha

			DateTime fechaNacimiento;
			DateTime.TryParse(fechaNacimientoString, out fechaNacimiento);

			usuario.fecha_nacimiento = fechaNacimiento;
            @ViewBag.fechaNacimiento = fechaNacimiento;

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

                @ViewBag.direccionUsuario = usuario.direccion;
                @ViewBag.contactoUsuario = usuario.contacto;
                
                return RedirectToAction("Login");
			}
			else
			{
                TempData["Error"] = "Ocurrio un error al crear la cuenta";
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
                TempData["idEmpresa"] = empresa.idEmpresa;

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
                cmd.Parameters.AddWithValue("correo_empresa", empresa.correo_empresa);
                cmd.Parameters.AddWithValue("clave_empresa", empresa.clave_empresa);
                cmd.Parameters.AddWithValue("descripcion_empresa", empresa.descripcion_empresa);
                cmd.Parameters.AddWithValue("ubicacion_empresa", empresa.ubicacion_empresa);
                cmd.Parameters.AddWithValue("sector", empresa.sector);
                cmd.Parameters.AddWithValue("sitio_web", empresa.sitio_web);

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


    }
}