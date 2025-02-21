using System;
using System.Data;
using MySqlConnector;
using Oracle.ManagedDataAccess.Client;

namespace RFIDCoreGit.BD
{
    public class Parametros
    {
        public String variable;
        public Object valor;
        public bool esSalida = false;
        public OracleDbType tipo;
        public SqlDbType tip;
        public MySqlDbType tipoM;
        public Parametros()
        {
        }

        public Parametros(String var, Object val, OracleDbType tip)
        {
            variable = var;
            valor = val;
            tipo = tip;
        }
        public Parametros(String var, Object val, SqlDbType tipo)
        {
            variable = var;
            valor = val;
            tip = tipo;
        }

        public Parametros(String var, Object val, MySqlDbType tipo)
        {
            variable = var;
            valor = val;
            tipoM = tipo;
        }


        public Parametros(String var, Object val, OracleDbType tip, bool salida)
        {
            variable = var;
            valor = val;
            tipo = tip;
            esSalida = salida;
        }
        public Parametros(String var, Object val, SqlDbType tipo, bool salida)
        {
            variable = var;
            valor = val;
            tip = tipo;
            esSalida = salida;
        }

        public Parametros(String var, Object val, MySqlDbType tipo, bool salida)
        {
            variable = var;
            valor = val;
            tipoM = tipo;
            esSalida = salida;
        }
    }
}
