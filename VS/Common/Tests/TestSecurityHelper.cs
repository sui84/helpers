using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Utils;
using System.Security.Cryptography;
using Common.Utils.Security;

namespace Common.Tests
{
    public class TestSecurityHelper
    {
        static void Main(string[] args)
        {
            string encstr = SecurityHelper.MD5Encrypt("asfgter65", "test");
            string msg = SecurityHelper.MD5Decrypt(encstr, "test");

            // Create a new DES key.
            DESCryptoServiceProvider key = new DESCryptoServiceProvider();
            byte[] bts = SecurityHelper.DESEncrypt("asfgter65", key);
            msg = SecurityHelper.DESDecrypt(bts, key);

            encstr = SecurityHelper.RijndaelEncrypt("asfgter65", "testtest");
            msg = SecurityHelper.RijndaelDecrypt(encstr, "testtest");

        }
    }
}
