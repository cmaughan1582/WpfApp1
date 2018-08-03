using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for WebWindow.xaml
    /// </summary>
    public partial class WebWindow : Window
    {
        public WebWindow(List<OfficialInspectorClass> inspectors, List<InspectionMapItem> assign, List<InspectionMapItem> with, List<InspectionMapItem> val, List<InspectionMapItem> accept)
        {
            InitializeComponent();

            string curDir = Directory.GetCurrentDirectory();
            Uri MapUrl = new Uri(String.Format("file:///{0}/Leaflet/testleaflet.html", curDir));
            Mapbrowser.Navigate(MapUrl); //this is the deployment url, update the html code and then move to here
            Object[] test = { 51.505, -0.09 };
            Mapbrowser.LoadCompleted += webb_LoadCompleted;
            void webb_LoadCompleted(object sender, NavigationEventArgs e)
            {
                
                for (int i = 0; i < inspectors.Count; i++)
                {
                    Mapbrowser.InvokeScript("mapInspectors", new Object[] { inspectors[i].latitude, inspectors[i].longitute, inspectors[i].Name, inspectors[i].Inspector_Ranking__c });
                }
                for(int i = 0; i < assign.Count; i++)
                {
                    Mapbrowser.InvokeScript("mapAssign", new Object[] { Convert.ToDouble(assign[i].Property_Latitude__c), Convert.ToDouble(assign[i].Property_Longitude__c), assign[i].Name });
                }
                for(int i = 0; i < with.Count; i++)
                {
                    Mapbrowser.InvokeScript("mapWith", new Object[] { Convert.ToDouble(with[i].Property_Latitude__c), Convert.ToDouble(with[i].Property_Longitude__c), with[i].Name });
                }
                for (int i = 0; i < val.Count; i++)
                {
                    Mapbrowser.InvokeScript("mapVal", new Object[] { Convert.ToDouble(val[i].Property_Latitude__c), Convert.ToDouble(val[i].Property_Longitude__c), val[i].Name });
                }
                for (int i = 0; i < accept.Count; i++)
                {
                    Mapbrowser.InvokeScript("mapAccept", new Object[] { Convert.ToDouble(accept[i].Property_Latitude__c), Convert.ToDouble(accept[i].Property_Longitude__c), accept[i].Name });
                }
                Mapbrowser.InvokeScript("centerMap", new Object[] { 41.750720, -111.840137 });

            }
            
        }
    }
}
