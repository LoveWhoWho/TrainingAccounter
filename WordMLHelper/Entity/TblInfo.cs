using System.Collections.Generic;

namespace WordMLHelperUtil.Entity
{
    /// <summary>
    /// 表格填充类型实体类
    /// </summary>
    public class TblInfo : AbstractInfo
    {
        public TblInfo()
        {
            m_fillContentType = FillContentType.TBL;
        }

        // 样式
        private int m_width = default(int);

        // 行集合
        private List<RowInfo> m_rows = new List<RowInfo>();

        /// <summary>
        /// 表格整体宽度
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
        /// 行集合
        /// </summary>
        public List<RowInfo> Rows 
        {
            get { return m_rows; }
            set { m_rows = value; }
        }

        /// <summary>
        /// 添加行
        /// </summary>
        /// <param name="row"></param>
        public void AddRow(RowInfo row)
        {
            m_rows.Add(row);
        }
    }
}
