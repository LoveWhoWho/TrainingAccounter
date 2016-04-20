using System.Collections.Generic;
using System.IO;

namespace WordMLHelperUtil.Entity
{
    /// <summary>
    /// 单元格信息实体
    /// </summary>
    public class CellStructureInfo : AbstractFillingContainer
    {
        private int m_index;// wordML中的列索引（大于或等于该单元格实体在行实体中的索引值）
        private int m_colSpan = 1;// 合并列数目（默认为1，即是不合并）
        private int m_rowSpan = 1;// 合并行数目（默认为1，即是不合并）
        private bool m_isTemplate;// 该单元格是否可填写
        private string m_tips = string.Empty;// 单元格中的提示内容

        /// <summary>
        /// wordML中的列索引（大于或等于该单元格实体在行实体中的索引值）（只读）
        /// </summary>
        public int Index 
        {
            get { return m_index; }
            internal set { m_index = value; }
        }

        /// <summary>
        /// 合并列数目（默认为1，即是不合并）（只读）
        /// </summary>
        public int ColSpan 
        {
            get { return m_colSpan; }
            internal set { m_colSpan = value; }
        }

        /// <summary>
        /// 合并行数目（默认为1，即是不合并）（只读）
        /// </summary>
        public int RowSpan 
        {
            get { return m_rowSpan; }
            internal set { m_rowSpan = value; }
        }

        /// <summary>
        /// 单元格中的提示内容（只读）
        /// </summary>
        public string Tips
        {
            get { return m_tips; }
            internal set { m_tips = value; }
        }

        /// <summary>
        /// 该单元格是否可填写（只读）
        /// </summary>
        public bool IsTemplate 
        {
            get { return m_isTemplate; }
            internal set { m_isTemplate = value; }
        }

        internal List<AbstractInfo> FillingContents
        {
            get { return m_fillingContents; }
        }

        internal bool HasFillingContents
        {
            get { return (m_fillingContents.Count >= 1 ? true : false ); }
        }
    }
}
