using System.Security.Cryptography;

class HashGenerator{
  public string GenerateMD5(string filePath){
    using (var md5 = MD5.Create()){
      using (var stream = File.OpenRead(filePath)){
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
      }
    }
  }
}
