namespace WordMLHelperUtil.Entity
{
    /// <summary>
    /// 用于从CellStructureInfo对象读取数据生成word表格时使用
    /// </summary>
    internal class CellOfRowInfo
    {
        private int m_rowIndex;// TblStructureInfo中Rows集合中的索引
        private int m_cellIndex;// CellStructureInfo对象在RowStructureInfo的Cells集合中的索引
        private bool m_isOver = false;// 标记该行是否已经遍历完

        internal int RowIndex 
        {
            get { return m_rowIndex; }
            set { m_rowIndex = value; }
        }

        internal int CellIndex 
        {
            get { return m_cellIndex; }
            set { m_cellIndex = value; }
        }

        internal bool IsOver
        {
            get { return m_isOver; }
            set { m_isOver = value; }
        }
    }
}
