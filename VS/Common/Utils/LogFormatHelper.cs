using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace  Common
{
    public class LogFormatHelper
    {
        public static string GetMethodDetail(MethodBase method){
                     // MethodBase.GetCurrentMethod().Name;
            StringBuilder methodDetail = new StringBuilder(method.Name); ;
            ParameterInfo[] paras = method.GetParameters();
            StringBuilder parasStr = new StringBuilder(); 
            foreach (ParameterInfo para in paras)
            {
                parasStr.Append(string.Format("{0} {1},",para.ParameterType.Name.ToString(),para.Name));
            }
            if( paras.Length > 0) parasStr.Remove(parasStr.Length - 1, 1);
            methodDetail.Append("(").Append(parasStr.ToString()).Append(")");
            return methodDetail.ToString();
        }
        
    
        public static Dictionary<string, string> GetLogInfo(string methodName)
        {
            Dictionary<string, string> dics = new Dictionary<string, string> { { "MethodName", methodName }};
            return dics;
        }

        public static Dictionary<string, string> GetLogInfo(string methodName, TimeSpan elapsed)
        {
            Dictionary<string, string> dics = GetLogInfo(methodName);
            dics.Add("RequestElapsed", elapsed.ToString());
            return dics;
        }

        public static void LogServiceError(Exception ex, string methodName, params object[] objs)
        {
            try
            {
                Dictionary<string, string> dics = LogFormatHelper.GetLogInfo(methodName);
                LogServiceError(ex, dics,objs);
            }
            catch (Exception innerExp)
            {
                innerExp.Data.Add("Logger error", innerExp.Message);
                AppLogger.LogError(innerExp);
            }
        }

        public static void LogServiceError(Exception ex, string methodName, TimeSpan elapsed, params object[] objs)
        {
            try
            {
                Dictionary<string, string> dics = LogFormatHelper.GetLogInfo(methodName, elapsed);
                LogServiceError(ex, dics, objs);
            }
            catch (Exception innerExp)
            {
                innerExp.Data.Add("Logger error", innerExp.Message);
                AppLogger.LogError(innerExp);
            }
        }

        public static void LogServiceError(Exception ex, Dictionary<string, string> dics, params object[] objs)
        {
            try
            {
                AddParametersToError(ex, dics , objs);
                AppLogger.LogError(ex);
            }
            catch (Exception innerExp)
            {
                innerExp.Data.Add("Logger error", innerExp.Message);
                AppLogger.LogError(innerExp);
            }
        }

        public static void LogFunction(string functionName)
        {
            try
            {
                AppLogger.Write(functionName);
            }
            catch (Exception innerExp)
            {
                innerExp.Data.Add("Logger error", innerExp.Message);
                AppLogger.LogError(innerExp);
            }
        }

        public static void LogFunction(string functionName, TimeSpan elapsed)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("FunctionName:{0}\r\nFunctionElapsed:{1}\r\n", functionName, elapsed);
                AppLogger.Write(sb);
            }
            catch (Exception innerExp)
            {
                innerExp.Data.Add("Logger error", innerExp.Message);
                AppLogger.LogError(innerExp);
            }
        }

        public static void LogRequestParams(string methodName, params object[] objs)
        {
            try
            {
                Dictionary<string, string> dics = GetLogInfo(methodName);
                LogRequestParams(dics, objs);
            }
            catch (Exception innerExp)
            {
                innerExp.Data.Add("Logger error", innerExp.Message);
                AppLogger.LogError(innerExp);
            }
        }

        public static void LogRequestParams(string methodName, TimeSpan elapsed , params object[] objs)
        {
            try
            {
                Dictionary<string, string> dics = GetLogInfo(methodName, elapsed);
                LogRequestParams(dics, objs);
            }
            catch (Exception innerExp)
            {
                innerExp.Data.Add("Logger error", innerExp.Message);
                AppLogger.LogError(innerExp);
            }
        }

        public static void LogRequestParams(Dictionary<string,string> dics,  params object[] objs)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var dic in dics)
                {
                    sb.AppendFormat("{0} = {1}", dic.Key, dic.Value).AppendLine();
                    //LogRequestParams(obj);
                }
                sb.Append("Other objects =").AppendLine();
                foreach (object obj in objs)
                {
                    LogRequestParams(sb ,obj);
                }
                AppLogger.Write(sb.ToString());
            }
            catch (Exception innerExp)
            {
                innerExp.Data.Add("Logger error", innerExp.Message);
                AppLogger.LogError(innerExp);
            }
        }

        public static void LogRequestParams(StringBuilder sb,object obj)
        {
            try
            {
                Dictionary<string, object> result = BuildParametersDictionary(obj, "");
             //   StringBuilder sb = new StringBuilder();
             //   if (elapsed != null) sb.AppendFormat("Request Elapsed: {0}", elapsed).AppendLine();
                sb.AppendFormat("{0}:", obj.GetType().Name).AppendLine();
                foreach (var p in result)
                {
                    sb.AppendFormat("{0} = {1}", p.Key, p.Value).AppendLine();
                }
                
            }
            catch (Exception innerExp)
            {
                innerExp.Data.Add("Logger error", innerExp.Message);
                AppLogger.LogError(innerExp);
            }
        }

        public static void AddParametersToError(Exception ex, Dictionary<string,string> dics ,params object[] objs)
        {
            foreach (var dic in dics)
           {
                ex.Data.Add(dic.Key, dic.Value);
           }

           foreach (object obj in objs)
           {
                 AddParametersToError(ex,obj,"");
           }

        }

        public static void AddParametersToError(Exception ex, object obj, string propertyNamePrefix)
        {
            if (obj == null)
                return;

            Dictionary<string, object> parameters = BuildParametersDictionary(obj, propertyNamePrefix);
            foreach (var p in parameters)
            {
                ex.Data.Add(p.Key, p.Value);
            }
        }

        private static Dictionary<string, object> BuildParametersDictionary(object obj, string propertyNamePrefix)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (obj == null)
                return result;

            if (obj is string || obj is DateTime || obj is Boolean)
            {
                result.Add(obj.GetType().ToString(), obj.ToString());
                return result;
            }

            var properties = obj.GetType().GetProperties();
            foreach (PropertyInfo p in properties)
            {
                Type pType = p.GetType();
                object pValue = p.GetValue(obj, null);
                if (pValue == null)
                {
                    string pName = string.IsNullOrEmpty(propertyNamePrefix) ? p.Name : propertyNamePrefix + "." + p.Name;
                    result.Add(pName, "null");
                }
                else if (pType.IsValueType || pValue is string || pValue is DateTime || pValue is Boolean)
                {
                    string pName = string.IsNullOrEmpty(propertyNamePrefix) ? p.Name : propertyNamePrefix + "." + p.Name;
                    if (p.Name.ToLower() == "password" || p.Name.ToLower() == "dateofbirth" || p.Name.ToLower() == "encrykey")
                        result.Add(pName, "******");
                    else
                        result.Add(pName, pValue);
                }
                else if (pType.FullName.Contains(typeof(List<>).FullName))
                {
                    var list = obj as System.Collections.ICollection;
                    if (list != null && list.Count > 0)
                    {
                        int index = 0;
                        foreach (object objInList in list)
                        {
                            if (objInList is ValueType || objInList is string)
                                result.Add(p.Name + "[" + index.ToString() + "]", objInList);
                            else
                            {
                                Dictionary<string, object> childResult = BuildParametersDictionary(objInList, p.Name + "[" + index.ToString() + "]");
                                foreach (var e in childResult)
                                {
                                    result.Add(e.Key, e.Value);
                                }
                            }
                            index++;
                        }
                    }
                }
                else if (pValue.GetType().FullName.Contains(typeof(List<>).FullName))
                {
                    var list = pValue as System.Collections.ICollection;
                    if (list != null && list.Count > 0)
                    {
                        int index = 0;
                        foreach (object objInList in list)
                        {
                            if (objInList is ValueType || objInList is string)
                                result.Add(p.Name + "[" + index.ToString() + "]", objInList);
                            else
                            {
                                Dictionary<string, object> childResult = BuildParametersDictionary(objInList, p.Name + "[" + index.ToString() + "]");
                                foreach (var e in childResult)
                                {
                                    result.Add(e.Key, e.Value);
                                }
                            }
                            index++;
                        }
                    }
                }
            }
            return result;
        }
    }
}
