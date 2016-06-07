using Microsoft.VisualStudio.TestTools.UnitTesting;
using CycleCity_6.Materials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esri.ArcGISRuntime;
using Esri.ArcGISRuntime.Geometry;

namespace CycleCity_6.Materials.Tests
{


    [TestClass()]
    public class PointTest
    {
        private Point _point1;
        private Point _point2;
        private Point _point3;
        private Point _point4;


        [TestInitialize]
        public void TestInitialize()
        {
            if (string.IsNullOrEmpty(ArcGISRuntimeEnvironment.InstallPath))
            {
                ArcGISRuntimeEnvironment.InstallPath = Directory.GetCurrentDirectory();
            }
            _point1 = new Point(new MapPoint(10.0, 10.0), new DateTime(2016, 05, 27,12,12,12));
            _point2 = new Point(new MapPoint(10.0, 10.0), new DateTime(2016, 05, 27,12 ,12 ,12));
            _point3 = new Point(new MapPoint(11.0, 11.0), new DateTime(2016, 05, 27, 11, 11, 11));
            _point4 = new Point(new MapPoint(10.0, 10.0), new DateTime(1999, 03, 22));
        }

        [TestMethod]
        public void EqualTest()
        {
            Assert.AreEqual(_point1, _point2);
            Assert.AreNotEqual(_point1,_point3);
        }
    }
}