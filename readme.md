# CsvProcessor
### David Seeto 2018

## Libraries
Log4Net 2.0.8 Logging library http://logging.apache.org/log4net/
Nunit 3.10.1 Unit Test framework http://nunit.org/

Written on macOS using .net Core 2.1.1

## Folder structure
* **Program.cs** - main program code
* **Helpers** - Common Maths functions
* **Model** - data models
* **Tests** - unit tests for maths functions
* **Properties/log4net.config** - log4net config file for file appender
* **SampleFiles/log.txt** - logfile
* **SampleFiles/** - location of csv file
* **Readme.md** - this file



## Overview
The program will load a directory 'SampleFiles 'and import files based on file prefix "LP" for "load profile"
or  "TOU" for "Time of Use" files.
It loads the CSV file by streaming (using StreamReader) the file into a data model of each row.
Each row is then added to a dataTable for futher manipulation.

Once the file is completed and the dataTable is ready, the program iterates over the data table to get the median and to compares
the comparision field against the median value.

Median calculation is in Maths class as 'Median'.
The function to determine if comparision field is beyond 20% percent is also in Maths class 'PercentageAboveOrBelow'

For large files, I streamed the file contents instead of trying to read the whole file.


## Program showcase:
I read the file one line at time streamed and create a datamodel of each files columns.
I then create and load each line into a dataTable. This way I can extract just the column I want and also re-iterate back over the dataTable in memory without 
having to reopen the file. I want to touch the file as least times as possible.
I choose this method - as it would be easy to extend this program to calculate/process other fields if needed. 
It could be easily extended to be saved to a database/warehousing database. Queries/analytics could then be run off this as needed.

I choose readiability and understandability over any complexity/speed or obsure code to showcase that maintianable code should always be at the forefront.
I choose to log as much as possible, both to the console and to the log file throughout the appliction. This was implented log4net library.

The use of StreamReader for sequential reading as opposed to memoryMappedFiles which are better suited to random access of the file.

### Results:
**LoadProfile Median**
* ...8049 - 1.25
* ...3712 - 0
* ...7915 - 1.89


**Time Of Use Median**
* ...2358 - 409646.7
* ...2240 - 378331.6
* ...0057 - 0.146

### Extending
The applicatin can be easily extended by registered a new file prefix
Line 35:  filesToProcess = GetFiles("XX");
and introducing a new file processor into function ProcessGivenFileType()  

## Improvements:
The application could move completed files out of the folder when completed into another folder or delete them.
The application could Poll the directory for new files added to it. If it sees new files, then it could automatically process them.  The files in the directory could be added by either 'ftping'' or 'copied' into.

Potentially for files with millions of rows, the program could save the results to a database and then work off the dataset when and as needed.

Utilise SqlBulk insert to insert the data into a properly indexed table and smartly query the table for the actual rows are wanted/needed for a much more efficient plan.

There are many ways to make it more effective, it would be better with a multithreaded asynchronous solution where
one thread reads file data into the datatable and another processees it as it becomes available to consume. 
(Producer Consumer)

For very large files, a good solution is to break the file into smaller 'batch' chunks. and the process the files with a counter;
reading a batch at a time and process the lines, then move onto the next batch of the file until the large file is completed.
This could be incorporated with Sql transactions - where transactions can be committed or rolledback if there was a failure.
(Divide & Conquer)

To process multiple files, I'd look at parrallelism of file processing.
The program could 'Task' - off each file in the directory and then wait for them to complete, introducing a level of parallelism.
(Parallelism)

Asynchronous program is best suited for I/O bound work. It'll increase the overall throughput.
Parallel program is best suited for CPU intensive work, or if there is a lot of work and want to split it up on multiple threads.








