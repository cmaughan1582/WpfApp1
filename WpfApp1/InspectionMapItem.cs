using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class InspectionMapItem
    {
        public String Name { get; set; }
        public String ADHOC__c { get; set; }
        public String Inspection_Folder__c { get; set; }
        public String Region__c { get; set; }
        public String Inspector__c { get; set; }
        public String Fee_Type__c { get; set; }
        public String Property_Longitude__c { get; set; }
        public String Property_Latitude__c { get; set; }
    }
}