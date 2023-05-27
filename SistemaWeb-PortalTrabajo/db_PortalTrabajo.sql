CREATE DATABASE db_PortalTrabajo;
go

USE db_PortalTrabajo;
go

CREATE TABLE Empresa(
idEmpresa int PRIMARY KEY IDENTITY,
nombre_empresa varchar(300),
correo_empresa varchar(100),
clave_empresa  varchar(50),
descripcion_empresa varchar(300), 
ubicacion_empresa varchar(400),
sector varchar(100),
sitio_web varchar(100)
);
go



CREATE TABLE Trabajo(
idTrabajo int PRIMARY KEY IDENTITY,
titulo varchar(300),
descripcion varchar(500),
requisitos varchar(500),
ubicacion varchar(100),
salario decimal(10, 2),
tipoContrato varchar(100),
fechaPublicacion datetime default getdate(),
fechaVencimiento datetime,
idEmpresa int,
estado varchar(20), --Disponible, No disponible
CONSTRAINT FK_Empresa_Trabajo FOREIGN KEY (idEmpresa) REFERENCES Empresa(idEmpresa)
);
go


CREATE TABLE Usuario(
idUsuario int PRIMARY KEY IDENTITY,
nombre varchar(100),
apellido varchar(100),
correo varchar(100),
clave varchar(50),
direccion varchar(500),
contacto varchar(12),
foto varbinary(max),
fecha_nacimiento DATE,
fechaCreacion datetime default getdate()
);
go


CREATE TABLE Curriculum(
idCurriculum int PRIMARY KEY IDENTITY,
idUsuario int,
titulo_profesional VARCHAR(200),
experiencia_laboral varchar(500),
educacion varchar(500),
habilidades varchar(500),
certificaciones varchar(300) null,
CONSTRAINT FK_Usuario_Curriculum FOREIGN KEY (idUsuario) REFERENCES Usuario(idUsuario)
);
go


CREATE TABLE Candidato (
idPostulacion int PRIMARY KEY IDENTITY,
idUsuario int,
idTrabajo int,
FechaSolicitud datetime,
Estado varchar(50), --pendiente, aceptada, rechazada
CONSTRAINT FK_Usuario_SolicitudTrabajo FOREIGN KEY (idUsuario) REFERENCES Usuario(idUsuario),
CONSTRAINT FK_Trabajo_SolicitudTrabajo FOREIGN KEY (idTrabajo) REFERENCES Trabajo(idTrabajo)
);


CREATE TABLE Categoria (
idCategoria int PRIMARY KEY IDENTITY,
nombre varchar(100),
descripcion varchar(max)
);

CREATE TABLE Subcategoria(
idSubcategoria int PRIMARY KEY IDENTITY,
nombre varchar(100),
descripcion varchar(max),
idCategoria int,
CONSTRAINT FK_Categoria_Subcategoria FOREIGN KEY (idCategoria) REFERENCES Categoria(idCategoria)
);


CREATE TABLE TrabajoSubcategoria(
id int PRIMARY KEY IDENTITY,
idTrabajo int,
idSubcategoria int,
CONSTRAINT FK_Trabajo_TrabajoSubcategoria FOREIGN KEY (idTrabajo) REFERENCES Trabajo(idTrabajo),
CONSTRAINT FK_Subcategoria_TrabajoSubcategoria FOREIGN KEY (idSubcategoria) REFERENCES Subcategoria(idSubcategoria)
);


CREATE TABLE Valoracion(
idValoracion int PRIMARY KEY IDENTITY,
idEmpresa int,
idUsuario int,
comentario varchar(500),
calificacion int,
CONSTRAINT FK_Empresa_ValoracionEmpresa FOREIGN KEY (idEmpresa) REFERENCES Empresa(idEmpresa),
CONSTRAINT FK_Usuario_ValoracionEmpresa FOREIGN KEY (idUsuario) REFERENCES Usuario(idUsuario)
);



--INSERTS

INSERT INTO Empresa (nombre_empresa, correo_empresa, clave_empresa,  descripcion_empresa, ubicacion_empresa, sector, sitio_web)
VALUES ('ABC Company', 'ABCompany@gmail.com', 'ABCompany',  'Empresa de tecnología especializada en desarrollo de software.', 'Ciudad de México', 'Tecnología', 'www.abccompany.com');


INSERT INTO Empresa (nombre_empresa, correo_empresa, clave_empresa, descripcion_empresa, ubicacion_empresa, sector, sitio_web)
VALUES ('XYZ Corporation', 'XYZCorporation@gmail.com', 'XYZCorporation',  'Empresa líder en el sector de la manufactura.', 'Madrid', 'Manufactura', 'www.xyzcorp.com');




