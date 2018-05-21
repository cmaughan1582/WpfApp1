using GeoCoordinatePortable;
using Newtonsoft.Json;
using SalesforceSharp;
using SalesforceSharp.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace WpfApp1
{

    /* TODO
    HUD only assigning option
    Include sort option for middle list
    */

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<String, String> assignDict = new Dictionary<String, String>();
        Boolean HUDmode = false;
        Dictionary<String, String> credentials = new Dictionary<String, String>();
        List<String> assignNames = new List<string>();
        public const String mapKey = "On5gRfRDnoozDkk8zKjo5GpXGbvYCycm";
        public SalesforceClient client = new SalesforceClient();
        List<OfficialInspectorClass> workingList = new List<OfficialInspectorClass>();
        String orderNumberSearch = "";
        InspectionJSONClass currentInspection = new InspectionJSONClass();
        List<String> tenInspectors = new List<String>();
        double inspectionHistoryHeight = 0;
        int currentten = 0;
        bool reSearch = false;
        String currentDate = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        const String consumerSecret = "2336265352872907529";
        const String clientID = "3MVG98SW_UPr.JFjKKrtQHBWMbQf..W2pXI.gHVL5J8AIH_lFnLkpIkaD5q.oinctEhqcKvRAzFqjkBJSDFZA";
        bool DirectoryExists = Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\S2Inspections");
        bool loginExists = Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\S2Inspections", "logininfo.bin"));
        int SortNumber = 0;

        public MainWindow()
        {
            InitializeComponent();
            if (!DirectoryExists)
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\S2Inspections");
            }
            login_page.Visibility = Visibility.Visible;
            if (!loginExists)
            {
                credentials.Add("username", "files@s2inspect.com");
                credentials.Add("password", "5200@Holladay");
                credentials.Add("securityToken", "3U3rVWo6TYVWJMhccR7hO9bhn");
                string saveFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\S2Inspections", "logininfo.bin");
                using (Stream stream = File.Open(saveFile, FileMode.Create))
                {
                    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    formatter.Serialize(stream, credentials);
                }
            }
            else
            {
                string saveFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\S2Inspections", "logininfo.bin");
                using (Stream stream = File.Open(saveFile, FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    credentials = (Dictionary<String, String>)bformatter.Deserialize(stream);
                }
            }


        }

        static private InspectionJSONClass findInspectionbyOrderNumber(String orderNumber, SalesforceClient client)
        {
            var record = client.Query<QueryforIDclass>("SELECT Id From Inspection__c WHERE Name='" + orderNumber + "'");
            if (record.Count != 1)
            {
                return null;
            }
            else
            {
                return client.FindById<InspectionJSONClass>("Inspection__c", record[0].Id);
            }


        }

        static private List<OfficialInspectorClass> LoadSavedDatabase(bool HUDmode)
        {
            List<OfficialInspectorClass> returnList = new List<OfficialInspectorClass>();
            string saveFile;
            if (HUDmode == false)
            {
                saveFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\S2Inspections", "nonhudinspectors.bin");
            }
            else
            {
                saveFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\S2Inspections", "hudinspectors.bin");
            }
            using (Stream stream = File.Open(saveFile, FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                returnList = (List<OfficialInspectorClass>)bformatter.Deserialize(stream);
            }
            return returnList;
        }

        private void sortByDistance(SalesforceClient client)
        {
            String modifiedstreet = currentInspection.Street_Address__c.Replace('&', '-');
            String currentInspectionAddress = (modifiedstreet + ", " + currentInspection.City__c + ", " + currentInspection.State__c);
            String JsonReturn = ("");
            using (var client1 = new HttpClient())
            {
                String restRequest = ("http://www.mapquestapi.com/geocoding/v1/address?key="
                    + mapKey + "&location=" + currentInspectionAddress);
                var request = new HttpRequestMessage(HttpMethod.Get, restRequest);
                request.Headers.Add("X-PrettyPrint", "1");
                var response = client1.SendAsync(request).Result;
                JsonReturn = response.Content.ReadAsStringAsync().Result;
            }
            LatLngClass.RootObject coordinates = new LatLngClass.RootObject();
            coordinates = JsonConvert.DeserializeObject<LatLngClass.RootObject>(JsonReturn);
            var array = coordinates.results.ToArray();
            var array2 = array[0].locations.ToArray();
            double currentInspectionLatitude = array2[0].displayLatLng.lat;
            double currentInspectionLongitute = array2[0].displayLatLng.lng;
            GeoCoordinate currentGeopoint = new GeoCoordinate(currentInspectionLatitude, currentInspectionLongitute);
            for (int i = 0; i < workingList.Count; i++)
            {
                GeoCoordinate compareVal = new GeoCoordinate(workingList[i].latitude.GetValueOrDefault(), workingList[i].longitute.GetValueOrDefault());
                workingList[i].currentDistance = (0.00062137 * currentGeopoint.GetDistanceTo(compareVal));
            }
            workingList.Sort((x, y) => x.currentDistance.CompareTo(y.currentDistance));
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Rebuild_page.Visibility = Visibility.Collapsed;
            workingList = LoadSavedDatabase(HUDmode);
            Search_Page.Visibility = Visibility.Visible;
            First_Search_Box1.Focus();

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            login_page.Visibility = Visibility.Collapsed;
            var authflow = new UsernamePasswordAuthenticationFlow(clientID, consumerSecret, credentials["username"], credentials["password"]+credentials["securityToken"]);
            try
            {
                client.Authenticate(authflow);
                mode_page.Visibility = Visibility.Visible;

            }
            catch (SalesforceException ex)
            {
                return;
            }
        }

        private void First_Search_Button_Click(object sender, RoutedEventArgs e)
        {
            orderNumberSearch = (First_Search_Box1.Text);
            Database_Loaded.Visibility = Visibility.Collapsed;
            ReSearch_Text.Visibility = Visibility.Visible;
            SearchResults();
        }

        private void First_Search_Box1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                First_Search_Button_Click(this, new RoutedEventArgs());
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            int index = ten_list_box.SelectedIndex;
            if (index == -1)
            {

            }
            else
            {
                //assigning part goes here!
                String assignID = workingList[index].contactID;
                UpdateInspectorClass updateInspector = new UpdateInspectorClass();
                updateInspector.Inspector__c = assignID;
                client.Update("Inspection__c", currentInspection.Id, updateInspector);
                orderNumberSearch = (currentInspection.Name);
                currentInspection = new InspectionJSONClass();
                SearchResults();

            }

        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            orderNumberSearch = (Search_Box2.Text);
            SearchResults();
        }

        private void Search_Box2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click_4(this, new RoutedEventArgs());
            }
            else
            {

            }

        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            Rebuild_page.Visibility = Visibility.Collapsed;
            rebuild_progress.Visibility = Visibility.Visible;
            var progress = new Progress<string>(s => Records_text.Text = s);
            await Task.Factory.StartNew(() => SecondThreadConcern.LongWork(progress, client, HUDmode),
                                        TaskCreationOptions.LongRunning);
            rebuild_progress.Visibility = Visibility.Collapsed;
            Button_Click_1(this, new RoutedEventArgs());
        }
        class SecondThreadConcern
        {
            public static void LongWork(IProgress<string> progress, SalesforceClient client, bool HUDmode)
            {
                const String mapKey = "On5gRfRDnoozDkk8zKjo5GpXGbvYCycm";
                //this is the query that finds all records and builds them into an ilist
                IList<NewInspectorClass> records;
                if (!HUDmode)
                {
                    records = client.Query<NewInspectorClass>(
                        "SELECT Id, Name, BillingPostalCode, ShippingPostalCode, Rep_ID__C, Comments__c, HUD_Certified__c, FNMA_Certified__c, Freddie_Mac_Certified__c, Inspector_Ranking__c, Status__c, ShippingLatitude, ShippingLongitude, FNMA_4260__c, FNMA_4261__c, FNMA_4262__c, No_Contact__c, Inspector_Rush__c, CMSA_2__c, Exterior_1__c, FNMA_HC_MBA__c, Exterior_2__c, CME_HC__c, CME_MF__c, Freddie_MF_MBA__c, MBA__c, MBA_2__c, HUD_REAC__c, Freddie_HC_MBA__c, FNMA_MF_MBA__c, CMSA__c, Cap_Improv__c " +
                        "From Account " +
                        "WHERE Account_Inactive__c=false AND HUD_Certified__c=false AND Rep_ID__c!=null");
                }
                else
                {
                    records = client.Query<NewInspectorClass>(
                        "SELECT Id, Name, BillingPostalCode, ShippingPostalCode, Rep_ID__C, Comments__c, HUD_Certified__c, FNMA_Certified__c, Freddie_Mac_Certified__c, Inspector_Ranking__c, Status__c, ShippingLatitude, ShippingLongitude, FNMA_4260__c, FNMA_4261__c, FNMA_4262__c, No_Contact__c, Inspector_Rush__c, CMSA_2__c, Exterior_1__c, FNMA_HC_MBA__c, Exterior_2__c, CME_HC__c, CME_MF__c, Freddie_MF_MBA__c, MBA__c, MBA_2__c, HUD_REAC__c, Freddie_HC_MBA__c, FNMA_MF_MBA__c, CMSA__c, Cap_Improv__c " +
                        "From Account " +
                        "WHERE Account_Inactive__c=false AND HUD_Certified__c=TRUE AND Rep_ID__c!=null");
                }
                //creates a regular list, excluding qc accounts
                List<OfficialInspectorClass> OfficialInspectorList = new List<OfficialInspectorClass>();
                List<int> missedSeconds = new List<int>();
                for (int i = 0; i < records.Count; i++)
                {
                    if (records[i].ShippingPostalCode != null)
                    {

                        OfficialInspectorClass addObject = new OfficialInspectorClass();
                        String compareId = records[i].Id;
                        try
                        {
                            var records2 = client.Query<TempInspectorClass>("SELECT Id, Assigned_Inspections__c, Name, Phone " +
                                "From Contact " +
                                "WHERE AccountId='" + compareId + "'");
                            addObject.contactID = records2[0].Id;
                            addObject.assignedInspections = records2[0].Assigned_Inspections__c;
                            addObject.Name = records2[0].Name;
                            addObject.accountId = records[i].Id;
                            if (records[i].BillingPostalCode.Length < 5)
                            {
                                addObject.BillingPostalCode = ("0" + records[i].BillingPostalCode);
                            }
                            else
                            {
                                addObject.BillingPostalCode = records[i].BillingPostalCode;
                            }
                            if (records[i].ShippingPostalCode.Length < 5)
                            {
                                addObject.ShippingPostalCode = ("0" + records[i].ShippingPostalCode);
                            }
                            else
                            {
                                addObject.ShippingPostalCode = records[i].ShippingPostalCode;
                            }
                            addObject.Phone = records2[0].Phone;
                            addObject.HUD_Certified__c = records[i].HUD_Certified__c;
                            addObject.Inspector_Ranking__c = records[i].Inspector_Ranking__c;
                            addObject.Status__c = records[i].Status__c;
                            addObject.Comments__c = records[i].Comments__c;
                            addObject.Rep_ID__c = records[i].Rep_ID__c;
                            addObject.FNMA_Certified__c = records[i].FNMA_Certified__c;
                            addObject.Freddie_Mac_Certified__c = records[i].Freddie_Mac_Certified__c;
                            addObject.feeDictionary = new Dictionary<string, double?>();
                            addObject.feeDictionary.Add("Cap. Improv.", records[i].Cap_Improv__c);
                            addObject.feeDictionary.Add("CMSA", records[i].CMSA__c);
                            addObject.feeDictionary.Add("FNMA / 4260", records[i].FNMA_4260__c);
                            addObject.feeDictionary.Add("FNMA / 4261", records[i].FNMA_4261__c);
                            addObject.feeDictionary.Add("FNMA / 4262", records[i].FNMA_4262__c);
                            addObject.feeDictionary.Add("FNMA MF-MBA", records[i].FNMA_MF_MBA__c);
                            addObject.feeDictionary.Add("Freddie HC-MBA", records[i].Freddie_HC_MBA__c);
                            addObject.feeDictionary.Add("Freddie MF-MBA", records[i].Freddie_MF_MBA__c);
                            addObject.feeDictionary.Add("HUD REAC", records[i].HUD_REAC__c);
                            addObject.feeDictionary.Add("MBA", records[i].MBA__c);
                            addObject.feeDictionary.Add("MBA-2", records[i].MBA_2__c);
                            addObject.feeDictionary.Add("No Contact", records[i].No_Contact__c);
                            addObject.feeDictionary.Add("Rush", records[i].Inspector_Rush__c);
                            addObject.feeDictionary.Add("CMSA-2", records[i].CMSA_2__c);
                            addObject.feeDictionary.Add("Exterior 1", records[i].Exterior_1__c);
                            addObject.feeDictionary.Add("FNMA HC-MBA", records[i].FNMA_HC_MBA__c);
                            addObject.feeDictionary.Add("Exterior 2", records[i].Exterior_2__c);
                            addObject.feeDictionary.Add("CME - HC", records[i].CME_HC__c);
                            addObject.feeDictionary.Add("CME - MF", records[i].CME_MF__c);
                            String Json2 = "";
                            
                            /*using (var client1 = new HttpClient())
                            {
                                String restRequest = ("http://www.mapquestapi.com/geocoding/v1/address?key="
                                    + mapKey + "&location=" + addObject.ShippingPostalCode);
                                var request = new HttpRequestMessage(HttpMethod.Get, restRequest);
                                request.Headers.Add("X-PrettyPrint", "1");
                                var response = client1.SendAsync(request).Result;
                                Json2 = response.Content.ReadAsStringAsync().Result;
                            }*/
                            //LatLngClass.RootObject coordinates = new LatLngClass.RootObject();
                            //coordinates = JsonConvert.DeserializeObject<LatLngClass.RootObject>(Json2);
                            //var array = coordinates.results.ToArray();
                            //var array2 = array[0].locations.ToArray();
                            if (records[i].ShippingLongitude != null & records[i].ShippingLatitude != null)
                            {
                                addObject.latitude = records[i].ShippingLatitude;
                                addObject.longitute = records[i].ShippingLongitude;
                            }
                            else
                            {
                                using (var client1 = new HttpClient())
                                {
                                    String restRequest = ("http://www.mapquestapi.com/geocoding/v1/address?key="
                                        + mapKey + "&location=" + addObject.ShippingPostalCode);
                                    var request = new HttpRequestMessage(HttpMethod.Get, restRequest);
                                    request.Headers.Add("X-PrettyPrint", "1");
                                    var response = client1.SendAsync(request).Result;
                                    Json2 = response.Content.ReadAsStringAsync().Result;
                                }
                                LatLngClass.RootObject coordinates = new LatLngClass.RootObject();
                                coordinates = JsonConvert.DeserializeObject<LatLngClass.RootObject>(Json2);
                                var array = coordinates.results.ToArray();
                                var array2 = array[0].locations.ToArray();
                                addObject.latitude = array2[0].displayLatLng.lat;
                                addObject.longitute = array2[0].displayLatLng.lng;
                                UpdateCoordinatesClass update5 = new UpdateCoordinatesClass();
                                update5.ShippingLatitude = addObject.latitude;
                                update5.ShippingLongitude = addObject.longitute;
                                client.Update("Account", addObject.accountId, update5);
                            }
                            OfficialInspectorList.Add(addObject);
                        }
                        catch (System.FormatException e)
                        {
                            missedSeconds.Add(i);
                        }
                    }
                    progress.Report("Records Processed: " + i + " of " + records.Count);
                
                
                }
                //saves database to .bin file
                string saveFile;
                if (!HUDmode)
                {
                    saveFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\S2Inspections", "nonhudinspectors.bin");
                }
                else{
                    saveFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\S2Inspections", "hudinspectors.bin");
                }
                    using (Stream stream = File.Open(saveFile, FileMode.Create))
                {
                    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    formatter.Serialize(stream, OfficialInspectorList);
                }
            }
        }

        private void Back_ten_Click(object sender, RoutedEventArgs e)
        {
            if (currentten == 0)
            {

            }
            else
            {
                currentten = currentten - 1;
                int modifier = currentten * 10;
                ten_list_box.Visibility = Visibility.Collapsed;
                Page_count.Visibility = Visibility.Collapsed;
                tenInspectors = new List<String>();
                for (int j = 0; j < 10; j++)
                {
                    int i = j + modifier;
                    String fnmaString = "No";
                    String fmacString = "No";
                    if (workingList[i].FNMA_Certified__c == true)
                    {
                        fnmaString = "Yes";
                    }
                    if (workingList[i].Freddie_Mac_Certified__c == true)
                    {
                        fmacString = "Yes";
                    }

                    String newString = createTenString(i, fnmaString, fmacString);

                    tenInspectors.Add(newString);
                }
                Page_count.Text = ("" + (currentten + 1));
                ten_list_box.ItemsSource = null;
                object top = tenInspectors[0];
                ten_list_box.ItemsSource = tenInspectors;
                ten_list_box.ScrollIntoView(top);
                ten_list_box.Visibility = Visibility.Visible;
                Page_count.Visibility = Visibility.Visible;
            }
        }

        private void Next_ten_Click(object sender, RoutedEventArgs e)
        {
            currentten = currentten + 1;
            Back_ten.Visibility = Visibility.Visible;
            int modifier = currentten * 10;
            ten_list_box.Visibility = Visibility.Collapsed;
            Page_count.Visibility = Visibility.Collapsed;
            tenInspectors = new List<String>();
            for (int j = 0; j < 10; j++)
            {
                int i = j + modifier;
                String fnmaString = "No";
                String fmacString = "No";
                if (workingList[i].FNMA_Certified__c == true)
                {
                    fnmaString = "Yes";
                }
                if (workingList[i].Freddie_Mac_Certified__c == true)
                {
                    fmacString = "Yes";
                }

                String newString = createTenString(i, fnmaString, fmacString);
                tenInspectors.Add(newString);
            }
            Page_count.Text = ("" + (currentten + 1));
            ten_list_box.ItemsSource = null;
            object top = tenInspectors[0];
            ten_list_box.ItemsSource = tenInspectors;
            ten_list_box.ScrollIntoView(top);
            ten_list_box.Visibility = Visibility.Visible;
            Page_count.Visibility = Visibility.Visible;
        }

        private void Assign_List(object sender, RoutedEventArgs e)
        {
            orderNumberSearch = (e.Source as Button).Content.ToString().Substring(0, 6);
            SearchResults();
        }

        private void Change_Date_Click(object sender, RoutedEventArgs e)
        {
            Results_Page.Visibility = Visibility.Collapsed;
            NewDateBox.Text = currentInspection.Inspector_Due_Date__c;
            Change_Date_Page.Visibility = Visibility.Visible;
            NewDateBox.Focus();
        }

        private void SearchResults()
        {
            currentten = 0;
            if (currentInspection != null)
            {
                if (orderNumberSearch == currentInspection.Name)
                {
                    reSearch = false;
                }
                else
                {
                    reSearch = true;
                }
            }
            else
            {
                reSearch = true;
            }
            Search_Page.Visibility = Visibility.Collapsed;

            Results_Page.Visibility = Visibility.Collapsed;
            if (reSearch)
            {
                currentInspection = findInspectionbyOrderNumber(orderNumberSearch, client);
            }
            if (currentInspection != null)
            {
                assignNames = new List<String>();
                sortByDistance(client);
                tenInspectors = new List<String>();
                for (int i = 0; i < 10; i++)
                {
                    String fnmaString;
                    String fmacString;
                    if (!HUDmode)
                    {
                        fnmaString = "No";
                        fmacString = "No";
                        if (workingList[i].FNMA_Certified__c == true)
                        {
                            fnmaString = "Yes";
                        }
                        if (workingList[i].Freddie_Mac_Certified__c == true)
                        {
                            fmacString = "Yes";
                        }
                    }
                    else
                    {
                        fnmaString = "No";
                        fmacString = "No";
                        if(workingList[i].HUD_Certified__c == true)
                        {
                            fmacString = "Yes";
                        }
                    }

                    String newString = createTenString(i, fnmaString, fmacString);
                    tenInspectors.Add(newString);
                }
                var history = client.Query<HistoryClass>("SELECT CreatedDate, Field, OldValue, NewValue From Inspection__History WHERE ParentId='" + currentInspection.Id + "' AND ((Field='Rep_ID_Inspector_history_tracking__c') OR (Field='Inspector_Rejected_Reason__c'))");
                List<HistoryClass> historyList = new List<HistoryClass>();
                for (int i = 0; i < history.Count; i++)
                {
                    if (history[i].Field.Equals("Rep_ID_Inspector_history_tracking__c") && history[i].NewValue.Equals("-") && history[i].OldValue != null)
                    {
                        historyList.Add(history[i]);
                    }
                    else if (history[i].Field.Equals("Inspector_Rejected_Reason__c") && history[i].OldValue == null)
                    {
                        historyList.Add(history[i]);
                    }

                }
                historyList.Sort((x, y) => x.CreatedDate.CompareTo(y.CreatedDate));
                List<String> ih = new List<String>();
                for (int i = (historyList.Count - 1); i >= 0; i--)
                {
                    String addstring = "";
                    if (historyList[i].Field.Equals("Rep_ID_Inspector_history_tracking__c"))
                    {
                        if (i == 0)
                        {
                            addstring = (historyList[i].CreatedDate.Month + "/" + historyList[i].CreatedDate.Day + ": "
                                + historyList[i].OldValue + ": No declined reason given.");
                            ih.Add(addstring);
                        }
                        else if (historyList[i - 1].Field.Equals("Inspector_Rejected_Reason__c"))
                        {
                            addstring = (historyList[i].CreatedDate.Month + "/" + historyList[i].CreatedDate.Day + ": "
                                + historyList[i].OldValue + ": " + historyList[i - 1].NewValue);
                            ih.Add(addstring);
                        }
                        else
                        {
                            addstring = (historyList[i].CreatedDate.Month + "/" + historyList[i].CreatedDate.Day + ": "
                                + historyList[i].OldValue + ": No declined reason given.");
                            ih.Add(addstring);
                        }
                    }
                }
                assignNames = sortAssignQueue();
                Region.Text = currentInspection.Region__c;
                Assign_queue.Visibility = Visibility.Collapsed;
                Assign_queue.ItemsSource = null;
                Assign_queue.ItemsSource = assignNames;
                History_List.ItemsSource = null;
                History_List.ItemsSource = ih;
                ten_list_box.ItemsSource = null;
                object top = tenInspectors[0];
                object atop = assignNames[0];
                ten_list_box.ItemsSource = tenInspectors;
                ten_list_box.ScrollIntoView(top);
                Order_number.Text = currentInspection.Name;
                Form_Name.Text = ("ADHOC: " + currentInspection.ADHOC__c);
                City_Name.Text = (currentInspection.City__c + ", " + currentInspection.State__c);
                inspectionHistoryHeight = Canvas1.ActualHeight - 122.0 - 50.0;
                History_List.Height = inspectionHistoryHeight;
                Assign_queue.Height = inspectionHistoryHeight;
                Search_Box2.Text = "";
                Assigned_Rep.Text = ("Assigned to: " + currentInspection.Inspector_Rep_ID__c);
                Inspector_due.Text = ("Inspector Due Date: " + currentInspection.Inspector_Due_Date__c.Substring(5, 5));
                Client_due.Text = ("Client Due Date: " + currentInspection.Client_Due_Date__c.Substring(5, 5));
                Page_count.Text = ("" + (currentten + 1));
                FormClass newForm = new FormClass();
                newForm = client.FindById<FormClass>("Form__c", currentInspection.Form_Name__c);
                Form_name.Text = ("Form: " + newForm.Name);
                if (currentInspection.Inspection_Folder__c != null)
                {
                    Folder_text.Text = "Folder: Yes";
                }
                else
                {
                    Folder_text.Text = "Folder: No";
                }

                Assign_queue.Visibility = Visibility.Visible;
                Results_Page.Visibility = Visibility.Visible;
            }
            else
            {
                First_Search_Box1.Text = "";
                Search_Page.Visibility = Visibility.Visible;
                First_Search_Box1.Focus();

            }
        }

        private void Change_date_button_Click(object sender, RoutedEventArgs e)
        {
            UpdateDateClass newDate = new UpdateDateClass();
            newDate.Inspector_Due_Date__c = NewDateBox.Text;
            client.Update("Inspection__c", currentInspection.Id, newDate);
            currentInspection.Inspector_Due_Date__c = newDate.Inspector_Due_Date__c;
            Change_Date_Page.Visibility = Visibility.Collapsed;
            orderNumberSearch = currentInspection.Name;
            SearchResults();
        }

        private void NewDateBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Change_date_button_Click(this, new RoutedEventArgs());
            }
        }

        private void Fees_Button_Click(object sender, RoutedEventArgs e)
        {
            Results_Page.Visibility = Visibility.Collapsed;
            remote_box.Text = "";
            fees_order.Text = currentInspection.Name;
            client_fees.Text = ("Client Fee: $" + currentInspection.Client_Fees__c.ToString());

            fees_type.Text = ("Fee Type: " + currentInspection.Fee_Type__c);
            if (currentInspection.Inspector__c == null)
            {
                remote_box.Visibility = Visibility.Collapsed;
                fees_submit.Visibility = Visibility.Collapsed;
                remote_prompt.Text = "Please assign before adding remote fees!";
                inspector_fees.Text = "Inspector Fee:";
                fees_assigned.Text = "Assigned to:";
                remote_fees.Text = "Other Fees:";
                Fees_page.Visibility = Visibility.Visible;
            }
            else
            {
                bool found = false;
                int index = 0;
                OfficialInspectorClass currentInspector = new OfficialInspectorClass();
                while (!found)
                {
                    if (currentInspection.Inspector__c == workingList[index].contactID)
                    {
                        currentInspector = workingList[index];
                        found = true;
                    }
                    else
                    {
                        index++;
                    }

                }
                inspector_fees.Text = ("Inspector Fee: $" + currentInspector.feeDictionary[currentInspection.Fee_Type__c].ToString());
                if (currentInspection.Queue__c == "Assign" || currentInspection.Queue__c == "Accept")
                {
                    remote_fees.Text = ("Other Fees: $" + (currentInspection.Inspector_Fees__c).ToString());
                }
                else
                {
                    remote_fees.Text = ("Other Fees: $" + (currentInspection.Inspector_Fees__c - currentInspector.feeDictionary[currentInspection.Fee_Type__c]).ToString());
                }
                fees_assigned.Text = ("Assigned to: " + currentInspector.Name);
                remote_prompt.Text = "Please enter a remote fee to add:";
                remote_box.Visibility = Visibility.Visible;
                fees_submit.Visibility = Visibility.Visible;
                Fees_page.Visibility = Visibility.Visible;

            }
        }

        private void Cancel_date_button_Click(object sender, RoutedEventArgs e)
        {
            Change_Date_Page.Visibility = Visibility.Collapsed;
            orderNumberSearch = currentInspection.Name;
            SearchResults();
        }

        private void Change_Adhoc_Click(object sender, RoutedEventArgs e)
        {
            Results_Page.Visibility = Visibility.Collapsed;
            newAdhocBox.Text = currentInspection.ADHOC__c;
            Change_Adhoc_page.Visibility = Visibility.Visible;
        }

        private void Save_adhoc_button_Click(object sender, RoutedEventArgs e)
        {
            UpdateAdhocClass updateAdhoc = new UpdateAdhocClass();
            updateAdhoc.ADHOC__c = newAdhocBox.Text;
            client.Update("Inspection__c", currentInspection.Id, updateAdhoc);
            orderNumberSearch = currentInspection.Name;
            Change_Adhoc_page.Visibility = Visibility.Collapsed;
            currentInspection.ADHOC__c = updateAdhoc.ADHOC__c;
            SearchResults();
        }

        private void Cancel_ADHOC_Click(object sender, RoutedEventArgs e)
        {
            Change_Adhoc_page.Visibility = Visibility.Collapsed;
            orderNumberSearch = currentInspection.Name;
            SearchResults();
        }

        private void fees_return_Click(object sender, RoutedEventArgs e)
        {
            orderNumberSearch = currentInspection.Name;
            Fees_page.Visibility = Visibility.Collapsed;
            SearchResults();
        }

        private void fees_submit_Click(object sender, RoutedEventArgs e)
        {
            if (remote_box.Text != "")
            {
                double feeamount;
                try
                {
                    feeamount = Convert.ToDouble(remote_box.Text);
                    bool found = false;
                    OfficialInspectorClass inspector = new OfficialInspectorClass();
                    int index = 0;
                    while (!found)
                    {
                        if (workingList[index].contactID == currentInspection.Inspector__c)
                        {
                            found = true;
                            inspector = workingList[index];
                        }
                        else
                        {
                            index++;
                        }
                    }
                    NewFeeClass newFee = new NewFeeClass();
                    newFee.Account__c = inspector.accountId;
                    newFee.Date__c = currentDate;
                    newFee.Fee_Type__c = "Remote Fee";
                    newFee.Inspection__c = currentInspection.Id;
                    newFee.Fee__c = feeamount;
                    client.Create("Inspection_Fee__c", newFee);
                    currentInspection.Inspector_Fees__c += feeamount;
                    orderNumberSearch = currentInspection.Name;
                    Fees_page.Visibility = Visibility.Collapsed;
                    SearchResults();
                }
                catch (System.FormatException)
                {
                    remote_prompt.Text = "That's not a valid number, please try again.";
                }
            }
            else
            {
                remote_prompt.Text = "Please enter a number:";
            }
        }
        private String createTenString(int i, String fnmaString, String fmacString)
        {
            String newtenString;
            if (!HUDmode)
            {
                newtenString = (workingList[i].Rep_ID__c + " " + workingList[i].Name + ":      " + Math.Round(workingList[i].currentDistance, 2) + " Miles" + "      Freddie: " + fmacString + "\n"
                    + "Grade: " + workingList[i].Inspector_Ranking__c + "      Assigned Inspections: " + workingList[i].assignedInspections + "      FNMA: " + fnmaString + "\n"
                    + "Status: " + workingList[i].Status__c + "      Fee: $" + workingList[i].feeDictionary[currentInspection.Fee_Type__c]
                    + "\nPhone: " + workingList[i].Phone
                    + "\nComments: " + workingList[i].Comments__c + "\n");
            }
            else
            {
                newtenString = newtenString = (workingList[i].Rep_ID__c + " " + workingList[i].Name + ":      " + Math.Round(workingList[i].currentDistance, 2) + " Miles" + "      HUD: " + fmacString + "\n"
                    + "Grade: " + workingList[i].Inspector_Ranking__c + "      Assigned Inspections: " + workingList[i].assignedInspections + "\n"
                    + "Status: " + workingList[i].Status__c + "      Fee: $" + workingList[i].feeDictionary[currentInspection.Fee_Type__c]
                    + "\nPhone: " + workingList[i].Phone
                    + "\nComments: " + workingList[i].Comments__c + "\n");
            }
            return newtenString;
        }

        private void sort_button_Click(object sender, RoutedEventArgs e)
        {
            if(sort_box.SelectedIndex == -1)
            {

            }
            else
            {
                SortNumber = sort_box.SelectedIndex;
                assignNames = sortAssignQueue();
                Assign_queue.Visibility = Visibility.Collapsed;
                Assign_queue.ItemsSource = null;
                Assign_queue.ItemsSource = assignNames;
                var atop = assignNames[0];
                Assign_queue.ScrollIntoView(atop);
                Assign_queue.Visibility = Visibility.Visible;
            }
        }

        private List<String> sortAssignQueue()
        {
            List<String> queue = new List<String>();
            List<InspectionListItem> worker = new List<InspectionListItem>();
            var iassign = client.Query<InspectionListItem>("SELECT Name, Fee_Type__c, Inspection_Folder__c, ADHOC__c, Region__c, Inspector__c From Inspection__c WHERE Queue__c='Assign'");
            for (int i = 0; i < iassign.Count; i++)
            {
                if (!HUDmode)
                {
                    //InspectionJSONClass checkI = findInspectionbyOrderNumber(iassign[i].Name, client);
                    if (iassign[i].Inspection_Folder__c == null || iassign[i].ADHOC__c.Contains("HUD") || iassign[i].ADHOC__c.Contains("OCI - Hold"))
                    {
                        //Console.WriteLine("This won't be added");
                    }
                    else
                    {

                        if (iassign[i].Inspector__c != null)
                        {
                            UpdateInspectorClass updateAssignqueue = new UpdateInspectorClass();
                            updateAssignqueue.Inspector__c = null;
                            InspectionJSONClass forupdate = findInspectionbyOrderNumber(iassign[i].Name, client);
                            bool hello1 = client.Update("Inspection__c", forupdate.Id, updateAssignqueue);
                            Console.WriteLine(hello1);
                        }
                        if (iassign[i].ADHOC__c == null)
                        {
                            iassign[i].ADHOC__c = "";
                        }

                        worker.Add(iassign[i]);
                    }
                }
                else
                {
                    if (iassign[i].Inspection_Folder__c != null || !iassign[i].ADHOC__c.Contains("HUD") || iassign[i].ADHOC__c == "OCI - Hold")
                    {
                        //Console.WriteLine("This won't be added");
                    }
                    else
                    {

                        if (iassign[i].Inspector__c != null)
                        {
                            UpdateInspectorClass updateAssignqueue = new UpdateInspectorClass();
                            updateAssignqueue.Inspector__c = null;
                            InspectionJSONClass forupdate = findInspectionbyOrderNumber(iassign[i].Name, client);
                            bool hello1 = client.Update("Inspection__c", forupdate.Id, updateAssignqueue);
                            Console.WriteLine(hello1);
                        }
                        if (iassign[i].ADHOC__c == null)
                        {
                            iassign[i].ADHOC__c = "";
                        }

                        worker.Add(iassign[i]);
                    }
                }
            }
            if (SortNumber == 0)
            {
                worker.Sort((x, y) => x.Name.CompareTo(y.Name));
            }
            else if (SortNumber == 1)
            {
                worker.Sort((x, y) => x.Region__c.CompareTo(y.Region__c));
            }
            else if (SortNumber == 2)
            {
                worker.Sort((x, y) => x.ADHOC__c.CompareTo(y.ADHOC__c));
            }
            else if (SortNumber == 3)
            {
                for (int i = 0; i < worker.Count; i++)
                {
                    if (worker[i].Fee_Type__c == "FNMA HC-MBA" || worker[i].Fee_Type__c == "FNMA MF-MBA")
                    {
                        InspectionListItem temp = worker[i];
                        worker.Remove(worker[i]);
                        worker.Insert(0, temp);
                    }
                }
            }
            else if (SortNumber == 4)
            {
                for (int i = 0; i < worker.Count; i++)
                {
                    if (worker[i].Fee_Type__c == "Freddie MF-MBA" || worker[i].Fee_Type__c == "Freddie HC-MBA")
                    {
                        InspectionListItem temp = worker[i];
                        worker.Remove(worker[i]);
                        worker.Insert(0, temp);
                    }
                }
            }
            for(int i = 0; i < worker.Count; i++)
            {
                queue.Add(worker[i].Name + "\nADHOC: " + worker[i].ADHOC__c + "\nRegion: " + worker[i].Region__c);
            }
            return queue;
        }

        private void newAdhocBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Save_adhoc_button_Click(this, new RoutedEventArgs());
            }
        }

        private void Non_Hud_button_Click(object sender, RoutedEventArgs e)
        {
            HUDmode = false;
            mode_page.Visibility = Visibility.Collapsed;
            Rebuild_page.Visibility = Visibility.Visible;
        }

        private void Hud_button_Click(object sender, RoutedEventArgs e)
        {
            HUDmode = true;
            mode_page.Visibility = Visibility.Collapsed;
            Rebuild_page.Visibility = Visibility.Visible;
        }
    }//nothing goes below here
}
