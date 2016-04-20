using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;

using WordMLHelperUtil.Entity;

namespace WordMLHelperUtil
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    internal static class ExtensionMethod
    {
        /// <summary>
        /// 去除字符串中的所有空格
        /// </summary>
        /// <param name="originalStr"></param>
        /// <returns></returns>
        internal static string TrimAllSpace(this string str)
        {
            return Regex.Replace(str, @"\s+", string.Empty);
        }

        /// <summary>
        /// 检查字符串是否全由数字组成
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static bool IsNumeric(this string str)
        {
            long num;
            return long.TryParse(str, out num);
        }

        /// <summary>
        /// 检查字符串数组中是否包含某字符串（不区分大小写）
        /// </summary>
        /// <param name="strArray"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static bool Contains(this string[] strArray, string str)
        {
            bool result = false;
            foreach (string item in strArray)
            {
                if (string.Equals(str, item, StringComparison.OrdinalIgnoreCase))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// 连接字符串并清空字符串中的空白字符
        /// </summary>
        /// <param name="txts"></param>
        /// <returns></returns>
        internal static string CatenateAllTrimString(this IEnumerable<Text> txts)
        {
            StringBuilder str = new StringBuilder();
            foreach (var txt in txts)
            {
                str.Append(txt.Text);
            }
            return str.ToString().TrimAllSpace();
        }

        /// <summary>
        /// 排除类型为参数列表中的类型的对象
        /// </summary>
        /// <param name="source"></param>
        /// <param name="exceptedEls"></param>
        /// <returns></returns>
        internal static List<AbstractInfo> Except(this List<AbstractInfo> source, List<FillContentType> exceptedEls)
        {
            List<AbstractInfo> result = new List<AbstractInfo>();
            for (int i = 0; i < source.Count; i++)
            {
                if (!exceptedEls.Contains(source[i].FillContentType))
                {
                    result.Add(source[i]);
                }
            }

            return result;
        }
        
        /// <summary>
        /// 返回集合中的最后一个元素
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static T LastEl<T>(this List<T> source)
        {
            return source[source.Count - 1];
        }

        /// <summary>
        /// 获取某个WordprocessingML对象的第一个子元素
        /// </summary>
        /// <typeparam name="K">子元素类型</typeparam>
        /// <param name="source">某个WordprocessingML对象</param>
        /// <returns>第一个子元素</returns>
        internal static K FirstChildOrDefault<K>(this OpenXmlElement source) 
            where K : OpenXmlElement
        {
            IEnumerable<K> children = source.Elements<K>();
            K result = null;
            foreach (K k in children)
            {
                result = k;
                break;
            }

            return result;
        }

        /// <summary>
        /// 从HighlightColor转换为HighlightColorValues
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static HighlightColorValues ToHighlightColorValues(this HighlightColor source)
        {
            HighlightColorValues result = HighlightColorValues.None;

            switch(source)
            {
                case HighlightColor.Black:
                    result = HighlightColorValues.Black;
                    break;
                case HighlightColor.Blue:
                    result = HighlightColorValues.Blue;
                    break;
                case HighlightColor.Cyan:
                    result = HighlightColorValues.Cyan;
                    break;
                case HighlightColor.DarkBlue:
                    result = HighlightColorValues.DarkBlue;
                    break;
                case HighlightColor.DarkCyan:
                    result = HighlightColorValues.DarkCyan;
                    break;
                case HighlightColor.DarkGray:
                    result = HighlightColorValues.DarkGray;
                    break;
                case HighlightColor.DarkGreen:
                    result = HighlightColorValues.DarkGreen;
                    break;
                case HighlightColor.DarkMagenta:
                    result = HighlightColorValues.DarkMagenta;
                    break;
                case HighlightColor.DarkRed:
                    result = HighlightColorValues.DarkRed;
                    break;
                case HighlightColor.DarkYellow:
                    result = HighlightColorValues.DarkYellow;
                    break;
                case HighlightColor.Green:
                    result = HighlightColorValues.Green;
                    break;
                case HighlightColor.LightGray:
                    result = HighlightColorValues.LightGray;
                    break;
                case HighlightColor.Magenta:
                    result = HighlightColorValues.Magenta;
                    break;
                case HighlightColor.None:
                    result = HighlightColorValues.None;
                    break;
                case HighlightColor.Red:
                    result = HighlightColorValues.Red;
                    break;
                case HighlightColor.White:
                    result = HighlightColorValues.White;
                    break;
                case HighlightColor.Yellow:
                    result = HighlightColorValues.Yellow;
                    break;
            }

            return result;
        }

        /// <summary>
        /// 从HighlightColorValues转换为HighlightColor
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static HighlightColor ToHighlightColor(this HighlightColorValues source)
        {
            HighlightColor result = HighlightColor.None;

            switch (source)
            {
                case HighlightColorValues.Black:
                    result = HighlightColor.Black;
                    break;
                case HighlightColorValues.Blue:
                    result = HighlightColor.Blue;
                    break;
                case HighlightColorValues.Cyan:
                    result = HighlightColor.Cyan;
                    break;
                case HighlightColorValues.DarkBlue:
                    result = HighlightColor.DarkBlue;
                    break;
                case HighlightColorValues.DarkCyan:
                    result = HighlightColor.DarkCyan;
                    break;
                case HighlightColorValues.DarkGray:
                    result = HighlightColor.DarkGray;
                    break;
                case HighlightColorValues.DarkGreen:
                    result = HighlightColor.DarkGreen;
                    break;
                case HighlightColorValues.DarkMagenta:
                    result = HighlightColor.DarkMagenta;
                    break;
                case HighlightColorValues.DarkRed:
                    result = HighlightColor.DarkRed;
                    break;
                case HighlightColorValues.DarkYellow:
                    result = HighlightColor.DarkYellow;
                    break;
                case HighlightColorValues.Green:
                    result = HighlightColor.Green;
                    break;
                case HighlightColorValues.LightGray:
                    result = HighlightColor.LightGray;
                    break;
                case HighlightColorValues.Magenta:
                    result = HighlightColor.Magenta;
                    break;
                case HighlightColorValues.None:
                    result = HighlightColor.None;
                    break;
                case HighlightColorValues.Red:
                    result = HighlightColor.Red;
                    break;
                case HighlightColorValues.White:
                    result = HighlightColor.White;
                    break;
                case HighlightColorValues.Yellow:
                    result = HighlightColor.Yellow;
                    break;
            }

            return result;
        }

        /// <summary>
        /// 检查填充列表中数据对象是否完整，不完整则补充完整
        /// </summary>
        /// <param name="source"></param>
        internal static void Complete(this List<TagInfo> source)
        {
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i].HasFillingContents)
                {
                    Check(source[i].FillingContents);
                }
                if (source[i].Tbl != null)
                {
                    for (int j = 0; j < source[i].Tbl.Rows.Count; j++)
                    {
                        for (int k = 0; k < source[i].Tbl.Rows[j].Cells.Count; k++)
                        {
                            if (source[i].Tbl[j][k].HasFillingContents)
                            {
                                Check(source[i].Tbl[j][k].FillingContents);
                            }
                        }
                    }
                }
            }
        }

        private static void Check(List<AbstractInfo> abstractInfos)
        {
            if (abstractInfos.Count >= 1
               && abstractInfos.LastEl().FillContentType != FillContentType.TBL
               && abstractInfos.LastEl().FillContentType != FillContentType.PARAGRAPH_END)
            {
                abstractInfos.Add(new ParagraphEnd());
            }

            List<AbstractInfo> tblInfos = abstractInfos
                .Except(new List<FillContentType>() { FillContentType.IMG
                    , FillContentType.PARAGRAPH_BEGIN
                    , FillContentType.PARAGRAPH_END
                    , FillContentType.TXT });

            TblInfo tblInfo = null;
            RowInfo rowInfo = null;
            CellInfo cellInfo = null;
            for (int i = 0; i < tblInfos.Count; i++)
            {
                tblInfo = tblInfos[i].ConvertToDescendent<TblInfo>();
                for (int j = 0; j < tblInfo.Rows.Count; j++)
                {
                    rowInfo = tblInfo.Rows[j];
                    for (int k = 0; k < rowInfo.Cells.Count; k++)
                    {
                        cellInfo = rowInfo.Cells[k];
                        if (cellInfo.HasFillingContents)
                        {
                            Check(cellInfo.FillingContents);
                        }
                    }
                }
            }
        }
    }
}
