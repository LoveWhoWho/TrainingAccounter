using System.Collections.Generic;

namespace WordMLHelperUtil.Entity
{
    /// <summary>
    /// 表格填充类型的行实体类
    /// </summary>
    public class RowInfo
    {
        private List<CellInfo> m_cells = new List<CellInfo>();// 行的单元格集合

        /// <summary>
        /// 行的单元格集合
        /// </summary>
        public List<CellInfo> Cells 
        {
            get { return m_cells; }
            set { m_cells = value; }
        }

        /// <summary>
        /// 添加单元格到该行
        /// </summary>
        /// <param name="cell"></param>
        public void AddCell(CellInfo cell)
        {
            m_cells.Add(cell);
        }
    }
}