INSERT INTO Trabajo (titulo, descripcion, requisitos, ubicacion, salario, tipoContrato, fechaVencimiento, idEmpresa, estado)
VALUES ('Gerente de ventas', 'Se requiere un gerente de ventas con habilidades de liderazgo y experiencia en el área comercial.', 'Mínimo 5 años de experiencia en ventas y habilidades de liderazgo comprobadas.', 'San Salvador, El Salvador', 8000.00, 'Tiempo completo', '2023-06-30', 1, 'Disponible');


INSERT INTO Trabajo (titulo, descripcion, requisitos, ubicacion, salario, tipoContrato, fechaVencimiento, idEmpresa, estado)
VALUES ('Desarrollador de software', 'Buscamos un desarrollador de software con experiencia en Java y Python.', 
'Experiencia mínima de 2 años en desarrollo de software. Conocimientos de Java y Python.', 'Santa Ana, El Salvador', 5000.00, 
'Tiempo completo', '2023-06-30', 1, 'Disponible');



INSERT INTO Trabajo (titulo, descripcion, requisitos, ubicacion, salario, tipoContrato, fechaVencimiento, idEmpresa, estado)
VALUES ('Diseñador gráfico', 'Estamos buscando un diseñador gráfico creativo y talentoso.', 'Experiencia en diseño gráfico y manejo de herramientas como Adobe Photoshop e Illustrator.', 'Santa Ana, El Salvador', 
3000.00, 'Medio tiempo', '2023-07-15', 2, 'Disponible');


INSERT INTO Trabajo (titulo, descripcion, requisitos, ubicacion, salario, tipoContrato, fechaVencimiento, idEmpresa, estado)
VALUES ('Desarrollador de software', 'Empresa líder busca un desarrollador de software con experiencia en Java', 
'Licenciatura en Ciencias de la Computación, experiencia mínima de 2 años en desarrollo de software', 
'La Union, El Salvador', 5000.00, 'Tiempo completo', '2023-06-30', 1, 'Disponible');




INSERT INTO Categoria (nombre, descripcion)
VALUES ('Desarrollo de software', 'Categoría relacionada con trabajos de desarrollo de software y programación.');
INSERT INTO Categoria (nombre, descripcion)
VALUES ('Diseño gráfico', 'Categoría relacionada con trabajos de diseño gráfico y creación visual.');
INSERT INTO Categoria (nombre, descripcion)
VALUES ('Ventas y marketing', 'Categoría relacionada con trabajos de ventas, publicidad y estrategias de marketing.');
INSERT INTO Categoria (nombre, descripcion)
VALUES ('Recursos humanos', 'Categoría relacionada con trabajos de gestión de recursos humanos y reclutamiento de personal.');
INSERT INTO Categoria (nombre, descripcion)
VALUES ('Servicio al cliente', 'Categoría relacionada con trabajos de atención al cliente y soporte técnico.');
INSERT INTO Categoria (nombre, descripcion)
VALUES ('Finanzas y contabilidad', 'Categoría relacionada con trabajos de contabilidad, finanzas y análisis económico.');

INSERT INTO Categoria (nombre, descripcion)
VALUES ('Ingeniería', 'Categoría relacionada con trabajos de ingeniería en diferentes disciplinas, como civil, eléctrica, mecánica, etc.');
INSERT INTO Categoria (nombre, descripcion)
VALUES ('Salud y medicina', 'Categoría relacionada con trabajos en el campo de la salud y la medicina, como médicos, enfermeros, terapeutas, etc.');
INSERT INTO Categoria (nombre, descripcion)
VALUES ('Educación', 'Categoría relacionada con trabajos en el sector educativo, como profesores, tutores, administrativos escolares, etc.');
INSERT INTO Categoria (nombre, descripcion)
VALUES ('Administración y finanzas', 'Categoría relacionada con trabajos en el ámbito de la administración de empresas y las finanzas corporativas.');
INSERT INTO Categoria (nombre, descripcion)
VALUES ('Arte y entretenimiento', 'Categoría relacionada con trabajos en el mundo del arte, el entretenimiento, el diseño y la creatividad.');
INSERT INTO Categoria (nombre, descripcion)
VALUES ('Logística y transporte', 'Categoría relacionada con trabajos en el campo de la logística, el transporte y la cadena de suministro.');



INSERT INTO Categoria (nombre, descripcion)
VALUES ('Arquitectura y construcción', 'Categoría relacionada con trabajos en el ámbito de la arquitectura, diseño y construcción de edificios y estructuras.');
INSERT INTO Categoria (nombre, descripcion)
VALUES ('Gastronomía y restauración', 'Categoría relacionada con trabajos en la industria de la gastronomía, restaurantes y servicios de alimentación.');
INSERT INTO Categoria (nombre, descripcion)
VALUES ('Consultoría', 'Categoría relacionada con trabajos en el campo de la consultoría empresarial y asesoramiento profesional.');
INSERT INTO Categoria (nombre, descripcion)
VALUES ('Marketing digital', 'Categoría relacionada con trabajos en el ámbito del marketing digital, publicidad en línea y estrategias digitales.');

