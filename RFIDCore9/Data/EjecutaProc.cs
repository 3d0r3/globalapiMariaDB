using System;
using RFIDCoreGit.Entidades;

namespace RFIDCoreGit.BD
{
    public interface EjecutaProc
    {
        //public Parametros[] GetParametrosSQL(ref string procedimiento);
        //public Parametros[] GetParametrosOracle(ref string procedimiento);
        //public Parametros[] GetParametrosMDB(ref string procedimiento);

        public Parametros[] GetParametros(ref string procedimiento);

        public Respuesta ejecutaProcedimiento();

    }
}
