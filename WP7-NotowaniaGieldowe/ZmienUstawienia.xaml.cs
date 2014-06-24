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
using Microsoft.Phone.Controls;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;


namespace WP7_NotowaniaGieldowe
{
    public partial class ZmienUstawienia : PhoneApplicationPage
    {
        public ZmienUstawienia()
        {
            InitializeComponent();
            this.Loaded +=new RoutedEventHandler(ZmienUstawienia_Loaded);
        }

        public void ZmienUstawienia_Loaded(object sender, RoutedEventArgs e)
        {
            SciagnijSymbole();
        }

        private void wpiszId_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                Ustawienia.idPozycji.Add(wpiszId.Text);
                Ustawienia.Zapisz();
                this.Focus();
                MessageBox.Show("Dodano pozycje " + wpiszId.Text);
            }
        }

        private void wrocGuzik_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void firmaGuzik_Click(object sender, RoutedEventArgs e)
        {
            Button wysylacz = (Button)sender;
            String symbol = (String)wysylacz.Tag;
            Ustawienia.idPozycji.Add(symbol);
            Ustawienia.Zapisz();
            MessageBox.Show("Dodano pozycje " + symbol);
        }

        public void SciagnijSymbole()
        {
            WebClient webClient = new WebClient();
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(sciagnijSymbole_DownloadStringCompleted);
            webClient.DownloadStringAsync(new Uri("http://kubavic.vdl.pl/cboesymboldir2.csv"));
        }

        void sciagnijSymbole_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {

            using (TextReader sr = new StringReader(e.Result))
            {
                
                var csv = new CsvReader(sr);
                csv.Configuration.RegisterClassMap<SymbolMap>();
                List<SymbolCSV> listaSymboli = csv.GetRecords<SymbolCSV>().ToList();
                listaFirm.ItemsSource = listaSymboli;
                
            }
        }
    }

    public class SymbolCSV
    {
        public String Nazwa { get; set; }
        public String Symbol { get; set; }
    }

    public sealed class SymbolMap : CsvClassMap<SymbolCSV>
    {
        public SymbolMap()
        {
            Map(m => m.Nazwa);
            Map(m => m.Symbol);
        }
    }
}