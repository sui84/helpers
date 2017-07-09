using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Data.Linq.Mapping;
using System.Text;

namespace Common.Utils
{

    public class ReflectionHelper {
        public static void CopyObject(object target, object source, params string[] excludedProperties) {
            if (source == null || target == null)
            {
                throw new ArgumentNullException("source/target object");
            }
            PropertyInfo[] props = source.GetType().GetProperties();
            foreach (PropertyInfo p in props) {
                if (excludedProperties != null && excludedProperties.Contains(p.Name)) {
                    continue;   //ignore the property if exists in the specified excluded properties
                }
                if (p.CanRead && p.PropertyType.Namespace == "System") {
                    PropertyInfo targetProperty = target.GetType().GetProperty(p.Name);
                    if (targetProperty != null && p.PropertyType == targetProperty.PropertyType && targetProperty.CanWrite
                            && !AreEqual(p.GetValue(source, null), targetProperty.GetValue(target, null))){
                        targetProperty.SetValue(target, p.GetValue(source, null), null);
                    }
                }
            }
        }

        public static void CopyObjectIgnoreNullable(object target, object source, params string[] excludedProperties)
        {
            if (source == null || target == null)
            {
                throw new ArgumentNullException("source/target object");
            }
            PropertyInfo[] props = source.GetType().GetProperties();
            foreach (PropertyInfo p in props)
            {
                if (excludedProperties != null && excludedProperties.Contains(p.Name))
                {
                    continue;   //ignore the property if exists in the specified excluded properties
                }
                if (p.CanRead && p.PropertyType.Namespace == "System")
                {
                    PropertyInfo targetProperty = target.GetType().GetProperty(p.Name);
                    if (targetProperty != null && (p.PropertyType == targetProperty.PropertyType || (p.PropertyType.Name == "DateTime" && typeof(Nullable<DateTime>) == targetProperty.PropertyType) || (p.PropertyType.Name == "Int32" && typeof(Nullable<Int32>) == targetProperty.PropertyType) || (p.PropertyType.Name == "Boolean" && typeof(Nullable<Boolean>) == targetProperty.PropertyType)) && targetProperty.CanWrite
                            && !AreEqual(p.GetValue(source, null), targetProperty.GetValue(target, null)))
                    {
                        targetProperty.SetValue(target, p.GetValue(source, null), null);
                    }
                    else if (targetProperty != null && p.GetValue(source, null)!=null && (p.PropertyType != targetProperty.PropertyType) && (p.PropertyType.Name == "String" && typeof(Nullable<Int32>) == targetProperty.PropertyType))
                    {
                        targetProperty.SetValue(target, p.GetValue(source, null) == null ? null : (int?)Convert.ToInt32(p.GetValue(source, null).ToString()), null);
                    }

                }
            }
        }

        public static void CopyObjectAnyNamespace(object target, object source)
        {
            if (source == null || target == null)
            {
                throw new ArgumentNullException("source/target object");
            }
            PropertyInfo[] props = target.GetType().GetProperties();
            foreach (PropertyInfo p in props)
            {
                try{
                    if (p.CanWrite)
                    {
                        PropertyInfo sourceProperty = source.GetType().GetProperty(p.Name);
                        if (sourceProperty != null && (p.PropertyType == sourceProperty.PropertyType 
                            || (p.PropertyType.Name == "DateTime" && typeof(Nullable<DateTime>) == sourceProperty.PropertyType)
                            || (sourceProperty.PropertyType.Name == "DateTime" && typeof(Nullable<DateTime>) == p.PropertyType) 
                            || (p.PropertyType.Name == "Boolean" && typeof(Nullable<Boolean>) == sourceProperty.PropertyType)
                            || (sourceProperty.PropertyType.Name == "Boolean" && typeof(Nullable<Boolean>) == p.PropertyType)
                            ) && sourceProperty.CanRead
                                && !AreEqual(p.GetValue(target, null), sourceProperty.GetValue(source, null)))
                        {
                            p.SetValue(target, sourceProperty.GetValue(source, null), null);
                        }
                        else if (sourceProperty != null && sourceProperty.GetValue(source, null) != null && (p.PropertyType != sourceProperty.PropertyType) && (p.PropertyType.Name == "String" ))
                        {
                            p.SetValue(target, sourceProperty.GetValue(source, null) == null ? null : sourceProperty.GetValue(source, null).ToString(), null);
                        }
                        else if (sourceProperty != null && sourceProperty.GetValue(source, null) != null && (p.PropertyType != sourceProperty.PropertyType) && (p.PropertyType.Name=="Int16" || typeof(Nullable<Int16>) == p.PropertyType ))
                        {
                            p.SetValue(target, sourceProperty.GetValue(source, null) == null ? null :(int?)Convert.ToInt16(sourceProperty.GetValue(source, null)), null);
                        }
                        else if (sourceProperty != null && sourceProperty.GetValue(source, null) != null && (p.PropertyType != sourceProperty.PropertyType) && (p.PropertyType.Name == "Int32" || typeof(Nullable<Int32>) == p.PropertyType ))
                        {
                            p.SetValue(target, sourceProperty.GetValue(source, null) == null ? null : (int?)Convert.ToInt32(sourceProperty.GetValue(source, null)), null);
                        }
                        else if (sourceProperty != null && sourceProperty.GetValue(source, null) != null && (p.PropertyType != sourceProperty.PropertyType) && (p.PropertyType.Name == "Int64"  || typeof(Nullable<Int64>) == p.PropertyType))
                        {
                            p.SetValue(target, sourceProperty.GetValue(source, null) == null ? null : (Int64?)Convert.ToInt64(sourceProperty.GetValue(source, null)), null);
                        }

                    }
                }
                catch (Exception ex){
                    LogHelper loghelper = new LogHelper();
                    loghelper.LogInfo(string.Format("Type:{0}\r\n{1}\r\n{2}", target.GetType().Name,ex.Message, ex.StackTrace), @"D:\TEMP\Log.txt");
                }
            }
        }

