using System;
using System.IO;
using System.Net;
using System.Text;

namespace CycleCity_6.Services
{
    class DatabaseContentService
    {
        /// <summary>
        /// @return: string aus angefragter Json Datei
        /// </summary>
        public static string GetNewData()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create ("https://preview.api.cycleyourcity.jmakro.de:4040/log_coordinates.php"); //TODO URL
            try
            {
                WebResponse response = request.GetResponse ();
                using(Stream responseStream = response.GetResponseStream ())
                {
                    StreamReader reader = new StreamReader (responseStream, Encoding.UTF8);
                    return reader.ReadToEnd (); //TODO liste von strings aus mehreren Json files ???
                }
            }
            catch(WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using(Stream responseStream = errorResponse.GetResponseStream ())
                {
                    StreamReader reader = new StreamReader (responseStream, Encoding.GetEncoding ("utf-8"));
                    String errorText = reader.ReadToEnd ();
                    // log errorText
                }
                throw;
            }
        }
    }

}
