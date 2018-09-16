using System;
namespace DataModels
{
    //
    // Data Model of the load profile file columns
    // 
    public class TimeOfUseModel
    {     
        public uint MeterPointCode { get; set; }  
        public uint SerialNumber { get; set; }
        public String PlantCode { get; set; }
        public DateTime DateTimeLogged { get; set; }
        public String DataType { get; set; }
        public double Energy { get; set; }
        public double MaximumDemand { get; set; }
        public DateTime TimeOfMaxDemand { get; set; }
        public String Units { get; set;  }
        public String Status { get; set; }
        public String Period { get; set; }  
        public bool DLSActive { get; set; }
        public int BillingReset { get; set; }
        public DateTime BillingResetDateTime { get; set; }
        public String Rate { get; set; }
    }
}
