using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvProcessor.Helpers;
using log4net;
using DataModels;

namespace CsvProcessor
{
    class Program
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        public static void Main()
        {
            double percentage = 0.2;          // percentage above or below median
            String LPField    = "DataValue";  // Load Profile field 
            String TOUField   = "Energy";     // Time Of Use field

            try
            {
                CreateLogFileIfNonExistent();  // create the file if it doesn't yet exist
                Console.WriteLine("CSV Processor!");
                ConsoleLog("Started: " + DateTime.Now);
                Directory.SetCurrentDirectory("../../SampleFiles");

                bool foundLoadProfileFiles = false;
                bool foundTimeOfUseFiles = false;
                List<string> filesToProcess;

                // Get any Load Profile files in the directory
                filesToProcess = GetFiles("LP");
                if (filesToProcess.Count == 0 || filesToProcess == null)
                {
                    foundLoadProfileFiles = false;
                    log.Error("No Load Profile files to process");
                }
                else
                {
                    foundLoadProfileFiles = true;
                    ProcessGivenFileType(percentage, LPField, null,foundLoadProfileFiles, false, filesToProcess);
                }

                // Get any Time of Use files in the directory
                filesToProcess = GetFiles("TOU");

                if (filesToProcess.Count == 0 || filesToProcess == null)
                {
                    foundTimeOfUseFiles = false;
                    log.Error("No Time of Use files to process");
                }
                else
                {
                    foundTimeOfUseFiles = true;
                    ProcessGivenFileType(percentage, null, TOUField, false, foundTimeOfUseFiles, filesToProcess);
                }

            }
            catch (Exception ex) {             
                ConsoleLog("Exception occured: " + ex.Message);
            }
            finally {
            }
        }

        //
        // Process the given file types LP , TOU within the directory.
        //
        private static void ProcessGivenFileType(double percentage, string LPField, string TOUField, bool foundLoadProfileFiles, bool foundTimeOfUseFiles, List<string> filesToProcess)
        {
            ConsoleLog("Files found: " + filesToProcess.Count);
            foreach (var filename in filesToProcess)
            {
                var dataValueList = new List<double>();
                List<LoadProfileModel> lp = new List<LoadProfileModel>();
                List<TimeOfUseModel> tou = new List<TimeOfUseModel>();   
      
                if (foundLoadProfileFiles) {
                    ProcessLPFile(percentage, LPField, lp, filename, dataValueList);
                }
                else if ((foundTimeOfUseFiles))
                {
                    ProcessTOUFile(percentage, TOUField, tou, filename, dataValueList);
                }
            }

            ConsoleLog("Completed: " + DateTime.Now);
        }

        //
        // Process a Load Profile CSV file. This is the entry function
        //
        private static void ProcessLPFile(double percentage, string FieldToCompare, List<LoadProfileModel> lp, string filename, List<double> dataValueList)
        {
            DataTable dt = CreateLoadProfileDataTableSchema();
            ImportLoadProfileFile(filename, lp);
            CreateLoadProfileDataTable(dt, lp);

            ProcessForPercentAwayFromMedian(dataValueList, filename, dt, FieldToCompare, percentage);
            dt.Clear();
                       
        }

        //
        // Process a Time of Use CSV file. This is the entry function
        //
        private static void ProcessTOUFile(double percentage, string FieldToCompare, List<TimeOfUseModel> tou, string filename, List<double> dataValueList)
        {
            DataTable dt = CreateTimeOfUseDataTableSchema();
            ImportTimeOfUseFile(filename, tou);
            CreateTimeOfUseDataTable(dt, tou);

            ProcessForPercentAwayFromMedian(dataValueList, filename, dt, FieldToCompare, percentage);
            dt.Clear();
        }

