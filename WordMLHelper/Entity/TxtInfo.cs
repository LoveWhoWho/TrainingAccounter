using System;
using System.Text.RegularExpressions;

using DocumentFormat.OpenXml.Wordprocessing;

namespace WordMLHelperUtil.Entity
{
    /// <summary>
    /// 文本填充类型实体类
    /// </summary>
    public class TxtInfo : AbstractInfo
    {
        public TxtInfo()
        {
            m_fillContentType = FillContentType.TXT;
        }

        public TxtInfo(string content) : this()
        {
            m_content = content;
        }

        // 样式
        private int m_size = default(int);
        private string m_foreColor = string.Empty;
        private HighlightColorValues m_hightLight = HighlightColorValues.None;
        private string m_fontFamily = string.Empty;

        // 文本内容
        private string m_content = string.Empty;

        /// <summary>
        /// 文本字体大小
        /// </summary>
        public int Size 
        {
            get { return m_size; }
            set
            {
                m_hasStyled = true;
                m_size = value; 
            }
        }

        /// <summary>
        /// 字体颜色（范围：000000-ffffff）
        /// </summary>
        public string ForeColor
        {
            get { return m_foreColor; }
            set
            {
                if (Regex.IsMatch(value.TrimAllSpace(), "^([0-9]|[a-f]|[A-F]){6,6}$"))
                {
                    m_hasStyled = true;
                    m_foreColor = value.TrimAllSpace();
                }
                else
                {
                    throw new ArgumentException("ForeColor只能输入6位16进制的字符串");
                }
            }
        }

        /// <summary>
        /// 字体背景色
        /// </summary>
        public HighlightColor HightLight
        {
            get { return m_hightLight.ToHighlightColor(); }
            set
            {
                m_hasStyled = true;
                m_hightLight = value.ToHighlightColorValues();
            }
        }

        internal HighlightColorValues HightLightColor
        {
            get { return m_hightLight; }
        }

        /// <summary>
        /// 字体
        /// </summary>
        public string FontFamily
        {
            get { return m_fontFamily; }
            set
            {
                m_hasStyled = true;
                m_fontFamily = value;
            }
        }

        /// <summary>
        /// 填充内容
        /// </summary>
        public string Content
        {
            get { return m_content; }
            set { m_content = value; }
        }
    }

    public enum HighlightColor 
    {
        None,
        Yellow,
        Red,
        Black,
        Blue,
        Cyan,
        DarkBlue,
        DarkCyan,
        DarkGray,
        DarkGreen,
        DarkMagenta,
        DarkRed,
        DarkYellow,
        Green,
        LightGray,
        Magenta,
        White
    }

    /// <summary>
    /// 高亮色
    /// </summary>
    /*public struct HighlightColor
    {
        public static HighlightColorValues None
        {
            get { return HighlightColorValues.None; }
        }

        public static HighlightColorValues Yellow
        { 
            get { return HighlightColorValues.Yellow; } 
        }

        public static HighlightColorValues Red
        {
            get { return HighlightColorValues.Red; } 
        }

        public static HighlightColorValues Black
        {
            get { return HighlightColorValues.Black; }
        }

        public static HighlightColorValues Blue
        {
            get { return HighlightColorValues.Blue; }
        }

        public static HighlightColorValues Cyan
        {
            get { return HighlightColorValues.Cyan; }
        }

        public static HighlightColorValues DarkBlue
        {
            get { return HighlightColorValues.DarkBlue; }
        }

        public static HighlightColorValues DarkCyan
        {
            get { return HighlightColorValues.DarkCyan; }
        }

        public static HighlightColorValues DarkGray
        {
            get { return HighlightColorValues.DarkGray; }
        }

        public static HighlightColorValues DarkGreen
        {
            get { return HighlightColorValues.DarkGreen; }
        }

        public static HighlightColorValues DarkMagenta
        {
            get { return HighlightColorValues.DarkMagenta; }
        }

        public static HighlightColorValues DarkRed
        {
            get { return HighlightColorValues.DarkRed; }
        }

        public static HighlightColorValues DarkYellow
        {
            get { return HighlightColorValues.DarkYellow; }
        }

        public static HighlightColorValues Green
        {
            get { return HighlightColorValues.Green; }
        }

        public static HighlightColorValues LightGray
        {
            get { return HighlightColorValues.LightGray; }
        }

        public static HighlightColorValues Magenta
        {
            get { return HighlightColorValues.Magenta; }
        }

        public static HighlightColorValues White
        {
            get { return HighlightColorValues.White; }
        }
    }*/
}
