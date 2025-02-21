using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;

namespace RFIDCoreGit.BD
{
    public class ConectaDBMaria
    {
        MySqlConnection con;
        public ConectaDBMaria()
        {
        }
        
        // Funcion encargada de hacer la conexion a la base de datos
        void Connect(int id)
        {
            con = new MySqlConnection();
            con.ConnectionString = Constantes.conStringMDB[id];
            con.Open();
            Console.WriteLine("Connected to Oracle" + con.ServerVersion);
        }
        //Funcion encargada de cerrar la conexion a la base de datos
        void Close()
        {
            con.Dispose();
        }
        //Funcion encargada de realizar una consulta a la base de datos
        public DataTable seleciona(string consulta, int id, ref string e)
        {
            DataTable dt = new DataTable();
            try
            {
                Connect(id);
                string sql = consulta;
                MySqlCommand sqlCmd = new MySqlCommand(sql, con);
                sqlCmd.CommandType = System.Data.CommandType.Text;
                MySqlDataAdapter sqlDa = new MySqlDataAdapter();
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
        //Funcion encargada de ejecutar un procedimiento almacenado en la base de datos
        public bool ejecutaProcedimiento(int id, string procedimiento, Parametros[] parametros, ref string e, ref List<object> val)
        {
            try
            {
                bool res = true;
                Connect(id);
                MySqlCommand cmd = new MySqlCommand(procedimiento, con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                foreach (Parametros parametro in parametros)
                {
                    if (parametro.esSalida)
                    {
                        MySqlParameter op1 = new MySqlParameter();
                        op1.MySqlDbType = parametro.tipoM;
                        op1.Size = 4000;
                        op1.Direction = ParameterDirection.InputOutput;
                        op1.ParameterName = parametro.variable;
                        op1.Value = parametro.valor;
                        cmd.Parameters.Add(op1);
                    }
                    else
                    {
                        cmd.Parameters.Add(parametro.variable, parametro.tipoM).Value = parametro.valor;
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