        //
        // Process datatable rows , given fieldToCompare against the median value
        // Writes to console if fieldToCompare is greater than percentage
        //
        private static void ProcessForPercentAwayFromMedian(List<double> dataValueList, string filename, DataTable dt,String fieldToCompare, double percentage)
        {
            foreach (DataRow rw in dt.Rows) {
                dataValueList.Add((Double)rw[fieldToCompare]);
            }

            double median = Maths.Median(dataValueList);
            log.Info("Median is: " + median.ToString());
            foreach (DataRow rw in dt.Rows) {
                double dataValue = (double)rw[fieldToCompare];
                DateTime dateTimeLogged = (DateTime)rw["DateTimeLogged"];
                if (Maths.PercentageAboveOrBelow(median, dataValue, percentage)) {
                    Console.WriteLine("Filename:{0} Date:{1} Data Value:{2} Median:{3}", filename, dateTimeLogged, dataValue, median);
                }
            }
        }

        //
        // Create a dataTable specific for Load Profile data
        //
        private static DataTable CreateLoadProfileDataTableSchema()
        {
            //add columns for Load Profile dataTable
            DataTable dt = new DataTable();
            dt.Columns.Add("MeterPointCode", typeof(uint));
            dt.Columns.Add("SerialNumber", typeof(uint));
            dt.Columns.Add("PlantCode", typeof(String));
            dt.Columns.Add("DateTimeLogged", typeof(DateTime));
            dt.Columns.Add("DataType", typeof(String));
            dt.Columns.Add("DataValue", typeof(Double));
            dt.Columns.Add("Units", typeof(String));
            dt.Columns.Add("Status", typeof(String));
            return dt;
        }

        //
        // Create a dataTable specific for Time Of Use data
        //
        private static DataTable CreateTimeOfUseDataTableSchema()
        {
            //add columns for Time Of Use databTable
            DataTable dt = new DataTable();
            dt.Columns.Add("MeterPointCode", typeof(uint));
            dt.Columns.Add("SerialNumber", typeof(uint));
            dt.Columns.Add("PlantCode", typeof(String));
            dt.Columns.Add("DateTimeLogged", typeof(DateTime));
            dt.Columns.Add("DataType", typeof(String));
            dt.Columns.Add("Energy", typeof(Double));
            dt.Columns.Add("MaximumDemand", typeof(String));
            dt.Columns.Add("TimeOfMaxDemand", typeof(DateTime));
            dt.Columns.Add("Units", typeof(String));
            dt.Columns.Add("Status", typeof(String));
            dt.Columns.Add("Period", typeof(String));
            dt.Columns.Add("DLSActive", typeof(Boolean));
            dt.Columns.Add("BillingReset", typeof(int));
            dt.Columns.Add("BillingResetDateTime", typeof(DateTime));
            dt.Columns.Add("Rate", typeof(String));
            return dt;
        }

        //// 
        // Create a datatable given the table type.
        // PARAM:  tableType: 'LoadProfile'
        //                    'TimeOfUse'  
        ////
        private static void CreateLoadProfileDataTable(DataTable dt, List<LoadProfileModel> lp)
        {
            log.Info("Creating datatable...");
            foreach (LoadProfileModel item in lp) 
            {
                dt.Rows.Add(new object[] { item.MeterPointCode, item.SerialNumber, item.PlantCode, item.DateTimeLogged,
                                        item.DataType, item.DataValue,item.Units, item.Status });
            }

            log.Info("Completed creating datatable...");
        }

        //// 
        // Create a datatable given the table type.
        // PARAM:  tableType: 'TimeOfUse' 
        //                     
        ////
        private static void CreateTimeOfUseDataTable(DataTable dt, List<TimeOfUseModel> tou)
        {
            log.Info("Creating datatable...");
           
            foreach (TimeOfUseModel item in tou)
            {
                dt.Rows.Add(new object[] { item.MeterPointCode, item.SerialNumber, item.PlantCode, item.DateTimeLogged,
                                        item.DataType, item.Energy,item.MaximumDemand, item.TimeOfMaxDemand,
                                        item.Units, item.Status, item.Period, item.DLSActive, item.BillingReset,
                                        item.BillingResetDateTime, item.Rate});
            }

            log.Info("Completed creating datatable...");
        }

        //// 
        // Get CSV files for directory.
        //
        //// 
        private static List<string> GetFiles(String filetype)
        {
            var myFiles = new List<string>();
            try {               
                myFiles = Directory.GetFiles("./", filetype + "*.csv", SearchOption.AllDirectories).ToList();
                return myFiles;
            }
            catch {
                return null;
            }
        }

