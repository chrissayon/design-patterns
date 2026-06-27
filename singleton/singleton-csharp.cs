// Run
OrderService order = new();
order.PlaceOrder();

PaymentService payment = new();
payment.ProcessPayment();

//C Classes

public class Logger
{
    private Logger(string outputFile)
    {
        _outputFile = outputFile;
    }

    private static Logger? _instance;
    private List<string> _logs = new();
    private string _outputFile;

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

public class OrderService
{
    public void PlaceOrder()
    {
        Logger.Instance.Log("Order placed\n");
    }
}

public class PaymentService
{
    public void ProcessPayment()
    {
        Logger.Instance.Log("Payment processed\n");
    }
}
