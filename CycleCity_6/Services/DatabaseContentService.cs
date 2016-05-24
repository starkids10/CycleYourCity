using System;
using System.CodeDom;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;


namespace CycleCity_6.Services
{
    class DatabaseContentService
    {
        private readonly string _token;

        public DatabaseContentService()
        {
            _token = getToken();
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
                return new StreamReader(stream: response.GetResponseStream()).ReadToEnd();
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
            var data = Encoding.ASCII.GetBytes("username=table&password=ftzfD3pz");

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
                throw new WebException("Keine Verbindung zum CycleYourCity-Server");
            }
        }
    }

}
