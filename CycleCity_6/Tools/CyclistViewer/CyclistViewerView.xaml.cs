using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Esri.ArcGISRuntime.Layers;

namespace CycleCity_6.Tools.CyclistViewer
{
    /// <summary>
    /// Interaktionslogik für CyclistViewerView.xaml
    /// </summary>
    public partial class CyclistViewerView
    {

        private int _monatselected; //dies ist nur eine flag die werte 0-2 annimmt
        private readonly Dictionary<string, List<Graphic>> _velografiken;
        private int _startmonat;
        private int _endmonat;
        private int _stundeVon;
        private int _stundeBis;
        private readonly UIElementCollection _buttonListe;
        private readonly ItemCollection _checkboxen;
        private bool _live;

        public CyclistViewerView()
        {
            InitializeComponent();
            GetViewModel().MapView = CycleMapView;

            _startmonat = 1;
            _endmonat = 12;
            _stundeVon = 0;
            _stundeBis = 23;
            _live = false;
            _velografiken = GetViewModel().TempVeloGraphics;
            _buttonListe = Zeitleiste.Children;
            _checkboxen = VeloroutenCheckboxContainer.Items;
        }

        private CyclistViewerViewModel GetViewModel()
        {
            Contract.Requires(DataContext is CyclistViewerViewModel);
            return (CyclistViewerViewModel)DataContext;
        }


        private void Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ZeitSliderVon.Value > ZeitSliderBis.Value -1)
            {
                ZeitSliderBis.Value = ZeitSliderVon.Value + 1;
            }
            //TODO Daten nach der ausgewählten Zeit anzeigen lassen
            _stundeVon = (int)ZeitSliderVon.Value;
            _stundeBis = (int)ZeitSliderBis.Value;
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
            if (_stundeVon == 24)
            {
                _stundeVon = 0;
            }
            if (_stundeBis == 24)
            {
                _stundeBis = 0;
            }
            if (_endmonat == 0)
            {
                GetViewModel().SetzeUhrzeit(new DateTime(2016, monat, 01, _stundeVon, 00, 00), DateTime.MaxValue);
            }
            else
            {
                int letzterTag = DateTime.DaysInMonth(2016, _endmonat);
                //TODO Aktuell wird von Monat x die Startzeit genommen und von Monat y die endzeit. Am Ende soll aber 
                // bsw. in einem Monat alles von 9-11 Uhr angezeigt werden. Dafür brauchen wir aber unsere eigene Datenbasis
                GetViewModel().SetzeUhrzeit(new DateTime(2016, _startmonat, 01, _stundeVon, 00, 00), new DateTime(2016, _endmonat, letzterTag, _stundeBis, 59, 59));
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
                              where button.Name == monat
                              select button;
            //holt sich das erste Element der Liste, da dieses der gewünschte Button ist
            var returnButton = monatreturn.ToList()[0];
            returnButton.Background = Brushes.CadetBlue;
            //returnt die Zahl im Namen des Buttons
            return int.Parse(returnButton.Name.Split('M')[1]);
        }

        /// <summary>
        /// setzt die Hintergründe der Buttons für die Monatsauswahl zurück
        /// </summary>
        private void ResetBackgrounds()
        {
            foreach (var button in _buttonListe.OfType<Button>())
            {
                button.Background = Brushes.PowderBlue;
                button.IsEnabled = true;
            }
            ZeitSliderBis.IsEnabled = true;
            ZeitSliderVon.IsEnabled = true;
        }

        private void LiveModus_Click(object sender, RoutedEventArgs e)
        {
            _startmonat = 1;
            _endmonat = 12;
            ZeitSliderVon.Value = 0;
            ZeitSliderBis.Value = 0;
            var button = sender as Button;
            button.Foreground = Equals(button.Foreground, Brushes.LightGray) ? Brushes.Red : Brushes.LightGray;
            _live = !_live;
            GetViewModel().AktiviereLive(_live);
            //(de)aktiviere Zeitleiste
            if (_live)
            {
                foreach (Button b in _buttonListe.OfType<Button>())
                {
                    b.Background = Brushes.LightGray;
                    b.IsEnabled = false;
                }
                ZeitSliderVon.IsEnabled = false;
                ZeitSliderBis.IsEnabled = false;
            }
            else
            {
                ResetBackgrounds();
            }
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

        }

        private void AlleVelorouten_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var box in _checkboxen.OfType<CheckBox>())
            {
                box.IsChecked = true;
            }
        }

        private void AlleVelorouten_Unchecked(object sender, RoutedEventArgs e)
        {

            foreach (var box in _checkboxen.OfType<CheckBox>())
            {
                box.IsChecked = false;
            }
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