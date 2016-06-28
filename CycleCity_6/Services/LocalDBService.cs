using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using CycleCity_6.Materials;
using CycleCity_6.Properties;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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

            var time = DateTime.Now.ToString();

            adapter.Insert ("id", time, time, "a", "b", "c");
            //adapter.Update ("id", time, time, "a", "b", "c", "id", time, time);


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
    }
}
