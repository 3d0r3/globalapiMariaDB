using System.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RFIDCoreGit.BD;
using RFIDCoreGit.Entidades;

namespace RFIDCore9.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EndPointsOracle : ControllerBase
    {
        public static IWebHostEnvironment Enviroment;

        public EndPointsOracle(IWebHostEnvironment env)
        {
            Enviroment = env;
        }
        
        
//--------------------------OBTENERMENUPERMISOS---------------------------------------------
        [HttpPost]
        [Route("obtenerMenuPermisos")]
        public IActionResult ObtenerMenuPermisos([FromBody] ObtenerMenuPermisosRequest inforequest)
        {
            if (inforequest == null)
            {
                return BadRequest(
                    "El campo 'requestData' es requerido por lo cual no puede estar vacio");
            }
            
            Respuesta res = GetMenuPermisos(inforequest);
            string strJson = JsonConvert.SerializeObject(res);
            return Content(strJson, "application/json");
        }

        public static Respuesta GetMenuPermisos(ObtenerMenuPermisosRequest infoRequest)
        {
            Respuesta res = new Respuesta();
            string er = "";
            try
            {
                String idEmpresa = infoRequest.EmpresaId;
                String idAgencia = infoRequest.AgenciaId;
                String user = infoRequest.UsuarioId;

                DataTable dt = new DataTable();
                if (Constantes.tipoDB == 0)
                {
                    string oracle = "SELECT FORM_DESCRIP, FORM_NOMFORMA, PERM_FORM_NOMFORMA from SG_VPERMISO " +
                                    "where FORM_WEB = 1 and FORM_NOMFORMA != 'SINGRUPO' and PERM_US_IDUSUARIO = '" +
                                    user +
                                    "' and EMPR_EMPRESAID = " + idEmpresa + " and PERM_AGEN_IDAGENCIA = " + idAgencia +
                                    " order by PERM_FORM_NOMFORMA asc";
                    dt = new ConectaDBOracle().seleciona(oracle, Constantes.ind, ref er);
                }

                if (Constantes.tipoDB == 1)
                {
                    string sql = "select FORM_DESCRIP, FORM_NOMFORMA, PERM_FORM_NOMFORMA from GDMS_SG_VPERMISO " +
                                 "where FORM_WEB = 1 and FORM_NOMFORMA != 'SINGRUPO' and PERM_US_IDUSUARIO = '" + user +
                                 "' and EMPR_EMPRESAID = " + idEmpresa + " and AGEN_IDAGENCIA = " + idAgencia +
                                 " order by PERM_FORM_NOMFORMA asc";
                    dt = new ConectaDBSql().seleciona(sql, Constantes.ind, ref er);
                }

                res.datos = new List<object>();

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

                foreach (DataRow row in dt.Rows)
                {
                    var menuPermisoResponse = new MenuPermisosResponse()
                    {
                        Descripcion = row["FORM_DESCRIP"].ToString(),
                        GrupoForma = row["FORM_NOMFORMA"].ToString(),
                        ClaveForma = row["PERM_FORM_NOMFORMA"].ToString()
                    };
                    
                    res.datos.Add(new List<object> { menuPermisoResponse });
                }

                res.response = "OK";
                res.message = "";
            }
            catch (Exception err)
            {
                res.response = "ERROR";
                res.message = err.Message;
            }

            return res;
        }
    }

    public class ObtenerMenuPermisosRequest
    {
        public string EmpresaId { get; set; }
        public string AgenciaId { get; set; }
        public string UsuarioId { get; set; }
    }
}

