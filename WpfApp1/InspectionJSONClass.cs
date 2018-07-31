﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WpfApp1
{
    class InspectionJSONClass
    {
            //public string type { get; set; }
            //public string url { get; set; }
            public string Id { get; set; }
            public string OwnerId { get; set; }
            public bool IsDeleted { get; set; }
            public string Name { get; set; }
            public string RecordTypeId { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string CreatedById { get; set; }
            public DateTime? LastModifiedDate { get; set; }
            public string LastModifiedById { get; set; }
            public DateTime? SystemModstamp { get; set; }
            public object LastActivityDate { get; set; }
            public DateTime? LastViewedDate { get; set; }
            public DateTime? LastReferencedDate { get; set; }
            public String Inspector__c { get; set; }
            public string Loan__c { get; set; }
            public object Sequence__c { get; set; }
            public string Form_Name__c { get; set; }
            public string Client_Due_Date__c { get; set; }
            public string Inspection_Start_Date__c { get; set; }
            public object Appointment_Date__c { get; set; }
            public string Order_Date__c { get; set; }
            public object Client_Fee_Comments__c { get; set; }
            public object Inspector_Fee_Comments__c { get; set; }
            public object QC_Fee_Comments__c { get; set; }
            public object Inspector_Rejected_Reason__c { get; set; }
            public string Property_Name__c { get; set; }
            public string Street_Address__c { get; set; }
            public string City__c { get; set; }
            public string State__c { get; set; }
            public string Zip_Code__c { get; set; }
            public string Property_Contact_Name__c { get; set; }
            public string Property_Contact_Phone__c { get; set; }
            public string Property_Contact_Email__c { get; set; }
            public object Appointment_Time__c { get; set; }
            public object X14_day__c { get; set; }
            public object With_Inspector_Start_Date__c { get; set; }
            public object With_Inspector_End_Date__c { get; set; }
            public object QC_Review_Start_Date__c { get; set; }
            public object QC_Review_End_Date__c { get; set; }
            public object QC_Rep_ID__c { get; set; }
            public object of_Days_With_Inspector__c { get; set; }
            public object QC_Contact__c { get; set; }
            public string Requester__c { get; set; }
            public string Client__c { get; set; }
            public string Validation_Start_Date__c { get; set; }
            public string Validation_End_Date__c { get; set; }
            public string Assign_Start_Date__c { get; set; }
            public object Assign_End_Date__c { get; set; }
            public object Accept_Start_Date__c { get; set; }
            public object Accept_End_Date__c { get; set; }
            public object QC_Assign_Start_Date__c { get; set; }
            public object QC_Assign_End_Date__c { get; set; }
            public object Final_Review_Start_Date__c { get; set; }
            public object Final_Review_End_Date__c { get; set; }
            public object Client_Delivery_Start_Date__c { get; set; }
            public object Client_Delivery_End_Date__c { get; set; }
            public string Community_Detail_Page_Link__c { get; set; }
            public string Account_Sharing__c { get; set; }
            public object Client_Returns_Start_Date__c { get; set; }
            public object Client_Returns_End_Date__c { get; set; }
            public object Inspector_Rep_ID__c { get; set; }
            public object Completed_Date__c { get; set; }
            public object Canceled_Date__c { get; set; }
            public object of_Days_in_Accept__c { get; set; }
            public object of_Days_in_Assign__c { get; set; }
            public object of_Days_in_Client_Delivery__c { get; set; }
            public object of_Days_In_Client_Returns__c { get; set; }
            public object of_Days_in_Final_Review__c { get; set; }
            public object QC_Account_Name__c { get; set; }
            public object of_Days_in_QC_Review__c { get; set; }
            public object of_Days_in_QC_Assign__c { get; set; }
            public double of_Days_in_Validation__c { get; set; }
            public object of_Buildings__c { get; set; }
            public object of_Units__c { get; set; }
            public object of_Units_to_Inspect__c { get; set; }
            public object Borrower_Name__c { get; set; }
            public object Current_Principal_Balance__c { get; set; }
            public object FHA__c { get; set; }
            public object HUD_Receipt_Number__c { get; set; }
            public object HUD_Inspection__c { get; set; }
            public object Inv__c { get; set; }
            public object Investor_Loan__c { get; set; }
            public object Investor__c { get; set; }
            public object Last_Inspection_Rating__c { get; set; }
            public object Original_Loan_Date__c { get; set; }
            public object Original_Principal_Balance__c { get; set; }
            public object Property_ID__c { get; set; }
            public object Rentable_Square_Footage__c { get; set; }
            public object Special_Instructions__c { get; set; }
            public object Total_Square_Footage__c { get; set; }
            public object Year_Built__c { get; set; }
            public string Inspector_Rush__c { get; set; }
            public object Inspector_Phone__c { get; set; }
            public object Inspector_Email__c { get; set; }
            public string Inspector_Due_Date__c { get; set; }
            public bool Inspection_Canceled__c { get; set; }
            public object Library__c { get; set; }
            public string Loan_Seq_text__c { get; set; }
            public double Roll_Up_Inspections__c { get; set; }
            public object O_M_Purpose__c { get; set; }
            public object Project__c { get; set; }
            public string Property_Type__c { get; set; }
            public object Secondary_Property_Type__c { get; set; }
            public object Completion_Repair__c { get; set; }
            public object Parent_Account__c { get; set; }
            public double Client_Fees__c { get; set; }
            public object Inspector_Rush_Fee__c { get; set; }
            public object Inspector_Deduction__c { get; set; }
            public object Inspector_Misc__c { get; set; }
            public object Inspector_Remote_Fee__c { get; set; }
            public object Inspector_Trip_Charge__c { get; set; }
            public double? Inspector_Fees__c { get; set; }
            public double QC_Fees__c { get; set; }
            public object QC_Deduction__c { get; set; }
            public object QC_Misc__c { get; set; }
            public string Fee_Type__c { get; set; }
            public object Rush__c { get; set; }
            public object Client_Misc__c { get; set; }
            public string Photo_Count__c { get; set; }
            public double Client_Rush_Fee__c { get; set; }
            public double Inspector_Fee__c { get; set; }
            public bool Inspector_Rejected__c { get; set; }
            public bool Inspection_Completed__c { get; set; }
            public bool QC_Completed__c { get; set; }
            public bool Inspection_Accepted__c { get; set; }
            public bool QC_Reject_Inspection__c { get; set; }
            public String ADHOC__c { get; set; }
            public string Fee_Type_Text__c { get; set; }
            public object O_M_Plan__c { get; set; }
            public string Queue__c { get; set; }
            public object Inspector_ID__c { get; set; }
            public object QC_ID__c { get; set; }
            public bool Inspection_Completed_1st_Time__c { get; set; }
            public bool Inspection_Validated__c { get; set; }
            public bool Needs_to_Be_Invoiced__c { get; set; }
            public object Inspector_Account__c { get; set; }
            public object QC_Account__c { get; set; }
            public bool Been_QC_Rejected__c { get; set; }
            public string Current_User_ID__c { get; set; }
            public bool Been_Inspector_Rejected__c { get; set; }
            public bool Client_Return_to_Vendor__c { get; set; }
            public object Inspector_Account_Name__c { get; set; }
            public object Last_Insp_Date__c { get; set; }
            public string Inspection_Folder__c { get; set; }
            public object Legacy_External_Id__c { get; set; }
            public bool Legacy_Update__c { get; set; }
            public string Client_Division__c { get; set; }
            public string Client_Division_ID__c { get; set; }
            public string Loan_Seq__c { get; set; }
            public double of_Days_in_Current_Queue__c { get; set; }
            public double of_Days_to_Complete_Inspection__c { get; set; }
            public object QC_Account_ID_text__c { get; set; }
            public object Requester_ID__c { get; set; }
            public object XClient_Due_Date__c { get; set; }
            public string Client_Division_Team__c { get; set; }
            public bool Inactive_Inspector__c { get; set; }
            public bool Inactive_QC__c { get; set; }
            public bool Inactive_Requester__c { get; set; }
            public bool Inspection_Auto_Decline__c { get; set; }
            public string Rep_ID_Inspector_Formula__c { get; set; }
            public string Rep_ID_Inspector_history_tracking__c { get; set; }
            public string Client_Form__c { get; set; }
            public string Division__c { get; set; }
            public object Form_URL__c { get; set; }
            public object ID_number__c { get; set; }
            public object Form_Instructions__c { get; set; }
            public object On_Time__c { get; set; }
            public object Code__c { get; set; }
            public double ETA__c { get; set; }
            public string Country__c { get; set; }
            public object Appointment_Entered_Date__c { get; set; }
            public object Management_Company__c { get; set; }
            public object FNMA_Insp_ID__c { get; set; }
            public object QC_Score__c { get; set; }
            public bool S2_Rejected_to_Inspector__c { get; set; }
            public bool On_Time_Exception__c { get; set; }
            public double On_Time_Except_Calc__c { get; set; }
            public object Rep_Follow_Up_Notes__c { get; set; }
            public object Loan_Balance_as_of_Date__c { get; set; }
            public object Status__c { get; set; }
            public object Deferred_Maintenance__c { get; set; }
            public object Safety_Issue__c { get; set; }
            public object Code_Violation__c { get; set; }
            public string Region__c { get; set; }
            public object Collateral_ID__c { get; set; }
            public object Property_Contact_2__c { get; set; }
            public object Assigned_To__c { get; set; }
            public object PIH_Project__c { get; set; }
            public object PIH_Code__c { get; set; }
            public object Surveillance_Specialist__c { get; set; }
            public object Operational_Repairs__c { get; set; }
            public double Gross_Margin__c { get; set; }
            public object Property_Rating__c { get; set; }
            public object PNA_Repairs__c { get; set; }
            public String Property_Longitude__c { get; set; }
            public String Property_Latitude__c { get; set; }
            public bool Auto_Assign_Skip__c { get; set; }
    }
}
