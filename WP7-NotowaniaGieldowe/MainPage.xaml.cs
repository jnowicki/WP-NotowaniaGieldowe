using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Microsoft.Phone.Controls;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using CsvHelper;

namespace WP7_NotowaniaGieldowe
{
    public partial class MainPage : PhoneApplicationPage
    {
        public List<String> idWyswietlonych = new List<String>();
        public ObservableCollection<Notowanie> listaNotowanSource = new ObservableCollection<Notowanie>();
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.Loaded +=new RoutedEventHandler(MainPage_Loaded);         
        }

        public void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // wczytaj ustawienia z local storage jesli lista jest pusta
            if(!Ustawienia.idPozycji.Any()) Ustawienia.Wczytaj();
            // pobierz wszystkie notowania ktorych id znajduje sie w Ustawieniach globalnych
            foreach (String id in Ustawienia.idPozycji)
            {
                SciagnijPozycjeOID(id);
            }
        }

        public void SciagnijPozycjeOID(String id)
        {
            String uri = "http://download.finance.yahoo.com/d/quotes.csv?s=" + id + "&f=nl1c1s0&e=.csv";
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(sciagnijPozycjeOID_DownloadStringCompleted);
            webClient.DownloadStringAsync(new Uri(uri));
        }

        void sciagnijPozycjeOID_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            /*
            String[] dane = e.Result.Split(',');
            listaNotowanSource.Add(new Notowanie(dane[0],dane[1],dane[2],dane[3]));
             * */
            using (TextReader sr = new StringReader(e.Result))
            {
                var csv = new CsvReader(sr);
                csv.Configuration.HasHeaderRecord = false;
                csv.Configuration.RegisterClassMap<NotowanieMap>();
                while (csv.Read())
                {
                    Notowanie rekord = csv.GetRecord<Notowanie>();
                    listaNotowanSource.Add(rekord);
                }
            }
        }

        private void ustawieniaGuzik_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ZmienUstawienia.xaml", UriKind.Relative));
        }

        // templatka na notowanie
        public class Notowanie : IComparable
        {
            public String NazwaFirmy { get; set; }
            public String Cena { get; set; }
            public String Zmiana { get; set; }
            public String Symbol { get; set; }

            public int CompareTo(object obj)
            {
                Notowanie other = (Notowanie)obj;
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;
                return comparer.Compare(this.NazwaFirmy, other.NazwaFirmy);
            }
        }

        private void kasujFirme_Click(object sender, RoutedEventArgs e)
        {
            Button wysylacz = (Button)sender;

            String nazwafirmy = (String)wysylacz.Tag;

            for (int i = listaNotowanSource.Count - 1; i >= 0; i--)
            {
                if (listaNotowanSource[i].NazwaFirmy == nazwafirmy)
                {
                    for (int j = Ustawienia.idPozycji.Count - 1; j >= 0; j--)
                    {
                        if(listaNotowanSource[i].Symbol == Ustawienia.idPozycji[j]) Ustawienia.idPozycji.RemoveAt(j);
                    }
                    listaNotowanSource.RemoveAt(i);
                }
            }
        }

        public sealed class NotowanieMap : CsvClassMap<Notowanie>
        {
            public NotowanieMap()
            {
                Map(m => m.NazwaFirmy);
                Map(m => m.Cena);
                Map(m => m.Zmiana);
                Map(m => m.Symbol);
            }
        }
    }
}