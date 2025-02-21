using System;
using System.Collections.Generic;

namespace RFIDCoreGit.Entidades
{
    public class Respuesta
    {
        public string response;
        
        public string message;
        public List<object> datos { get; set; } = new List<object>();

        public Respuesta()
        {
        }
    }
}
