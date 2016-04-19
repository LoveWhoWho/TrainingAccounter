using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using SystemDrawing = System.Drawing;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using WordMLHelperUtil.Entity;
using System.Windows;


namespace WordMLHelperUtil
{
    public class WordMLHelper
    {
        #region 获取模板需填写的项目

        /// <summary>
        /// 获取模板填充域及附加信息
        /// </summary>
        /// <param name="templatePath">模板文档的路径(后缀为.docx或.dotx)</param>
        /// <returns></returns>
        public List<TagInfo> GetAllTagInfo(string templatePath)
        {
            Assistance.IsDocxEtc(templatePath);

            FileStream fileStream = null;
            try
            {
                fileStream = File.OpenRead(templatePath);
                return GetAllTagInfo(fileStream);
            }
            finally
            {
                if (null != fileStream)
                {
                    fileStream.Close();
                }
            }
        }

        /// <summary>
        /// 获取模板填充域及附加信息
        /// </summary>
        /// <param name="templateStream">模板文档流</param>
        /// <returns></returns>
        public List<TagInfo> GetAllTagInfo(Stream templateStream)
        {
            try
            {
                List<TagInfo> tagInfos = new List<TagInfo>();
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(templateStream, false))
                {
                    Body body = wordDoc.MainDocumentPart.Document.Body;

                    // 逐行读取文档内容，查看是否含有输入内容标签
                    List<OpenXmlElement> els = body.Elements<OpenXmlElement>().ToList();
                    int tagSeq = 0;// 内容输入标签的索引（自上而下，从0开始）
                    TagInfo tagInfo = null;
                    for (int i = 0; i < els.Count; i++)
                    {
                        /* 仅处理两种情况
                         *1)模板填充类型为段落
                         *2)表格（不管是否含模板填充标记）
                        */ 
                        if (HasTag(els[i]) 
                            && els[i] is Paragraph)
                        {
                            Paragraph p = els[i] as Paragraph;
                            tagInfos.AddRange(DevideTagInfoFromParagraph(p, ref tagSeq, i));
                        }
                        else if (els[i] is Table)
                        {
                            tagInfo = new TagInfo();
                            tagInfo.Seq = tagSeq;
                            tagInfo.Index = i;
                            tagInfo.Tbl = new TableStructureInfo();

                            // 对于表格的处理
                            Table tbl = els[i] as Table;
                            List<TableRow> trs = tbl.Elements<TableRow>().ToList();
                            List<TableCell> tcs = null;
                            for (int j = 0; j < trs.Count; j++)
                            {
                                tagInfo.Tbl.AddRow(new RowStructureInfo());
                                tagInfo.Tbl.Rows[j].Index = j;
                                tcs = trs[j].Elements<TableCell>().ToList();
                                for (int x = 0; x < tcs.Count; x++)
                                {
                                    // 单元格的垂直方向
                                    // 为行合并首单元格或非行合并单元格式，处理;否则不处理
                                    if (IsHeaderRowOfRowSpan(tcs[x]))
                                    {
                                        CellStructureInfo cell = new CellStructureInfo();
                                        cell.Index = x;

                                        VerticalMerge vm = tcs[x].Descendants<VerticalMerge>().FirstOrDefault();
                                        // 为行合并首单元格时，获取行合并数
                                        if (null != vm)
                                        {
                                            int rowSpan = 1;
                                            List<TableCell> tmpNextTcs = null;
                                            TableCell tmpNextTc = null;
                                            for (int z = j + 1; z < trs.Count; z++)
                                            {
                                                tmpNextTcs = trs[z].Elements<TableCell>().ToList();
                                                tmpNextTc = tmpNextTcs[x];
                                                if (IsHeaderRowOfRowSpan(tmpNextTc))
                                                {
                                                    break;
                                                }
                                                ++rowSpan;
                                            }

                                            cell.RowSpan = rowSpan;
                                        }

                                        // 单元格的水平方向
                                        cell.ColSpan = GetColSpanNum(tcs[x]);

                                        // 当单元格内容为空或存在内容输入标签时，该单元格标记为需输入的单元格
                                        if (string.IsNullOrEmpty(GetText(tcs[x]))
                                            || HasTag(tcs[x]))
                                        {
                                            cell.IsTemplate = true;
                                        }
                                        else
                                        {
                                            cell.IsTemplate = false;
                                        }

                                        // 单元格的内容
                                        cell.Tips = GetText(tcs[x]);

                                        tagInfo.Tbl.Rows[j].AddCell(cell);
                                    }
                                }
                            }

                            tagInfos.Add(tagInfo);
                            ++tagSeq;
                        }
                    }

                    // 过滤表格行，并设置表格类型
                    List<TagInfo> tagTbls = tagInfos.Where(a => a.Tbl != null).ToList();
                    List<TableStructureInfo> tbls = new List<TableStructureInfo>();
                    foreach (TagInfo tagTbl in tagTbls)
                    {
                        tbls.Add(tagTbl.Tbl);
                    }
                    for (int i = 0; i < tbls.Count; i++)
                    {
                        List<long> deleteRowIndex = new List<long>();
                        List<RowStructureInfo> rows = tbls[i].Rows;
                        for (int j = rows.Count - 1; j >= 0 ; j--)
                        {
                            List<CellStructureInfo> cells = rows[j].Cells;
                            bool isDelete = true;
                            foreach (CellStructureInfo cell in cells)
                            {
                                if (!string.IsNullOrEmpty(cell.Tips)
                                    && !cell.Tips.TrimAllSpace().IsNumeric())
                                {
                                    isDelete = false;
                                    break;
                                }
                            }

                            if (isDelete)
                            {
                                deleteRowIndex.Add(rows[j].Index);
                                rows.Remove(rows[j]);
                            }
                        }

                        // 设置表格类型
                        if (deleteRowIndex.Count >= 1)
                        {
                            tbls[i].TblType = TblType.HORIZONTAL_HEADER;
                        }
                        else
                        {
                            tbls[i].TblType = TblType.HORIZONTAL_VERTICAL_HEADER;
                        }
                    }

                    return tagInfos;
                }
            }
            finally
            {
                if (null != templateStream)
                {
                    templateStream.Close();
                }
            }
        }

