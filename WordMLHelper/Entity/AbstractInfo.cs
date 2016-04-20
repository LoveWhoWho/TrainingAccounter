using System;

namespace WordMLHelperUtil.Entity
{
    /// <summary>
    /// 填充类型的抽象类
    /// </summary>
    public abstract class AbstractInfo
    {
        protected FillContentType m_fillContentType = FillContentType.TXT;// 填充类型，默认为文本类型
        protected bool m_hasStyled = false;// 是否已设置样式，默认为否

        /// <summary>
        /// 填充类型
        /// </summary>
        internal FillContentType FillContentType 
        {
            get { return m_fillContentType; }
        }

        /// <summary>
        /// 是否已设置样式
        /// </summary>
        internal bool HasStyled
        {
            get { return m_hasStyled; }
        }

        /// <summary>
        /// 获取具体的填充类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal T ConvertToDescendent<T>() where T : AbstractInfo
        {
            return this as T;
        }
    }

    /// <summary>
    /// 填充内容类型
    /// </summary>
    public enum FillContentType
    {
        TXT,// 文本
        IMG,// 图片
        TBL,// 表格
        PARAGRAPH_BEGIN,// 段落起始
        PARAGRAPH_END// 段落结束
    }
}
