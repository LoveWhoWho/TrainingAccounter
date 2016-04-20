using System.IO;
using System.Collections.Generic;

namespace WordMLHelperUtil.Entity
{
    /// <summary>
    /// 填充域信息实体
    /// </summary>
    public class TagInfo : AbstractFillingContainer
    {
        private long m_index;// 填充域所在的Word文档段落索引（自上而下，从0开始）
        private long m_seq;// 填充域的序号（自上而下，从左到右，从0开始）
        private string m_tagTips;//填充域的填充提示

        private TableStructureInfo m_tbl = null;// 模板中存在表格对象(默认为null)
        

        /// <summary>
        /// 填充域所在的Word文档段落索引（自上而下，从0开始）
        /// </summary>
        internal long Index 
        {
            get { return m_index; }
            set { m_index = value; }
        }

        /// <summary>
        /// 填充域的序号（自上而下，从左到右，从0开始）（只读）
        /// </summary>
        public long Seq 
        {
            get { return m_seq; }
            internal set { m_seq = value; }
        }

        /// <summary>
        /// 填充域的填充提示（只读）
        /// </summary>
        public string TagTips
        {
            get { return m_tagTips; }
            internal set { m_tagTips = value; }
        }

        /// <summary>
        /// 模板中存在表格对象(默认为null)(只读)
        /// </summary>
        public TableStructureInfo Tbl 
        {
            get { return m_tbl; }
            internal set { m_tbl = value; }
        }

        internal List<AbstractInfo> FillingContents 
        {
            get { return m_fillingContents; }
        }

        internal bool HasFillingContents
        {
            get { return (m_fillingContents.Count >= 1 ? true : false); }
        }

    }
}
