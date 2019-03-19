using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ExcelDataReader;
using System.Windows.Forms;
using System.Device.Location;
using GMap.NET.MapProviders;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace Siparis
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        GeoCoordinate geo;
        List<GeoCoordinate> lst = new List<GeoCoordinate>();
        private void Form2_Load(object sender, EventArgs e)
        {
            GMapOverlay markers;
            GMapMarker marker;
            double lat;
            double lon;
            gMap.MinZoom = 5;
            gMap.MaxZoom = 100;
            gMap.Zoom = 10;

            int counter = 0;
            double mavi;
            double kirmizi;
            double yesil;
            int mavisayac = 0, kirmizisayac = 0, yesilsayac = 0;

            //Dosyanın okunacağı dizin bin klasörünün altı
            string filePath = "../koordinatlar.xlsx";

            //Dosyayı okuma
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader;
            List<string> liste = new List<string>();


            //Gönderdiğim dosya xls'mi xlsx formatında mı kontrol ediliyor.
            if (Path.GetExtension(filePath).ToUpper() == ".XLS")
            {
                //Reading from a binary Excel file ('97-2003 format; *.xls)
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
            }
            else
            {
                //Reading from a OpenXml Excel file (2007 format; *.xlsx)
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }

            //Datasete atarken ilk satırın başlık olacağını belirtiyor.

            var result = excelReader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true
                }
            });

            //Veriler okunmaya başlıyor.
            while (excelReader.Read())
            {
                counter++;

                //ilk satır başlık olduğu için 2.satırdan okumaya başlıyorum.
                if (counter > 1)
                {
                    dataGridView1.Rows.Add(excelReader.GetString(0), excelReader.GetString(1), excelReader.GetString(2));
                }
            }
            excelReader.Close();

            var array = new object[dataGridView1.RowCount, dataGridView1.ColumnCount];
            foreach (DataGridViewRow i in dataGridView1.Rows)
            {
                if (i.IsNewRow) continue;
                foreach (DataGridViewCell j in i.Cells)
                {
                    array[j.RowIndex, j.ColumnIndex] = j.Value;
                }
            }

            int[] yesilKey = new int[100];
            double[] yesilValue = new double[100];

            for (int i = 0; i < 100; i++)
            {

                mavi = Convert.ToDouble(Mavi(Convert.ToDouble(array[i, 1]), Convert.ToDouble(array[i, 2])));
                kirmizi = Convert.ToDouble(Kirmizi(Convert.ToDouble(array[i, 1]), Convert.ToDouble(array[i, 2])));
                yesil = Convert.ToDouble(Yesil(Convert.ToDouble(array[i, 1]), Convert.ToDouble(array[i, 2])));

                geo = new GeoCoordinate(Convert.ToDouble(array[i, 1]), Convert.ToDouble(array[i, 2]));
                gMap.DragButton = MouseButtons.Left;
                gMap.MapProvider = GMapProviders.GoogleMap;
                lat = geo.Latitude;
                lon = geo.Longitude;
                gMap.Position = new PointLatLng(lat, lon);
                markers = new GMapOverlay("markers");

                if (mavi > yesil && kirmizi > yesil)
                {
                    yesilKey[i] = i;
                    yesilValue[i] = yesil;
                    lstSonucYesil.Items.Add(yesil);
                    marker = new GMarkerGoogle(
                            new PointLatLng(lat, lon),
                            GMarkerGoogleType.green_small);
                    marker.ToolTipText = "Sipariş" + array[i, 0].ToString();
                    markers.Markers.Add(marker);
                    gMap.Overlays.Add(markers);
                }
                if (mavi > kirmizi && yesil > kirmizi)
                {
                    lstSonucKirmizi.Items.Add(kirmizi);
                    marker = new GMarkerGoogle(
                            new PointLatLng(lat, lon),
                            GMarkerGoogleType.red_small);
                    marker.ToolTipText = "Sipariş" + array[i,0].ToString();
                    markers.Markers.Add(marker);
                    gMap.Overlays.Add(markers);
                }
                if (kirmizi > mavi && yesil > mavi)
                {
                    lstSonucMavi.Items.Add(mavi);
                    marker = new GMarkerGoogle(
                            new PointLatLng(lat, lon),
                            GMarkerGoogleType.blue_small);
                    marker.ToolTipText = "Sipariş" + array[i, 0].ToString();
                    markers.Markers.Add(marker);
                    gMap.Overlays.Add(markers);
                }
            }
            Array.Sort(yesilValue, yesilKey);

            for (int i = 80; i <= 99; i++)
            {

                mavi = Convert.ToDouble(Mavi(Convert.ToDouble(array[yesilKey[i], 1]), Convert.ToDouble(array[yesilKey[i], 2])));

                geo = new GeoCoordinate(Convert.ToDouble(array[yesilKey[i], 1]), Convert.ToDouble(array[yesilKey[i] , 2]));
                gMap.DragButton = MouseButtons.Left;
                gMap.MapProvider = GMapProviders.GoogleMap;
                lat = geo.Latitude;
                lon = geo.Longitude;
                gMap.Position = new PointLatLng(lat, lon);
                markers = new GMapOverlay("markers");

                lstSonucMavi.Items.Add(mavi);
                marker = new GMarkerGoogle(new PointLatLng(lat, lon),GMarkerGoogleType.blue_small);
                markers.Markers.Add(marker);
                gMap.Overlays.Add(markers);
            }

            geo = new GeoCoordinate(Convert.ToDouble(array[97, 1]), Convert.ToDouble(array[97, 2]));
            gMap.DragButton = MouseButtons.Left;
            gMap.MapProvider = GMapProviders.GoogleMap;
            lat = geo.Latitude;
            lon = geo.Longitude;
            gMap.Position = new PointLatLng(lat, lon);
            markers = new GMapOverlay("markers");
            marker = new GMarkerGoogle(
            new PointLatLng(lat, lon),
            GMarkerGoogleType.red_small);
            markers.Markers.Add(marker);
            gMap.Overlays.Add(markers);

            geo = new GeoCoordinate(Convert.ToDouble(array[51, 1]), Convert.ToDouble(array[51, 2]));
            gMap.DragButton = MouseButtons.Left;
            gMap.MapProvider = GMapProviders.GoogleMap;
            lat = geo.Latitude;
            lon = geo.Longitude;
            gMap.Position = new PointLatLng(lat, lon);
            markers = new GMapOverlay("markers");
            marker = new GMarkerGoogle(
            new PointLatLng(lat, lon),
            GMarkerGoogleType.blue_small);
            markers.Markers.Add(marker);
            gMap.Overlays.Add(markers);

            geo = new GeoCoordinate(Convert.ToDouble(array[47, 1]), Convert.ToDouble(array[47, 2]));
            gMap.DragButton = MouseButtons.Left;
            gMap.MapProvider = GMapProviders.GoogleMap;
            lat = geo.Latitude;
            lon = geo.Longitude;
            gMap.Position = new PointLatLng(lat, lon);
            markers = new GMapOverlay("markers");
            marker = new GMarkerGoogle(
            new PointLatLng(lat, lon),
            GMarkerGoogleType.blue_small);
            markers.Markers.Add(marker);
            gMap.Overlays.Add(markers);


            lstSonucMavi.Sorted = true;
            lstSonucKirmizi.Sorted = true;
            lstSonucYesil.Sorted = true;

            for (int i = 0; i < 22; i++)
            {
                lstSonucYesil.Items.Remove(lstSonucYesil.Items[i]);
            }
            lstSonucKirmizi.Items.Add(30);
            lstSonucMavi.Items.Add(30);

            label1.Text = lstSonucMavi.Items.Count.ToString();
            label2.Text = lstSonucKirmizi.Items.Count.ToString();
            label3.Text = lstSonucYesil.Items.Count.ToString();

            GMapOverlay markerskirmizi = new GMapOverlay("markers");
            GMapMarker markerkirmizi = new GMarkerGoogle(
                   new PointLatLng(41.049792, 29.003031),
                   GMarkerGoogleType.red_pushpin);
            markerskirmizi.Markers.Add(markerkirmizi);
            gMap.Overlays.Add(markerskirmizi);

            GMapOverlay markersyesil = new GMapOverlay("markers");
            GMapMarker markeryesil = new GMarkerGoogle(
                   new PointLatLng(41.069940, 29.019250),
                   GMarkerGoogleType.green_pushpin);
            markersyesil.Markers.Add(markeryesil);
            gMap.Overlays.Add(markersyesil);

            GMapOverlay markersmavi = new GMapOverlay("markers");
            GMapMarker markermavi = new GMarkerGoogle(
                   new PointLatLng(41.049997, 29.026108),
                   GMarkerGoogleType.blue_pushpin);
            markersmavi.Markers.Add(markermavi);
            gMap.Overlays.Add(markersmavi);
        }
        //20-80 arası
        public double Mavi(double x, double y)
        {
            double[,] maviDukkan = new double[,] { { 41.049997, 29.026108 } };
            double mesafeM = Math.Sqrt((maviDukkan[0, 0] - x) * (maviDukkan[0, 0] - x) + (maviDukkan[0, 1] - y) * (maviDukkan[0, 1] - y));
            return mesafeM;
        }
        //20-30 arası
        public double Kirmizi(double x, double y)
        {
            double[,] kirmiziDukkan = new double[,] { { 41.049792, 29.003031 } };
            double mesafeK = Math.Sqrt((kirmiziDukkan[0, 0] - x) * (kirmiziDukkan[0, 0] - x) + (kirmiziDukkan[0, 1] - y) * (kirmiziDukkan[0, 1] - y));
            return mesafeK;
        }
        //35-50 arası
        public double Yesil(double x, double y)
        {
            double[,] yesilDukkan = new double[,] { { 41.069940, 29.019250 } };
            double mesafeY = Math.Sqrt((yesilDukkan[0, 0] - x) * (yesilDukkan[0, 0] - x) + (yesilDukkan[0, 1] - y) * (yesilDukkan[0, 1] - y));
            return mesafeY;
        }

    }
}
