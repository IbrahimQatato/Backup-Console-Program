class Configuration{
  public string SourceFolderPath { get; set; }
  public string BackupFolderPath { get; set; }
  public int BackupInterval { get; set; }
  public string LogFilePath {get; set;}

  public Configuration(string sourceFolder, string backupFolder, int backupInterval, string logFilePath){
    SourceFolderPath = sourceFolder;
    BackupFolderPath = backupFolder;
    BackupInterval = backupInterval;
    LogFilePath = logFilePath;
  }
}
