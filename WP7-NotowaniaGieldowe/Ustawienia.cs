using System;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace WP7_NotowaniaGieldowe
{
    public static class Ustawienia
    {
        // Lista pozycji do ściągnięcia w MainPage
        public static List<String> idPozycji = new List<String>();
        // Lista symboli
        //public static List<SymbolCSV> listaSymboli = new List<SymbolCSV>();

        public static void Zapisz()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("idPozycji"))
            {
                settings.Remove("idPozycji");
                settings.Add("idPozycji", idPozycji);
            }
            else
            {
                settings.Add("idPozycji", idPozycji);
            }
            settings.Save();
        }

        public static void Wczytaj()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

            if (settings.Contains("idPozycji")) idPozycji = (List<String>)settings["idPozycji"];
        }
    }
}
