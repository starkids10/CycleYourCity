using Esri.ArcGISRuntime.Layers;
using System.Diagnostics.Contracts;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using System.Windows.Media;
using System.Globalization;
using System.Linq;

namespace CycleCity_6.Tools.CyclistViewer
{
    /// <summary>
    /// Interaktionslogik für CyclistViewerView.xaml
    /// </summary>
    public partial class CyclistViewerView : UserControl
    {

        private int _monatselected = 0; //dies ist nur eine flag die werte 0-2 annimmt
        private Dictionary<string, Graphic> _velografiken;
        private int _startmonat = 1;
        private int _endmonat = 12;
        int stundeVon = 0;
        int stundeBis = 0;
        private UIElementCollection _buttonListe;

        public CyclistViewerView()
        {
            InitializeComponent();

            GetViewModel().MapView = CycleMapView;
            _velografiken = GetViewModel().TempVeloGraphics;
            _buttonListe = Zeitleiste.Children;
        }

        private CyclistViewerViewModel GetViewModel()
        {
            Contract.Requires(DataContext is CyclistViewerViewModel);
            return (CyclistViewerViewModel)DataContext;
        }


        private void Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //TODO Daten nach der ausgewählten Zeit anzeigen lassen
            stundeVon = (int)ZeitSliderVon.Value;
            stundeBis = (int)ZeitSliderBis.Value;
            int monat;

            SliderVonText.Text = ZeitSliderVon.Value + ":00";
            SliderBisText.Text = ZeitSliderBis.Value + ":00";

            if (_startmonat == 0)
            {
                monat = 1;
            }
            else
            {
                monat = _startmonat;
            }
            if (stundeVon == 24)
            {
                stundeVon = 0;
            }
            if (stundeBis == 24)
            {
                stundeBis = 0;
            }
            if (_endmonat == 0)
            {
                GetViewModel().SetzeUhrzeit(new DateTime(2016, monat, 01, stundeVon, 00, 00), DateTime.MaxValue);
            }
            else
            {
                int letzterTag = DateTime.DaysInMonth(2016, _endmonat);
                //TODO Aktuell wird von Monat x die Startzeit genommen und von Monat y die endzeit. Am Ende soll aber 
                // bsw. in einem Monat alles von 9-11 Uhr angezeigt werden. Dafür brauchen wir aber unsere eigene Datenbasis
                GetViewModel().SetzeUhrzeit(new DateTime(2016, _startmonat, 01, stundeVon, 00, 00), new DateTime(2016, _endmonat, letzterTag, stundeBis, 59, 59));
            }

        }

        // TODO wieder einbauen wenn auf touch verwendet werden soll "System.Windows.Input.TouchEventArgs"

        private void Monatsauswahl_OnTouch(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string monat = button.Name;

            if (_monatselected == 0 || _monatselected == 2)
            {
                //Click Flag setzten 
                _monatselected = 1;
                // von allen Buttons Hintergrundfarbe zurücksetzten
                ResetBackgrounds();

                //Hintergrundfarbe setzten
                _startmonat = SetBackground(monat);

                //Server Zeitraum mitteilen ("SetzeUhrzeit();")
                GetViewModel().SetzeUhrzeit(new DateTime(2016, _startmonat, 01, 00, 00, 00), DateTime.MaxValue);
            }
            else if (_monatselected == 1)
            {
                _monatselected = 2;
                //diesen und ersten monat auswählen;
                _endmonat = SetBackground(monat);

                //buttons dazwischen mit neuer hintergrundfarbe anpassen;
                if (_startmonat > _endmonat)
                {
                    int y = _startmonat;
                    _startmonat = _endmonat;
                    _endmonat = y;
                }
                for (int x = _startmonat; x < _endmonat; x++)
                {
                    SetBackground("M" + x);
                }

                //Server Zeitraum mitteilen ("SetzeUhrzeit();")
                int letzterTag = DateTime.DaysInMonth(2016, _endmonat);
                GetViewModel()
                    .SetzeUhrzeit(new DateTime(2016, _startmonat, 01, 00, 00, 00),
                        new DateTime(2016, _endmonat, letzterTag, 23, 59, 59));
            }
        }


