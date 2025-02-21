using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace RFIDCoreGit.BD
{
    public class Constantes
    {
        public Constantes()
        {
        }

        public static int tipoDB = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()
            .GetValue<int>("Constantes:tipoDB"); //Configura el tipo de Basededatos a la que se va a conectar

        public static int ind = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build()
            .GetValue<int>("Constantes:ind"); //Configura la cadena de conecccion que se va a hacer 

        public static List<string> DBType = new List<string>()
        {
            "ORACLE", //0
            "SQL", //1
            "MARIADB" //2
        };

        public static List<string> conStringOracle = new List<string>()
        {
            "User Id=autos;Password=AUTOS9405CPS;Data Source=localhost/SISTEMAS",                //0 - LocalHost
            "User Id=autos;Password=AUTOS9405CPS;Data Source=187.210.227.98/SISTEMAS",           //1 - Guadalajara
            "User Id=autos;Password=AUTOS9405CPS;Data Source=201.120.5.15/SISTEMAS",             //2 - MOCHIS
            "User Id=autos;Password=AUTOS9405CPS;Data Source=192.168.2.3/SISTEMAS",              //3 - vegusa
            "User Id=autos;Password=AUTOS9405CPS;Data Source=192.168.1.10/SISTEMAS",             //4 - Global Lcoal
            "User Id=autos;Password=AUTOS9405CPS;Data Source=globaldmsdemo.dyndns.org/SISTEMAS", //5 - Global Remoto
            "User Id=autos;Password=V1904CPSAUTOS; Data Source=10.5.1.11/SISTEMAS",              //6 - vegusa
            "User Id=autos;Password=AUTOS9405CPS; Data Source=192.168.11.88/SISTEMAS",           //7 - gonzalitos
            "User Id=autos;Password=AUTOS9405CPS; Data Source=187.210.227.98/SISTEMAS",          //8 - Jalisco
            "User Id=autos;Password=AUTOS9405CPS; Data Source=20.169.82.24/SISTEMAS"             //9 - Demo Jetour
        };

        public static List<string> conStringSQL = new List<string>()
        {
            "Data Source=localhost; Initial Catalog=sistemas; User id=sa; Password=polanco" //LocalHost
        };

        public static List<string> conStringMDB = new List<string>()
        {
            "Server=127.0.0.1; Port=3306; Database=servidoresinfo; Uid=root; Pwd=AUTOS9405CPS;",                               //0 LocalHost
            "Server=127.0.0.1; Port=3306; Database=servidorgdms; Uid=root; Pwd=AUTOS9405CPS;",                     //1.-GlobalDMSq
            "Server=187.210.227.98; Port=3306; Database=servidorgdms; Uid=root; Pwd=AUTOS9405CPS;",                //2.-Jalisco
            "User Id=autos;Password=AUTOS9405CPS; DataSource=20.169.82.24/SISTEMAS",                               //3 - Demo Jetour
            "Server=127.0.0.1; Port=3306; Database=globaldms; Uid=root; Pwd=Autos9405CPS;AllowZeroDateTime=True;", //4.-GlobalDMS SoporteGlobalDMS
            "Server=localhost; Port=3306; Database=globaldms; Uid=Admin; Pwd=Global9405;AllowZeroDateTime=True;"   //5.-LocalHostGlobalDMS
        };
    }
}