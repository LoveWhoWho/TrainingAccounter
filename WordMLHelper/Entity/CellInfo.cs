using System.Collections.Generic;

namespace WordMLHelperUtil.Entity
{
    /// <summary>
    /// 表格填充类型的单元格实体类
    /// </summary>
    public class CellInfo : AbstractFillingContainer
    {
        // 样式
        private bool m_hasStyled = false;
        private int m_width = default(int);

        private int m_colSpan = 1;// 合并列数目（默认为1，即是不合并）
        private int m_rowSpan = 1;// 合并行数目（默认为1，即是不合并）

        /// <summary>
        /// 是否设置了样式
        /// </summary>
        internal bool HasStyled
        {
            get { return m_hasStyled; }
        }

        /// <summary>
        /// 单元格宽度
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set
            {
                m_hasStyled = true;
                m_width = value;
            }
        }

        /// <summary>
        /// 合并列数目（默认为1，即是不合并）
        /// </summary>
        public int ColSpan
        {
            get { return m_colSpan; }
            set { m_colSpan = value; }
        }

        /// <summary>
        /// 合并行数目（默认为1，即是不合并）
        /// </summary>
        public int RowSpan
        {
            get { return m_rowSpan; }
            set { m_rowSpan = value; }
        }

        /// <summary>
        /// 填充内容
        /// </summary>
        internal List<AbstractInfo> FillingContents
        {
            get { return m_fillingContents; }
        }

        /// <summary>
        /// 是否有填充内容
        /// </summary>
        internal bool HasFillingContents
        {
            get { return (m_fillingContents.Count >= 1 ? true : false); }
        }
    }
}
