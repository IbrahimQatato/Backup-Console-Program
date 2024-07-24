using static System.Console;

class Program {
  static void Main(string[] args){
    if (args.Length != 4){
      WriteLine("Usage: <source folder> <backup folder> <synchronization interval in milliseconds> <log file path>");
      return;
    }

    string sourceFolder = args[0];
    string backupFolder = args[1];
    
    if (!Directory.Exists(sourceFolder)){// check source folder exists
      WriteLine("Source folder doesn't exist");
      WriteLine("Exiting the program.");
      Environment.Exit(0);
    }else{
      HandleFolderReadPermission(sourceFolder);
    }
    
    if (!Directory.Exists(backupFolder)){// check backup folder and warn user
    try {
      Directory.CreateDirectory(backupFolder);
    }catch (ArgumentException ex){
      WriteLine($"Error: the path {backupFolder} is invalid");
      WriteLine(ex.Message);
      Environment.Exit(1);
    } catch(Exception ex){
      WriteLine(ex.Message);
      Environment.Exit(1);
    }
      WriteLine("Backup directory created");
    }
    else{
      HandleFolderWritePermission(backupFolder);
      HandleFolderReadPermission(backupFolder);
      WriteLine($"The backup directory already exists. Are you sure you want to use {backupFolder} as your backup? It will delete all files not found in the source: {sourceFolder} (y/N)?");
      string? input = Console.ReadLine();

      if (string.IsNullOrEmpty(input)){
        input = "n";
      }

      if (input.Equals("n", StringComparison.OrdinalIgnoreCase)){
        Console.WriteLine("Exiting the program.");
        Environment.Exit(0);
      }
      else if (!input.Equals("y", StringComparison.OrdinalIgnoreCase)){
        Console.WriteLine("Exiting the program.");
        Environment.Exit(0);
      }

    }
        
    if (!int.TryParse(args[2], out int intervalInSeconds) || intervalInSeconds <= 0){
      WriteLine("The synchronization interval must be a positive integer.");
      return;
    }

    //handle logFilePath
    string logFilePath = args[3];
    string logFileDirectory;
    try{
      logFileDirectory = Path.GetDirectoryName(logFilePath);
      if (!Directory.Exists(logFileDirectory)){
        Directory.CreateDirectory(logFileDirectory);
        WriteLine($"created logfile directory at {logFileDirectory}");
      }
    }catch (ArgumentException ex){
      WriteLine($"Error: the path {logFilePath} is invalid");
      WriteLine(ex.Message);
      Environment.Exit(1);
    } catch (Exception ex){
      WriteLine(ex.Message);
      Environment.Exit(2);
    }

    WriteLine("Running main program... (You can press Enter at any time to stop)");

    Configuration config = new Configuration(sourceFolder, backupFolder, intervalInSeconds, logFilePath);
    Synchronizer synchronizer = new Synchronizer(config);

    synchronizer.Run();
  }

  static void HandleFolderWritePermission(string directoryPath){
    if(!HasWritePermission(directoryPath)){
        WriteLine($"Not authorised to write to {directoryPath}, please check the permissions");
        WriteLine("Exiting the program.");
        Environment.Exit(0);
      }
  }
  static void HandleFolderReadPermission(string directoryPath){
    if(!HasReadPermission(directoryPath)){
        WriteLine($"Not authorised to read from {directoryPath}, please check the permissions");
        WriteLine("Exiting the program.");
        Environment.Exit(0);
      }
  }

  static bool HasReadPermission(string directoryPath)
    {
        try
        {
            Directory.GetFiles(directoryPath);
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    static bool HasWritePermission(string directoryPath)
    {
        try
        {
            string testFilePath = Path.Combine(directoryPath, Path.GetRandomFileName());
            using (FileStream fs = File.Create(testFilePath, 1, FileOptions.DeleteOnClose)) { }
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

}
// possible improvement: check if source contains a file named like logFile to avoid overwriting

