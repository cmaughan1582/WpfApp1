using Microsoft.Win32;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for WebWindow.xaml
    /// </summary>
    public partial class WebWindow : Window
    {
        public WebWindow()
        {
            InitializeComponent();

            Mapbrowser.Navigate("file:///C:/Users/cmaug/Desktop/Leaflet/testleaflet.html");
            Object[] test = { 51.505, -0.09 };
            Mapbrowser.LoadCompleted += webb_LoadCompleted;
            void webb_LoadCompleted(object sender, NavigationEventArgs e)
            {
                Mapbrowser.InvokeScript("centerMap", new Object[] { 25.520581, -103.40607 });

            }
            
        }
    }
}
