using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;


namespace Common.Utils.Security
{
    public  class SecurityHelper
    {
           // deny access if POST request comes from other server
       //public static void CheckRequestValidity(HttpRequest request)
       // {
       //   // ip with 
       //   // deny access if POST request comes from other server
       //   if (request.HttpMethod == "POST" && request.UrlReferrer != null && request.Url.Host != null && request.UrlReferrer.Host != request.Url.Host)
       //   {
       //     YafBuildLink.AccessDenied();
       //   }
      //  }

       //create Random password.
       public static string CreatePassword(int length)
       {
           string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!#%&()@${[]}";
           string res = string.Empty;
           var rnd = new Random();
           while (0 < length--)
           {
               res += valid[rnd.Next(valid.Length)];
           }

           return res;
       }

        //给散列算法加salt
        private string CreateSalt() 
        { 
        byte[] bytSalt = new byte[8]; 
        RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider(); 
        rng.GetBytes(bytSalt); 
        return Convert.ToBase64String(bytSalt); 
        }

       public static string SHA1EncryptString(string sourceString)
       {
           byte[] bytes = Encoding.UTF8.GetBytes(sourceString);
           SHA1 sha = new SHA1CryptoServiceProvider();
           string encryptedString = Convert.ToBase64String(sha.ComputeHash(bytes));
           return encryptedString;
       }

       // MD5
       #region
       public static string MD5Encrypt(string Message, string passphrase)
       {
           byte[] Results;
           System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
           MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
           byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(passphrase));
           TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
           TDESAlgorithm.Key = TDESKey;
           TDESAlgorithm.Mode = CipherMode.ECB;
           TDESAlgorithm.Padding = PaddingMode.PKCS7;
           byte[] DataToEncrypt = UTF8.GetBytes(Message);
           try
           {
               ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
               Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
           }
           finally
           {
               TDESAlgorithm.Clear();
               HashProvider.Clear();
           }
           return Convert.ToBase64String(Results);
       }