        public static object GetValueByProName(object obj, string proName)
        {
            object value = new object();
            PropertyInfo pro = obj.GetType().GetProperty(proName);
            if (pro != null) value=pro.GetValue(obj, null);
            return value;
        }

        public static void SetValueByProName(object obj, string proName , object value)
        {
            PropertyInfo targetProp = obj.GetType().GetProperty(proName);
            if (targetProp != null)
            {
                if (targetProp.PropertyType == typeof(decimal))
                {
                    targetProp.SetValue(obj, Convert.ToDecimal(value), null);
                }
                else if (targetProp.PropertyType == typeof(Int32))
                {
                    targetProp.SetValue(obj, Convert.ToInt32(value), null);
                }
                else if (targetProp.PropertyType == typeof(String))
                {
                    targetProp.SetValue(obj, value.ToString(), null);
                }
                else
                {
                    targetProp.SetValue(obj, value, null);
                }
            }
        }
        
        public static void CopyObjectWithSimilarProperty(object targetWithStringAndDecimalOnlyProperty, object sourceWithStringOnlyProperty, params string[] excludedTargetProperties) {
            object source = sourceWithStringOnlyProperty;
            object target = targetWithStringAndDecimalOnlyProperty;
            PropertyInfo[] targetProps = target.GetType().GetProperties();
            foreach (PropertyInfo targetProp in targetProps) {
                if (excludedTargetProperties != null && excludedTargetProperties.Contains(targetProp.Name)) {
                    continue;   //ignore the property if exists in the specified excluded properties
                }
                if (targetProp.CanRead && targetProp.PropertyType.Namespace == "System") {
                    PropertyInfo[] sourceProps = source.GetType().GetProperties();
                    foreach (PropertyInfo sourceProp in sourceProps) {
                        if ((sourceProp.Name.Contains(targetProp.Name) || targetProp.Name.Contains(sourceProp.Name))
                            && (targetProp != null && targetProp.CanWrite)) {
                            object sourceValue = sourceProp.GetValue(source, null);
                            object targetValue = targetProp.GetValue(target, null);
                            if (targetProp.PropertyType == typeof(decimal)) {
                                targetProp.SetValue(target,Convert.ToDecimal(sourceProp.GetValue(source, null).ToString()), null);
                            } else {
                                targetProp.SetValue(target, sourceProp.GetValue(source, null), null);
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Compare property value of source object and target object, return true if 2 object are same.
        /// If autoUpdateTarget is true, will auto update the target object based on source object.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="autoUpdateTarget"></param>
        /// <param name="ignoreReadOnlyProperty"></param>
        /// <param name="compareStr"></param>
        /// <param name="excludedProperties"></param>
        /// <returns></returns>
        public static bool CompareObject(object target, object source, bool autoUpdateTarget, bool ignoreReadOnlyProperty, out string compareStr, params string[] excludedProperties)
        {
            IList<string> unmatch;
            return CompareObject(target, source, autoUpdateTarget, ignoreReadOnlyProperty, out compareStr, out unmatch, excludedProperties);
        }

        /// <summary>
        /// Compare property value of source object and target object, return true if 2 object are same.
        /// If autoUpdateTarget is true, will auto update the target object based on source object.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <param name="autoUpdateTarget"></param>
        /// <param name="ignoreReadOnlyProperty"></param>
        /// <param name="compareStr"></param>
        /// <param name="unmatchedProperties"></param>
        /// <param name="excludedProperties"></param>
        /// <returns></returns>
        public static bool CompareObject(object target, object source, bool autoUpdateTarget, bool ignoreReadOnlyProperty, out string compareStr, out IList<string> unmatchedProperties, params string[] excludedProperties) {
            if (source == null || target == null) {
                throw new ArgumentNullException("source/target object");
            }
            PropertyInfo[] props = source.GetType().GetProperties();
            StringBuilder sb = new StringBuilder();
            unmatchedProperties = new List<string>();
            bool same = true;
            foreach (PropertyInfo p in props) {
                if (excludedProperties != null && excludedProperties.Contains(p.Name)) {
                    continue;   //ignore the property if exists in the specified excluded properties
                }
                if (!p.CanWrite && ignoreReadOnlyProperty) {
                    continue;   //ignore readonly property
                }
                if (p.CanRead && p.PropertyType.Namespace == "System") {
                    PropertyInfo targetProperty = target.GetType().GetProperty(p.Name);
                    if (targetProperty == null && p.PropertyType != targetProperty.PropertyType) {
                        continue;
                    }
                    object sourceValue = HandleNullalbe(p, p.GetValue(source, null));
                    object targetValue = HandleNullalbe(p, targetProperty.GetValue(target, null));
                    if (!AreEqual(sourceValue, targetValue)) {
                        unmatchedProperties.Add(p.Name);
                        sb.AppendFormat("{0}: Source[{1}] Target[{2}]; ", p.Name, sourceValue == null ? "" : sourceValue.ToString(), targetValue == null ? "" : targetValue.ToString());
                        same = false;
                        if (!autoUpdateTarget) {
                            // if no need to autoupdate, should return false once difference exists for performance
                            break;
                        } else if (targetProperty.CanWrite) {
                            targetProperty.SetValue(target, sourceValue, null);
                        }
                    }
                }
            }
            compareStr = sb.ToString();
            return same;
        }

        /// <summary>
        /// Object to Json string, this is not a complete method(ignore all non-system type properties)
        /// </summary>
        /// <param name="sourceObj"></param>
        /// <returns></returns>
        public static string ObjectToJson(object sourceObj)
        {
            if (sourceObj == null) return "{}";
            StringBuilder sb = new StringBuilder();
            PropertyInfo[] props = sourceObj.GetType().GetProperties();
            sb.Append("{\"$type\":").Append(sourceObj.GetType().FullName).Append(",");
            foreach (PropertyInfo pi in props)
            {
                if (pi.CanRead && pi.PropertyType.Namespace == "System")
                {
                    if (IsNumeric(pi) || IsBool(pi))
                    {
                        sb.AppendFormat("\"{0}\":{1},", pi.Name, pi.GetValue(sourceObj, null));
                    }
                    else
                    {
                        sb.AppendFormat("\"{0}\":\"{1}\",", pi.Name, pi.GetValue(sourceObj, null));
                    }
                }
            }
            sb.Remove(sb.Length - 1, 1);    //remove last ","
            sb.Append("}");
            return sb.ToString();
        }

        private static bool IsNumeric(PropertyInfo pi) {
            return pi.PropertyType == typeof(decimal) || pi.PropertyType == typeof(decimal?) ||
                pi.PropertyType == typeof(Int16) || pi.PropertyType == typeof(Int16?) ||
                pi.PropertyType == typeof(Int32) || pi.PropertyType == typeof(Int32?);
        }

        private static bool IsNumeric(object o) {
            return o.GetType() == typeof(decimal) || o.GetType() == typeof(decimal?) ||
                o.GetType() == typeof(Int16) || o.GetType() == typeof(Int16?) ||
                o.GetType() == typeof(Int32) || o.GetType() == typeof(Int32?);
        }

        private static bool IsBool(PropertyInfo pi)
        {
            return pi.PropertyType == typeof(bool) || pi.PropertyType == typeof(bool?);
        }

        private static object HandleNullalbe(PropertyInfo propertyInfo, object obj)
        {
            if (obj == null)
            {
                if (propertyInfo.PropertyType == typeof(decimal) || propertyInfo.PropertyType == typeof(decimal?))
                {
                    obj = (decimal)0;
                }
                else if (propertyInfo.PropertyType == typeof(Int16) || propertyInfo.PropertyType == typeof(Int16?))
                {
                    obj = (Int16)0;
                }
                else if (propertyInfo.PropertyType == typeof(Int32) || propertyInfo.PropertyType == typeof(Int32?))
                {
                    obj = (Int32)0;
                }
                else if (IsBool(propertyInfo))
                {
                    obj = false;
                }
                else if (propertyInfo.PropertyType == typeof(string))
                {
                    obj = string.Empty;
                }
            }            
            return obj;
        }


        /// <summary>
        /// Using object.Equal() to judge two objects are equal or not
        /// Specially, if both objects are null, return true; if only one object is null, return false.
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <returns></returns>
        public static bool AreEqual(object object1, object object2)
        {
            //decimal object1Decimal = 0;
            //decimal object2Decimal = 0;

            if (object1 == null && object2 == null)
            {
                return true;
            }
            else if (object1 == null ^ object2 == null)
            {
                return false;
            }
            //else if (Decimal.TryParse(object1.ToString(), out object1Decimal) && Decimal.TryParse(object2.ToString(), out object2Decimal)) {
            //    return object1Decimal == object2Decimal;
            //}
            else
            {
                return object1.Equals(object2);
            }
        }

        public static bool IsExistPropertiesBesides(Type typeOfObject, string[] subsetOfProps) {
            PropertyInfo[] props = typeOfObject.GetProperties();
            foreach (PropertyInfo p in props) {
                if (subsetOfProps.Where(r => r.Trim().ToLower() == p.Name.ToLower()).SingleOrDefault() == null) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get Method fully qualified name e.g. GetMyName becomes VML.ServiceRequest.Web.GetMyName
        /// </summary>
        /// <param name="method"> use new StackFrame().GetMethod() to get this parameter from your application.</param>
        /// <returns></returns>
        public static string GetMethodFullName(MethodBase method) {
            return method.ReflectedType.FullName + "." + method.Name;
        }

        public static void SetStaticProperty(Type type, String propertyName, String propertyValue) {
            PropertyInfo p = type.GetProperty(propertyName, (BindingFlags.Static | BindingFlags.Public));
            if (p != null && p.CanWrite) {
                if (p.PropertyType.Equals(typeof(StringCollection))) {
                    StringCollection stringCollection = new StringCollection();
                    stringCollection.AddRange(propertyValue.Split(",".ToCharArray()));
                    p.SetValue(type, stringCollection, null);
                } else {
                    p.SetValue(type, Convert.ChangeType(propertyValue, p.PropertyType), null);
                }
                return;
            }
        }

        public static List<KeyValuePair<string, string>> GetPublicStaticPropertiesInKeyValuePairs(Type classType) {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            foreach (FieldInfo field in classType.GetFields(BindingFlags.Public | BindingFlags.Static)) {
                KeyValuePair<string, string> kv = new KeyValuePair<string, string>(field.Name, field.GetValue(null).ToString());
                list.Add(kv);
            }
            return list;
        }

        public static Type GetType(string typeName) {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies()) {
                Type t = a.GetType(typeName);
                if (t != null) return t;
            }
            return null;
        }

        /// <summary>
        /// Set all non-nullable string column value to empty string of the specified Linq To Sql Entity
        /// </summary>
        /// <param name="linqToSqlEntity"></param>
        public static void SetNonNullableStringPropertiesToEmpty(object linqToSqlEntity)
        {
            PropertyInfo[] props = linqToSqlEntity.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(string) && prop.CanWrite && prop.GetSetMethod().IsPublic)
                {
                    object[] colAttrs = prop.GetCustomAttributes(typeof(ColumnAttribute), true);
                    if (colAttrs.Length > 0)
                    {
                        ColumnAttribute colAttr = (ColumnAttribute)colAttrs[0];
                        if (!colAttr.CanBeNull)
                        {
                            prop.SetValue(linqToSqlEntity, "", null);
                        }
                    }
                }
            }
        }
    }
}
