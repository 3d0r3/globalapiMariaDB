using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace RFIDCoreGit.BD
{
    public class ConectaDBOracle
    {
        OracleConnection con;
        public ConectaDBOracle()
        {
        }
        // Funcion encargada de realizar la conexion a la base de datos
        void Connect(int id)
        {
            con = new OracleConnection();
            con.ConnectionString = Constantes.conStringOracle[id];
            con.Open();
            Console.WriteLine("Connected to Oracle" + con.ServerVersion);
        }
        // Funcion encargada de cerrar la conexion a la base de datos
        void Close()
        {
            con.Close();
            con.Dispose();
        }
        
        // Funcion encargada de hacer una consulta a la base de datos
        public DataTable seleciona(string consulta, int id, ref string e)
        {
            DataTable dt = new DataTable();
            try
            {
                Connect(id);
                string sql = consulta;
                OracleCommand sqlCmd = new OracleCommand(sql, con);
                sqlCmd.CommandType = System.Data.CommandType.Text;
                OracleDataAdapter sqlDa = new OracleDataAdapter();
                sqlDa.SelectCommand = sqlCmd;
                DataSet DsRetorno = new DataSet();
                sqlDa.Fill(DsRetorno);
                dt = DsRetorno.Tables[0];
                Close();

            }
            catch (Exception err)
            {
                Close();
                e = err.Message;
                e.Trim('"');
            }
            return dt;
        }
        // Funcion encargada de ejecutar un procedimiento almacenado en la base de datos
        public bool ejecutaProcedimiento(int id, string procedimiento, Parametros[] parametros, ref string e, ref List<object> val)
        {
            try
            {
                bool res = true;
                Connect(id);
                OracleCommand cmd = new OracleCommand(procedimiento, con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                if (parametros != null)
                {
                    foreach (Parametros parametro in parametros)
                    {
                        if (!parametro.esSalida)
                        {
                            cmd.Parameters.Add(parametro.variable, parametro.tipo).Value = parametro.valor;
                        }
                        else
                        {
                            OracleParameter op1 = new OracleParameter();
                            op1.OracleDbType = parametro.tipo;
                            op1.Size = 2000;
                            op1.Direction = ParameterDirection.InputOutput;
                            op1.ParameterName = parametro.variable;
                            op1.Value = parametro.valor;
                            cmd.Parameters.Add(op1);
                        }

                    }
                }

                cmd.ExecuteNonQuery();

                foreach (Parametros parametro in parametros)
                {
                    if (parametro.esSalida)
                    {
                        val.Add(cmd.Parameters[parametro.variable].Value.ToString());
                    }
                }
                Close();
                return res;
            }
            catch (Exception err)
            {
                e = err.Message;
                e.Trim('"');
                return false;
            }
        }
        
    }
}
