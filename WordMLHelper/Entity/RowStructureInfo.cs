using System.Collections.Generic;

namespace WordMLHelperUtil.Entity
{
    /// <summary>
    /// 表格行信息实体
    /// </summary>
    public class RowStructureInfo
    {
        private int m_index;// 该行在模板表格中的行索引
        private List<CellStructureInfo> m_cells;// 单元格信息实体集合

        public RowStructureInfo()
        {
            m_cells = new List<CellStructureInfo>();
        }

        /// <summary>
        /// 该行在模板表格中的行索引（只读）
        /// </summary>
        public int Index 
        {
            get { return m_index; }
            internal set { m_index = value; }
        }

        /// <summary>
        /// 单元格信息实体集合
        /// </summary>
        public List<CellStructureInfo> Cells 
        {
            get { return m_cells; }
            set { m_cells = value; }
        }

        /// <summary>
        /// 添加单元格对象
        /// </summary>
        /// <param name="cell">单元格对象</param>
        public void AddCell(CellStructureInfo cell)
        {
            m_cells.Add(cell);
        }

        /// <summary>
        /// 获取索引为i的单元格对象
        /// </summary>
        /// <param name="i">索引项</param>
        /// <returns></returns>
        public CellStructureInfo this[int i]
        {
            get { return m_cells[i]; }
            set { m_cells[i] = value; }
        }
    }
}
