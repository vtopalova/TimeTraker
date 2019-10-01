# WorkingTimeTracker
An application for processing the working time information.

# Usage
The application processes the input data aboout booked meetings and employee's check-in.
Тhe input file format must be:
```
HH:mm HH:mm                      // The office start and end time.
yyyy-MM-dd HH:mm:ss {employee}   // An employee record for check in date and time, and employee acronym.
yyyy-MM-dd HH:mm {duration}      // A meeting record with date and duration.

```

As result it creates a calendar with chronological records in format.
```
yyyy-MM-dd          // Date of the booking.
HH:mm:ss {employee} // An employee record for check in time and employee acronym.
HH:mm HH:mm         // A meeting record for start end time.

```


# How to run
`WorkingTimeTracker` expects an input file with booking information for employee and meetings.

## Without output file
By default the result will be presented only on the console.

```
dotnet run {inputFile}

```
where

 `{inputFile}` is the path to the input file.

## With output file
In case the ouput file is needed, please run:
```
dotnet run {inputFile} > {outputFile}

```
where 

`{inputFile}` is the path to the input file.

`{outputFile}` is the path to the input file.

# Exit codes
Here is a list of the possible exit codes:
| Code  | Description |
| ------------- | ------------- |
| 0  | Success  |
| 1  | Invalid duration  |
| 2  | Invalid date  |
| 3  | Duplicated employee's date  |
| 4  | Overlapped meeting  |
| 5  | Invalid office hours  |
| 500  | Unknown error  |

