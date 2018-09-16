using System;
namespace DataModels
{
    //
    // Data Model of the load profile file columns
    // 

    public class LoadProfileModel
    {
        public uint MeterPointCode { get; set; }  
        public uint SerialNumber { get; set; }
        public String PlantCode { get; set; }
        public DateTime DateTimeLogged { get; set; }
        public String DataType { get; set; }
        public Double DataValue { get; set; }
        public String Units { get; set; }
        public String Status { get; set; }
    } 
}
