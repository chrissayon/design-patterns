# Factory Method

The Factory Method is a creational pattern that delegates the decision of what object to create to a method, rather than it being defined directly.

## What problem are we solving?

In my previous job, we dealt with reading data from different sources and processing it. Below is an example starting with just SQL:

```C#
class SqlDataReader
{
    public void Read(string source)
    {
        // parse Sql
    }
}

class DataService
{
    public void ProcessSql(string source)
    {
        SqlDataReader reader = new();
        reader.Read(source);
    }
}
```

Now we get a request that we're also going to need to deal with CSV files. The code changes then to be like the following:

```C#
class SqlDataReader
{
    public void Read(string source)
    {
        // query SQL
    }
}

class CsvDataReader
{
    public void Read(string source)
    {
        // parse CSV
    }
}

class DataService
{
    public void ProcessCsv(string source)
    {
        CsvDataReader reader = new();
        reader.Read(source);
    }

    public void ProcessSql(string source)
    {
        SqlDataReader reader = new();
        reader.Read(source);
    }
}
```

The issue is now that you need to create a new method for every new data type you deal with. So the service is tightly coupled to the specific data type.

## The solution

The solution is to centralize the object creation behind a common interface. This way `DataService` doesn't need to know about the specific reader, it only talks to `IDataReader`. Adding a new data type means adding a new subclass, not modifying existing code.

### How to implement

- You will need an interface for IDataReader, the different readers will implement this interface
- You will need to create an abstract factory class, which in this case is DataService
- The abstract factory class will contain a `protected abstract` to contain the method signature which is implemented by the child class
- Optionally, the abstract class can contain methods that use the reader, such as `Process()`

```C#
// Interface for Data Reader
interface IDataReader
{
    void Read(string source);
}

// DataReader implementations
class CsvDataReader : IDataReader
{
    public void Read(string source) => Console.WriteLine($"Reading CSV from {source}");
}

class SqlDataReader : IDataReader
{
    public void Read(string source) => Console.WriteLine($"Reading SQL from {source}");
}

// The factory class/service that is responsible for creating the reader
abstract class DataService
{
    // Subclasses must implement this to return their specific reader
    protected abstract IDataReader CreateReader();

    public void Process(string source)
    {
        IDataReader reader = CreateReader();
        reader.Read(source);
    }
}

class CsvDataService : DataService
{
    protected override IDataReader CreateReader() => new CsvDataReader();
}

class SqlDataService : DataService
{
    protected override IDataReader CreateReader() => new SqlDataReader();
}

// Program.cs
var dataService = new CsvDataService();
dataService.Process("DATA");
```

The factory here is the `DataService` class, we've defined the signature for `CreateReader` which can then be implemented by the child classes.

### Other Questions
> In `CsvDataService` for `protected override IDataReader CreateReader() => new CsvDataReader();`, wouldn't it be better to do this in a constructor?
This is an "expression-bodied member" which is shorthand syntax for defining:

```
protected override IDataReader CreateReader()
{
    return new CsvDataReader();
}
``` 

The reader is only created when `Process()` is actually called, avoiding unnecessary instantiation.

> In `DataService` for `protected abstract IDataReader CreateReader();`, can't we just pass this out?
Yes you can, what happens after doesn't matter so just make it public if you want to. The factory class doesn't need to have the functionality.