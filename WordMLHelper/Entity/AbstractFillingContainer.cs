using System.Collections.Generic;

namespace WordMLHelperUtil.Entity
{
    public abstract class AbstractFillingContainer
    {
        protected List<AbstractInfo> m_fillingContents = new List<AbstractInfo>();// 填充内容集合

        /// <summary>
        /// 添加填充内容到段落中
        /// </summary>
        /// <param name="content"></param>
        public void AddContent(AbstractInfo content)
        {
            AddContent(content, false);
        }

        /// <summary>
        /// 添加填充内容到段落中，并另起新段落
        /// </summary>
        /// <param name="content"></param>
        public void AddContentLine(AbstractInfo content)
        {
            AddContent(content, true);
        }

        /// <summary>
        /// 添加填充内容到填充列表中
        /// </summary>
        /// <param name="content">填充内容</param>
        /// <param name="endParagraph">是否另起新段落</param>
        /// <param name="m_fillingContents">填充列表</param>
        private void AddContent(AbstractInfo content, bool endParagraph)
        {
            switch (content.FillContentType)
            {
                case FillContentType.TXT:
                case FillContentType.IMG:
                    if (m_fillingContents.Count == 0)
                    {
                        m_fillingContents.Add(new ParagraphBegin());
                        m_fillingContents.Add(content);
                    }
                    else if (m_fillingContents.LastEl().FillContentType == FillContentType.TBL)
                    {
                        m_fillingContents.Add(new ParagraphBegin());
                        m_fillingContents.Add(content);
                    }
                    else
                    {
                        m_fillingContents.Add(content);
                    }

                    if (endParagraph)
                    {
                        m_fillingContents.Add(new ParagraphEnd());
                        m_fillingContents.Add(new ParagraphBegin());
                    }
                    break;
                case FillContentType.TBL:
                    if (m_fillingContents.Count == 0)
                    {
                        m_fillingContents.Add(content);
                    }
                    else if (m_fillingContents.LastEl().FillContentType == FillContentType.PARAGRAPH_BEGIN)
                    {
                        m_fillingContents.Remove(m_fillingContents.LastEl());
                        m_fillingContents.Add(content);
                    }
                    else if (m_fillingContents.LastEl().FillContentType != FillContentType.TBL)
                    {
                        m_fillingContents.Add(new ParagraphEnd());
                        m_fillingContents.Add(content);
                    }
                    else
                    {
                        m_fillingContents.Add(content);
                    }
                    break;
            }
        }
    }
}
