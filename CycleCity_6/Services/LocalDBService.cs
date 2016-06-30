using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;

using CycleCity_6.TrackDBDataSetTableAdapters;

namespace CycleCity_6.Services
{
    public class LocalDBService
    {
        private SqlCommand command;

        private TableTableAdapter adapter = new TableTableAdapter ();


        public LocalDBService()
        {
            //SqlConnection con = new SqlConnection (CycleCity_6.Properties.Settings.Default.TrackDBConnectionString);
            //adapter.Connection = new SqlConnection (CycleCity_6.Properties.Settings.Default.TrackDBConnectionString);

            //adapter.Insert ("id", "test");
            //adapter.Update ("id","test", "id");


            var a = adapter.GetData ();
            Console.WriteLine (a.Count + "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");


            //try
            //{
            //    con.Open ();
            //    Console.WriteLine ("Connection open");
            //    command = new SqlCommand ("", con);
            //    con.Close ();
            //}
            //catch
            //{
            //    Console.WriteLine ("Connection could not be open");
            //}
        }

        public void AddJson(string json)
        {
            Console.WriteLine ("updating table");
            if(json != "[]" && json != "auth_token invalid")
            {
                JObject jObject = JObject.Parse (json);

                var tracks = jObject.Values ();

                foreach(var track in tracks)
                {
                    var id = track.Path;
                    var str = track.ToString ();

                    adapter.Update (str, id);
                }
            }
        }

        public List<Tuple<String, String>> LoadTrackFromDB(string ID)
        {
            var data = adapter.GetData ();

            var toprint = data.Where (x => x.Id == ID).First ();

            return new List<Tuple<String, String>> { Tuple.Create (toprint.Id, toprint.Json) };
        }

        public List<Tuple<String, String>> LoadAllTracksFromDB()
        {
            var tracks = new List<Tuple<String, String>> ();

            var data = adapter.GetData ();

            foreach(var a in data)
            {
                tracks.Add(Tuple.Create (a.Id, a.Json));
            }

            return tracks;
        }
    }
}