       public static string MD5Decrypt(string Message, string passphrase)
       {
           byte[] Results;
           System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
           MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
           byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(passphrase));
           TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();
           TDESAlgorithm.Key = TDESKey;
           TDESAlgorithm.Mode = CipherMode.ECB;
           TDESAlgorithm.Padding = PaddingMode.PKCS7;
           byte[] DataToDecrypt = Convert.FromBase64String(Message);
           try
           {
               ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
               Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
           }
           finally
           {
               TDESAlgorithm.Clear();
               HashProvider.Clear();
           }
           return UTF8.GetString(Results);
       }
       #endregion

        // DES(64位)
        #region
       public static byte[] DESEncrypt(string PlainText, SymmetricAlgorithm key)
       {
           // Create a memory stream.
           MemoryStream ms = new MemoryStream();

           // Create a CryptoStream using the memory stream and the 
           // CSP DES key.  
           CryptoStream encStream = new CryptoStream(ms, key.CreateEncryptor(), CryptoStreamMode.Write);

           // Create a StreamWriter to write a string
           // to the stream.
           StreamWriter sw = new StreamWriter(encStream);

           // Write the plaintext to the stream.
           sw.WriteLine(PlainText);

           // Close the StreamWriter and CryptoStream.
           sw.Close();
           encStream.Close();

           // Get an array of bytes that represents
           // the memory stream.
           byte[] buffer = ms.ToArray();

           // Close the memory stream.
           ms.Close();

           // Return the encrypted byte array.
           return buffer;
       }

       // Decrypt the byte array.
       public static string DESDecrypt(byte[] CypherText, SymmetricAlgorithm key)
       {
           // Create a memory stream to the passed buffer.
           MemoryStream ms = new MemoryStream(CypherText);

           // Create a CryptoStream using the memory stream and the 
           // CSP DES key. 
           CryptoStream encStream = new CryptoStream(ms, key.CreateDecryptor(), CryptoStreamMode.Read);

           // Create a StreamReader for reading the stream.
           StreamReader sr = new StreamReader(encStream);

           // Read the stream as a string.
           string val = sr.ReadLine();

           // Close the streams.
           sr.Close();
           encStream.Close();
           ms.Close();

           return val;
       }
        #endregion

        //RijndaelManaged
        #region
       /// Change the Inputkey GUID when you use this code in your own program.
       /// Keep this inputkey very safe and prevent someone from decoding it some way!!
       /// </summary>
       internal const string Inputkey = "560A18CD-6346-4CF0-A2E8-671F9B6B9EA9";

       public static string RijndaelEncrypt(string text, string salt)
       {
           if (string.IsNullOrEmpty(text))
               throw new ArgumentNullException("text");

           var aesAlg = NewRijndaelManaged(salt);

           var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
           var msEncrypt = new MemoryStream();
           using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
           using (var swEncrypt = new StreamWriter(csEncrypt))
           {
               swEncrypt.Write(text);
           }

           return Convert.ToBase64String(msEncrypt.ToArray());
       }

       public static string RijndaelDecrypt(string cipherText, string salt)
       {
           if (string.IsNullOrEmpty(cipherText))
               throw new ArgumentNullException("cipherText");

           if (!RegexHelper.IsBase64String(cipherText))
               throw new Exception("The cipherText input parameter is not base64 encoded");

           string text;

           var aesAlg = NewRijndaelManaged(salt);
           var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
           var cipher = Convert.FromBase64String(cipherText);

           using (var msDecrypt = new MemoryStream(cipher))
           {
               using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
               {
                   using (var srDecrypt = new StreamReader(csDecrypt))
                   {
                       text = srDecrypt.ReadToEnd();
                   }
               }
           }
           return text;
       }

       private static RijndaelManaged NewRijndaelManaged(string salt)
       {
           if (salt == null) throw new ArgumentNullException("salt");
           var saltBytes = Encoding.ASCII.GetBytes(salt);
           var key = new Rfc2898DeriveBytes(Inputkey, saltBytes);

           var aesAlg = new RijndaelManaged();
           aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
           aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

           return aesAlg;
       }

        #endregion

        //RSA sign
        #region
       public enum KeyType
       {
           PrivatePEM,
           PublicPEM,
           PrivateXML,
           PublicXML
       }

       public string SignData(string privateKeyStr, KeyType keyType, string dataStr)
       {
           byte[] data = Encoding.UTF8.GetBytes(dataStr);
           string base64 = string.Empty;
           if (keyType == KeyType.PrivatePEM)
           {

               string keyStr = GetKey(privateKeyStr, KeyType.PrivatePEM);
               byte[] privateKey = Helpers.GetBytesFromPEM(keyStr, PemStringType.RsaPrivateKey);
               RSACryptoServiceProvider rsa = Crypto.DecodeRsaPrivateKey(privateKey);
               byte[] hash = rsa.SignData(data, SHA1.Create());
               base64 = Convert.ToBase64String(hash);
           }
           if (keyType == KeyType.PrivateXML)
           {
               string privatekey = GetKey(privateKeyStr, keyType);
               RSACryptoServiceProvider oRSA3 = new RSACryptoServiceProvider();
               oRSA3.FromXmlString(privatekey);
               byte[] BOutput = oRSA3.SignData(data, "SHA1");
               base64 = Convert.ToBase64String(BOutput);
           }
           return base64;
       }

       public bool VerifySignData(RSACryptoServiceProvider rsa, KeyType keyType, string dataStr, byte[] dataHash)
       {
           byte[] data = Encoding.UTF8.GetBytes(dataStr);
           bool valid = false;
           if (keyType == KeyType.PublicPEM)
           {
               RSAParameters key = rsa.ExportParameters(false); // false:公钥  true:私钥
               RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
               RSAalg.ImportParameters(key);
               valid = RSAalg.VerifyData(data, new SHA1CryptoServiceProvider(), dataHash);
           }
           else if (keyType == KeyType.PublicXML)
           {
               string publickey = rsa.ToXmlString(false);//false:公钥  true:私钥
               valid = VerifySignData(publickey, keyType, dataStr, dataHash);
           }
           return valid;
       }

       public bool VerifySignData(string keyStr, KeyType keyType, string dataStr, byte[] dataHash)
       {
           byte[] data = Encoding.UTF8.GetBytes(dataStr);
           bool valid = false;
           if (keyType == KeyType.PublicXML)
           {
               RSACryptoServiceProvider oRSA4 = new RSACryptoServiceProvider();
               string publickey = GetKey(keyStr, KeyType.PublicXML);
               oRSA4.FromXmlString(publickey);
               valid = oRSA4.VerifyData(data, "SHA1", dataHash);
           }
           return valid;
       }

       private string GetKey(string keyStr, KeyType keyType)
       {
           string key = keyStr;
           if (string.IsNullOrWhiteSpace(keyStr))
           {
               string KeyFile = AppDomain.CurrentDomain.BaseDirectory;
               switch (keyType)
               {
                   case KeyType.PrivatePEM:
                       KeyFile += @"Keys\private.pem";
                       break;
                   case KeyType.PublicPEM:
                       KeyFile += @"Keys\public.pem";
                       break;
                   case KeyType.PrivateXML:
                       KeyFile += @"Keys\private.xml";
                       break;
                   case KeyType.PublicXML:
                       KeyFile += @"Keys\public.xml";
                       break;
                   default:
                       KeyFile += @"Keys\private.pem";
                       break;
               }
               key = File.ReadAllText(KeyFile);
           }
           return key;
       }
        #endregion
    }
}
