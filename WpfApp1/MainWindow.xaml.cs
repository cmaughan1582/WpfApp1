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
    HUD only assigning option DONE
    Include sort option for middle list DONE
    Multiple Assign DONE? Maybe add "assigning records, 1 of x done page like the database thing"
    ask jed if he still wants the multiple assign option even though it doesn't save any time
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
            //all of this stuff is hopefully going to be used in the future to create a re-enter credentials screen for
            //when the password is changed.
            if (!DirectoryExists)
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\S2Inspections");
            }
            login_page.Visibility = Visibility.Visible;

        }

        //LOGIN PAGE FUNCTIONS
        //this function is for the initial login button
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            
            string saveFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\S2Inspections", "logininfo.bin");
            if (File.Exists(saveFile))
            {
                using (Stream stream = File.Open(saveFile, FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    credentials = (Dictionary<String, String>)bformatter.Deserialize(stream);
                }
                login_page.Visibility = Visibility.Collapsed;
                var authflow = new UsernamePasswordAuthenticationFlow(clientID, consumerSecret, credentials["username"], credentials["password"] + credentials["securityToken"]);
                try
                {
                    client.Authenticate(authflow);
                    mode_page.Visibility = Visibility.Visible;

                }
                catch (SalesforceException ex)
                {
                    Login_Prompt.Text = "Those Credentials didn't work, please enter new info.";
                }
            }
            else
            {
                Login_Prompt.Text = "No Saved Login info found, please enter new info.";
            }
        }

        //MODE PAGE FUNCTIONS
        //this is to set the mode to NON HUD for the program, second page after login
        private void Non_Hud_button_Click(object sender, RoutedEventArgs e)
        {
            HUDmode = false;
            mode_page.Visibility = Visibility.Collapsed;
            Rebuild_page.Visibility = Visibility.Visible;
        }
        //this is to set the mode to HUD for the program, second page after login
        private void Hud_button_Click(object sender, RoutedEventArgs e)
        {
            HUDmode = true;
            mode_page.Visibility = Visibility.Collapsed;
            Rebuild_page.Visibility = Visibility.Visible;
        }

        //DATABASE PAGE FUNCTIONS
        //this button is for the load button on the rebuild database page
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Rebuild_page.Visibility = Visibility.Collapsed;
            workingList = LoadSavedDatabase(HUDmode);
            Search_Page.Visibility = Visibility.Visible;
            First_Search_Box1.Focus();

        }
        //this function loads a database already saved on the computer, used with the load button function
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
        //this function is for the rebuild database button
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

        //SEARCH PAGE AND SEARCH FUNCTIONS
        //this function finds an inspection by the order number and returns that inspection. Used by search results to store
        //current inspection variable
        //This is the function for the search page after you rebuild the database
        private void First_Search_Button_Click(object sender, RoutedEventArgs e)
        {
            orderNumberSearch = (First_Search_Box1.Text);
            Database_Loaded.Visibility = Visibility.Collapsed;
            ReSearch_Text.Visibility = Visibility.Visible;
            SearchResults();
        }
        //this function links the enter key from the first search page box to the first search function
        private void First_Search_Box1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                First_Search_Button_Click(this, new RoutedEventArgs());
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
        //this function is the search function, creates a results page from an order number
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
                        if (workingList[i].HUD_Certified__c == true)
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

        //SEARCH HELPER FUNCTIONS
        //this function sorts the working list by distance to the current inspection...used during search results function
        private void sortByDistance(SalesforceClient client)
        {
            String currentInspectionAddress;
            if (!currentInspection.Street_Address__c.Contains(","))
            {
                String modifiedstreet = currentInspection.Street_Address__c.Replace('&', '-');
                currentInspectionAddress = (modifiedstreet + ", " + currentInspection.City__c + ", " + currentInspection.State__c);
            }
            else
            {
                currentInspectionAddress = (currentInspection.City__c + ", " + currentInspection.State__c);
            }
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

        //this button creates strings for the closest inspectors list box on the results page, returns a list
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

        //this is the function that does the actual sorting, used by search results and sort button functions
        //this function also builds a list of inspections in the assign queue, based on HUD or non HUD mode
        private List<String> sortAssignQueue()
        {
            List<String> queue = new List<String>();
            List<InspectionListItem> worker = new List<InspectionListItem>();
            var iassign = client.Query<InspectionListItem>("SELECT Name, Fee_Type__c, Inspection_Folder__c, ADHOC__c, Region__c, Inspector__c From Inspection__c WHERE Queue__c='Assign'");
            for (int i = 0; i < iassign.Count; i++)
            {
                if (iassign[i].ADHOC__c == null)
                {
                    iassign[i].ADHOC__c = "";
                }
                if (!HUDmode)
                {

                    if (iassign[i].Inspection_Folder__c == null || iassign[i].ADHOC__c.Contains("HUD") || iassign[i].ADHOC__c.Contains("OCI - Hold"))
                    {
                        //These inspections won't be added to the list
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
                        //These inspections won't be added to the list
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
            //sorts by name
            if (SortNumber == 0)
            {
                worker.Sort((x, y) => x.Name.CompareTo(y.Name));
            }
            //sorts by region EAST vs WEST
            else if (SortNumber == 1)
            {

                worker.Sort((x, y) => x.Name.CompareTo(y.Name));
                worker = MergeSort(worker, 1);
            }
            //Sorts by adhoc
            else if (SortNumber == 2)
            {
                worker.Sort((x, y) => x.Name.CompareTo(y.Name));
                worker = MergeSort(worker, 2);
            }
            //puts the fnma inspections on the top of the list
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
            //puts the freddie inspections on the top of the list
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
            //this finally puts the inspections into a string for the buttons in the middle
            for (int i = 0; i < worker.Count; i++)
            {
                queue.Add(worker[i].Name + "\nADHOC: " + worker[i].ADHOC__c + "\nRegion: " + worker[i].Region__c);
            }
            return queue;
        }

        //SORT HELPER FUNCTIONS
        //these two functions are a merge sort to keep the middle list in order after sort
        static private List<InspectionListItem> MergeSort(List<InspectionListItem> workingList, int mode)
        {
            List<InspectionListItem> returnList = new List<InspectionListItem>();
            if (workingList.Count == 1)
            {
                return workingList;
            }
            else
            {
                int mid = (workingList.Count / 2);

                List<InspectionListItem> left = new List<InspectionListItem>();
                List<InspectionListItem> right = new List<InspectionListItem>();
                for (int i = 0; i < workingList.Count; i++)
                {
                    if (i < mid)
                    {
                        left.Add(workingList[i]);
                    }
                    else
                    {
                        right.Add(workingList[i]);
                    }
                }
                left = MergeSort(left, mode);
                right = MergeSort(right, mode);
                returnList = RealSort(left, right, mode);
            }
            return returnList;
        }
        private static List<InspectionListItem> RealSort(List<InspectionListItem> left, List<InspectionListItem> right, int mode)
        {
            List<InspectionListItem> temp = new List<InspectionListItem>();
            int lindex = 0;
            int rindex = 0;
            while (lindex < left.Count && rindex < right.Count)
            {
                int modecompare;
                if (mode == 1)
                {
                    modecompare = String.Compare(left[lindex].Region__c, right[rindex].Region__c);
                }
                else if (mode == 2)
                {
                    modecompare = String.Compare(left[lindex].ADHOC__c, right[rindex].ADHOC__c);
                }
                else
                {
                    return temp;
                }
                if (modecompare == 1)
                {
                    temp.Add(right[rindex]);
                    rindex++;
                }
                else
                {
                    temp.Add(left[lindex]);
                    lindex++;
                }
            }
            while (lindex < left.Count)
            {
                temp.Add(left[lindex]);
                lindex++;
            }
            while (rindex < right.Count)
            {
                temp.Add(right[rindex]);
                rindex++;
            }
            return temp;
        }

        //RESULTS PAGE FUNCTIONS
        //this function is for the assign button on the results screen
        async private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            int index = ten_list_box.SelectedIndex;
            if (assignDict.Count == 0)
            {

            }
            else
            {
                //assigning part goes here
                //if i want to add progress screen, put this loop in a function in the class that has the build
                //database function, copy paste the button_clicked function to here and modify as needed!
                Results_Page.Visibility = Visibility.Collapsed;
                rebuild_progress.Visibility = Visibility.Visible;
                Building_database.Text = "Assigning Orders...";
                Records_text.Text = "Orders Assigned:";
                var progress = new Progress<string>(s => Records_text.Text = s);
                await Task.Factory.StartNew(() => SecondThreadConcern.Longwork2(progress, client, assignDict),
                                            TaskCreationOptions.LongRunning);
                rebuild_progress.Visibility = Visibility.Collapsed;
                Button_Click_1(this, new RoutedEventArgs());
                rebuild_progress.Visibility = Visibility.Collapsed;
                orderNumberSearch = (currentInspection.Name);
                currentInspection = new InspectionJSONClass();
                assignDict = new Dictionary<string, string>();
                SearchResults();

            }

        }

        //ASSIGNING HELPER FUNCTION
        //This function is for the new assigning idea
        private void ten_list_box_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ten_list_box.SelectedIndex == -1)
            {
                if (assignDict.ContainsKey(currentInspection.Id))
                {
                    assignDict.Remove(currentInspection.Id);
                }
            }
            else
            {
                if (assignDict.ContainsKey(currentInspection.Id))
                {
                    assignDict[currentInspection.Id] = workingList[ten_list_box.SelectedIndex].contactID;
                }
                else
                {
                    assignDict.Add(currentInspection.Id, workingList[ten_list_box.SelectedIndex].contactID);
                }
            }
        }

        //this function is for the search button on the results page
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            orderNumberSearch = (Search_Box2.Text);
            SearchResults();
        }

        //this function links the enter key for the search box on the results page to the function for the search button
        //on the results page
        private void Search_Box2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click_4(this, new RoutedEventArgs());
            }
        }

        //this function is for the back arrow button on the results page
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

        //this function is for the next arrow button on the results page
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

        //this button sorts the list based on which drop down option you choose
        private void sort_button_Click(object sender, RoutedEventArgs e)
        {
            //-1 means no option was chosen on the drop down box
            if (sort_box.SelectedIndex == -1)
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

        //this function is for when a button is clicked on the middle list, which lists the inspections in the assign queue
        private void Assign_List(object sender, RoutedEventArgs e)
        {
            orderNumberSearch = (e.Source as Button).Content.ToString().Substring(0, 6);
            SearchResults();
        }

        //this function is for the button to take you to the change date page from the results page
        private void Change_Date_Click(object sender, RoutedEventArgs e)
        {
            Results_Page.Visibility = Visibility.Collapsed;
            NewDateBox.Text = currentInspection.Inspector_Due_Date__c;
            Change_Date_Page.Visibility = Visibility.Visible;
            NewDateBox.Focus();
        }

        //this button changes the page from the results page to the view fees page
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
                remote_fees.Text = "Other Fees: $" + currentInspection.Inspector_Fees__c;
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

        //this function is for the button that moves you from the results page to the change adhoc page
        private void Change_Adhoc_Click(object sender, RoutedEventArgs e)
        {
            Results_Page.Visibility = Visibility.Collapsed;
            newAdhocBox.Text = currentInspection.ADHOC__c;
            Change_Adhoc_page.Visibility = Visibility.Visible;
        }

        //CHANGE DATE PAGE FUNCTIONS
        //this function is for the change date button on the change date page
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

        //this function links the enter button on the change date box to the change date button on the change date page
        private void NewDateBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Change_date_button_Click(this, new RoutedEventArgs());
            }
        }

        //this function is for the button that returns you to the results page from the change date page
        private void Cancel_date_button_Click(object sender, RoutedEventArgs e)
        {
            Change_Date_Page.Visibility = Visibility.Collapsed;
            orderNumberSearch = currentInspection.Name;
            SearchResults();
        }

        //CHANGE ADHOC PAGE FUNCTIONS
        //this button is the confirm button on the change adhoc page, saves the new adhoc
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

        //this function links the textbox to the change adhoc button on the change adhoc page
        private void newAdhocBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Save_adhoc_button_Click(this, new RoutedEventArgs());
            }
        }

        //this button returns you from the change adhoc page to the results page
        private void Cancel_ADHOC_Click(object sender, RoutedEventArgs e)
        {
            Change_Adhoc_page.Visibility = Visibility.Collapsed;
            orderNumberSearch = currentInspection.Name;
            SearchResults();
        }

        //VIEW FEES PAGE FUNCTIONS
        //the button returns you from the view fees page to the results page
        private void fees_return_Click(object sender, RoutedEventArgs e)
        {
            orderNumberSearch = currentInspection.Name;
            Fees_page.Visibility = Visibility.Collapsed;
            SearchResults();
        }
        //this button adds the remote fee to the inspection, on the view fees page
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

        //this function links the textbox with the remote fees to the submit button on the view fees page
        private void remote_box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                fees_submit_Click(this, new RoutedEventArgs());
            }
        }

        private void New_credentials_Click(object sender, RoutedEventArgs e)
        {
            login_page.Visibility = Visibility.Collapsed;
            credentials_text.Text = "Enter New Information Below:";
            PasswordBox.Text = "";
            UsernameBox.Text = "";
            SecurityTokenBox.Text = "";
            credentials_page.Visibility = Visibility.Visible;
        }

        private void Submit_credentials_Click(object sender, RoutedEventArgs e)
        {
            if (UsernameBox.Text == "" || PasswordBox.Text == "" || SecurityTokenBox.Text == "")
            {
                credentials_text.Text = "Please don't leave any boxes empty!";
            }
            else
            {
                credentials.Add("username", UsernameBox.Text);
                credentials.Add("password", PasswordBox.Text);
                credentials.Add("securityToken", SecurityTokenBox.Text);
                string saveFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\S2Inspections", "logininfo.bin");
                using (Stream stream = File.Open(saveFile, FileMode.Create))
                {
                    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    formatter.Serialize(stream, credentials);
                }
                credentials_page.Visibility = Visibility.Collapsed;
                Login_Prompt.Text = "Click the Button Below to Login";
                login_page.Visibility = Visibility.Visible;
            }
        }
        private void return_credentials_Click(object sender, RoutedEventArgs e)
        {
            credentials_page.Visibility = Visibility.Collapsed;
            Login_Prompt.Text = "Click the Button Below to Login";
            login_page.Visibility = Visibility.Visible;
        }
    }//nothing goes below here
}
