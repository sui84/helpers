﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Common.Utils.Utfs;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Common.Utils.Ntfs
{
    [Flags]
    internal enum UsnErrorCode
    {
        SUCCESS = 0,
        ERROR_INVALID_FUNCTION = 0x1,
        ERROR_INVALID_PARAMETER = 0x57,
        ERROR_JOURNAL_DELETE_IN_PROGRESS = 0x49A,
        ERROR_JOURNAL_NOT_ACTIVE = 0x49B
    }

    public class UsnOperator : IDisposable
    {
        public delegate void GetEntriesHandler(DriveInfo drive, List<UsnEntry> data);

        protected Common.Utils.Utfs.Win32Api.USN_JOURNAL_DATA ntfsUsnJournalData;

        public DriveInfo Drive
        {
            get;
            private set;
        }

        public string DriveLetter
        {
            get
            {
                return this.Drive.Name.TrimEnd(new char[] { '\\', ':' });
            }
        }

        public IntPtr DriveRootHandle
        {
            get;
            private set;
        }

        public UsnOperator(DriveInfo drive)
        {
            if (string.Compare(drive.DriveFormat, "ntfs", true) != 0)
            {
                throw new ArgumentException("USN journal only exists in NTFS drive.");
            }

            this.Drive = drive;
            this.DriveRootHandle = this.GetRootHandle();
            this.ntfsUsnJournalData = new Win32Api.USN_JOURNAL_DATA();
        }

        public UsnJournalData GetUsnJournal()
        {
            UsnErrorCode usnErrorCode = this.QueryUSNJournal();

            UsnJournalData result = null;

            if (usnErrorCode == UsnErrorCode.SUCCESS)
            {
                result = new UsnJournalData(this.Drive, this.ntfsUsnJournalData);
            }

            return result;
        }

        public bool CreateUsnJournal(UInt64 maximumSize = 0x10000000, UInt64 allocationDelta = 0x100000)
        {
            uint bytesReturnedCount;

            var createUsnJournalData = new Win32Api.CREATE_USN_JOURNAL_DATA();
            createUsnJournalData.MaximumSize = maximumSize;
            createUsnJournalData.AllocationDelta = allocationDelta;

            int sizeCujd = Marshal.SizeOf(createUsnJournalData);

            IntPtr cujdBuffer = GetHeapGlobalPtr(sizeCujd);

            Marshal.StructureToPtr(createUsnJournalData, cujdBuffer, true);

            bool isSuccess = Win32Api.DeviceIoControl(
                this.DriveRootHandle,
                UsnControlCode.FSCTL_CREATE_USN_JOURNAL,
                cujdBuffer,
                sizeCujd,
                IntPtr.Zero,
                0,
                out bytesReturnedCount,
                IntPtr.Zero);

            Marshal.FreeHGlobal(cujdBuffer);

            return isSuccess;
        }

        public List<UsnEntry> GetEntries()
        {
            var result = new List<UsnEntry>();

            UsnErrorCode usnErrorCode = this.QueryUSNJournal();
            if (usnErrorCode == UsnErrorCode.SUCCESS)
            {
                Win32Api.MFT_ENUM_DATA mftEnumData = new Win32Api.MFT_ENUM_DATA();
                mftEnumData.StartFileReferenceNumber = 0;
                mftEnumData.LowUsn = 0;
                mftEnumData.HighUsn = this.ntfsUsnJournalData.NextUsn;
                int sizeMftEnumData = Marshal.SizeOf(mftEnumData);
                IntPtr ptrMftEnumData = GetHeapGlobalPtr(sizeMftEnumData);
                Marshal.StructureToPtr(mftEnumData, ptrMftEnumData, true);
                int ptrDataSize = sizeof(UInt64) + 10000;
                IntPtr ptrData = GetHeapGlobalPtr(ptrDataSize);
                uint outBytesCount;

                while (false != Win32Api.DeviceIoControl(
                    this.DriveRootHandle,
                    UsnControlCode.FSCTL_ENUM_USN_DATA,
                    ptrMftEnumData,
                    sizeMftEnumData,
                    ptrData,
                    ptrDataSize,
                    out outBytesCount,
                    IntPtr.Zero))
                {
                    // ptrData includes following struct:
                    //typedef struct
                    //{
                    //    USN             LastFileReferenceNumber;
                    //    USN_RECORD_V2   Record[1];
                    //} *PENUM_USN_DATA;

                    long purvalue = ptrData.ToInt64() + sizeof(long);
                    IntPtr ptrUsnRecord = new IntPtr(purvalue);

                    while (outBytesCount > 60)
                    {
                        var usnRecord = new USN_RECORD_V2(ptrUsnRecord);
                        result.Add(new UsnEntry(usnRecord));
                        ptrUsnRecord = new IntPtr(ptrUsnRecord.ToInt64() + usnRecord.RecordLength);
                        outBytesCount -= usnRecord.RecordLength;
                    }
                    Marshal.WriteInt64(ptrMftEnumData, Marshal.ReadInt64(ptrData, 0));
                }

                Marshal.FreeHGlobal(ptrData);
                Marshal.FreeHGlobal(ptrMftEnumData);
            }

            return result;
        }
        public void GetEntries(long usn, ulong fileNumber, GetEntriesHandler handler, int count)
        {
            List<UsnEntry> result = new List<UsnEntry>();
            UsnErrorCode usnErrorCode = this.QueryUSNJournal();
            if (usnErrorCode == UsnErrorCode.SUCCESS)
            {
                Win32Api.MFT_ENUM_DATA mftEnumData = new Win32Api.MFT_ENUM_DATA();
                mftEnumData.StartFileReferenceNumber = fileNumber;
                mftEnumData.LowUsn = 0;
                mftEnumData.HighUsn = this.ntfsUsnJournalData.NextUsn;
                int sizeMftEnumData = Marshal.SizeOf(mftEnumData);
                IntPtr ptrMftEnumData = GetHeapGlobalPtr(sizeMftEnumData);
                Marshal.StructureToPtr(mftEnumData, ptrMftEnumData, true);
                int ptrDataSize = sizeof(UInt64) + 10000;
                IntPtr ptrData = GetHeapGlobalPtr(ptrDataSize);
                uint outBytesCount;

                while (false != Win32Api.DeviceIoControl(
                    this.DriveRootHandle,
                    UsnControlCode.FSCTL_ENUM_USN_DATA,
                    ptrMftEnumData,
                    sizeMftEnumData,
                    ptrData,
                    ptrDataSize,
                    out outBytesCount,
                    IntPtr.Zero))
                {
                    long purvalue = ptrData.ToInt64() + sizeof(long);
                    IntPtr ptrUsnRecord = new IntPtr(purvalue);

                    while (outBytesCount > 60)
                    {
                        var usnRecord = new USN_RECORD_V2(ptrUsnRecord);

                        UsnEntry rec = new UsnEntry(usnRecord);
                        if (rec.FileReferenceNumber > fileNumber || rec.Usn > usn)
                        {
                            result.Add(rec);
                        }

                        ptrUsnRecord = new IntPtr(ptrUsnRecord.ToInt64() + usnRecord.RecordLength);
                        outBytesCount -= usnRecord.RecordLength;

                        if (result.Count >= count)
                        {
                            handler.Invoke(Drive, result);
                            result = new List<UsnEntry>();
                        }
                    }
                    Marshal.WriteInt64(ptrMftEnumData, Marshal.ReadInt64(ptrData, 0));
                }

                Marshal.FreeHGlobal(ptrData);
                Marshal.FreeHGlobal(ptrMftEnumData);
            }
            handler.Invoke(Drive, result);
        }

        public bool UsnIsExist(long usn)
        {
            bool rs = false;
            UsnErrorCode usnErrorCode = QueryUSNJournal();
            if (ntfsUsnJournalData.NextUsn < usn) return rs;

            if (usnErrorCode == UsnErrorCode.SUCCESS)
            {
                Win32Api.MFT_ENUM_DATA mftEnumData = new Win32Api.MFT_ENUM_DATA();
                mftEnumData.StartFileReferenceNumber = 0;
                mftEnumData.LowUsn = usn;
                mftEnumData.HighUsn = usn;
                int sizeMftEnumData = Marshal.SizeOf(mftEnumData);
                IntPtr ptrMftEnumData = GetHeapGlobalPtr(sizeMftEnumData);
                Marshal.StructureToPtr(mftEnumData, ptrMftEnumData, true);
                int ptrDataSize = sizeof(UInt64) + 10000;
                IntPtr ptrData = GetHeapGlobalPtr(ptrDataSize);
                uint outBytesCount;

                while (false != Win32Api.DeviceIoControl(
                    this.DriveRootHandle,
                    UsnControlCode.FSCTL_ENUM_USN_DATA,
                    ptrMftEnumData,
                    sizeMftEnumData,
                    ptrData,
                    ptrDataSize,
                    out outBytesCount,
                    IntPtr.Zero))
                {
                    IntPtr ptrUsnRecord = new IntPtr(ptrData.ToInt32() + sizeof(Int64));

                    while (outBytesCount > 60)
                    {
                        var usnRecord = new USN_RECORD_V2(ptrUsnRecord);
                        ptrUsnRecord = new IntPtr(ptrUsnRecord.ToInt32() + usnRecord.RecordLength);
                        outBytesCount -= usnRecord.RecordLength;
                        rs = true;
                    }
                    Marshal.WriteInt64(ptrMftEnumData, Marshal.ReadInt64(ptrData, 0));
                }

                Marshal.FreeHGlobal(ptrData);
                Marshal.FreeHGlobal(ptrMftEnumData);
            }
            return rs;
        }
        private static IntPtr GetHeapGlobalPtr(int size)
        {
            IntPtr buffer = Marshal.AllocHGlobal(size);
            Win32Api.ZeroMemory(buffer, size);

            return buffer;
        }

        private UsnErrorCode QueryUSNJournal()
        {
            int sizeUsnJournalData = Marshal.SizeOf(this.ntfsUsnJournalData);

            Win32Api.USN_JOURNAL_DATA tempUsnJournalData;

            uint bytesReturnedCount;

            bool isSuccess = Win32Api.DeviceIoControl(
                this.DriveRootHandle,
                UsnControlCode.FSCTL_QUERY_USN_JOURNAL,
                IntPtr.Zero,
                0,
                out tempUsnJournalData,
                sizeUsnJournalData,
                out bytesReturnedCount,
                IntPtr.Zero);


            this.ntfsUsnJournalData = tempUsnJournalData;

            //if (isSuccess)
            //{
            //    return tempUsnJournalData;
            //}
            //else
            //{
            //int win32ErrorCode = Marshal.GetLastWin32Error();
            //if (Enum.IsDefined(typeof(UsnErrorCode), win32ErrorCode))
            //{
            //    var usnErrorCode = (UsnErrorCode)win32ErrorCode;
            //}

            //    throw new IOException("Drive returned false for Query Usn Journal", new Win32Exception(win32ErrorCode));
            //}

            return (UsnErrorCode)Marshal.GetLastWin32Error();
        }

        private IntPtr GetRootHandle()
        {
            string volume = string.Format("\\\\.\\{0}:", this.DriveLetter);

            var result = Win32Api.CreateFile(
                volume,
                Win32Api.GENERIC_READ,
                Win32Api.FILE_SHARE_READ | Win32Api.FILE_SHARE_WRITE,
                IntPtr.Zero,
                Win32Api.OPEN_EXISTING,
                0,
                IntPtr.Zero);

            if (result.ToInt32() == Win32Api.INVALID_HANDLE_VALUE)
            {
                throw new IOException("Drive returned invalid root handle", new Win32Exception(Marshal.GetLastWin32Error()));
            }

            return result;
        }

        public void Dispose()
        {
            if (this.DriveRootHandle != null && this.DriveRootHandle.ToInt32() != Win32Api.INVALID_HANDLE_VALUE)
            {
                Win32Api.CloseHandle(this.DriveRootHandle);
            }
        }
    }
}
