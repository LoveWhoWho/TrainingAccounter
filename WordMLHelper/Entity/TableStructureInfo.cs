using System.Collections.Generic;

namespace WordMLHelperUtil.Entity
{
    /// <summary>
    /// 表格信息实体
    /// </summary>
    public class TableStructureInfo
    {
        private TblType m_tblType = TblType.HORIZONTAL_HEADER;// 表格类型，默认为HorizontalHeader

        private List<RowStructureInfo> m_rows;// 表格行信息实体集合

        public TableStructureInfo()
        {
            m_rows = new List<RowStructureInfo>();
        }

        /// <summary>
        /// 表格类型（只读）
        /// </summary>
        public TblType TblType
        {
            get { return m_tblType; }
            internal set { m_tblType = value; }
        }

        /// <summary>
        /// 表格行信息实体集合
        /// </summary>
        public List<RowStructureInfo> Rows
        {
            get { return m_rows; }
            set { m_rows = value; }
        }

        /// <summary>
        /// 添加行对象
        /// </summary>
        /// <param name="row">行对象</param>
        public void AddRow(RowStructureInfo row)
        {
            m_rows.Add(row);
        }

        /// <summary>
        /// 获取索引为i的行对象
        /// </summary>
        /// <param name="i">索引项</param>
        /// <returns></returns>
        public RowStructureInfo this[int i]
        {
            get { return m_rows[i]; }
            set { m_rows[i] = value; }
        }

        /// <summary>
        /// 获取行索引为i，单元格索引为j的单元格对象
        /// </summary>
        /// <param name="i">行索引</param>
        /// <param name="j">单元格索引</param>
        /// <returns>单元格对象</returns>
        public CellStructureInfo this[int i, int j]
        {
            get { return m_rows[i].Cells[j]; }
            set { m_rows[i].Cells[j] = value; }
        }
    }

    /// <summary>
    /// 表格类型
    /// </summary>
    public enum TblType
    {
        HORIZONTAL_HEADER = 0,// 仅存在水平表头
        HORIZONTAL_VERTICAL_HEADER = 1// 存在水平和垂直表头
    }
}
