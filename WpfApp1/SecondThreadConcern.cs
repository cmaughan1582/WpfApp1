using GeoCoordinatePortable;
using Newtonsoft.Json;
using SalesforceSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    //this class is for functions that need to thread AKA progress functions
    class SecondThreadConcern
    {
        //this function builds a database with a separate thread, 
        public static void LongWork(IProgress<string> progress, SalesforceClient client, bool HUDmode)
        {
            const String mapKey = "On5gRfRDnoozDkk8zKjo5GpXGbvYCycm";
            //this is the query that finds all records and builds them into an ilist
            IList<NewInspectorClass> records;
            if (!HUDmode)
            {
                records = client.Query<NewInspectorClass>(
                    "SELECT Id, Name, BillingPostalCode, ShippingPostalCode, Rep_ID__C, Comments__c, HUD_Certified__c, FNMA_Certified__c, Freddie_Mac_Certified__c, Inspector_Ranking__c, Status__c, ShippingLatitude, ShippingLongitude, FNMA_4260__c, FNMA_4261__c, FNMA_4262__c, No_Contact__c, Inspector_Rush__c, CMSA_2__c, Exterior_1__c, FNMA_HC_MBA__c, Exterior_2__c, CME_HC__c, CME_MF__c, Freddie_MF_MBA__c, MBA__c, MBA_2__c, HUD_REAC__c, Freddie_HC_MBA__c, FNMA_MF_MBA__c, CMSA__c, Cap_Improv__c, Max_Insp_Count__c, Coverage_Area_Radius__c, Blacklist__c " +
                    "From Account " +
                    "WHERE Account_Inactive__c=false AND (HUD_Certified__c=false OR SicDesc!=null) AND Rep_ID__c!=null");
                //records.Add(client.FindById<NewInspectorClass>("Account", "0013700000W5oq8")); //this adds nc128 jessica jackson
                //records.Add(client.FindById<NewInspectorClass>("Account", "00137000009jaml"));
            }
            else
            {
                records = client.Query<NewInspectorClass>(
                    "SELECT Id, Name, BillingPostalCode, ShippingPostalCode, Rep_ID__C, Comments__c, HUD_Certified__c, FNMA_Certified__c, Freddie_Mac_Certified__c, Inspector_Ranking__c, Status__c, ShippingLatitude, ShippingLongitude, FNMA_4260__c, FNMA_4261__c, FNMA_4262__c, No_Contact__c, Inspector_Rush__c, CMSA_2__c, Exterior_1__c, FNMA_HC_MBA__c, Exterior_2__c, CME_HC__c, CME_MF__c, Freddie_MF_MBA__c, MBA__c, MBA_2__c, HUD_REAC__c, Freddie_HC_MBA__c, FNMA_MF_MBA__c, CMSA__c, Cap_Improv__c, Coverage_Area_Radius__c, Max_Insp_Count__c " +
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
                        addObject.Blacklist__c = records[i].Blacklist__c;
                        addObject.Coverage_Area_Radius__c = records[i].Coverage_Area_Radius__c;
                        addObject.Max_Insp_Count__c = records[i].Max_Insp_Count__c;
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
                    catch (Exception e)
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
            else
            {
                saveFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\S2Inspections", "hudinspectors.bin");
            }
            using (Stream stream = File.Open(saveFile, FileMode.Create))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(stream, OfficialInspectorList);
            }
        }
        public static void Longwork2(IProgress<string> progress, SalesforceClient client, Dictionary<string, string> assignDict)
        {
            int i = 0;
            foreach (KeyValuePair<string, string> entry in assignDict)
            {
                // do something with entry.Value or entry.Key
                String assignID = entry.Value;
                UpdateInspectorClass updateInspector = new UpdateInspectorClass();
                updateInspector.Inspector__c = assignID;
                client.Update("Inspection__c", entry.Key, updateInspector);
                i++;
                
                progress.Report("Orders Assigned: " + i + " of " + assignDict.Count);
            }
        }
        public static List<OfficialInspectorClass> Longwork3(IProgress<string> progress, SalesforceClient client, List<OfficialInspectorClass> workingList, List<String> autoqueue)
        {
            InspectionJSONClass currentInspection = new InspectionJSONClass();
            List<String> assignedarray = new List<String>();
            List<String> skippedarray = new List<String>();
            string inspectorAssign = "";
            string inspectorAssign1 = "";
            List<OfficialInspectorClass> tempList = new List<OfficialInspectorClass>();
            List<OfficialInspectorClass> templist2 = new List<OfficialInspectorClass>();
            //SortNumber = 0;
            //List<String> autoqueue = sortAssignQueue();
            //these two lines can test the logic for a specific order
            //List<String> autoqueue = new List<string>();
            //autoqueue.Add("163707");
            for (int i = 0; i < autoqueue.Count; i++)
            {
                tempList = new List<OfficialInspectorClass>();
                templist2 = new List<OfficialInspectorClass>();
                String searchnumber = autoqueue[i].Substring(0, 6);
                currentInspection = findInspectionbyOrderNumber(searchnumber, client);
                if(currentInspection.ADHOC__c == null)
                {
                    currentInspection.ADHOC__c = "";
                }
                if (currentInspection.Auto_Assign_Skip__c == false)
                {
                    List<string> historyList = new List<string>();
                    inspectorAssign = "";
                    inspectorAssign1 = "";

                    workingList = sortByDistance(client, workingList, currentInspection);
                    for (int j = 0; j < workingList.Count; j++)
                    {
                        if (workingList[j].currentDistance <= Convert.ToInt32(workingList[j].Coverage_Area_Radius__c) && workingList[j].assignedInspections < workingList[j].Max_Insp_Count__c)
                        {
                            tempList.Add(workingList[j]);
                        }
                    }
                    //Console.WriteLine(tempList.Count);
                    //Console.ReadLine();
                    var history = client.Query<HistoryClass>("SELECT CreatedDate, Field, OldValue, NewValue From Inspection__History WHERE ParentId='" + currentInspection.Id + "' AND Field='Rep_ID_Inspector_history_tracking__c'");
                    for (int j = 0; j < history.Count; j++)
                    {
                        if (history[j].NewValue.Equals("-") && history[j].OldValue != null)
                        {
                            historyList.Add(history[j].OldValue);
                        }
                    }
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        String compareString = tempList[j].Rep_ID__c + " - " + tempList[j].Name;
                        if (tempList[j].Blacklist__c == null)
                        {
                            tempList[j].Blacklist__c = "";
                        }
                        if (!historyList.Contains(compareString) && tempList[j].Status__c != "On Hold" && tempList[j].feeDictionary[currentInspection.Fee_Type_Text__c] != null && !tempList[j].Blacklist__c.Contains(currentInspection.Division__c))
                        {
                            templist2.Add(tempList[j]);
                        }
                    }
                    tempList = templist2;
                    templist2 = new List<OfficialInspectorClass>();
                    if (tempList.Count > 0)
                    {
                        if (tempList.Count > 1)
                        {
                            for (int j = 0; j < tempList.Count; j++)
                            {
                                if (tempList[j].Status__c == "New Rep")
                                {
                                    templist2.Add(tempList[j]);
                                }
                            }
                            if (templist2.Count == 0)
                            {
                                //TOP REP LOGIC GOES HERE!!!
                                templist2 = new List<OfficialInspectorClass>();
                                for (int j = 0; j < tempList.Count; j++)
                                {
                                    if (tempList[j].Status__c == "Top Rep")
                                    {
                                        templist2.Add(tempList[j]);
                                    }
                                }
                                if (templist2.Count == 0)
                                {
                                    tempList.Sort((x, y) => y.Inspector_Ranking__c.CompareTo(x.Inspector_Ranking__c));
                                    if (tempList[0].Inspector_Ranking__c == tempList[1].Inspector_Ranking__c)
                                    {
                                        if (tempList[0].feeDictionary[currentInspection.Fee_Type_Text__c] < tempList[1].feeDictionary[currentInspection.Fee_Type_Text__c])
                                        {
                                            inspectorAssign1 = tempList[0].contactID;
                                            inspectorAssign = tempList[0].Name;
                                            updateInspectorCount(tempList[0].contactID, workingList);
                                        }
                                        else if (tempList[0].feeDictionary[currentInspection.Fee_Type_Text__c] > tempList[1].feeDictionary[currentInspection.Fee_Type_Text__c])
                                        {
                                            inspectorAssign1 = tempList[1].contactID;
                                            inspectorAssign = tempList[1].Name;
                                            updateInspectorCount(tempList[1].contactID,workingList);
                                        }
                                        else
                                        {
                                            if (tempList[0].currentDistance < tempList[1].currentDistance)
                                            {
                                                inspectorAssign1 = tempList[0].contactID;
                                                inspectorAssign = tempList[0].Name;
                                                updateInspectorCount(tempList[0].contactID,workingList);
                                            }
                                            else if (tempList[0].currentDistance > tempList[1].currentDistance)
                                            {
                                                inspectorAssign1 = tempList[1].contactID;
                                                inspectorAssign = tempList[1].Name;
                                                updateInspectorCount(tempList[1].contactID, workingList);
                                            }
                                            else
                                            {
                                                inspectorAssign1 = tempList[0].contactID;
                                                inspectorAssign = tempList[0].Name;
                                                updateInspectorCount(tempList[0].contactID, workingList);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        inspectorAssign1 = tempList[0].contactID;
                                        inspectorAssign = tempList[0].Name;
                                        updateInspectorCount(tempList[0].contactID, workingList);
                                    }
                                }
                                else if (templist2.Count == 1)
                                {
                                    inspectorAssign1 = templist2[0].contactID;
                                    inspectorAssign = templist2[0].Name;
                                    updateInspectorCount(templist2[0].contactID, workingList);
                                }
                                else
                                {
                                    tempList = templist2;
                                    tempList.Sort((x, y) => x.Inspector_Ranking__c.CompareTo(y.Inspector_Ranking__c));
                                    if (tempList[0].Inspector_Ranking__c == tempList[1].Inspector_Ranking__c)
                                    {
                                        if (tempList[0].feeDictionary[currentInspection.Fee_Type_Text__c] < tempList[1].feeDictionary[currentInspection.Fee_Type_Text__c])
                                        {
                                            inspectorAssign1 = tempList[0].contactID;
                                            inspectorAssign = tempList[0].Name;
                                            updateInspectorCount(tempList[0].contactID, workingList);
                                        }
                                        else if (tempList[0].feeDictionary[currentInspection.Fee_Type_Text__c] > tempList[1].feeDictionary[currentInspection.Fee_Type_Text__c])
                                        {
                                            inspectorAssign1 = tempList[1].contactID;
                                            inspectorAssign = tempList[1].Name;
                                            updateInspectorCount(tempList[1].contactID, workingList);
                                        }
                                        else
                                        {
                                            if (tempList[0].currentDistance < tempList[1].currentDistance)
                                            {
                                                inspectorAssign1 = tempList[0].contactID;
                                                inspectorAssign = tempList[0].Name;
                                                updateInspectorCount(tempList[0].contactID, workingList);
                                            }
                                            else if (tempList[0].currentDistance > tempList[1].currentDistance)
                                            {
                                                inspectorAssign1 = tempList[1].contactID;
                                                inspectorAssign = tempList[1].Name;
                                                updateInspectorCount(tempList[1].contactID, workingList);
                                            }
                                            else
                                            {
                                                inspectorAssign1 = tempList[0].contactID;
                                                inspectorAssign = tempList[0].Name;
                                                updateInspectorCount(tempList[0].contactID, workingList);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        inspectorAssign1 = tempList[0].contactID;
                                        inspectorAssign = tempList[0].Name;
                                        updateInspectorCount(tempList[0].contactID, workingList);
                                    }
                                }
                                

                            }
                            else if (templist2.Count == 1)
                            {
                                inspectorAssign1 = templist2[0].contactID;
                                inspectorAssign = templist2[0].Name;
                                updateInspectorCount(templist2[0].contactID, workingList);
                            }
                            else
                            {
                                //NEW REP MULTIPLE GOES HERE
                                tempList = templist2;
                                tempList.Sort((x, y) => x.Inspector_Ranking__c.CompareTo(y.Inspector_Ranking__c));
                                if (tempList[0].Inspector_Ranking__c == tempList[1].Inspector_Ranking__c)
                                {
                                    if (tempList[0].feeDictionary[currentInspection.Fee_Type_Text__c] < tempList[1].feeDictionary[currentInspection.Fee_Type_Text__c])
                                    {
                                        inspectorAssign1 = tempList[0].contactID;
                                        inspectorAssign = tempList[0].Name;
                                        updateInspectorCount(tempList[0].contactID, workingList);
                                    }
                                    else if (tempList[0].feeDictionary[currentInspection.Fee_Type_Text__c] > tempList[1].feeDictionary[currentInspection.Fee_Type_Text__c])
                                    {
                                        inspectorAssign1 = tempList[1].contactID;
                                        inspectorAssign = tempList[1].Name;
                                        updateInspectorCount(tempList[1].contactID, workingList);
                                    }
                                    else
                                    {
                                        if (tempList[0].currentDistance < tempList[1].currentDistance)
                                        {
                                            inspectorAssign1 = tempList[0].contactID;
                                            inspectorAssign = tempList[0].Name;
                                            updateInspectorCount(tempList[0].contactID, workingList);
                                        }
                                        else if (tempList[0].currentDistance > tempList[1].currentDistance)
                                        {
                                            inspectorAssign1 = tempList[1].contactID;
                                            inspectorAssign = tempList[1].Name;
                                            updateInspectorCount(tempList[1].contactID, workingList);
                                        }
                                        else
                                        {
                                            inspectorAssign1 = tempList[0].contactID;
                                            inspectorAssign = tempList[0].Name;
                                            updateInspectorCount(tempList[0].contactID, workingList);
                                        }
                                    }
                                }
                                else
                                {
                                    inspectorAssign1 = tempList[0].contactID;
                                    inspectorAssign = tempList[0].Name;
                                    updateInspectorCount(tempList[0].contactID, workingList);
                                }

                            }
                        }
                        else
                        {
                            inspectorAssign1 = tempList[0].contactID;
                            inspectorAssign = tempList[0].Name;
                            updateInspectorCount(tempList[0].contactID, workingList);
                        }
                        UpdateInspectorClass updateInspector = new UpdateInspectorClass();
                        updateInspector.Inspector__c = inspectorAssign1;
                        assignedarray.Add((currentInspection.Name + ": " + inspectorAssign));
                        //updateInspectorCount(tempList[0].contactID);
                        client.Update("Inspection__c", currentInspection.Id, updateInspector);
                    }
                    else
                    {
                        skippedarray.Add((currentInspection.Name + ": Skipped"));
                        UpdateAdhocClass repad = new UpdateAdhocClass();
                        if(currentInspection.ADHOC__c == null)
                        {
                            currentInspection.ADHOC__c = "";
                        }
                        if(!currentInspection.ADHOC__c.Contains("Rep Needed"))
                        {
                            repad.ADHOC__c = "Rep Needed " + currentInspection.ADHOC__c;
                            client.Update("Inspection__c", currentInspection.Id, repad);
                        }
                        //currentInspection.ADHOC__c = "Rep Needed " + currentInspection.ADHOC__c; this code isn't even correct, make sure to fix
                    }
                }
                //put progress here, end of for loop
                progress.Report("Orders Assigned: " + i + " of " + autoqueue.Count);
            }
            System.IO.File.WriteAllLines(@"C:\Users\Public\S2 Inspections\Assigned.txt", assignedarray);
            System.IO.File.WriteAllLines(@"C:\Users\Public\S2 Inspections\Skipped.txt", skippedarray);
            return workingList;
        }
        public static void updateInspectorCount(string contactID, List<OfficialInspectorClass> workingList)
        {
            for (int i = 0; i < workingList.Count; i++)
            {
                if (workingList[i].contactID == contactID)
                {
                    if (workingList[i].assignedInspections == null)
                    {
                        workingList[i].assignedInspections = 1;
                    }
                    else
                    {
                        workingList[i].assignedInspections++;
                    }
                }
            }
        }
        private static List<OfficialInspectorClass> sortByDistance(SalesforceClient client, List<OfficialInspectorClass> workingList, InspectionJSONClass currentInspection)
        {
            String mapKey = "On5gRfRDnoozDkk8zKjo5GpXGbvYCycm";
            GeoCoordinate currentGeopoint;
            if (currentInspection.Property_Latitude__c == null)
            {
                String currentInspectionAddress;
                String JsonReturn = ("");
                LatLngClass.RootObject coordinates = new LatLngClass.RootObject();
                //var array;
                if (!currentInspection.Street_Address__c.Contains(","))
                {
                    String modifiedstreet = currentInspection.Street_Address__c.Replace('&', '-');
                    currentInspectionAddress = (modifiedstreet + ", " + currentInspection.City__c + ", " + currentInspection.State__c);
                }
                else
                {
                    currentInspectionAddress = (currentInspection.City__c + ", " + currentInspection.State__c);
                }
                JsonReturn = ("");
                using (var client1 = new HttpClient())
                {
                    String restRequest = ("http://www.mapquestapi.com/geocoding/v1/address?key="
                        + mapKey + "&location=" + currentInspectionAddress);
                    var request = new HttpRequestMessage(HttpMethod.Get, restRequest);
                    request.Headers.Add("X-PrettyPrint", "1");
                    var response = client1.SendAsync(request).Result;
                    JsonReturn = response.Content.ReadAsStringAsync().Result;
                }
                coordinates = new LatLngClass.RootObject();
                coordinates = JsonConvert.DeserializeObject<LatLngClass.RootObject>(JsonReturn);
                var array = coordinates.results.ToArray();
                var array2 = array[0].locations.ToArray();
                if (array2.Length == 1)
                {

                }
                else
                {
                    currentInspectionAddress = (currentInspection.City__c + ", " + currentInspection.State__c + ", " + currentInspection.Zip_Code__c);
                    JsonReturn = ("");
                    using (var client1 = new HttpClient())
                    {
                        String restRequest = ("http://www.mapquestapi.com/geocoding/v1/address?key="
                            + mapKey + "&location=" + currentInspectionAddress);
                        var request = new HttpRequestMessage(HttpMethod.Get, restRequest);
                        request.Headers.Add("X-PrettyPrint", "1");
                        var response = client1.SendAsync(request).Result;
                        JsonReturn = response.Content.ReadAsStringAsync().Result;
                    }
                    coordinates = new LatLngClass.RootObject();
                    coordinates = JsonConvert.DeserializeObject<LatLngClass.RootObject>(JsonReturn);
                    array = coordinates.results.ToArray();
                    array2 = array[0].locations.ToArray();
                    if (array2.Length == 1)
                    {

                    }
                    else
                    {

                        currentInspectionAddress = (currentInspection.City__c + ", " + currentInspection.State__c);
                        JsonReturn = ("");
                        using (var client1 = new HttpClient())
                        {
                            String restRequest = ("http://www.mapquestapi.com/geocoding/v1/address?key="
                                + mapKey + "&location=" + currentInspectionAddress);
                            var request = new HttpRequestMessage(HttpMethod.Get, restRequest);
                            request.Headers.Add("X-PrettyPrint", "1");
                            var response = client1.SendAsync(request).Result;
                            JsonReturn = response.Content.ReadAsStringAsync().Result;
                        }
                        coordinates = new LatLngClass.RootObject();
                        coordinates = JsonConvert.DeserializeObject<LatLngClass.RootObject>(JsonReturn);
                        array = coordinates.results.ToArray();
                        array2 = array[0].locations.ToArray();
                        if (array2.Length == 1)
                        {

                        }
                        else
                        {
                            currentInspectionAddress = (currentInspection.Zip_Code__c);
                            JsonReturn = ("");
                            using (var client1 = new HttpClient())
                            {
                                String restRequest = ("http://www.mapquestapi.com/geocoding/v1/address?key="
                                    + mapKey + "&location=" + currentInspectionAddress);
                                var request = new HttpRequestMessage(HttpMethod.Get, restRequest);
                                request.Headers.Add("X-PrettyPrint", "1");
                                var response = client1.SendAsync(request).Result;
                                JsonReturn = response.Content.ReadAsStringAsync().Result;
                            }
                            coordinates = new LatLngClass.RootObject();
                            coordinates = JsonConvert.DeserializeObject<LatLngClass.RootObject>(JsonReturn);
                            array = coordinates.results.ToArray();
                            array2 = array[0].locations.ToArray();
                        }
                    }
                }
                double currentInspectionLatitude = array2[0].displayLatLng.lat;
                double currentInspectionLongitute = array2[0].displayLatLng.lng;
                string savelat = currentInspectionLatitude.ToString();
                string savelon = currentInspectionLongitute.ToString();
                currentGeopoint = new GeoCoordinate(currentInspectionLatitude, currentInspectionLongitute);
                SaveCoordinatesClass updateCoordinates = new SaveCoordinatesClass();
                updateCoordinates.Property_Latitude__c = savelat;
                updateCoordinates.Property_Longitude__c = savelon;
                client.Update("Inspection__c", currentInspection.Id, updateCoordinates);
            }
            else
            {
                double currentInspectionLatitude = Convert.ToDouble(currentInspection.Property_Latitude__c);
                double currentInspectionLongitute = Convert.ToDouble(currentInspection.Property_Longitude__c);
                currentGeopoint = new GeoCoordinate(currentInspectionLatitude, currentInspectionLongitute);
            }
            for (int i = 0; i < workingList.Count; i++)
            {
                GeoCoordinate compareVal = new GeoCoordinate(workingList[i].latitude.GetValueOrDefault(), workingList[i].longitute.GetValueOrDefault());
                workingList[i].currentDistance = (0.00062137 * currentGeopoint.GetDistanceTo(compareVal));
            }
            workingList.Sort((x, y) => x.currentDistance.CompareTo(y.currentDistance));
            return workingList;
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
        static public List<OfficialInspectorClass> Longwork4(IProgress<string> progress, SalesforceClient client, List<OfficialInspectorClass> workingList, List<String> autoqueue)
        {
            InspectionJSONClass currentInspection = new InspectionJSONClass();
            List<String> assignedarray = new List<String>();
            List<String> skippedarray = new List<String>();
            string inspectorAssign = "";
            string inspectorAssign1 = "";
            //List<OfficialInspectorClass> tempList = new List<OfficialInspectorClass>();
            //List<OfficialInspectorClass> templist2 = new List<OfficialInspectorClass>();
            for(int i = 0; i < autoqueue.Count; i++)
            {
                List<OfficialInspectorClass> tempList = new List<OfficialInspectorClass>();
                List<OfficialInspectorClass> templist2 = new List<OfficialInspectorClass>();
                currentInspection = findInspectionbyOrderNumber(autoqueue[i].Substring(0, 6), client);

                if (currentInspection.Auto_Assign_Skip__c == false)
                {
                    List<string> historyList = new List<string>();
                    inspectorAssign = "";
                    inspectorAssign1 = "";

                    workingList = sortByDistance(client, workingList, currentInspection);
                    for (int j = 0; j < workingList.Count; j++)
                    {
                        if (workingList[j].currentDistance <= 500)
                        {
                            tempList.Add(workingList[j]);
                        }
                    }
                    //Console.WriteLine(tempList.Count);
                    //Console.ReadLine();
                    var history = client.Query<HistoryClass>("SELECT CreatedDate, Field, OldValue, NewValue From Inspection__History WHERE ParentId='" + currentInspection.Id + "' AND Field='Rep_ID_Inspector_history_tracking__c'");
                    for (int j = 0; j < history.Count; j++)
                    {
                        if (history[j].NewValue.Equals("-") && history[j].OldValue != null)
                        {
                            historyList.Add(history[j].OldValue);
                        }
                    }
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        String compareString = tempList[j].Rep_ID__c + " - " + tempList[j].Name;
                        if (!historyList.Contains(compareString) && tempList[j].Status__c != "On Hold")
                        {
                            templist2.Add(tempList[j]);
                        }
                    }
                    tempList = templist2;
                    templist2 = new List<OfficialInspectorClass>();
                    if(tempList.Count > 0)
                    {                                    
                        inspectorAssign1 = tempList[0].contactID;
                        inspectorAssign = tempList[0].Name;
                        updateInspectorCount(tempList[0].contactID, workingList);
                        UpdateInspectorClass updateInspector = new UpdateInspectorClass();
                        updateInspector.Inspector__c = inspectorAssign1;
                        assignedarray.Add((currentInspection.Name + ": " + inspectorAssign));
                        //updateInspectorCount(tempList[0].contactID);
                        client.Update("Inspection__c", currentInspection.Id, updateInspector);
                    }
                    else
                    {
                        skippedarray.Add((currentInspection.Name + ": Skipped"));
                        UpdateAdhocClass repad = new UpdateAdhocClass();
                        if (currentInspection.ADHOC__c == null)
                        {
                            currentInspection.ADHOC__c = "";
                        }
                        if (!currentInspection.ADHOC__c.Contains("Rep Needed"))
                        {
                            repad.ADHOC__c = "Rep Needed " + currentInspection.ADHOC__c;
                            client.Update("Inspection__c", currentInspection.Id, repad);
                        }                       
                    }
                }
                progress.Report("Orders Assigned: " + i + " of " + autoqueue.Count);
            }
            System.IO.File.WriteAllLines(@"C:\Users\Public\S2 Inspections\HUD Assigned.txt", assignedarray);
            System.IO.File.WriteAllLines(@"C:\Users\Public\S2 Inspections\HUD Skipped.txt", skippedarray);
            return workingList;
        }
    }
}
