using System;
using System.Collections.Generic;
using RFIDCoreGit.Entidades;

namespace RFIDCoreGit.BD
{
    public class EjecutaInstruciones
    {
        public EjecutaInstruciones()
        {
        }
        
        // Aqui debo de ver como modificar el tipo de base de dato que recibe
        public Respuesta ejecutaProcedimiento(EjecutaProc obj)
        {
            Respuesta res = new Respuesta();
            List<object> val = new List<object>();
            string procedimiento = "";
            string e = "";
            Parametros[] param = null;
            if (Constantes.tipoDB == 0)
            {
                param = obj.GetParametros(ref procedimiento);
                if (!new ConectaDBOracle().ejecutaProcedimiento(Constantes.ind, procedimiento, param, ref e, ref val))
                {
                    res.response = "ERROR";
                    res.message = e;
                }
                else
                {
                    res.response = "OK";
                    res.datos = new List<object>();
                    res.datos.Add(val);
                }
            }
            if (Constantes.tipoDB == 1)
            {
                param = obj.GetParametros(ref procedimiento);
                if (!new ConectaDBSql().ejecutaProcedimiento(Constantes.ind, procedimiento, param, ref e, ref val))
                {
                    res.response = "ERROR";
                    res.message = e;
                }
                else
                {
                    res.response = "OK";
                    res.datos = new List<object>();
                    res.datos.Add(val);
                }
            }
            if (Constantes.tipoDB == 2)
            {
                param = obj.GetParametros(ref procedimiento);
                if (!new ConectaDBMaria().ejecutaProcedimiento(Constantes.ind, procedimiento, param, ref e, ref val))
                {
                    res.response = "ERROR";
                    res.message = e;
                }
                else
                {
                    res.response = "OK";
                    res.datos = new List<object>();
                    res.datos.Add(val);
                }

            }

            return res;
        }
    }
}