        /// <summary>
        /// 判断单元格是否为跨行单元格集合中的首个单元格
        /// </summary>
        /// <param name="tc">单元格</param>
        /// <returns>true——是；false——否</returns>
        private bool IsHeaderRowOfRowSpan(TableCell tc)
        {
            bool result = false;

            VerticalMerge vm = tc.Descendants<VerticalMerge>().FirstOrDefault();
            if (null != vm
                && (null == vm.Val
                || (null != vm.Val && vm.Val.Value == MergedCellValues.Continue)))
            {
            }
            else if ((null != vm
                    && null != vm.Val && vm.Val.Value == MergedCellValues.Restart)
                    || null == vm)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// 获取跨列数目
        /// </summary>
        /// <param name="tc">wordML单元格</param>
        /// <returns></returns>
        private int GetColSpanNum(TableCell tc)
        {
            int colSpan = 1;
            GridSpan gs = tc.Descendants<GridSpan>().FirstOrDefault();
            if (null != gs
                && null != gs.Val
                && gs.Val.HasValue)
            {
                colSpan = gs.Val.Value;
            }

            return colSpan;
        }

        /// <summary>
        /// 标志中是否含填充域
        /// </summary>
        /// <param name="el">标志对象</param>
        /// <returns>true——有；false——无</returns>
        private bool HasTag(OpenXmlElement el)
        {
            bool result = false;

            if (el.Descendants<FieldChar>().Count() >= 2
                && el.Descendants<FieldCode>()
                .Where(a => a.InnerText.IndexOf(Const.FIELDCODE_TAG) >= 0).Count() >= 1)
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// 获取段落中的填充域
        /// </summary>
        /// <param name="p">段落</param>
        /// <param name="seq" direction="inputoutput">填充域序号</param>
        /// <param name="pIndex">段落序号</param>
        /// <returns>填充域列表</returns>
        private List<TagInfo> DevideTagInfoFromParagraph(Paragraph p, ref int seq, int pIndex)
        {
            List<TagInfo> tagInfos = new List<TagInfo>();

            List<Run> runOfP = p.Elements<Run>().ToList();
            List<OpenXmlElement> childOfRun = new List<OpenXmlElement>();
            foreach (Run run in runOfP)
            {
                childOfRun.AddRange(run.Elements());
            }

            ResearchState researchState = ResearchState.NOT_IN_FILL_AREA;
            StringBuilder fieldCodeTxt = null;
            TagInfo tagInfo = null;
            for (int i = 0; i < childOfRun.Count; i++)
            {
                // 进入填充域
                if (childOfRun[i] is FieldChar
                    && (childOfRun[i] as FieldChar).FieldCharType.Value == FieldCharValues.Begin
                    && (researchState == ResearchState.NOT_IN_FILL_AREA))
                {
                    researchState = ResearchState.IN_FILL_AREA;
                    fieldCodeTxt = new StringBuilder();
                    tagInfo = new TagInfo();
                }
                
                // 在填充域
                if (childOfRun[i] is FieldCode
                    && researchState == ResearchState.IN_FILL_AREA)
                {
                    fieldCodeTxt.Append(childOfRun[i].InnerText);
                }

                // 离开填充域
                if (childOfRun[i] is FieldChar
                    && (childOfRun[i] as FieldChar).FieldCharType.Value == FieldCharValues.End
                    && researchState == ResearchState.IN_FILL_AREA)
                {
                    researchState = ResearchState.NOT_IN_FILL_AREA;
                    if (fieldCodeTxt.ToString().IndexOf(Const.FIELDCODE_TAG) >= 0)
                    {
                        string tagTips = fieldCodeTxt
                                    .Replace(Const.FIELDCODE_TAG, string.Empty).ToString();
                        tagInfo.TagTips = tagTips;
                        tagInfo.Seq = seq;
                        ++seq;
                        tagInfo.Index = pIndex;

                        tagInfos.Add(tagInfo);
                    }
                }

                // 在非填充域
                if (!(childOfRun[i] is FieldChar)
                    && !(childOfRun[i] is FieldCode)
                    && researchState == ResearchState.NOT_IN_FILL_AREA)
                {
                    researchState = ResearchState.NOT_IN_FILL_AREA;
                }
            }


            return tagInfos;
        }

        /// <summary>
        /// 获取标签对象内的所有Text标签的文本内容
        /// </summary>
        /// <param name="el">标签对象</param>
        /// <returns>所有Text标签的文本内容</returns>
        private string GetText(OpenXmlElement el)
        {
            List<Text> texts = el.Descendants<Text>().ToList();
            StringBuilder textSB = new StringBuilder(texts.Count * 5);
            foreach (Text text in texts)
            {
                if (!string.IsNullOrEmpty(text.InnerText))
                {
                    textSB.Append(text.InnerText);
                }
            }

            return textSB.ToString();
        }

        #endregion

        #region 填充填充域

        /// <summary>
        /// 快速填充纯文本内容到填充域
        /// </summary>
        /// <param name="tagInfo">填充域</param>
        /// <param name="inputString">纯文本内容</param>
        public void FillContentWithoutStyle(TagInfo tagInfo, string inputString)
        {
            TxtInfo txtInfo = new TxtInfo();
            txtInfo.Content = inputString;
            tagInfo.AddContent(txtInfo);
        }

        /// <summary>
        /// 快速填充纯文本内容到填充域
        /// </summary>
        /// <param name="tagInfo">填充域</param>
        /// <param name="inputStrings">纯文本内容集合</param>
        /// <param name="isBreak">是否每份内容作为独立的一个段落</param>
        public void FillContentWithoutStyle(TagInfo tagInfo, string[] inputStrings, bool isBreak)
        {
            TxtInfo txtInfo = null;

            for (int i = 0; i < inputStrings.Length; i++)
            {
                txtInfo = new TxtInfo();
                txtInfo.Content = inputStrings[i];
                if (isBreak)
                {
                    tagInfo.AddContentLine(txtInfo);
                }
                else
                {
                    tagInfo.AddContent(txtInfo);
                }
            }
        }

        /// <summary>
        /// 快速填充纯文本内容到填充域
        /// </summary>
        /// <param name="tagInfo">填充域</param>
        /// <param name="inputStrings">纯文本内容集合</param>
        /// <param name="isBreak">是否每份内容作为独立的一个段落</param>
        public void FillContentWithoutStyle(TagInfo tagInfo, List<string> inputStrings, bool isBreak)
        {
            TxtInfo txtInfo = null;

            for (int i = 0; i < inputStrings.Count; i++)
            {
                txtInfo = new TxtInfo();
                txtInfo.Content = inputStrings[i];
                if (isBreak)
                {
                    tagInfo.AddContentLine(txtInfo);
                }
                else
                {
                    tagInfo.AddContent(txtInfo);
                }
            }
        }

        /// <summary>
        /// 快速填充、生成纯文本表格
        /// </summary>
        /// <param name="tagInfo">填充域</param>
        /// <param name="dt">数据表</param>
        public void FillContentToTable(TagInfo tagInfo, DataTable dt)
        {
            if (null == dt 
                || null == tagInfo)
            {
                throw new ArgumentNullException();
            }

            // 生成表格
            if (tagInfo.Tbl == null)
            {
                TblInfo tbl = new TblInfo();
                DataRowCollection dataRows = dt.Rows;
                int dataColCount = dt.Columns.Count;

                // 生成表头
                RowInfo row = new RowInfo();
                tbl.AddRow(row);
                for (int i = 0; i < dataColCount; i++)
                {
                    CellInfo cell = new CellInfo();
                    cell.AddContent(new TxtInfo() { Content = dt.Columns[i].ColumnName });
                    row.AddCell(cell);
                }

                for (int i = 0; i < dataRows.Count; i++)
                {
                    row = new RowInfo();
                    tbl.AddRow(row);
                    for (int j = 0; j < dataColCount; j++)
                    {
                        CellInfo cell = new CellInfo();
                        cell.AddContent(new TxtInfo() { Content = Convert.ToString(dataRows[i][j]) });
                        row.AddCell(cell);
                    }
                }

                tagInfo.AddContent(tbl);
            }
            else if (tagInfo.Tbl.TblType == TblType.HORIZONTAL_HEADER)
            {
                // 填充表格
                TableStructureInfo tbl = tagInfo.Tbl;
                DataRowCollection dataRows = dt.Rows;
                int colCount = tbl.Rows.LastEl().Cells.Count;
                int dataColCount = dt.Columns.Count;

                for (int i = 0; i < dataRows.Count; i++)
                {
                    RowStructureInfo row = new RowStructureInfo();

                    for (int j = 0; j < colCount; j++)
                    {
                        if (dataColCount <= j)
                        {
                            break;
                        }
                        else
                        {
                            CellStructureInfo cell = new CellStructureInfo();
                            TxtInfo txtInfo = new TxtInfo();
                            txtInfo.Content = Convert.ToString(dataRows[i][j]);
                            cell.AddContent(txtInfo);
                            row.AddCell(cell);
                        }
                    }

                    tbl.AddRow(row);
                }

                
            }
            else
            {
                throw new ArgumentOutOfRangeException("TblType不能为HorizontalVerticalHeader");
            }
        }

        #endregion

        #region 生成word文档(.docx)

        /// <summary>
        /// 根据模板生成Word文档
        /// </summary>
        /// <param name="templatePath">模板文档路径</param>
        /// <param name="outputPath">输出文档路径</param>
        /// <param name="tags">填充域集合</param>
        public void GenerateWordDocument(string templatePath, string outputPath, List<TagInfo> tags)
        {
            Assistance.IsDocxEtc(templatePath);
            Assistance.IsDocxEtc(outputPath);

            FileStream templateStream = null;
            try
            {
                templateStream = File.OpenRead(templatePath);
                GenerateWordDocument(templateStream, outputPath, tags);
            }
            finally
            {
                if (null != templateStream)
                {
                    templateStream.Close();
                }
            }
        }

        /// <summary>
        /// 根据模板生成Word文档
        /// </summary>
        /// <param name="templateStream">模板文档流</param>
        /// <param name="outputPath">输出文档路径</param>
        /// <param name="tags">填充域集合</param>
        public void GenerateWordDocument(Stream templateStream, string outputPath, List<TagInfo> tags)
        {
            Assistance.IsDocxEtc(outputPath);

            try
            {
                // 若模板流来自文件，则要先生成word输出文件
                if (templateStream is FileStream)
                {
                    bool result = CreateOutputFile(templateStream, outputPath);
                    if (!result)
                    {
                        return;
                    }

                    templateStream = File.Open(outputPath, FileMode.Open);
                }

                tags.Complete();

                using (WordprocessingDocument outputDoc = WordprocessingDocument.Open(templateStream, true))
                {
                    Body body = outputDoc.MainDocumentPart.Document.Body;

                    // 逐行读取文档内容，查看是否含有输入内容标签
                    List<OpenXmlElement> els = body.Elements<OpenXmlElement>().ToList();
                    int tagSeq = 0;// 内容输入标签的索引（自上而下，从0开始）
                    for (int i = 0; i < els.Count; i++)
                    {
                        TagInfo curTagInfo = tags.Where(a => a.Index == i && a.Seq == tagSeq)
                                .FirstOrDefault();
                        if (curTagInfo == null)
                        {
                            continue;
                        }

                        if (HasTag(els[i])
                            && els[i] is Paragraph)
                        {

                            Paragraph p = els[i] as Paragraph;

                            InsertTagInfoToParagraph(p, tags, i, ref tagSeq, outputDoc);

                            ++tagSeq;
                        }
                        else if ((els[i] is Table) && (null != curTagInfo.Tbl))
                        {
                            Table tbl = els[i] as Table;
                            List<TableRow> trs = tbl.Elements<TableRow>().ToList();
                            List<TableCell> tcs = null;

                            // 获取修改了的行
                            List<RowStructureInfo> modifiedRows = curTagInfo.Tbl.Rows
                                .Where(a => a.Cells.Where(b => b.HasFillingContents).Count() >= 1).ToList();

                            // 对于只有水平表头的表，可有自定义行数的记录
                            // 对于有水平和垂直表头的表，要严格遵守模板中表格的设置
                            switch (curTagInfo.Tbl.TblType)
                            {
                                case TblType.HORIZONTAL_HEADER:

                                    // 仅保留文档的表头行
                                    int headerRowCount = curTagInfo.Tbl.Rows
                                        .Where(a => a.Cells.Where(b => b.HasFillingContents).Count() == 0).Count();
                                    for (int j = trs.Count - 1; j >= headerRowCount; --j)
                                    {
                                        tbl.RemoveChild(trs[j]);
                                    }

                                    TransformToWordMLTable(tbl, curTagInfo, outputDoc.MainDocumentPart);
                                    break;
                                case TblType.HORIZONTAL_VERTICAL_HEADER:
                                    foreach (RowStructureInfo modifiedRow in modifiedRows)
                                    {
                                        tcs = trs[modifiedRow.Index].Elements<TableCell>().ToList();
                                        List<CellStructureInfo> modifiedCells = modifiedRow.Cells
                                            .Where(a => a.HasFillingContents).ToList();
                                        foreach (CellStructureInfo modifiedCell in modifiedCells)
                                        {
                                            TableCell cell = tcs[modifiedCell.Index];

                                            // 清除单元格内非rcPr子节点
                                            for (int j = cell.ChildElements.Count - 1; j >= 0; --j)
                                            {
                                                if (!(cell.ChildElements[j] is TableCellProperties))
                                                {
                                                    cell.RemoveChild(cell.ChildElements[j]);
                                                }
                                            }

                                            // 填充新内容
                                            List<AbstractInfo> legalFillingContents = modifiedCell
                                                .FillingContents
                                                .Except(new List<FillContentType>() { FillContentType.TBL });
                                            Stack<Paragraph> pStack = new Stack<Paragraph>();
                                            for (int j = 0; j < legalFillingContents.Count; j++)
                                            {
                                                switch (legalFillingContents[j].FillContentType)
                                                {
                                                    case FillContentType.PARAGRAPH_BEGIN:
                                                        Paragraph p = new Paragraph();
                                                        pStack.Push(p);
                                                        break;
                                                    case FillContentType.PARAGRAPH_END:
                                                        cell.AppendChild(pStack.Pop());
                                                        break;
                                                    case FillContentType.TXT:
                                                        Run txtRun = AssembleTxtRun(legalFillingContents[j].ConvertToDescendent<TxtInfo>());
                                                        pStack.Peek().AppendChild(txtRun);
                                                        break;
                                                    case FillContentType.IMG:
                                                        Run imgRun = AssembleImgRun(legalFillingContents[j].ConvertToDescendent<ImgInfo>()
                                                            , outputDoc.MainDocumentPart);
                                                        pStack.Peek().AppendChild(imgRun);
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }

                            ++tagSeq;
                        }
                    }
                }
            }
            finally
            {
                if (null != templateStream)
                {
                    templateStream.Close();
                }
            }
        }

        /// <summary>
        /// 生成输出文件
        /// </summary>
        /// <param name="templateFs"></param>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        private bool CreateOutputFile(Stream templateFs, string outputPath)
        {
            bool result = false;

            // 存在则先删除旧文件，再新建输出文件
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
            FileStream targetFs = File.Create(outputPath);
            byte[] templateBytes = new byte[templateFs.Length];
            templateFs.Read(templateBytes, 0, templateBytes.Length);
            templateFs.Close();
            targetFs.Write(templateBytes, 0, templateBytes.Length);
            targetFs.Flush();
            targetFs.Close();

            result = true;

            return result;
        }

        /// <summary>
        /// 向单元格添加文本和图片
        /// </summary>
        /// <param name="tc"></param>
        /// <param name="cellContent"></param>
        private void InsertContentToTc(TableCell tc, CellStructureInfo cellContent, MainDocumentPart mainPart)
        {
            List<AbstractInfo> fillingContents = cellContent.FillingContents;
            List<AbstractInfo> legalFillingContents = fillingContents.Except(new List<FillContentType>() { FillContentType.TBL });
            Stack<Paragraph> pStack = new Stack<Paragraph>();
            for (int i = 0; i < legalFillingContents.Count; i++)
            {
                switch(legalFillingContents[i].FillContentType)
                {
                    case FillContentType.PARAGRAPH_BEGIN:
                        Paragraph p = new Paragraph();
                        pStack.Push(p);
                        break;
                    case FillContentType.PARAGRAPH_END:
                        tc.AppendChild(pStack.Pop());
                        break;
                    case FillContentType.TXT:
                        Run txtRun = AssembleTxtRun(legalFillingContents[i].ConvertToDescendent<TxtInfo>());
                        pStack.Peek().AppendChild(txtRun);
                        break;
                    case FillContentType.IMG:
                        Run imgRun = AssembleImgRun(legalFillingContents[i].ConvertToDescendent<ImgInfo>(), mainPart);
                        pStack.Peek().AppendChild(imgRun);
                        break;
                }
            }
        }

        /// <summary>
        /// 删除段落中的标签内容
        /// </summary>
        /// <param name="p">段落</param>
        private void DeleteContentBetweenFldChars(Paragraph p)
        {
            List<OpenXmlElement> deletingRuns = new List<OpenXmlElement>();
            bool isDeletingItem = false;

            for (int i = 0; i < p.ChildElements.Count; i++)
            {
                FieldChar fieldChar = p.ChildElements[i].Descendants<FieldChar>().FirstOrDefault();
                if (fieldChar != null && fieldChar.FieldCharType == FieldCharValues.Begin)
                {
                    isDeletingItem = true;
                }

                if (isDeletingItem )
                {
                    deletingRuns.Add(p.ChildElements[i]);
                }

                if (fieldChar != null && fieldChar.FieldCharType == FieldCharValues.End)
                {
                    isDeletingItem = false;
                } 
            }

            for (int i = deletingRuns.Count - 1; i >= 0; i--)
            {
                p.RemoveChild(deletingRuns[i]);
            }
        }

        /// <summary>
        /// 将TableStructureInfo的表格格式转换为wordML的表格格式
        /// </summary>
        /// <param name="tbl">wordML的表格</param>
        /// <param name="tblInfo">TableStructureInfo的表格</param>
        private void TransformToWordMLTable(Table tbl, TagInfo curTagInfo, MainDocumentPart mainPart)
        {
            TableStructureInfo tblInfo = curTagInfo.Tbl;
            List<RowStructureInfo> modifiedRows = tblInfo.Rows.Where(a => a.Cells.Where(b => b.HasFillingContents).Count() >= 1).ToList();

            // 添加表行
            if (modifiedRows.Count >= 1)
            {
                List<TableRow> curTrs = new List<TableRow>();
                List<CellOfRowInfo> cellOfRowInfos = new List<CellOfRowInfo>();
                bool isOver = false;
                while (curTrs.Count == 0
                    || !isOver)
                {
                    isOver = true;

                    for (int j = 0; j < modifiedRows.Count; j++)
                    {
                        CellOfRowInfo cellOfRowInfo = null;
                        if (curTrs.Count - 1 < j)
                        {
                            curTrs.Add(new TableRow());
                            cellOfRowInfo = new CellOfRowInfo();
                            cellOfRowInfo.CellIndex = 0;
                            cellOfRowInfo.RowIndex = j;
                            cellOfRowInfos.Add(cellOfRowInfo);
                        }
                        else
                        {
                            cellOfRowInfo = cellOfRowInfos
                                .Where(a => a.RowIndex == j).FirstOrDefault();
                        }

                        // 当该行已遍历后，跳过对该行的操作
                        if (cellOfRowInfo.IsOver)
                        {
                            continue;
                        }

                        List<CellStructureInfo> cells = modifiedRows[j].Cells;
                        CellStructureInfo cell = cells[cellOfRowInfo.CellIndex];

                        TableCell tc = new TableCell();
                        curTrs[j].InsertAt(tc, curTrs[j].ChildElements.Count);

                        // 插入内容
                        InsertContentToTc(tc, cell, mainPart);

                        if (cell.ColSpan >= 2)
                        {
                            TableCellProperties tcPr = new TableCellProperties();
                            GridSpan gs = new GridSpan();
                            gs.Val = cell.ColSpan;
                            tcPr.InsertAt(gs, 0);
                            tc.InsertAt(tcPr, 0);
                        }
                        if (cell.RowSpan >= 2)
                        {
                            TableCellProperties tcPr = tc
                                .Descendants<TableCellProperties>().FirstOrDefault();
                            if (null == tcPr)
                            {
                                tcPr = new TableCellProperties();
                                tc.InsertAt(tcPr, 0);
                            }

                            VerticalMerge vm = new VerticalMerge();
                            vm.Val = MergedCellValues.Restart;
                            tcPr.InsertAt(vm, tcPr.ChildElements.Count);

                            // 生成行并和的continue行单元格
                            for (int x = j + 1; x < j + cell.RowSpan; x++)
                            {
                                CellOfRowInfo continueCellOfRowInfo = null;
                                if (curTrs.Count - 1 < x)
                                {
                                    curTrs.Add(new TableRow());
                                    continueCellOfRowInfo = new CellOfRowInfo();
                                    continueCellOfRowInfo.CellIndex = 0;
                                    continueCellOfRowInfo.RowIndex = x;
                                    cellOfRowInfos.Add(continueCellOfRowInfo);
                                }
                                else
                                {
                                    continueCellOfRowInfo = cellOfRowInfos
                                        .Where(a => a.RowIndex == x).FirstOrDefault();
                                }

                                // 当该行已遍历后，跳过对该行的操作
                                if (continueCellOfRowInfo.IsOver)
                                {
                                    continue;
                                }

                                List<CellStructureInfo> continueCells = modifiedRows[x].Cells;
                                CellStructureInfo continueCell = continueCells[continueCellOfRowInfo.CellIndex];

                                TableCell continueTc = new TableCell();
                                TableCellProperties continueTcPr = null;
                                if (continueCell.ColSpan >= 1)
                                {
                                    continueTcPr = new TableCellProperties();
                                    GridSpan gs = new GridSpan();
                                    gs.Val = cell.ColSpan;
                                    continueTcPr.InsertAt(gs, 0);
                                    continueTc.InsertAt(tcPr, 0);
                                }

                                curTrs[x].InsertAt(continueTc
                                    , curTrs[x].ChildElements.Count);
                                continueTcPr = continueTc
                                .Descendants<TableCellProperties>().FirstOrDefault();
                                if (null == continueTcPr)
                                {
                                    continueTcPr = new TableCellProperties();
                                    continueTc.InsertAt(continueTcPr, 0);
                                }

                                VerticalMerge continueVm = new VerticalMerge();
                                continueVm.Val = MergedCellValues.Continue;
                                continueTcPr.InsertAt(continueVm, continueTcPr
                                    .ChildElements.Count);

                                curTrs[x].InsertAt(continueTc, curTrs[x].ChildElements.Count);

                                if (continueCellOfRowInfo.CellIndex + 1 < continueCells.Count)
                                {
                                    ++continueCellOfRowInfo.CellIndex;
                                }
                                else
                                {
                                    continueCellOfRowInfo.IsOver = true;
                                }
                            }

                            j = j + cell.RowSpan - 1;
                        }

                        if (cellOfRowInfo.CellIndex + 1 < cells.Count)
                        {
                            ++cellOfRowInfo.CellIndex;
                        }
                        else
                        {
                            cellOfRowInfo.IsOver = true;
                        }
                    }

                    foreach (CellOfRowInfo isOverItem in cellOfRowInfos)
                    {
                        isOver = isOver && isOverItem.IsOver;
                    }
                }

                // 添加数据行到word的表格中
                for (int j = 0; j < curTrs.Count; j++)
                {
                    tbl.AppendChild(curTrs[j]);
                }
            }
        }

        /// <summary>
        /// 将段落的样式转换到表格中
        /// </summary>
        /// <param name="p">段落</param>
        /// <param name="tbl">表格</param>
        private void TransformStyleFormPToTbl(Paragraph p, Table tbl)
        {
            TableProperties tblPr = tbl.Descendants<TableProperties>().FirstOrDefault();
            if (tblPr == null)
            {
                tblPr = new TableProperties();
                tbl.AppendChild(tblPr);
            }

            // 添加表格边框
            TableBorders tblBorders = new TableBorders();
            tblPr.TableBorders = tblBorders;
            TopBorder top = new TopBorder();
            top.Color = "auto";
            top.Size = 4;
            top.Space = 0;
            top.Val = BorderValues.Single;
            tblBorders.AppendChild(top);

            LeftBorder left = new LeftBorder();
            left.Color = "auto";
            left.Size = 4;
            left.Space = 0;
            left.Val = BorderValues.Single;
            tblBorders.AppendChild(left);

            RightBorder right = new RightBorder();
            right.Color = "auto";
            right.Size = 4;
            right.Space = 0;
            right.Val = BorderValues.Single;
            tblBorders.AppendChild(right);

            BottomBorder bottom = new BottomBorder();
            bottom.Color = "auto";
            bottom.Size = 4;
            bottom.Space = 0;
            bottom.Val = BorderValues.Single;
            tblBorders.AppendChild(bottom);

            InsideHorizontalBorder insideH = new InsideHorizontalBorder();
            insideH.Color = "auto";
            insideH.Size = 4;
            insideH.Space = 0;
            insideH.Val = BorderValues.Single;
            tblBorders.AppendChild(insideH);

            InsideVerticalBorder insideV = new InsideVerticalBorder();
            insideV.Color = "auto";
            insideV.Size = 4;
            insideV.Space = 0;
            insideV.Val = BorderValues.Single;
            tblBorders.AppendChild(insideV);


            // 通过段落样式表设置样式
            ParagraphStyleId pStyle = p.Descendants<ParagraphStyleId>().FirstOrDefault();
            if (null != pStyle && null != pStyle.Val)
            {
                string pStyleVal = pStyle.Val.Value;

                TableStyle tblStyle = new TableStyle();
                tblStyle.Val = pStyleVal;
                tblPr.TableStyle = tblStyle;
            }
        }

        /// <summary>
        /// 插入图片
        /// </summary>
        /// <param name="mainPart">文档的主要部分</param>
        /// <param name="imgPath">图片路径</param>
        /// <param name="customWidth">自定义图片宽度</param>
        /// <param name="customHeight">自定义图片高度</param>
        private Run AddImage(MainDocumentPart mainPart
            , string imgPath
            , int? customWidth
            , int? customHeight)
        {
            FileStream fs = null;
            Run run = null;
            try
            {
                fs = new FileStream(imgPath, FileMode.Open);
                ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);
                imagePart.FeedData(fs);
                fs.Close();

                int bmWidth;
                int bmHeight;
                float bmHorizontalResolution;
                float bmVerticalResolution;
                Assistance.GetImgInfo(imgPath
                    , out bmWidth
                    , out bmHeight
                    , out bmHorizontalResolution
                    , out bmVerticalResolution);

                if (customWidth != null)
                {
                    bmWidth = customWidth.Value;
                }
                if (customHeight != null)
                {
                    bmHeight = customHeight.Value;
                }

                run = AddImageToBody( mainPart.GetIdOfPart(imagePart), bmWidth, bmHeight, bmHorizontalResolution, bmVerticalResolution);
            }
            finally
            {
                if (null != fs)
                {
                    fs.Close();
                }
            }

            return run;
        }

        /// <summary>
        /// 添加图片到文档体
        /// </summary>
        /// <param name="relationshipId">关系ID</param>
        /// <param name="bmWidth">图片宽度</param>
        /// <param name="bmHeight">图片高度</param>
        /// <param name="bmHorizontalResolution">图片水平分辨率</param>
        /// <param name="bmVerticalResolution">图片垂直分辨率</param>
        /// <returns></returns>
        private static Run AddImageToBody(string relationshipId
            , int bmWidth
            , int bmHeight
            , float bmHorizontalResolution
            , float bmVerticalResolution)
        {
            var element =
                 new Drawing(
                     new DW.Inline(
                         new DW.Extent() { Cx = bmWidth * (long)((float)914400 / bmHorizontalResolution), Cy = bmHeight * (long)((float)914400 / bmVerticalResolution) },
                         new DW.EffectExtent()
                         {
                             LeftEdge = 0L,
                             TopEdge = 0L,
                             RightEdge = 0L,
                             BottomEdge = 0L
                         },
                         new DW.DocProperties()
                         {
                             Id = (UInt32Value)1U,
                             Name = "Picture 1"
                         },
                         new DW.NonVisualGraphicFrameDrawingProperties(
                             new A.GraphicFrameLocks() { NoChangeAspect = true }),
                         new A.Graphic(
                             new A.GraphicData(
                                 new PIC.Picture(
                                     new PIC.NonVisualPictureProperties(
                                         new PIC.NonVisualDrawingProperties()
                                         {
                                             Id = (UInt32Value)0U,
                                             Name = "New Bitmap Image.jpg"
                                         },
                                         new PIC.NonVisualPictureDrawingProperties()),
                                     new PIC.BlipFill(
                                         new A.Blip(
                                             new A.BlipExtensionList(
                                                 new A.BlipExtension()
                                                 {
                                                     Uri =
                                                       "{28A0092B-C50C-407E-A947-70E740481C1C}"
                                                 })
                                         )
                                         {
                                             Embed = relationshipId,
                                             CompressionState =
                                             A.BlipCompressionValues.Print
                                         },
                                         new A.Stretch(
                                             new A.FillRectangle())),
                                     new PIC.ShapeProperties(
                                         new A.Transform2D(
                                             new A.Offset() { X = 0L, Y = 0L },
                                             new A.Extents() { Cx = 990000L, Cy = 792000L }),
                                         new A.PresetGeometry(
                                             new A.AdjustValueList()
                                         ) { Preset = A.ShapeTypeValues.Rectangle }))
                             ) { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                     )
                     {
                         DistanceFromTop = (UInt32Value)0U,
                         DistanceFromBottom = (UInt32Value)0U,
                         DistanceFromLeft = (UInt32Value)0U,
                         DistanceFromRight = (UInt32Value)0U
                     });

            return new Run(element);
        }

        /// <summary>
        /// 添加填充内容到段落中的填充域
        /// </summary>
        /// <param name="p">段落对象</param>
        /// <param name="tagInfos">所以填充内容集合</param>
        /// <param name="pIndex">段落索引</param>
        /// <param name="seq">填充内容索引</param>
        /// <param name="outputDoc">输出文档对象</param>
        private void InsertTagInfoToParagraph(Paragraph p, List<TagInfo> tagInfos, int pIndex, ref int seq,WordprocessingDocument outputDoc)
        {
            List<TagInfo> pTagInfos = tagInfos.Where(a => a.Index == pIndex).ToList();
            seq = seq + pTagInfos.Count - 1;
            IEnumerable<Run> runs = p.Elements<Run>();
            List<OpenXmlElement> childOfRun = new List<OpenXmlElement>();
            foreach (Run run in runs)
            {
                childOfRun.AddRange(run.Elements());
            }

            List<Run> deletingRuns = new List<Run>();// 要删除的run标签集合

            // 段落中含多个填充域或只含一个填充域同时含其他非填充域内容时，只允许插入文本和图片
            // 段落中只含一个填充域且不含其他非填充域内容时，可插入文本、段落文本、表格、图片、段落图片
            if ((!String.IsNullOrEmpty(p.Descendants<Text>().CatenateAllTrimString()) && pTagInfos.Count == 1)
                || pTagInfos.Count >= 2)
            {
                for (int i = 0; i < pTagInfos.Count; i++)
                {
                    if (pTagInfos[i].HasFillingContents)
                    {
                        int curSeq = 0;
                        ResearchState researchState = ResearchState.NOT_IN_FILL_AREA;

                        for (int j = 0; j < childOfRun.Count; j++)
                        {
                            // 进入填充域
                            if (childOfRun[j] is FieldChar
                                && (childOfRun[j] as FieldChar).FieldCharType.Value == FieldCharValues.Begin
                                && researchState == ResearchState.NOT_IN_FILL_AREA)
                            {
                                researchState = ResearchState.IN_FILL_AREA;
                                if (i == curSeq)
                                {
                                    // 插入填充内容
                                    Run curRun = childOfRun[j].Parent as Run;
                                    List<AbstractInfo> fillingContents = pTagInfos[i].FillingContents;

                                    // 去除无效的填充内容
                                    List<AbstractInfo> legalFillingContents = fillingContents
                                        .Except(new List<FillContentType>(){FillContentType.PARAGRAPH_BEGIN
                                            , FillContentType.PARAGRAPH_END
                                            , FillContentType.TBL});

                                    foreach (var legalFillingContent in legalFillingContents)
                                    {
                                        switch(legalFillingContent.FillContentType)
                                        {
                                            case FillContentType.TXT:
                                                TxtInfo txtFilling = legalFillingContent.ConvertToDescendent<TxtInfo>();

                                                Run txtRun = AssembleTxtRun(txtFilling);
                                                p.InsertAfter(txtRun, curRun);
                                                curRun = txtRun;
                                                break;
                                            case FillContentType.IMG:
                                                ImgInfo imgFilling = legalFillingContent.ConvertToDescendent<ImgInfo>();

                                                Run imgRun = AssembleImgRun(imgFilling, outputDoc.MainDocumentPart);
                                                p.InsertAfter(imgRun, curRun);
                                                curRun = imgRun;
                                                break;
                                        }
                                    }

                                    deletingRuns.Add(childOfRun[j].Parent as Run);
                                }
                            }

                            // 在填充域
                            if (!(childOfRun[j] is FieldChar)
                                && researchState == ResearchState.IN_FILL_AREA)
                            {
                                if (i == curSeq)
                                {
                                    deletingRuns.Add(childOfRun[j].Parent as Run);
                                }
                            }

                            // 离开填充域
                            if (childOfRun[j] is FieldChar
                                && (childOfRun[j] as FieldChar).FieldCharType.Value == FieldCharValues.End
                                && researchState == ResearchState.IN_FILL_AREA)
                            {
                                researchState = ResearchState.NOT_IN_FILL_AREA;

                                if (i == curSeq)
                                {
                                    deletingRuns.Add(childOfRun[j].Parent as Run);
                                    break;
                                }
                                else
                                {
                                    ++curSeq;
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < deletingRuns.Count; i++)
                {
                    if (p.Contains(deletingRuns[i]))
                    {
                        p.RemoveChild(deletingRuns[i]);
                    }
                }
            }
            else if (String.IsNullOrEmpty(p.Descendants<Text>().CatenateAllTrimString()) && pTagInfos.Count == 1)
            {
                if (pTagInfos[0].HasFillingContents)
                {
                    List<OpenXmlElement> fillingEls = new List<OpenXmlElement>();// 添加的WordprocessingML标签
                    Stack<Paragraph> pStack = new Stack<Paragraph>();
                    List<AbstractInfo> fillingContents = pTagInfos[0].FillingContents;
                    for (int i = 0; i < fillingContents.Count; i++)
                    {
                        switch (fillingContents[i].FillContentType)
                        {
                            case FillContentType.PARAGRAPH_BEGIN:
                                Paragraph fillingParagraph = new Paragraph();
                                pStack.Push(fillingParagraph);
                                break;
                            case FillContentType.PARAGRAPH_END:
                                fillingEls.Add(pStack.Pop());
                                break;
                            case FillContentType.TXT:
                                Run txtRun = AssembleTxtRun(fillingContents[i].ConvertToDescendent<TxtInfo>());
                                pStack.Peek().AppendChild(txtRun);
                                break;
                            case FillContentType.IMG:
                                Run imgRun = AssembleImgRun(fillingContents[i].ConvertToDescendent<ImgInfo>(), outputDoc.MainDocumentPart);
                                pStack.Peek().AppendChild(imgRun);
                                break;
                            case FillContentType.TBL:
                                Table tbl = AssembleTbl(fillingContents[i].ConvertToDescendent<TblInfo>(), outputDoc.MainDocumentPart);
                                fillingEls.Add(tbl);
                                break;
                        }
                    }

                    OpenXmlElement curInsertPoint = p;
                    for (int i = 0; i < fillingEls.Count; i++)
                    {
                        // 将P的Style复制到新加入的元素中
                        int indentation;
                        string alignment;
                        GetParagraphIndentation(outputDoc, p, out indentation, out alignment);

                        /* 暂仅实现Left和Right两种缩进类型
                         * 信息：
                         * 表格缩进类型仅有（Left、Center和Right）
                         * 段落缩进类型多样
                        */
                        if (indentation != 0
                            && !string.IsNullOrEmpty(alignment))
                        {
                            if (fillingEls[i] is Table)
                            {
                                TableJustification jc = new TableJustification();
                                jc.Val = (TableRowAlignmentValues)Enum.Parse(typeof(TableRowAlignmentValues), alignment, true);
                                TableIndentation tblInd = new TableIndentation();
                                tblInd.Type = TableWidthUnitValues.Dxa;
                                tblInd.Width = indentation;
                                OpenXmlElement pr = fillingEls[i].FirstChildOrDefault<TableProperties>();
                                if (pr != null)
                                {
                                    pr.AppendChild(jc);
                                    pr.AppendChild(tblInd);
                                }
                                else
                                {
                                    TableProperties tblPr = new TableProperties();
                                    tblPr.AppendChild(jc);
                                    tblPr.AppendChild(tblInd);
                                    fillingEls[i].InsertAt(tblPr, 0);
                                }
                            }
                            else if (fillingEls[i] is Paragraph)
                            {
                                OpenXmlElement pPr = fillingEls[i].FirstChildOrDefault<ParagraphProperties>();
                                if (null != pPr)
                                {
                                    Indentation pInd = new Indentation();
                                    if (string.Equals("Left", alignment, StringComparison.OrdinalIgnoreCase))
                                    {
                                        pInd.Left = indentation.ToString();
                                    }
                                    else if (string.Equals("Right", alignment, StringComparison.OrdinalIgnoreCase))
                                    {
                                        pInd.Right = indentation.ToString();
                                    }

                                    pPr.AppendChild(pInd);
                                }
                                else
                                {
                                    ParagraphProperties customPPr = new ParagraphProperties();
                                    Indentation pInd = new Indentation();
                                    if (string.Equals("Left", alignment, StringComparison.OrdinalIgnoreCase))
                                    {
                                        pInd.Left = indentation.ToString();
                                    }
                                    else if (string.Equals("Right", alignment, StringComparison.OrdinalIgnoreCase))
                                    {
                                        pInd.Right = indentation.ToString();
                                    }

                                    customPPr.AppendChild(pInd);
                                    fillingEls[i].InsertAt(customPPr, 0);
                                }
                            }
                        }

                        // 将填充内容写入word文档
                        p.Parent.InsertAfter(fillingEls[i], curInsertPoint);
                        curInsertPoint = fillingEls[i];
                    }
                    p.Parent.RemoveChild(p);
                }
            }
        }

        /// <summary>
        /// 获取段落缩进信息
        /// 功能详述：
        ///   仅对段落缩进为左、右两种的段落获取其缩进信息（不包含居中、自左至右项右左缩进等）
        /// </summary>
        /// <param name="workDoc"></param>
        /// <param name="p"></param>
        /// <param name="indentation" direction="out">缩进距离</param>
        /// <param name="alignment" direction="out">缩进类型（Left、Right）</param>
        private void GetParagraphIndentation(WordprocessingDocument workDoc, Paragraph p, out int indentation, out string alignment)
        {
            indentation = 0;
            alignment = string.Empty;

            ParagraphProperties pPr = p.FirstChildOrDefault<ParagraphProperties>();
            if (null != pPr)
            {
                // 检查段落是否含有缩进样式设置，无则查找其样式表
                Indentation pInd = pPr.FirstChildOrDefault<Indentation>();
                Justification pJc = pPr.FirstChildOrDefault<Justification>();
                if (null != pInd
                    && null != pJc)
                {
                    if (null != pJc.Val)
                    {
                        switch (pJc.Val.Value)
                        {
                            case JustificationValues.Left:
                                indentation = (null != pInd.Left ? int.Parse(pInd.Left.Value) : 0);
                                alignment = pJc.Val.InnerText;
                                break;
                            case JustificationValues.Right:
                                indentation = (null != pInd.Right ? int.Parse(pInd.Right.Value) : 0);
                                alignment = pJc.Val.InnerText;
                                break;
                        }
                    }
                }
                else
                {
                    ParagraphStyleId pStyle = pPr.FirstChildOrDefault<ParagraphStyleId>();
                    if (null != pStyle
                        && null != pStyle.Val)
                    {
                        string styleId = pStyle.Val.Value;
                        Styles styles = workDoc.MainDocumentPart.StyleDefinitionsPart.Styles;
                        IEnumerable<Style> styleList = styles.Elements<Style>();
                        foreach (Style style in styleList)
                        {
                            if (string.Equals(style.StyleId, styleId))
                            {
                                StyleParagraphProperties pStylePr = style.FirstChildOrDefault<StyleParagraphProperties>();
                                if (null != pStylePr)
                                {
                                    Indentation pStyleInd = pStylePr.FirstChildOrDefault<Indentation>();
                                    if (null != pStyleInd.Left)
                                    {
                                        indentation = int.Parse(pStyleInd.Left.Value);
                                        alignment = "Left";
                                    }
                                    else if (null != pStyleInd.Right)
                                    {
                                        indentation = int.Parse(pStyleInd.Right.Value);
                                        alignment = "Right";
                                    }
                                }

                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 组装表格（简单表：1.无合并单元格；2.无同一列的单元格宽度不同；3.边框样式固定）
        /// </summary>
        /// <param name="tblInfo"></param>
        /// <param name="mainDocumentPart"></param>
        /// <returns></returns>
        private Table AssembleTbl(TblInfo tblInfo, MainDocumentPart mainDocumentPart)
        {
            Table tbl = new Table();
            TableProperties tblPr = new TableProperties();
            // 设置表格边框（single）
            TableBorders tblBorders = new TableBorders();
            TopBorder topBorder = new TopBorder();
            topBorder.Val = BorderValues.Single;
            topBorder.Space = 0U;
            topBorder.Color = "000000";
            tblBorders.AppendChild(topBorder);
            LeftBorder leftBorder = new LeftBorder();
            leftBorder.Val = BorderValues.Single;
            leftBorder.Space = 0U;
            leftBorder.Color = "000000";
            tblBorders.AppendChild(leftBorder);
            BottomBorder bottomBorder = new BottomBorder();
            bottomBorder.Val = BorderValues.Single;
            bottomBorder.Space = 0U;
            bottomBorder.Color = "000000";
            tblBorders.AppendChild(bottomBorder);
            RightBorder rightBorder = new RightBorder();
            rightBorder.Val = BorderValues.Single;
            rightBorder.Space = 0U;
            rightBorder.Color = "000000";
            tblBorders.AppendChild(rightBorder);
            tblPr.AppendChild(tblBorders);

            // 表格布局标签
            TableGrid tblGrid = new TableGrid();
            tbl.AppendChild(tblGrid);
            bool hasEverSet = false;
            bool hasSet = false;

            if (tblInfo.HasStyled)
            {
                if (tblInfo.Width != default(int))
                {
                    TableWidth tblW = new TableWidth();
                    tblW.Type = TableWidthUnitValues.Dxa;
                    tblW.Width = tblInfo.Width.ToString();

                    tblPr.AppendChild(tblW);
                }

                tbl.AppendChild(tblPr);
            }

            List<RowInfo> rows = tblInfo.Rows;
            for (int i = 0; i < rows.Count; i++)
            {
                TableRow tr = new TableRow();
                List<CellInfo> cells = rows[i].Cells;
                for (int j = 0; j < cells.Count; j++)
                {
                    TableCell tc = new TableCell();
                    TableCellProperties tcPr = new TableCellProperties();
                    tc.AppendChild(tcPr);

                    // 设置单元格边框（single）
                    TableCellBorders tcBorders = new TableCellBorders();
                    TopBorder tcTopBorder = new TopBorder();
                    tcTopBorder.Val = BorderValues.Single;
                    tcTopBorder.Space = 0U;
                    tcTopBorder.Color = "000000";
                    tcBorders.AppendChild(tcTopBorder);
                    LeftBorder tcLeftBorder = new LeftBorder();
                    tcLeftBorder.Val = BorderValues.Single;
                    tcLeftBorder.Space = 0U;
                    tcLeftBorder.Color = "000000";
                    tcBorders.AppendChild(tcLeftBorder);
                    BottomBorder tcBottomBorder = new BottomBorder();
                    tcBottomBorder.Val = BorderValues.Single;
                    tcBottomBorder.Space = 0U;
                    tcBottomBorder.Color = "000000";
                    tcBorders.AppendChild(tcBottomBorder);
                    RightBorder tcRightBorder = new RightBorder();
                    tcRightBorder.Val = BorderValues.Single;
                    tcRightBorder.Space = 0U;
                    tcRightBorder.Color = "000000";
                    tcBorders.AppendChild(tcRightBorder);
                    tcPr.AppendChild(tcBorders);

                    if (cells[j].HasStyled)
                    {
                        if (cells[j].Width != default(int))
                        {
                            TableCellWidth tcW = new TableCellWidth();
                            tcW.Type = TableWidthUnitValues.Dxa;
                            tcW.Width = cells[j].Width.ToString();

                            tcPr.AppendChild(tcW);

                            if (!hasSet)
                            {
                                GridColumn gridCol = new GridColumn();
                                gridCol.Width = cells[j].Width.ToString();
                                tblGrid.AppendChild(gridCol);
                                hasEverSet = true;
                            }
                        }
                    }

                    if (hasEverSet && j == cells.Count - 1)
                    {
                        hasSet = true;
                    }

                    // 暂不提供单元格合并功能
                    //if (cells[j].ColSpan != 1)
                    //{
                    //    GridSpan gridSpan = new GridSpan();
                    //    gridSpan.Val = cells[j].ColSpan;

                    //    tcPr.AppendChild(gridSpan);
                    //}
                    //if (cells[j].RowSpan != 1)
                    //{
                    //    // TODO
                    //}

                    List<AbstractInfo> legalFillingContents = cells[j]
                        .FillingContents
                        .Except(new List<FillContentType>() { FillContentType.TBL });
                    Stack<Paragraph> pStack = new Stack<Paragraph>();

                    for (int k = 0; k < legalFillingContents.Count; k++)
                    {
                        switch (legalFillingContents[k].FillContentType)
                        {
                            case FillContentType.PARAGRAPH_BEGIN:
                                Paragraph p = new Paragraph();
                                pStack.Push(p);
                                break;
                            case FillContentType.PARAGRAPH_END:
                                tc.AppendChild(pStack.Pop());
                                break;
                            case FillContentType.TXT:
                                Run txtRun = AssembleTxtRun(legalFillingContents[k].ConvertToDescendent<TxtInfo>());
                                pStack.Peek().AppendChild(txtRun);
                                break;
                            case FillContentType.IMG:
                                Run imgRun = AssembleImgRun(legalFillingContents[k].ConvertToDescendent<ImgInfo>(), mainDocumentPart);
                                pStack.Peek().AppendChild(imgRun);
                                break;
                        }
                    }

                    tr.AppendChild(tc);
                }

                tbl.AppendChild(tr);
            }

            return tbl;
        }

        /// <summary>
        /// 组装文本（只提供所有内容统一一种字体的功能）
        /// </summary>
        /// <param name="txtInfo"></param>
        /// <returns></returns>
        private Run AssembleTxtRun(TxtInfo txtInfo)
        {
            Run txtRun = new Run();
            Text txt = new Text();

            if (txtInfo.HasStyled)
            {
                RunProperties rPr = new RunProperties();
                if (!string.IsNullOrEmpty(txtInfo.ForeColor))
                {
                    Color color = new Color();
                    color.Val = txtInfo.ForeColor;
                    rPr.AppendChild(color);
                }
                if (txtInfo.HightLightColor != HighlightColorValues.None)
                {
                    Highlight hightlight = new Highlight();
                    hightlight.Val = txtInfo.HightLightColor;
                    rPr.AppendChild(hightlight);
                }
                if (txtInfo.Size != default(int))
                {
                    FontSize fontSize = new FontSize();
                    fontSize.Val = txtInfo.Size.ToString();
                    rPr.AppendChild(fontSize);
                }
                if (!string.IsNullOrEmpty(txtInfo.FontFamily))
                {
                    RunFonts rFont = new RunFonts();
                    rFont.Ascii = txtInfo.FontFamily;
                    rFont.EastAsia = txtInfo.FontFamily;
                    rFont.HighAnsi = txtInfo.FontFamily;

                    rPr.AppendChild(rFont);
                }

                txtRun.AppendChild(rPr);
            }

            txt.Text = txtInfo.Content;
            txtRun.AppendChild(txt);

            return txtRun;
        }

        /// <summary>
        /// 组装图片
        /// </summary>
        /// <param name="imgInfo"></param>
        /// <param name="mainDocumentPart"></param>
        /// <returns></returns>
        private Run AssembleImgRun(ImgInfo imgInfo, MainDocumentPart mainDocumentPart)
        {
            Run imgRun = AddImage(mainDocumentPart
                    , imgInfo.ImgPath
                    , imgInfo.Width
                    , imgInfo.Height);

            return imgRun;
        }

        #endregion
    }

    /// <summary>
    /// 在段落中遍历时的状态
    /// </summary>
    public enum ResearchState
    {
        NOT_IN_FILL_AREA,// 在非填充域
        IN_FILL_AREA// 在填充区
    }

}
