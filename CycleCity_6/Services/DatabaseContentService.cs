using System;
using System.CodeDom;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Text;
using CycleCity_6.Materials;
using Newtonsoft.Json.Linq;


namespace CycleCity_6.Services
{
    class DatabaseContentService
    {
        private readonly string _token;

        /// <summary>
        /// Der DatabaseContentService erzeugt eine Verbindung zu unserem Server und holt Daten von diesem.
        /// 
        /// Wenn während des Erzeugens keine Verbindung zum Server besteht, wird hier eine Exceptions geworfen.
        /// </summary>
        public DatabaseContentService()
        {
            try
            {
                _token = getToken();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// @return: string aus angefragter Json Datei
        /// </summary>
        public string GetNewData()
        {
            var request = (HttpWebRequest)WebRequest.Create("https://api.cyc.jmakro.de:4040/get_latest_coordinates.php");

            var data = Encoding.ASCII.GetBytes("auth_token=" + _token);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            try
            {

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                return new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception)
            {
                throw new WebException("Keine Verbindung zum CycleYourCity-Server");
            }
        }


        /// <summary>
        /// Holt Daten aus dem angebeben Zeitraum vom Server.
        /// Wenn 'to' den 'max'-Value von DateTime hat, werden alle Daten vom Startzeitpunkt bis zum aktuellen Datum geholt.
        /// </summary>
        /// <param name="from">Startzeit des Intervalls</param>
        /// <param name="to">Endzeit des Intervalls</param>
        /// <returns>Ein Json String zum parsen</returns>
        public string GetDataFromTo(DateTime from, DateTime to)
        {
            Contract.Assert(from != null);
            Contract.Assert(to != null);
            Contract.Assert(from < to);

            string start = from.ToString("yyyy-MM-dd hh:mm:ss");
            string ende;
            if (to.Equals(DateTime.MaxValue))
            {
                ende = "";
            }
            else
            {
                ende = to.ToString("yyyy-MM-dd hh:mm:ss");
            }

            var request = (HttpWebRequest)WebRequest.Create("https://api.cyc.jmakro.de:4040/get_coordinates.php");

            var data = Encoding.ASCII.GetBytes("auth_token=" + _token + "&" + "from=" + start + "&" + "to=" + ende);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            try
            {

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                return new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception)
            {
                throw new WebException("Keine Verbindung zum CycleYourCity-Server");
            }

        }

        private string getToken()
        {
            HttpWebRequest request =
                (HttpWebRequest)WebRequest.Create("https://api.cyc.jmakro.de:4040/get_auth_token.php");
            var data = Encoding.ASCII.GetBytes("username=" + User.Name + "&password=" + User.Passwort);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            try
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                JObject jObject = JObject.Parse(responseString);
                return jObject["auth_token"].ToString();
            }
            catch (Exception)
            {
                throw new WebException("Token kann nicht angefordert werden.");
            }
        }
    }

}
