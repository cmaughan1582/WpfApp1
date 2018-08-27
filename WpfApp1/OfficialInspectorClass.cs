using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    [Serializable]
    public class OfficialInspectorClass
    {
        public String accountId { get; set; }
        public String Name { get; set; }
        public String BillingPostalCode { get; set; }
        public String ShippingPostalCode { get; set; }
        public string Rep_ID__c { get; set; }
        public String Comments__c { get; set; }
        public bool HUD_Certified__c { get; set; }
        public bool FNMA_Certified__c { get; set; }
        public bool Freddie_Mac_Certified__c { get; set; }
        public double Inspector_Ranking__c { get; set; }
        public String Status__c { get; set; }
        public Double? latitude { get; set; }
        public Double? longitute { get; set; }
        public String contactID { get; set; }
        public double? assignedInspections { get; set; }
        public double currentDistance { get; set; }
        public String Phone { get; set; }
        public Dictionary<String, double?> feeDictionary { get; set; }
        public double? Max_Insp_Count__c { get; set; }
        public String Coverage_Area_Radius__c { get; set; }
        public String Blacklist__c { get; set; }
    }
}
