using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using Common.Utils;
using Newtonsoft.Json;
using Common.Utils.ConvertType;

namespace Common.Utils.Web
{
    /// <summary>
    /// JqGrid表格 表头属性
    /// <author>
    ///		<name>shecixiong</name>
    ///		<date>2014.09.15</date>
    /// </author>
    /// </summary>
    public class JqGridColumn
    {
        /// <summary>
        /// 定义表格单元格（非表头）的对齐方式，可取值：left, center, right
        /// </summary>
        public string align { get; set; }
        /// <summary>
        /// 此属性用于定义列的类名，当有多个类名时，用空格间隔，例如：“class1 class2”。在表格的CSS中，有一个预定义的类ui-ellipsis用于定义特定的行
        /// </summary>
        public string classes { get; set; }
        /// <summary>
        /// 日期格式，可用/，-和.。作为间隔符。y、Y、yyyy用于4位年，YY、yy用于2位的月，d、dd用于日期
        /// </summary>
        public string datefmt { get; set; }
        /// <summary>
        /// 搜索字段的缺省值，此参数只用于自定义搜索是的初始值。
        /// </summary>
        public string defval { get; set; }
        /// <summary>
        /// 定义字段是否可编辑，用于单元格编辑、行编辑和表单模式
        /// </summary>
        public bool editable { get; set; }
        /// <summary>
        /// 根据edittype 参数定义可用的值数组
        /// </summary>
        public string[] editoptions { get; set; }
        /// <summary>
        /// 设置可编辑字段的补充规则
        /// </summary>
        public string[] editrules { get; set; }
        /// <summary>
        /// 定义行编辑和表单模式的编辑类型，可以是text、textarea、select、checkbox、 password、button、image和file。
        /// </summary>
        public string edittype { get; set; }
        /// <summary>
        /// 定义表单编辑的各种选项
        /// </summary>
        public string[] formoptions { get; set; }
        /// <summary>
        /// 定义初始化时，列是否隐藏。
        /// </summary>
        public string hidden { get; set; }
        /// <summary>
        /// 通过sidx参数设置排序时的索引名。
        /// </summary>
        public string index { get; set; }
        /// <summary>
        /// 当colNames数组为空时，定义此列的标题。若colNames数组和此属性都为空，标题为该列的name属性值。
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// 设置列在表格中的唯一名称，此属性是必须的。或者使用保留字subgrid、cb和rn.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 定义是否可变列宽
        /// </summary>
        public bool resizable { get; set; }
        /// <summary>
        /// 定义是否可以排序
        /// </summary>
        public string sortable { get; set; }
        /// <summary>
        /// 当datatype为local时，用于定义排序列类型。可取int/integer（整数）、float/number/currency（小数）、date（日期）、text（文本）
        /// </summary>
        public string sorttype { get; set; }
        /// <summary>
        /// 当设置为false时，鼠标滑向单元格时不显示title属性
        /// </summary>
        public bool title { get; set; }
        /// <summary>
        /// 设置列的初始宽度，可用pixels和百分比
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// 自定义格式化
        /// </summary>
        public string formatter { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public int? SortCode { get; set; }
        /// <summary>
        /// 有效
        /// </summary>
        public int? Enabled { get; set; }
    }
    /// <summary>
    /// 导出Excel帮助类
    /// </summary>
    public class DeriveExcelHelper
    {
        /// <summary>
        /// IList导出Excel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">集合</param>
        /// <param name="DataColumn">字段</param>
        /// <param name="fileName"></param>
        public static void ListToExcel<T>(IList list, string[] DataColumn, string fileName)
        {
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            HttpContext.Current.Response.Charset = "Utf-8";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName + ".xls", System.Text.Encoding.UTF8));
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            sbHtml.AppendLine("<table cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\">");
            //写出列名
            sbHtml.AppendLine("<tr style=\"background-color: #FFE88C;font-weight: bold; white-space: nowrap;\">");
            foreach (string item in DataColumn)
            {
                string[] stritem = item.Split(':');
                sbHtml.AppendLine("<td>" + stritem[0] + "</td>");
            }
            sbHtml.AppendLine("</tr>");
            //写数据
            foreach (T entity in list)
            {
                Hashtable ht = HashtableHelper.GetModelToHashtable<T>(entity);
                sbHtml.Append("<tr>");
                foreach (string item in DataColumn)
                {
                    string[] stritem = item.Split(':');
                    sbHtml.Append("<td>").Append(ht[stritem[1]]).Append("</td>");
                }
                sbHtml.AppendLine("</tr>");
            }
            sbHtml.AppendLine("</table>");
            HttpContext.Current.Response.Write(sbHtml.ToString());
            HttpContext.Current.Response.End();
        }
        /// <summary>
        /// DataTable导出Excel
        /// </summary>
        /// <param name="data">集合</param>
        /// <param name="DataColumn">字段</param>
        /// <param name="fileName">文件名称</param>
        public static void DataTableToExcel(DataTable data, string[] DataColumn, string fileName)
        {
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            HttpContext.Current.Response.Charset = "Utf-8";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName + ".xls", System.Text.Encoding.UTF8));
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            sbHtml.AppendLine("<table cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\">");
            //写出列名
            sbHtml.AppendLine("<tr style=\"background-color: #FFE88C;font-weight: bold; white-space: nowrap;\">");
            foreach (string item in DataColumn)
            {
                sbHtml.AppendLine("<td>" + item + "</td>");
            }
            sbHtml.AppendLine("</tr>");
            //写数据
            foreach (DataRow row in data.Rows)
            {
                sbHtml.Append("<tr>");
                foreach (string item in DataColumn)
                {
                    sbHtml.Append("<td>").Append(row[item]).Append("</td>");
                }
                sbHtml.AppendLine("</tr>");
            }
            sbHtml.AppendLine("</table>");
            HttpContext.Current.Response.Write(sbHtml.ToString());
            HttpContext.Current.Response.End();
        }
        /// <summary>
        /// Table标签导出Excel
        /// </summary>
        /// <param name="sbHtml">html标签</param>
        /// <param name="fileName">文件名</param>
        public static void HtmlToExcel(StringBuilder sbHtml, string fileName)
        {
            if (sbHtml.Length > 0)
            {
                HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
                HttpContext.Current.Response.Charset = "Utf-8";
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName + ".xls", System.Text.Encoding.UTF8));
                HttpContext.Current.Response.Write(sbHtml.ToString());
                HttpContext.Current.Response.End();
            }
        }
        /// <summary>
        /// JqGrid导出Excel
        /// </summary>
        /// <param name="JsonColumn">表头</param>
        /// <param name="JsonData">数据</param>
        /// <param name="ExportField">导出字段</param>
        /// <param name="fileName">文件名</param>
        public static void JqGridToExcel(string JsonColumn, string JsonData, string ExportField, string fileName)
        {
            List<JqGridColumn> ListColumn = JsonConvert.DeserializeObject<List<JqGridColumn>>(JsonColumn);
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.UTF8;
            HttpContext.Current.Response.Charset = "Utf-8";
            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName + ".xls", System.Text.Encoding.UTF8));
            StringBuilder sbHtml = new StringBuilder();
            sbHtml.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
            sbHtml.AppendLine("<table cellspacing=\"0\" cellpadding=\"5\" rules=\"all\" border=\"1\">");
            //写出列名
            sbHtml.AppendLine("<tr style=\"background-color: #FFE88C;font-weight: bold; white-space: nowrap;\">");
            string[] FieldInfo = ExportField.Split(',');
            foreach (string item in FieldInfo)
            {
                IList list = ListColumn.FindAll(t => t.name == item);
                foreach (JqGridColumn jqgridcolumn in list)
                {
                    if (jqgridcolumn.hidden.ToLower() == "false" && jqgridcolumn.label != null)
                    {
                        sbHtml.AppendLine("<td style=\"width:" + jqgridcolumn.width + "px;text-align:" + jqgridcolumn.align + "\">" + jqgridcolumn.label + "</td>");
                    }
                }
            }
            sbHtml.AppendLine("</tr>");
            //写数据
            DataTable dt = JsonData.JsonToDataTable();
            foreach (DataRow row in dt.Rows)
            {
                sbHtml.Append("<tr>");
                foreach (string item in FieldInfo)
                {
                    IList list = ListColumn.FindAll(t => t.name == item);
                    foreach (JqGridColumn jqgridcolumn in list)
                    {
                        if (jqgridcolumn.hidden.ToLower() == "false" && jqgridcolumn.label != null)
                        {
                            object text = row[jqgridcolumn.name];
                            sbHtml.Append("<td style=\"width:" + jqgridcolumn.width + "px;text-align:" + jqgridcolumn.align + "\">").Append(text).Append("</td>");
                        }
                    }
                }
                sbHtml.AppendLine("</tr>");
            }
            sbHtml.AppendLine("</table>");
            HttpContext.Current.Response.Write(sbHtml.ToString());
            HttpContext.Current.Response.End();
        }
    }
}