INSERT INTO Categoria (nombre, descripcion)
VALUES ('Medios de comunicación', 'Categoría relacionada con trabajos en el sector de los medios de comunicación, periodismo y producción audiovisual.');
INSERT INTO Categoria (nombre, descripcion)
VALUES ('Transporte', 'Categoría relacionada con trabajos en el campo de la logística, distribución y transporte de mercancías.');




--PROCEDIMIENTOS

CREATE OR ALTER PROCEDURE sp_RegistrarUsuario (@nombre varchar(50),@apellido varchar(50),
@correo VARCHAR(100), @clave VARCHAR(20),@direccion varchar(500), @contacto varchar(12), @foto varbinary(max),
@fecha_nacimiento DATE, @registrado BIT OUTPUT, @mensaje VARCHAR(500) OUTPUT)
AS
BEGIN

	DECLARE @resultado VARCHAR(20)

	IF(exists(SELECT * FROM Usuario WHERE correo = @correo AND clave = @clave))
	   SELECT 'El usuario ya existe' AS resultado 

    ELSE BEGIN
	IF (@correo LIKE '%_@__%.__%') BEGIN

			 IF (PATINDEX('%[^a-zA-Z0-9]%', @clave) = 0) BEGIN

			 INSERT INTO Usuario(nombre, apellido, correo, clave, direccion, contacto, 
	                          foto, fecha_nacimiento) 
							  VALUES 
					            (@nombre, @apellido,@correo, @clave, @direccion, @contacto,
						      @foto, @fecha_nacimiento);
							  
				SET @registrado = 1
				SET @mensaje = 'Usuario Registrado'
			 END
			 ELSE
				SET @registrado = 0
				SET @mensaje = 'Correo o contraseña inválidos.'
		END 
		ELSE
			SET @registrado = 0
			SET @mensaje = 'Correo o contraseña inválidos.'
	END
END;


--2

CREATE OR ALTER PROCEDURE sp_ValidarUsuario(@correo VARCHAR(100), @clave VARCHAR(20))
AS
BEGIN
    IF(exists(SELECT * FROM Usuario WHERE correo = @correo AND clave = @clave))
	   SELECT idUsuario, nombre, apellido, correo FROM Usuario WHERE correo = @correo AND clave = @clave;

    ELSE
	    SELECT '0'

END;



--3
CREATE OR ALTER PROCEDURE sp_RegistrarEmpresa (@nombre_empresa varchar(50), @correo_empresa VARCHAR(100), 
@clave_empresa VARCHAR(20),@descripcion_empresa varchar(500), @ubicacion_empresa varchar(500), @sector varchar(500), @sitio_web varchar(500),
@registrado BIT OUTPUT, @mensaje VARCHAR(500) OUTPUT)
AS
BEGIN

	DECLARE @resultado VARCHAR(20)

	IF(exists(SELECT * FROM Empresa WHERE correo_empresa = @correo_empresa AND clave_empresa = @clave_empresa))
	   SELECT 'La Empresa ya existe' AS resultado 

    ELSE BEGIN
	IF (@correo_empresa LIKE '%_@__%.__%') BEGIN

			 IF (PATINDEX('%[^a-zA-Z0-9]%', @clave_empresa) = 0) BEGIN

		INSERT INTO Empresa(nombre_empresa, correo_empresa, clave_empresa, descripcion_empresa, ubicacion_empresa, sector, sitio_web) 
				VALUES 
					(@nombre_empresa, @correo_empresa, @clave_empresa, @descripcion_empresa, @ubicacion_empresa, @sector, @sitio_web);
							  
				SET @registrado = 1
				SET @mensaje = 'Empresa Registrada'
			 END
			 ELSE
				SET @registrado = 0
				SET @mensaje = 'Correo o contraseña inválidos.'
		END 
		ELSE
			SET @registrado = 0
			SET @mensaje = 'Correo o contraseña inválidos.'
	END
END;


--4

CREATE OR ALTER PROCEDURE sp_ValidarEmpresa(@correo_empresa VARCHAR(100), @clave_empresa VARCHAR(20))
AS
BEGIN
    IF(exists(SELECT * FROM Empresa WHERE correo_empresa= @correo_empresa AND clave_empresa = @clave_empresa))
	   SELECT idEmpresa, nombre_empresa, correo_empresa FROM Empresa WHERE correo_empresa = @correo_empresa AND clave_empresa = @clave_empresa;

    ELSE
	    SELECT '0'

END;
