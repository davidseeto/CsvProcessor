using System;
using System.Collections.Generic;
using System.Linq;
namespace CsvProcessor.Helpers
{
    public static class Maths
    {
        // Checks actualvalue more than of percentage basevalue 
        // PARAM: median (median value for comparision against
        // PARAM: actualvalue (value to compare against the median)
        // PARAM: percentage - percentage as double 
        //        value determines whether actualvalue is outside
        //        the percentage range of median.
        // RETURNS: boolean 
        ////

        public static Boolean PercentageAboveOrBelow(double median,double actualvalue,double percentage)
        {
            double percentagevalueBelow = (1 - percentage) * median;
            double percentagevalueAbove = (1 + percentage) * median;
            if (actualvalue < percentagevalueBelow ) {
                return true;
            }

            if (actualvalue > percentagevalueAbove) {
                return true;
            }

            return false;
        }

        // Finds median value given unsorted list of numbers 
        // PARAM: List of numbers (Double) 
        // RETURNS: Median value as Double
        //// 

        public static double Median(List<double> sourceNumbers)
        {
            if (sourceNumbers == null || sourceNumbers.Count == 0) {
                throw new System.Exception("Unable to get Median from an empty list!");
            }
            
            // make sure the list is sorted, but use a new sorted List
            var sortedNumbers = sourceNumbers.OrderBy(n => n).ToList();
                  
            int size = sortedNumbers.Count;
            int mid = size / 2;
            // is the mid element even?

            double median = (size % 2 != 0) ? (double)sortedNumbers.ElementAt(mid) 
                                            : ((double)sortedNumbers.ElementAt(mid) + (double)sortedNumbers.ElementAt(mid - 1)) / 2;
              
            return median;
        }
    }
}