        //// 
        // Import a Load Profile File.
        ////       
        private static void ImportLoadProfileFile(string filename, List<LoadProfileModel> lp)
        {
            log.Info("Started Processing: " + filename);
            string inputLine = String.Empty;

            try {
                StreamReader csvreader = new StreamReader(filename);

                inputLine = csvreader.ReadLine();  // read headerline & ignore

                string[] csvArray = inputLine.Split(new char[] { ',' });
              
                if (csvArray.Length != 8) {
                    throw new ArgumentException("Expected number of columns don't match. Please verify");
                }

                while ((inputLine = csvreader.ReadLine()) != null) {
                    csvArray = inputLine.Split(new char[] { ',' });

                    var LoadProfileItem = new LoadProfileModel();

                    LoadProfileItem.MeterPointCode       = uint.Parse(csvArray[0]);
                    LoadProfileItem.SerialNumber         = uint.Parse(csvArray[1]);
                    LoadProfileItem.PlantCode            = csvArray[2];
                    LoadProfileItem.DateTimeLogged       = DateTime.ParseExact(csvArray[3], "dd/MM/yyyy HH:mm:ss", CultureInfo.GetCultureInfo("en-AU"));
                    LoadProfileItem.DataType             = csvArray[4];
                    LoadProfileItem.DataValue            = double.Parse(csvArray[5]);
                    LoadProfileItem.Units                = csvArray[6];
                    LoadProfileItem.Status               = csvArray[7];
       
                    lp.Add(LoadProfileItem);

                }
                log.Info("Completed Processing Successfully: " + filename);
            }
            catch (Exception ex) {
                throw ex;
            }     
        }

        // 
        //  Import a TOU File
        //
        private static void ImportTimeOfUseFile(string filename, List<TimeOfUseModel> tou)
        {
            log.Info("Started Processing: " + filename);
            string inputLine = String.Empty;
          
            try {
                StreamReader csvreader = new StreamReader(filename);

                inputLine = csvreader.ReadLine();  //read headerline & ignore
                string[] csvArray = inputLine.Split(new char[] { ',' });
                if (csvArray.Length != 15) //check for expected header size
                {
                    throw new ArgumentException("Expected number of columns don't match. Please verify");
                }

                while ((inputLine = csvreader.ReadLine()) != null) {
                    csvArray = inputLine.Split(new char[] { ',' });
                  
                    var TimeOfUseItem = new TimeOfUseModel();

                    TimeOfUseItem.MeterPointCode       = uint.Parse(csvArray[0]);
                    TimeOfUseItem.SerialNumber         = uint.Parse(csvArray[1]);
                    TimeOfUseItem.PlantCode            = csvArray[2];
                    TimeOfUseItem.DateTimeLogged       = DateTime.Parse(csvArray[3]);
                    TimeOfUseItem.DataType             = csvArray[4];
                    TimeOfUseItem.Energy               = double.Parse(csvArray[5]);
                    TimeOfUseItem.MaximumDemand        = double.Parse(csvArray[6]);
                    TimeOfUseItem.TimeOfMaxDemand      = DateTime.Parse(csvArray[7]);
                    TimeOfUseItem.Units                = csvArray[8];
                    TimeOfUseItem.Status               = csvArray[9];
                    TimeOfUseItem.Period               = csvArray[10];
                    TimeOfUseItem.DLSActive            = Boolean.Parse(csvArray[11]);
                    TimeOfUseItem.BillingReset         = int.Parse(csvArray[12]);
                    TimeOfUseItem.BillingResetDateTime = DateTime.Parse(csvArray[13]);
                    TimeOfUseItem.Rate                 = csvArray[14];

                    tou.Add(TimeOfUseItem);
                }
                log.Info("Completed Processing Successfully: " + filename);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        // 
        // Log a line of text to log file and console
        //
        private static void ConsoleLog(String logLine)
        {
            Console.WriteLine(logLine);
            log.Info(logLine);
        }

        ////
        /// 
        // Create a log file if it doesn't exist.
        // Log.txt
        private static void CreateLogFileIfNonExistent()
        {
            string path = "../../SampleFiles/Log.txt";
            if (!File.Exists(path))
                File.Create(path);
        }
    }
}
