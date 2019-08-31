using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Common.Utils.ConvertType;

namespace Common.Utils.Regedit
{
    public class RegHelper
    {
        public void Key(string keystr)
        {
            //在HKEY_LOCAL_MACHINE\SOFTWARE下新建名为test的注册表项。如果已经存在则不影响！
            RegistryKey key = Registry.LocalMachine;
            RegistryKey software = key.CreateSubKey(keystr);

           // RegistryKey key = Registry.LocalMachine;
          //  RegistryKey software = key.OpenSubKey("software\\test", true);
            //注意该方法后面还可以有一个布尔型的参数，true表示可以写入。

            //RegistryKey key = Registry.LocalMachine;
            //key.DeleteSubKey("software\\test", true); //该方法无返回值，直接调用即可
            //key.Close();
            
        }

        public void SetKey(string keystr)
        {
            RegistryKey key = Registry.LocalMachine;
            RegistryKey software = key.OpenSubKey("software\\test", true); //该项必须已存在
            software.SetValue("test", "博客园");
            //在HKEY_LOCAL_MACHINE\SOFTWARE\test下创建一个名为“test”，值为“博客园”的键值。如果该键值原本已经存在，则会修改替换原来的键值，如果不存在则是创建该键值。
            // 注意：SetValue()还有第三个参数，主要是用于设置键值的类型，如：字符串，二进制，Dword等等~~默认是字符串。如：
            // software.SetValue("test", "0", RegistryValueKind.DWord); //二进制信息
            key.Close();
        }

        public void GetKey(string keystr)
        {
            string info = "";
            RegistryKey Key;
            Key = Registry.LocalMachine;
            RegistryKey myreg = Key.OpenSubKey("software\\test");
            // myreg = Key.OpenSubKey("software\\test",true);
            info = myreg.GetValue("test").ToString();
            myreg.Close();
        }

        private bool IsRegeditItemExist()
        {
            string[] subkeyNames;
            RegistryKey hkml = Registry.LocalMachine;
            RegistryKey software = hkml.OpenSubKey("SOFTWARE");
            //RegistryKey software = hkml.OpenSubKey("SOFTWARE", true);  
            subkeyNames = software.GetSubKeyNames();
            //取得该项下所有子项的名称的序列，并传递给预定的数组中  
            foreach (string keyName in subkeyNames)
            //遍历整个数组  
            {
                if (keyName == "test")
                //判断子项的名称  
                {
                    hkml.Close();
                    return true;
                }
            }
            hkml.Close();
            return false;
        }

        public bool IsRegeditKeyExit(string keystr)
        {
            string[] subkeyNames;
            RegistryKey hkml = Registry.CurrentUser;
            RegistryKey software = hkml.OpenSubKey(keystr);
            //RegistryKey software = hkml.OpenSubKey("SOFTWARE\\test", true);
            subkeyNames = software.GetValueNames();
            //取得该项下所有键值的名称的序列，并传递给预定的数组中
            foreach (string keyName in subkeyNames)
            {
             //   byte[] bs = StringHelper.ObjectToBytes(software.GetValue("1"));
               // string str = StringHelper.ConvertBytesToString(bs);

               // software.GetValueKind("1")
                byte[] array = (byte[])software.GetValue("1");//获取DriverDesc值的字节数组
                string decoded = System.Text.Encoding.UTF8.GetString(array);//将字节数组转换成字符串
                decoded = decoded.Replace("\0", String.Empty);//由于将字节数组转换成字符串的过程中，一般会包含\0字符，所以要将它替换成空字符串，

                if (keyName == "test") //判断键值的名称
                {
                    hkml.Close();
                    return true;
                }

            }
            hkml.Close();
            return false;
        }
    }
}
