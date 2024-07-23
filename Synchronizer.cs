using System;
using System.Timers;
class Synchronizer{

  private Configuration _config;
  private HashGenerator _hashgenerator;
  private Logger _logger;
  private System.Timers.Timer _timer;
  //add flag for absolute path vs relative path

  public Synchronizer(Configuration config){
    _config = config;
    _hashgenerator = new HashGenerator();
    _logger = new Logger(_config.LogFilePath);
    _timer = new System.Timers.Timer(_config.BackupInterval);
  }
  public void CopyFile(string sourcePath, string backupPath){
    Directory.CreateDirectory(Path.GetDirectoryName(backupPath));
    File.Copy(sourcePath, backupPath, true);
  }

  public void Run(){
    _timer.Elapsed += PeriodicCheck;
    _timer.AutoReset = true;
    _timer.Enabled = true;
    
    
    Console.ReadLine(); // Keep the application running
  }

  public void PeriodicCheck(Object s, ElapsedEventArgs e){
    
    Console.WriteLine($"Running periodic check at {e.SignalTime}");

    HashSet<string> sourceFiles = GetFiles(_config.SourceFolderPath);
    HashSet<string> backupFiles = GetFiles(_config.BackupFolderPath);

    List<string> missingFiles = sourceFiles.Except(backupFiles).ToList();

    foreach (string file in missingFiles)
    {
      string sourceFilePath = Path.Combine(_config.SourceFolderPath, file);
      string targetFilePath = Path.Combine(_config.BackupFolderPath, file);
      CopyFile(sourceFilePath, targetFilePath);
      //log that the file has been added 
      _logger.LogAdd(file);
    }
    
    List<string> deletedFiles = backupFiles.Except(sourceFiles).ToList();
    deletedFiles.Remove(Path.GetFileName(_config.LogFilePath));// removes the log file from the list if it is in the backup directory

    foreach (string file in deletedFiles)
    {
      string targetFilePath = Path.Combine(_config.BackupFolderPath, file);
      File.Delete(targetFilePath);
      _logger.LogDelete(file);
    }

    List<string> previouslyAddedFiles = sourceFiles.Except(missingFiles).Except(deletedFiles).ToList();
    
    foreach (string file in previouslyAddedFiles)
    {
      //see if checksum is same otherwise copy and log the update
      string sourceFilePath = Path.Combine(_config.SourceFolderPath, file);
      string targetFilePath = Path.Combine(_config.BackupFolderPath, file);
      if (FileChanged(sourceFilePath, targetFilePath)){
        CopyFile(sourceFilePath, targetFilePath);
        _logger.LogUpdate(file);
      }
    }

  }

  private bool FileChanged(string sourceFile, string targetFile){
    string sourceHash = _hashgenerator.GenerateMD5(sourceFile);
    string targetHash = _hashgenerator.GenerateMD5(targetFile);
    return !(sourceHash.Equals(targetHash));
  }

  private HashSet<string> GetFiles(string directory){
    HashSet<string> fileSet = new HashSet<string>();

    foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
    {
      string relativePath = file.Substring(directory.Length+1); // take the filename only
      fileSet.Add(relativePath);
    }
    return fileSet;
  }

  private void CheckFilesExist(){
    HashSet<string> sourceFiles = GetFiles(_config.SourceFolderPath);
    HashSet<string> backupFiles = GetFiles(_config.BackupFolderPath);

    List<string> missingFiles = sourceFiles.Except(backupFiles).ToList();

  }
}
