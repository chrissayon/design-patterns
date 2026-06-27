# Singleton Pattern

Singleton Pattern is a creational design pattern where only one instance of the object is ever present.

## What problem are we solving?

For classes such as logging, caching, or connections, these only need to be initialized once because multiple instances can cause problems in the code.

These all have unique issues, but we'll focus on logging.


```C#
class Logger(string outputFile)
{
    private List<string> _logs = new();

    public void Log(string message)
    {
        _logs.Add(message);
        File.AppendAllText(outputFile, message);
    }
}

class OrderService
{
    public void PlaceOrder()
    {
        Logger logger = new("output.txt");
        logger.Log("Order placed");
    }
}

class PaymentService
{
    public void ProcessPayment()
    {
        Logger logger = new("output.txt");
        logger.Log("Payment processed");
    }
}
```

The code above shows:
- A `Logger` class holds a list of `_logs` and writes to an `outputFile` text file
- The `Logger` class being initialized and used in the `OrderService` and `PaymentService`

The issue here is that both loggers are separate instances which now raises multiple issues when being used. 
- Separate Logs - the logs held in the `_logs` field will only contain the logging in that service, when you want it for both
- Corruption - Both loggers write to the same file, if both of them write at the same time then the data will be corrupt
- No Central Control - You'd need to update each logger separately and select a different file

## The solution

The singleton pattern solves these issues by introducing a `static` instance that is shared across the entire application. The class controls its own instantiation, ensuring only one instance can ever exist.

The `static` keyword causes the field to belong to the type (`Logger` in this instance), rather than the object. Hence, sharing the field across all instances. 

### How to implement

There are a few things we need to do:
- Create a `private static <CLASS> _instance` field to store the initialized object
- Create a `public static <CLASS> Instance` property to grab that instance, otherwise it will initialize the object  
- The constructor needs to be private so that the `<CLASS>` itself cannot be initialized directly and only initialized through `INSTANCE`

```C#
class Logger
{
    // The constructor is private so that no one is able to instantiate this 
    // via "new Logger()" so that it can only be instantiated via the Instance property
    private Logger(string outputFile)
    {
        _outputFile = outputFile;
    }

    // The instance of the logger is stored in a static field that is shared across all initializations
    private static Logger? _instance;
    private List<string> _logs = new();
    private string _outputFile;

    // The Instance property is created to get the logger, or initialize it if it doesn't exist
    public static Logger Instance
    {
        get
        {
            _instance ??= new Logger("output.txt");
            return _instance;
        }
    }

    public void Log(string message)
    {
        _logs.Add(message);
        File.AppendAllText(_outputFile, message);
    }
}

class OrderService
{
    public void PlaceOrder()
    {
        Logger.Instance.Log("Order placed");
    }
}

class PaymentService
{
    public void ProcessPayment()
    {
        Logger.Instance.Log("Payment processed");
    }
}
```

The code above shows the `Logger` created with a singleton pattern.

The difference here now is that the `Logger` isn't instantiated directly via the constructor, but done when `Logger.Instance` is called. 

`Logger.Instance` will try to obtain the current instance, otherwise it will initialize itself.

### Pros 
- There is only one single instance
- We only need to initialize the object once then call it back later
- The object can't be overwritten
- Accessible anywhere without needing to be passed around

### Cons
- Testing is difficult — because `_instance` is global state, one test can affect another. If a test modifies the logger, the next test inherits that state
- Not thread-safe by default — two threads could both check `_instance` at the same time before it's been set, and each create their own instance, defeating the pattern entirely
