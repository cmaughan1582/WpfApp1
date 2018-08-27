using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    [Serializable]
    class NewInspectorClass
    {
        public int? SicDesc { get; set; }
        public Double? ShippingLatitude { get; set; }
        public Double? ShippingLongitude { get; set; }
        public String Id { get; set; }
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
        public Double? Cap_Improv__c { get; set; }
        public Double? CMSA__c { get; set; }
        public Double? FNMA_4260__c { get; set; }
        public Double? FNMA_4261__c { get; set; }
        public Double? FNMA_4262__c { get; set; }
        public Double? FNMA_MF_MBA__c { get; set; }
        public Double? Freddie_HC_MBA__c { get; set; }
        public Double? Freddie_MF_MBA__c { get; set; }
        public Double? HUD_REAC__c { get; set; }
        public Double? MBA__c { get; set; }
        public Double? MBA_2__c { get; set; }
        public Double? No_Contact__c { get; set; }
        public Double? Inspector_Rush__c { get; set; }
        public Double? CMSA_2__c { get; set; }
        public Double? Exterior_1__c { get; set; }
        public Double? FNMA_HC_MBA__c { get; set; }
        public Double? Exterior_2__c { get; set; }
        public Double? CME_HC__c { get; set; }
        public Double? CME_MF__c { get; set; }
        public String Coverage_Area_Radius__c { get; set; }
        public Double? Max_Insp_Count__c { get; set; }
        public String Blacklist__c { get; set; }
    }
}
