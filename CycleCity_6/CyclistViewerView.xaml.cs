﻿using Esri.ArcGISRuntime.Layers;
using System.Diagnostics.Contracts;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using System.Windows.Media;

namespace CycleCity_6.Tools.CyclistViewer
{
    /// <summary>
    /// Interaktionslogik für CyclistViewerView.xaml
    /// </summary>
    public partial class CyclistViewerView : UserControl
    {

        private int monatselected = 0; //dies ist nur eine flag die werte 0-2 annimt
        private int startmonat = 0;
        private int endmonat = 0;

        public CyclistViewerView()
        {
            InitializeComponent();

            GetViewModel().mapView = CycleMapView;
        }

        private CyclistViewerViewModel GetViewModel()
        {
            Contract.Requires(DataContext is CyclistViewerViewModel);

            return (CyclistViewerViewModel)DataContext;
        }

        private void HeatMapOrTracksAnzeigen_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender.Equals(TrackAnzeigen))
            {
                GetViewModel().HeatmapAnzeigen(false);

                TrackAnzeigen.Visibility = Visibility.Collapsed;
                HeatMapAnzeigen.Visibility = Visibility.Visible;

            }
            if (sender.Equals(HeatMapAnzeigen))
            {
                GetViewModel().HeatmapAnzeigen(true);

                TrackAnzeigen.Visibility = Visibility.Visible;
                HeatMapAnzeigen.Visibility = Visibility.Collapsed;
            }

        }

        private void Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //TODO Daten nach der ausgewählten Zeit anzeigen lassen
            int stunde = (int)ZeitSlider.Value;
            int monat;
            if (startmonat == 0)
            {
                monat = 1;
            }
            else
            {
                monat = startmonat;
            }
            if (stunde == 24)
            {
                stunde = 0;
            }
            if (endmonat == 0)
            {
                GetViewModel().SetzeUhrzeit(new DateTime(2016, monat, 01, stunde, 00, 00), DateTime.MaxValue);
            }
            else
            {
                int letzterTag = DateTime.DaysInMonth(2016, endmonat);
                //TODO mehr als nur eine Stunde anzeigen
                GetViewModel().SetzeUhrzeit(new DateTime(2016, startmonat, 01, stunde, 00, 00), new DateTime(2016, endmonat, letzterTag, stunde, 59, 59));
            }

        }

        // TODO wieder einbauen wenn auf touch verwendet werden soll "System.Windows.Input.TouchEventArgs"

        private void Monatsauswahl_OnTouch(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string monat = button.Name;

            if (monatselected == 0 || monatselected == 2)
            {
                //Click Flag setzten 
                monatselected = 1;
                // von allen Buttons Hintergrundfarbe zurücksetzten
                resetBackgrounds();

                //Hintergrundfarbe setzten
                startmonat = setBackground(monat);

                //Server Zeitraum mitteilen ("SetzeUhrzeit();")
                GetViewModel().SetzeUhrzeit(new DateTime(2016, startmonat, 01, 00, 00, 00), DateTime.MaxValue);

            }
            else if (monatselected == 1)
            {
                monatselected = 2;

                //diesen und ersten monat auswählen;
                endmonat = setBackground(monat);


                //buttons dazwischen mit neuer hintergrundfarbe anpassen;
                if (startmonat > endmonat)
                {
                    int y = startmonat;
                    startmonat = endmonat;
                    endmonat = y;
                }
                for (int x = startmonat; x < endmonat; x++)
                {
                    setBackground("M" + x);
                }

                //Server Zeitraum mitteilen ("SetzeUhrzeit();")
                int letzterTag = DateTime.DaysInMonth(2016, endmonat);
                GetViewModel().SetzeUhrzeit(new DateTime(2016, startmonat, 01, 00, 00, 00), new DateTime(2016, endmonat, letzterTag, 23, 59, 59));


            }
        }


        /// <summary>
        /// Setzt den Hintergrund eines übergebenen Buttons auf "Aqua" und gibt dessen Monat als Int zurück (1-12)
        /// </summary>
        /// <param name="monat"></param>
        /// <returns></returns>
        private int setBackground(string monat)
        {
            switch (monat)
            {
                case "M1":
                    M1.Background = Brushes.Aqua;
                    return 1;
                case "M2":
                    M2.Background = Brushes.Aqua;
                    return 2;
                case "M3":
                    M3.Background = Brushes.Aqua;
                    return 3;
                case "M4":
                    M4.Background = Brushes.Aqua;
                    return 4;
                case "M5":
                    M5.Background = Brushes.Aqua;
                    return 5;
                case "M6":
                    M6.Background = Brushes.Aqua;
                    return 6;
                case "M7":
                    M7.Background = Brushes.Aqua;
                    return 7;
                case "M8":
                    M8.Background = Brushes.Aqua;
                    return 8;
                case "M9":
                    M9.Background = Brushes.Aqua;
                    return 9;
                case "M10":
                    M10.Background = Brushes.Aqua;
                    return 10;
                case "M11":
                    M11.Background = Brushes.Aqua;
                    return 11;
                case "M12":
                    M12.Background = Brushes.Aqua;
                    return 12;
                default:
                    return 0;
            }
        }

        private void resetBackgrounds()
        {
            M1.Background = Brushes.Gold;
            M2.Background = Brushes.Gold;
            M3.Background = Brushes.Gold;
            M4.Background = Brushes.Gold;
            M5.Background = Brushes.Gold;
            M6.Background = Brushes.Gold;
            M7.Background = Brushes.Gold;
            M8.Background = Brushes.Gold;
            M9.Background = Brushes.Gold;
            M10.Background = Brushes.Gold;
            M11.Background = Brushes.Gold;
            M12.Background = Brushes.Gold;

        }

        private void LiveModus_Click(object sender, RoutedEventArgs e)
        {
            resetBackgrounds();
            startmonat = 0;
            endmonat = 0;
            ZeitSlider.Value = 0;
        }
    }
}