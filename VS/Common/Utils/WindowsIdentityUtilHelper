using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.DirectoryServices;

namespace Common.Utils
{
    public class WindowsIdentityUtil
    {
        private WindowsIdentityUtil() { }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(string lpszUsername,
            string lpszDomain, string lpszPassword,
            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private extern static bool CloseHandle(IntPtr handle);

        public static WindowsIdentity CreateIdentity(
            string userName, string domain, string password)
        {
            IntPtr tokenHandle = new IntPtr(0);

            const int LOGON32_PROVIDER_DEFAULT = 0;
            const int LOGON32_LOGON_NETWORK_CLEARTEXT = 3;

            tokenHandle = IntPtr.Zero;
            bool returnValue = LogonUser(userName, domain, password,
                LOGON32_LOGON_NETWORK_CLEARTEXT,
                LOGON32_PROVIDER_DEFAULT,
                ref tokenHandle);

            if (false == returnValue)
            {
                int ret = Marshal.GetLastWin32Error();
                throw new Exception("LogonUser failed with error code: " + ret);
            }

            WindowsIdentity id = new WindowsIdentity(tokenHandle);
            CloseHandle(tokenHandle);
            return id;
        }

        public static void EnableUserAccount(DirectoryEntry user)
        {
            try
            {
                int val = (int)user.Properties["userAccountControl"].Value;
                user.Properties["userAccountControl"].Value = val & ~0x2;
                //ADS_UF_NORMAL_ACCOUNT;

                user.CommitChanges();
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException E)
            {
                //DoSomethingWith --> E.Message.ToString();
            }
            finally
            {
                user.Close();
                user.Dispose();
            }
        }

        public static void DisableUserAccount(DirectoryEntry user)
        {
            try
            {
                int val = (int)user.Properties["userAccountControl"].Value;
                user.Properties["userAccountControl"].Value = val | 0x2;
                //ADS_UF_ACCOUNTDISABLE;

                user.CommitChanges();
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException E)
            {
                //DoSomethingWith --> E.Message.ToString();

            }
            finally
            {
                user.Close();
                user.Dispose();
            }
        }

        public static void UnlockUserAccount(DirectoryEntry user)
        {
            try
            {
                if (user != null) {
                    user.Properties["LockOutTime"].Value = 0; //unlock account
                    user.CommitChanges(); //may not be needed but adding it anyways
                }
            }
            catch (System.DirectoryServices.DirectoryServicesCOMException E)
            {
                //DoSomethingWith --> E.Message.ToString();

            }
            catch (System.UnauthorizedAccessException uae)
            {
                throw uae;

            }
            finally
            {
                if (user != null) {
                    user.Close();
                    user.Dispose();
                }
            }
        }

        public static void ResetPassword(DirectoryEntry user, string password, bool mustChangePasswordAtNextLogon)
        {
            try
            {
                user.Invoke("SetPassword", new object[] { password });
                user.Properties["LockOutTime"].Value = 0; //unlock account
                if (mustChangePasswordAtNextLogon)
                {
                    //uEntry.Properties["PasswordExpired"].Value = 1;
                    //int val = (int)user.Properties["userAccountControl"].Value;
                    //user.Properties["userAccountControl"].Value = val | 0x800000; //PASSWORD_EXPIRED
                    user.Properties["pwdLastSet"].Value = 0;
                    user.CommitChanges();
                }
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                throw new ApplicationException("Unable to reset account.", e.InnerException);
            }
            finally
            {
                user.Close();
                user.Dispose();
            }
        }

        public static bool IsUserLockedOut(DirectoryEntry user)
        {
            if (user == null) {
                return true;
            }
            return Convert.ToBoolean(user.InvokeGet("IsAccountLocked"));
        }
    }
}