        /// <summary>
        /// Setzt den Hintergrund eines übergebenen Buttons auf "CadetBlue" und gibt dessen Monat als Int zurück (1-12)
        /// </summary>
        /// <param name="monat"></param>
        /// <returns></returns>
        private int SetBackground(string monat)
        {
            //geht nur, wenn alle Monatsbuttons in einem eigenen Container sind.

            //holt sich den Button aus der Buttonlist den Button dessen Namen 'monat' gleicht
            var monatreturn = from button in _buttonListe.OfType<Button>()
                              where button.Name == monat select button;
            //holt sich das erste Element der Liste, da dieses der gewünschte Button ist
            var returnButton = monatreturn.ToList()[0];
            returnButton.Background = Brushes.CadetBlue;
            //returnt die Zahl im Namen des Buttons
            return int.Parse(returnButton.Name.Split('M')[1]);


            switch (monat)
            {
                case "M1":
                    M1.Background = Brushes.CadetBlue;
                    return 1;
                case "M2":
                    M2.Background = Brushes.CadetBlue;
                    return 2;
                case "M3":
                    M3.Background = Brushes.CadetBlue;
                    return 3;
                case "M4":
                    M4.Background = Brushes.CadetBlue;
                    return 4;
                case "M5":
                    M5.Background = Brushes.CadetBlue;
                    return 5;
                case "M6":
                    M6.Background = Brushes.CadetBlue;
                    return 6;
                case "M7":
                    M7.Background = Brushes.CadetBlue;
                    return 7;
                case "M8":
                    M8.Background = Brushes.CadetBlue;
                    return 8;
                case "M9":
                    M9.Background = Brushes.CadetBlue;
                    return 9;
                case "M10":
                    M10.Background = Brushes.CadetBlue;
                    return 10;
                case "M11":
                    M11.Background = Brushes.CadetBlue;
                    return 11;
                case "M12":
                    M12.Background = Brushes.CadetBlue;
                    return 12;
                default:
                    return 0;
            }
        }

        private void ResetBackgrounds()
        {
            foreach (var button in _buttonListe.OfType<Button>())
            {
                button.Background = Brushes.PowderBlue;
            }
            return; //geht nur, wenn alle Monatsbuttons in einem eigenem Container sind.
            M1.Background = Brushes.PowderBlue;
            M2.Background = Brushes.PowderBlue;
            M3.Background = Brushes.PowderBlue;
            M4.Background = Brushes.PowderBlue;
            M5.Background = Brushes.PowderBlue;
            M6.Background = Brushes.PowderBlue;
            M7.Background = Brushes.PowderBlue;
            M8.Background = Brushes.PowderBlue;
            M9.Background = Brushes.PowderBlue;
            M10.Background = Brushes.PowderBlue;
            M11.Background = Brushes.PowderBlue;
            M12.Background = Brushes.PowderBlue;
        }

        private void LiveModus_Click(object sender, RoutedEventArgs e)
        {
            ResetBackgrounds();
            _startmonat = 1;
            _endmonat = 12;
            ZeitSliderVon.Value = 0;
            ZeitSliderBis.Value = 0;
        }

        private void CYC_Checked(object sender, RoutedEventArgs e)
        {
            //TODO Checkbox passende Tracks anzeigen
        }

        private void CYC_Unchecked(object sender, RoutedEventArgs e)
        {
            //TODO Checkbox passende Tracks ausblenden
        }

        private void Naviki_Checked(object sender, RoutedEventArgs e)
        {
            //TODO Checkbox passende Tracks anzeigen
        }

        private void Naviki_Unchecked(object sender, RoutedEventArgs e)
        {
            //TODO Checkbox passende Tracks ausblenden
        }

        private void AlleVelorouten_Checked(object sender, RoutedEventArgs e)
        {
            //TODO Checkbox passende Velorouten anzeigen
        }

        private void AlleVelorouten_Unchecked(object sender, RoutedEventArgs e)
        {
            //TODO Checkbox passende Velorouten ausblenden
        }

        private void Veloroute_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox box = sender as CheckBox;
            string name = box.Name;
            //trennt Index vom Namen
            string[] index = name.Split('R');
            //fügt der Map im ViewModel eine zu zeichnende Veloroute hinzu
            if (!_velografiken.ContainsKey(name))
            {
                _velografiken.Add(name, GetViewModel().GetVeloRouteAt(int.Parse(index[1]) - 1));
            }

            GetViewModel().ZeichneVelorouten();
        }

        private void Veloroute_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox box = sender as CheckBox;
            string name = box.Name;
            _velografiken.Remove(name);
            GetViewModel().ZeichneVelorouten();
        }

    }
}