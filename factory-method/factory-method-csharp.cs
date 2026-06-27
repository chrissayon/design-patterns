var dataService = new CsvDataService();
dataService.Process("DATA");

// Class
public interface IDataReader
{
    public void Read(string source);
}

public class SqlDataReader : IDataReader
{
    public void Read(string source) => Console.WriteLine($"Reading SQL from {source}");
}

public class CsvDataReader : IDataReader
{
    public void Read(string source) => Console.WriteLine($"Reading CSV from {source}");
}

// The factory class/service that is responsible for creating the reader
public abstract class DataService
{
    // Subclasses must implement this to return their specific reader
    protected abstract IDataReader CreateReader();

    public void Process(string source)
    {
        IDataReader reader = CreateReader();
        reader.Read(source);
    }
}

public class CsvDataService : DataService
{
    protected override IDataReader CreateReader() => new CsvDataReader();
}

public class SqlDataService : DataService
{
    protected override IDataReader CreateReader() => new SqlDataReader();
}