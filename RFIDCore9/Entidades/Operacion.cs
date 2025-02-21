using System;
using System.Collections.Generic;
using RFIDCoreGit.BD;
using Oracle.ManagedDataAccess.Client;

namespace RFIDCoreGit.Entidades;

public class Operacion
    {

        public Int32 empresaID = 0;
        public Int32 agenciaID = 0;
        public Int32 folio = 0;
        public string prefijo = "";
        public string usuario = "";
        public string renglon = "";
        public string mecanico = "";
        public DateTime fechaInicio = DateTime.Now;
        public DateTime fechaFin = DateTime.Now;
        public Int32 cual;


        public Parametros[] getParametros(ref string procedimiento)
        {
            procedimiento = "Osp_Se_AsignaoperXoRDEN";
            Parametros[] parameters = new Parametros[] {
                new Parametros("pn_empresaid", empresaID.ToString() , OracleDbType.Int32),
                new Parametros("pn_agenciaid", agenciaID.ToString(), OracleDbType.Int32),
                new Parametros("pn_orden", folio.ToString(), OracleDbType.Int32),
                new Parametros("pv_PREFIJO", prefijo, OracleDbType.Varchar2),
                new Parametros("pn_renglon", renglon, OracleDbType.Int32),
                new Parametros("pn_mecanico", mecanico, OracleDbType.Int32),
                new Parametros("pv_codigodefecto", "", OracleDbType.Varchar2),
                new Parametros("pd_fechaINICIO", fechaInicio, OracleDbType.Date),
                new Parametros("pd_fechaFIN", fechaFin, OracleDbType.Date),
                new Parametros("pv_usuario", usuario, OracleDbType.Varchar2)
            };
            return parameters;
        }


        public Parametros[] getParametros3(ref string procedimiento)
        {
            procedimiento = "Osp_Se_Asignaoperacion";
            Parametros[] parameters = new Parametros[] {
                new Parametros("pn_empresaid", empresaID.ToString() , OracleDbType.Int32),
                new Parametros("pn_agenciaid", agenciaID.ToString(), OracleDbType.Int32),
                new Parametros("pn_orden", folio.ToString(), OracleDbType.Int32),
                new Parametros("pv_PREFIJO", prefijo, OracleDbType.Varchar2),
                new Parametros("pn_renglon", renglon, OracleDbType.Int32),
                new Parametros("pn_mecanico", mecanico, OracleDbType.Int32),
                new Parametros("pv_codigodefecto", "", OracleDbType.Varchar2),
                new Parametros("pv_usuario", usuario, OracleDbType.Varchar2)
            };
            return parameters;
        }



        public Parametros[] getParametrosFin(ref string procedimiento)
        {
            procedimiento = "Osp_Se_AsignaoperCFechafIN";
            Parametros[] parameters = new Parametros[] {
                new Parametros("pn_empresaid", empresaID.ToString() , OracleDbType.Int32),
                new Parametros("pn_agenciaid", agenciaID.ToString(), OracleDbType.Int32),
                new Parametros("pn_orden", folio.ToString(), OracleDbType.Int32),
                new Parametros("pv_PREFIJO", prefijo, OracleDbType.Varchar2),
                new Parametros("pn_renglon", renglon, OracleDbType.Int32),
                new Parametros("pn_mecanico", mecanico, OracleDbType.Int32),
                new Parametros("pv_codigodefecto", "", OracleDbType.Varchar2),
                new Parametros("pd_fechaINICIO", fechaInicio, OracleDbType.Date),
                new Parametros("pd_fechaFIN", fechaFin, OracleDbType.Date),
                new Parametros("pv_usuario", usuario, OracleDbType.Varchar2)
            };
            return parameters;
        }
        public string ejecutaProcedimiento()
        {
            string e = "";
            String resultados = "";
            string procedimiento = "";
            Parametros[] param = getParametrosFin(ref procedimiento);
            List<object> val = new List<object>();
            if (cual == 3)
            {
                param = getParametros(ref procedimiento);
            }

            new ConectaDBOracle().ejecutaProcedimiento(Constantes.ind,procedimiento, param, ref e,ref val);
            if (e != "")
            {
                resultados = "[[\"" + e + "\"]]";
            }
            else
            {
                resultados = "[[\"correcto\"]]";
            }
            return resultados;
        }


    }