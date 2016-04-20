using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordMLHelperUtil.Entity
{
    /// <summary>
    /// 图片填充类型实体类
    /// </summary>
    public class ImgInfo : AbstractInfo
    {
        public ImgInfo()
        {
            m_fillContentType = FillContentType.IMG;
        }

        public ImgInfo(string imgPath) : this()
        {
            m_imgPath = imgPath;
        }

        // 样式
        private int m_width = default(int);
        private int m_height = default(int);

        // 图片路径
        private string m_imgPath = string.Empty;

        /// <summary>
        /// 图片宽度
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
        /// 图片高度
        /// </summary>
        public int Height 
        {
            get { return m_height; }
            set
            {
                m_hasStyled = true;
                m_height = value;
            }
        }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImgPath 
        {
            get { return m_imgPath; }
            set { m_imgPath = value; }
        }

    }
}
