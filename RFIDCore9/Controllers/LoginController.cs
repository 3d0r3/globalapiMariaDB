using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RFIDCoreGit.BD;
using RFIDCoreGit.Entidades;

namespace RFIDCore9.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class LoginController : ControllerBase
    {
        public static IWebHostEnvironment Enviroment;
        private readonly IConfiguration _configuration;

        public LoginController(IWebHostEnvironment env, IConfiguration configuration)
        {
            Enviroment = env;
            _configuration = configuration;
        }
        
        //--------------------------LOGIN---------------------------------------------------------
        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null)
            {
                return BadRequest(
                    "El campo 'requestData' es requerido por lo que no puede estar vacio");
            }
            Respuesta res = login(loginRequest);
            string strJson = JsonConvert.SerializeObject(res);
            return Content(strJson, "application/json");
        }

        public static Respuesta login(LoginRequest loginRequest)
        {
            Respuesta res = new Respuesta();
            string er = "";
            string idEmpresa = loginRequest.EmpresaId;
            string idAgencia = loginRequest.AgenciaId;
            string user = loginRequest.NombreUsuario;
            string password = loginRequest.PassUsuario;

            try
            {
                DataTable dt = new DataTable();
                if (Constantes.tipoDB == 0)
                {
                    string oracle =
                        "select EMPR_NOMBRE, US_NOMBRE, US_ASESORSERV, AGEN_NOMAGENCIA,  US_TELEFONO from MS_VUSUARIOS " +
                        "WHERE EMPR_EMPRESAID = " + idEmpresa + " and USA_AGEN_IDAGENCIA = " + idAgencia +
                        " and US_IDUSUARIO = '" +
                        user + "' and US_PASSWORD = '" + password + "'";
                    dt = new ConectaDBOracle().seleciona(oracle, Constantes.ind, ref er);
                }

                if (Constantes.tipoDB == 1)
                {
                    string sql =
                        "select EMPR_NOMBRE, US_NOMBRE, US_ASESORSERV, AGEN_NOMAGENCIA, US_TELEFONO from GDMS_MS_VUSUARIOS " +
                        "WHERE EMPR_EMPRESAID = " + idEmpresa + " and AGEN_IDAGENCIA = " + idAgencia +
                        " AND US_IDUSUARIO = '" +
                        user + "' and US_PASSWORD = '" + password + "'";
                    dt = new ConectaDBSql().seleciona(sql, Constantes.ind, ref er);
                }

                if (!string.IsNullOrEmpty(er))
                {
                    res.response = "ERROR";
                    res.message = "Error en la consulta de Agencias: " + er;
                    return res;
                }

                if (dt == null || dt.Rows.Count == 0)
                {
                    res.response = "NoData";
                    res.message = "No se encontraron datos en la consulta realizada";
                    return res;
                }

                res.datos = new List<object>(); // Ahora la lista contiene objetos con propiedades
                foreach (DataRow row in dt.Rows)
                {
                    var loginResponse = new LoginResponse()
                    {
                        EmpresaNombre = row["EMPR_NOMBRE"].ToString(),
                        UsuarioNombre = row["US_NOMBRE"].ToString(),
                        AsesorServicio = Convert.ToInt32(row["US_ASESORSERV"]),
                        AgenciaNombre = row["AGEN_NOMAGENCIA"].ToString(),
                        Telefono = row["US_TELEFONO"].ToString()
                    };
                    res.datos.Add(loginResponse);
                }

                res.response = "OK";
                res.message = "Login exitoso";
            }
            catch (Exception err)
            {
                res.response = "ERROR";
                res.message = err.Message;
            }

            return res;
        }
        
        //------------------------JWT---------------------------------------------
        [HttpPost("obtenerToken")]
        public IActionResult IniciaSesion([FromBody] Usuario user)
        {
            Respuesta res = new Respuesta();

            if (user == null || !user.login())
            {
                res.response = "ERROR";
                res.message = "Usuario o contraseña errónea";
                string strJson = JsonConvert.SerializeObject(res);
                return Content(strJson, "application/json");
            }

            var jwtSettings = _configuration.GetSection("Jwt");
            var key = jwtSettings.GetSection("Key").Value;
            var bytekey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(bytekey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, jwtSettings["Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString()),
                new Claim("USUARIO", user.usuario),
                new Claim("EMPRESAID", user.empresaId),
                new Claim("AGENCIAID", user.agenciaId)
            };

            var token = new JwtSecurityToken(
                jwtSettings["Issuer"],
                jwtSettings["Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            res.response = "OK";
            res.message = "Sesión iniciada correctamente";
            res.datos.Add(new List<object> { tokenString });
            return Ok(res);
        }
        
    }

    public class LoginRequest
    {
        public string EmpresaId { get; set; }
        public string AgenciaId { get; set; }
        public string NombreUsuario { get; set; }
        public string PassUsuario { get; set; }
    }
    
}

