using System;
namespace RFIDCoreGit.Entidades
{
    public class Solicitud
    {
        public List<List<object>> requestData { get; set; } = new List<List<object>>();

        public List<string> Content 
        { 
            get 
            { 
                return requestData.Count > 0 ? requestData[0].ConvertAll(o => o.ToString()) : new List<string>(); 
            } 
        }

        public Solicitud()
        {
        }
    }
}
