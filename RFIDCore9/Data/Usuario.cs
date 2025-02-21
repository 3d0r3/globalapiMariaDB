using System.Data;
using System.Security.Claims;
using RFIDCoreGit.Entidades;

namespace RFIDCoreGit.BD;

public class Usuario
	{
        public string usuario { get; set; }
        public string password { get; set; }
        public string empresaId { get; set; }
        public string agenciaId { get; set; }


        public bool login()
        {
            string er = "";
            if (er == "")
            {
             string oracle = "SELECT US_IDUSUARIO, EMPR_EMPRESAID, USA_AGEN_IDAGENCIA FROM MS_VUSUARIOS WHERE US_IDUSUARIO = '" +
                    usuario + "' AND US_PASSWORD  =  '" + password + "'";
                DataTable dt = new ConectaDBOracle().seleciona(oracle, Constantes.ind, ref er);
                
                if (!string.IsNullOrEmpty(er))
                {
                    return false;
                }

                if (dt == null || dt.Rows.Count == 0)
                {
                    return false;
                }
                    foreach (DataRow row in dt.Rows)
                    {
                        usuario = row["US_IDUSUARIO"].ToString();
                        empresaId = row["EMPR_EMPRESAID"].ToString();
                        agenciaId = row["USA_AGEN_IDAGENCIA"].ToString();
                        return true;
                    }
                
            }
            return false;
        }

        public Respuesta validaToken(ClaimsIdentity claims)
        {
            Respuesta res = new Respuesta();
            try
            {
                var user = claims.Claims.FirstOrDefault(x => x.Type == "USUARIO").Value;
                usuario = user;
                var emp = claims.Claims.FirstOrDefault(x => x.Type == "EMPRESAID").Value;
                empresaId = emp;
                var agen = claims.Claims.FirstOrDefault(x => x.Type == "AGENCIAID").Value;
                agenciaId = agen;
                res.response = "OK";
                return res;
            }
            catch
            {
                res.response = "Error";
                res.message = "Error al momento de validar el Bearer Token";
                return res;
            }

        }


    }