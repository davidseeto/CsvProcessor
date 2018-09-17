# CsvProcessor
### David Seeto 2018

## Libraries
Log4Net 2.0.8 Logging library http://logging.apache.org/log4net/
Nunit 3.10.1 Unit Test framework http://nunit.org/

Written on macOS using .net Core 2.1.1

## Folder structure
**Program.cs** - main program code
**Helpers** - Common Maths functions
**Model** - data models
**Tests** - unit tests for maths functions
**Properties/log4net.config** - log4net config file for file appender
**log.txt** - logfile
**SampelFiles/** - location of csv file
**Readme.md** - this file

##Overview
Will load directory and import file based on prefix "LP" for load profile or  "TOU" for Time of Use files.
Loads the CSV by Streaming (StreamReader) the file into a data model of each row.
Each row is added to a dataTable for futher manipulation.

For large files, stream the file contents instead of trying to read the whole file.
Potentially for files with millions of rows, the program could save the results to a database and then work off the data when and as needed.

Utilise SqlBulk insert to insert the data into a properly indexed table and 
smartly query the table for the actual rows are wanted/needed for a much more efficient plan.


## Program showcase:
I read the file one line at time streamed and create a datamodel of each files columns.
I then create and load each line into a dataTable. This way I can extract just the column I want and also re-iterate back over the dataTable in memory without having to reopen the file. I want to touch the file as least times as possible.
I choose this - as it would be easy to extend this program to calculate/process other fields. It could be easily extended to be saved to a database/warehousing database. Queries/analytics could then be run off this as needed.

I choose readiability and understandability over any complexity/speed or obsure code to showcase that maintianable code should always be forefront.

I use StreamReader as its better for sequential reading as opposed to memoryMappedFiles which are better suited to random access of the file.

I choose StreamReader as the file will need to be sequentially processed and I don't want the whole file to be opened in memory at once or to go through the file more than once.

## Improvements:
Run the program using x64 bit architecture. This will give greater access to larger computer memory.

There are however ways to make it more effective, it would be better with a multithreaded asynchronous solution where 
one thread reads data and another processees it as it becomes available to consume. 

For very large files, a good solution is to break the file into smaller 'batch' chunks. and the process the files with a counter;
reading a batch at a time and process the lines, then move onto the next batch of the file until the large file is completed.
This could be incorporated with Sql transactions - where transactions can be committed or rolledback if there was a failure.

The ultimate solution would be to have multiple threads read & process different parts of the files at the same asynchronously

To process multiple files, I'd look at parrallelism of file processing.







