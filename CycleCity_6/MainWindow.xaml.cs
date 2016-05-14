using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CycleCity_6.Services;

namespace CycleCity_6
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent ();
            var test = new GpsToEsriParser();
            test.ParseJsonToEsriPolyline(@"[{'tourid':'1','WayPoints':[{'cmt':'0','time':'2011-12-31
23:59:56','lat':'52.518611','lon':'33.376111','ele':'0.0'},{'cmt':'1','time':'2011-12-31
23:59:59','lat':'62.518611','lon':'43.376111','ele':'0.0'}]},
{'tourid':'1','WayPoints':[{'cmt':'0','time':'2011-12-31
23:59:56','lat':'52.518611','lon':'33.376111','ele':'0.0'},{'cmt':'1','time':'2011-12-31
23:59:59','lat':'62.518611','lon':'43.376111','ele':'0.0'}]},
{'tourid':'1','WayPoints':[{'cmt':'0','time':'2011-12-31
23:59:56','lat':'52.518611','lon':'33.376111','ele':'0.0'},{'cmt':'1','time':'2011-12-31
23:59:59','lat':'62.518611','lon':'43.376111','ele':'0.0'}]},
{'tourid':'1','WayPoints':[{'cmt':'0','time':'2011-12-31
23:59:56','lat':'52.518611','lon':'33.376111','ele':'0.0'},{'cmt':'1','time':'2011-12-31
23:59:59','lat':'62.518611','lon':'43.376111','ele':'0.0'}]}]

");
        }
    }
}
