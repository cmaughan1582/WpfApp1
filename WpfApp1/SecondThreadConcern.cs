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
                    "SELECT Id, Name, BillingPostalCode, ShippingPostalCode, Rep_ID__C, Comments__c, HUD_Certified__c, FNMA_Certified__c, Freddie_Mac_Certified__c, Inspector_Ranking__c, Status__c, ShippingLatitude, ShippingLongitude, FNMA_4260__c, FNMA_4261__c, FNMA_4262__c, No_Contact__c, Inspector_Rush__c, CMSA_2__c, Exterior_1__c, FNMA_HC_MBA__c, Exterior_2__c, CME_HC__c, CME_MF__c, Freddie_MF_MBA__c, MBA__c, MBA_2__c, HUD_REAC__c, Freddie_HC_MBA__c, FNMA_MF_MBA__c, CMSA__c, Cap_Improv__c, Coverage_Area_Radius__c, Max_Insp_Count__c " +
                    "From Account " +
                    "WHERE Account_Inactive__c=false AND HUD_Certified__c=false AND Rep_ID__c!=null");
                records.Add(client.FindById<NewInspectorClass>("Account", "0013700000W5oq8")); //this adds nc128 jessica jackson
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
    }
}
