class Logger
{
    private string _logFilePath;

    public Logger(string logFilePath){
      _logFilePath = logFilePath;
    }

    public void LogAdd(string filename){
      Log($"Added File: {filename}");
    }

    public void LogUpdate(string filename){
      Log($"Updated File: {filename}");
    }

    public void LogDelete(string filename){
      Log($"Deleted File: {filename}");
    }

    public void Log(string message)
    {
        using (StreamWriter writer = new StreamWriter(_logFilePath, true)){
            writer.WriteLine($"{DateTime.Now}: {message}");
        }
        Console.WriteLine($"{DateTime.Now}: {message}");
    }
}
