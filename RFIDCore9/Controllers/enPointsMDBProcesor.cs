using System.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RFIDCoreGit.BD;
using RFIDCoreGit.Entidades;

namespace RFIDCore9.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnPointsMdbProcesor : ControllerBase
    {
        public static IWebHostEnvironment Enviroment;

        public EnPointsMdbProcesor(IWebHostEnvironment env)
        {
            Enviroment = env;
        }

//--------------------------GETEMPRESASAGENCIAS-------------------------------------------
        
        [HttpGet]
        [Authorize]
        [Route("getEmpresasAgencias")]
        public IActionResult GetEmpresasAgencias()
        {
            
            Respuesta res = ObtenerEmpresasAgencias(); // Llama a tu función
            string strJson = JsonConvert.SerializeObject(res);
            return Content(strJson, "application/json");
        }

        public static Respuesta ObtenerEmpresasAgencias()
        {
            Respuesta res = new Respuesta();
            string er = "";
            try
            {
                DataTable dt = new DataTable();
                string sql = "SELECT MVEM_EMPRESAIDSERVER, MVEM_ALIAS, MVSU_IDSUCURSAL, MVSU_SUCURSAL_ALIAS, " +
                             "MVSU_EMPRESAID, MVSU_IDAGENCIA, MVSU_URL_RFIDCORE, MVEM_LOGO " +
                             "FROM MV_EMPRESAS JOIN MV_SUCURSALES ON MVEM_EMPRESAIDSERVER = MVSU_EMPRESAIDSERVER";

                dt = new ConectaDBMaria().seleciona(sql, Constantes.ind, ref er);

                res.datos = new List<object>(); // Ahora la lista contiene objetos con propiedades

                if (string.IsNullOrEmpty(er))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var empresaAgencia = new EmpresaAgenciaResponse()
                        {
                            EmpresaIdServer = Convert.ToInt32(row["MVEM_EMPRESAIDSERVER"]),
                            EmpresaAlias = row["MVEM_ALIAS"].ToString(),
                            SucursalId = Convert.ToInt32(row["MVSU_IDSUCURSAL"]),
                            SucursalAlias = row["MVSU_SUCURSAL_ALIAS"].ToString(),
                            EmpresaId = Convert.ToInt32(row["MVSU_EMPRESAID"]),
                            AgenciaId = Convert.ToInt32(row["MVSU_IDAGENCIA"]),
                            UrlRFIDCore = row["MVSU_URL_RFIDCORE"].ToString(),
                            Logo = row["MVEM_LOGO"].ToString()
                        };

                        res.datos.Add(new List<object> { empresaAgencia });
                    }

                    res.response = "OK";
                    res.message = "Se obtuvieron los datos correctamente";
                }
                else
                {
                    res.response = "ERROR";
                    res.message = er;
                }
            }
            catch (Exception err)
            {
                res.response = "ERROR";
                res.message = "Sigue llegando aquí: " + err.Message;
            }

            return res;
        }
        


//--------------------------GETCOLORES-----------------------------------------------------
        [HttpPost]
        [Route("getColores")]
        public IActionResult getColores([FromBody] Solicitud request)
        {
            if (request == null || request.requestData == null || request.requestData.Count == 0 ||
                request.requestData[0].Count < 4)
            {
                return BadRequest(
                    "El campo 'requestData' es requerido y debe contener al menos una lista con 4 elementos.");
            }

            string contentJson = JsonConvert.SerializeObject(request.Content);
            Respuesta res = getColores(contentJson);
            string strJson = JsonConvert.SerializeObject(res);
            return Content(strJson, "application/json");
        }

        public static Respuesta getColores(string json)
        {
            Respuesta resp = new Respuesta();
            try
            {
                List<string> datos = JsonConvert.DeserializeObject<List<string>>(json);
                String idSucursal = datos[0];
                String portal = datos[1];

                string er = "";
                DataTable dt = new DataTable();
                if (Constantes.tipoDB == 0)
                {
                    string oracle = "";
                    dt = new ConectaDBOracle().seleciona(oracle, Constantes.ind, ref er);
                }

                if (Constantes.tipoDB == 1)
                {
                    string sql = "";
                    dt = new ConectaDBSql().seleciona(sql, Constantes.ind, ref er);
                }

                if (Constantes.tipoDB == 2)
                {
                    string mariadb = "SELECT MVCO_COLORP, MVCO_COLORS, MVCO_COLOR1, MVCO_COLOR2, " +
                                     "MVCO_COLOR3, MVCO_COLOR4, MVCO_COLORV, MVCO_COLORA, MVCO_COLORR  " +
                                     "FROM mv_colores_sucursales WHERE MVCO_IDSUCURSAL = " + idSucursal +
                                     " AND MVCO_PORTAL = '" + portal + "'";
                    dt = new ConectaDBMaria().seleciona(mariadb, Constantes.ind, ref er);
                }

                List<object> miLista = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    miLista.Add(new List<object>
                    {
                        row["MVCO_COLORP"].ToString(),
                        row["MVCO_COLORS"].ToString(),
                        row["MVCO_COLOR1"].ToString(),
                        row["MVCO_COLOR2"].ToString(),
                        row["MVCO_COLOR3"].ToString(),
                        row["MVCO_COLOR4"].ToString(),
                        row["MVCO_COLORV"].ToString(),
                        row["MVCO_COLORA"].ToString(),
                        row["MVCO_COLORR"].ToString()
                    });
                }

                resp.response = "OK";
                resp.datos = miLista;
            }
            catch (Exception err)
            {
                String ee = err.Message;
                ee = ee.Trim('"');
                resp.response = "ERROR";
                resp.message = ee;
            }

            return resp;
        }
    }
    
    
}

public class enPointsMDBProcesor
{
}