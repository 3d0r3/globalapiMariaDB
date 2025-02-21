using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace RFIDCoreGit.BD
{
    public class ConectaDBSql
    {
        public ConectaDBSql()
        {
        }
        SqlConnection con;
        
        // Funcion encargada de hacer la conexion a la base de Datos
        void Connect(int id)
        {
            con = new SqlConnection();
            // Justo en esta linea es donde se hace la conexion a la base de datos mediante la cadena de conexion que se obtiene en "Constantes"
            con.ConnectionString = Constantes.conStringSQL[id];
            con.Open();
            Console.WriteLine("Connected to Oracle" + con.ServerVersion);
        }

        // Funcion encargada de cerrar la conexion a la base de Datos
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
                SqlCommand sqlCmd = new SqlCommand(sql, con);
                sqlCmd.CommandType = System.Data.CommandType.Text;
                SqlDataAdapter sqlDa = new SqlDataAdapter();
                sqlCmd.CommandTimeout = 300;
                sqlDa.SelectCommand = sqlCmd;
                DataSet DsRetorno = new DataSet();
                sqlDa.Fill(DsRetorno);
                dt = DsRetorno.Tables[0];
                Close();

            }
            catch (Exception err)
            {
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
                SqlCommand cmd = new SqlCommand(procedimiento, con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                if (parametros != null)
                {
                    foreach (Parametros parametro in parametros)
                    {
                        if (!parametro.esSalida)
                        {
                            cmd.Parameters.Add(parametro.variable, parametro.tip).Value = parametro.valor;
                        }
                        else
                        {
                            SqlParameter op1 = new SqlParameter();
                            op1.SqlDbType = parametro.tip;
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
