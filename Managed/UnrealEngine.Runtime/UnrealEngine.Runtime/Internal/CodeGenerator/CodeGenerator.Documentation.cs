using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    // TODO: Improvements on the "@see" tag
    // - Currently not handling comma seperated values (C# would require these as seperate tags)
    // - Currently just putting all text in the cref - need to instead seperate comment text and the cref part

    // TODO: Provide an option to put tags which aren't visible in intellisense into the main summary (<see> <remarks>)

    public partial class CodeGenerator
    {
        private void AppendDocComment(CSharpTextBuilder builder, UField field, bool isBlueprintType)
        {
            if (field == null || Settings.SkipDocumentation || string.IsNullOrEmpty(field.GetMetaData("Tooltip")))
            {
                return;
            }

            string tooltip = null;

            if (isBlueprintType)
            {
                // Blueprint metadata seems to have an internal representation which doesn't update the main metadata until reload.
                // TODO: Find the correct metadata for functions/variables for blueprint.
                // - Functions: Get function graph, call FBlueprintEditorUtils::GetGraphFunctionMetaData(graph), Metadata->ToolTip
                // - Variables: Call FBlueprintEditorUtils::GetBlueprintVariableMetaData?
                tooltip = field.GetToolTip();
            }
            else
            {
                tooltip = field.GetToolTip();
            }

            if (!string.IsNullOrEmpty(tooltip))
            {
                AppendDocComment(builder, tooltip, true);
            }
        }

        private void AppendDocComment(CSharpTextBuilder builder, string summary)
        {
            AppendDocComment(builder, summary, true);
        }

        private void AppendDocComment(CSharpTextBuilder builder, string summary, bool renameArgs)
        {
            if (Settings.SkipDocumentation || string.IsNullOrEmpty(summary))
            {
                return;
            }

            if (renameArgs)
            {
                AppendDocCommentAndRename(builder, summary);
            }
            else
            {
                AppendDocCommentSimple(builder, summary);
            }
        }

        private void AppendDocCommentSimple(CSharpTextBuilder builder, string summary)
        {
            string[] splitted = summary.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            List<StringBuilder> lines = new List<StringBuilder>();
            foreach (string line in splitted)
            {
                lines.Add(new StringBuilder(line));
            }

            // Trim the whitespace / '*' leading comment chars
            for (int i = lines.Count - 1; i >= 0; --i)
            {
                bool lineEmpty = true;
                StringBuilder line = lines[i];
                for (int charIndex = 0; charIndex < line.Length; ++charIndex)
                {
                    bool isWhiteSpace = char.IsWhiteSpace(line[charIndex]) || line[charIndex] == '*';

                    if (!isWhiteSpace)
                    {
                        lineEmpty = false;
                        if (charIndex > 0)
                        {
                            line.Remove(0, charIndex);
                        }
                        DocTimTrailingChars(line);
                        break;
                    }
                }
                if (lineEmpty)
                {
                    lines.RemoveAt(i);
                }
            }

            for (int i = 0; i < lines.Count; ++i)
            {
                StringBuilder line = lines[i];

                if (i == 0)
                {
                    if (lines.Count == 1)
                    {
                        builder.AppendLine("/// <summary>" + line.ToString() + "</summary>");
                    }
                    else
                    {
                        builder.AppendLine("/// <summary>");
                        builder.AppendLine("/// " + line.ToString());
                    }
                }
                else
                {
                    builder.AppendLine("/// " + line.ToString());
                }
            }

            if (lines.Count > 1)
            {
                builder.AppendLine("/// </summary>");
            }
        }

        private void AppendDocCommentAndRename(CSharpTextBuilder builder, string summary)
        {
            if (Settings.SkipDocumentation || string.IsNullOrEmpty(summary))
            {
                return;
            }

            string[] splitted = summary.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            List<StringBuilder> tempLines = new List<StringBuilder>();
            foreach (string line in splitted)
            {
                tempLines.Add(new StringBuilder(line));
            }

            bool injectReturnSummaryHeader = false;// An additional "Returns " header into injected summary
            int injectReturnSummaryIndex = -1;
            int returnSummaryStart = -1;

            DocSubSummary subSummaryType = DocSubSummary.None;
            Dictionary<int, DocSubSummary> beginSubSummaryLines = new Dictionary<int, DocSubSummary>();
            int firstSubSummaryIndex = -1;

            List<StringBuilder> lines = new List<StringBuilder>();

            int minMainSummaryTextOffset = int.MaxValue;

            // Get the sub summary lines and trim the whitespace / '*' leading comment chars
            for(int i = 0; i < tempLines.Count; i++)
            {
                StringBuilder line = tempLines[i];

                int firstNonWhitespaceIndex = -1;
                int atIndex = -1;//'@'
                int startTagIndex = -1;
                int endTagIndex = -1;
                int startContentIndex = -1;

                for (int charIndex = 0; charIndex < line.Length; ++charIndex)
                {
                    // Skip the whitespace / '*' leading comment chars
                    bool isWhiteSpace = char.IsWhiteSpace(line[charIndex]) || line[charIndex] == '*';

                    if (firstNonWhitespaceIndex == -1 && !isWhiteSpace)
                    {
                        firstNonWhitespaceIndex = charIndex;
                        if (firstSubSummaryIndex < 0)
                        {
                            minMainSummaryTextOffset = Math.Min(minMainSummaryTextOffset, firstNonWhitespaceIndex);
                        }
                    }

                    // Treat '-' as whitespace when getting the tag
                    if (line[charIndex] == '-')
                    {
                        isWhiteSpace = true;
                    }

                    if (atIndex == -1)
                    {
                        if (line[charIndex] == '@')
                        {
                            atIndex = charIndex;
                        }
                    }
                    else if (startTagIndex == -1)
                    {
                        if (!isWhiteSpace)
                        {
                            startTagIndex = charIndex;
                        }
                    }
                    else if (endTagIndex == -1)
                    {
                        if (isWhiteSpace)
                        {
                            endTagIndex = charIndex;
                        }
                    }
                    else if (startContentIndex == -1)
                    {
                        if (!isWhiteSpace)
                        {
                            startContentIndex = charIndex;
                            break;
                        }
                    }
                }

                DocSubSummary lineSubSummaryType = DocSubSummary.None;
                if (endTagIndex != -1)
                {
                    string tag = line.ToString(startTagIndex, endTagIndex - startTagIndex);
                    lineSubSummaryType = GetDocSubSummaryFromTag("@" + tag);
                }

                if (lineSubSummaryType != DocSubSummary.None)
                {
                    // Remove any empty trailing lines which came before this sub summary
                    for (int j = lines.Count - 1; j >= 0; j--)
                    {
                        if (lines[j].Length == 0)
                        {
                            lines.RemoveAt(j);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (firstSubSummaryIndex == -1)
                    {
                        firstSubSummaryIndex = lines.Count;
                    }

                    beginSubSummaryLines[lines.Count] = lineSubSummaryType;
                    if (startContentIndex <= 0)
                    {
                        line.Clear();
                    }
                    else
                    {
                        line.Remove(0, startContentIndex);
                    }
                    lines.Add(line);
                }
                else
                {
                    if (firstNonWhitespaceIndex < 0)
                    {
                        line.Clear();
                    }
                    else if (firstNonWhitespaceIndex > 0)
                    {
                        // Trim the whitespace (if not using a common offset or this line isn't part of the main summary)
                        if (!Settings.DocUseCommonSummaryTextOffset || firstSubSummaryIndex >= 0)
                        {                            
                            line.Remove(0, firstNonWhitespaceIndex);
                        }
                    }
                    DocTimTrailingChars(line);
                    lines.Add(line);
                }
            }

            if (Settings.DocUseCommonSummaryTextOffset && minMainSummaryTextOffset != int.MaxValue)
            {
                int mainSummaryEnd = Math.Min(lines.Count, firstSubSummaryIndex >= 0 ? firstSubSummaryIndex : int.MaxValue);
                for (int i = 0; i < mainSummaryEnd; i++)
                {
                    StringBuilder line = lines[i];
                    if (line.Length >= minMainSummaryTextOffset)
                    {
                        line.Remove(0, minMainSummaryTextOffset);
                    }
                }
            }

            // Remove any empty trailing lines
            for (int i = lines.Count - 1; i >= 0; --i)
            {
                if (lines[i].Length == 0 && !beginSubSummaryLines.ContainsKey(i))
                {
                    lines.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < lines.Count; ++i)
            {
                StringBuilder line = lines[i];

                bool nextLineBeginSummary = beginSubSummaryLines.ContainsKey(i + 1) || i == lines.Count - 1;
                if (beginSubSummaryLines.ContainsKey(i))
                {
                    if (i == 0)
                    {
                        builder.AppendLine("/// <summary></summary>");
                        if (Settings.DocInjectReturnSummary)
                        {
                            foreach (KeyValuePair<int, DocSubSummary> lineSubSummary in beginSubSummaryLines)
                            {
                                if (lineSubSummary.Value == DocSubSummary.Return)
                                {
                                    injectReturnSummaryIndex = builder.Length - ("</summary>".Length);
                                    break;
                                }
                            }
                        }
                    }
                    else if (subSummaryType == DocSubSummary.None && i > 1)
                    {
                        builder.AppendLine("/// </summary>");
                    }

                    subSummaryType = beginSubSummaryLines[i];

                    if (subSummaryType == DocSubSummary.Param ||
                        subSummaryType == DocSubSummary.Out ||
                        subSummaryType == DocSubSummary.outparam)
                    {
                        int paramNameEndIndex = -1;
                        for (int charIndex = 0; charIndex < line.Length; ++charIndex)
                        {
                            if (char.IsWhiteSpace(line[charIndex]))
                            {
                                paramNameEndIndex = charIndex;
                                break;
                            }
                        }

                        if (paramNameEndIndex != -1)
                        {
                            int paramSummaryIndex = -1;
                            for (int charIndex = paramNameEndIndex + 1; charIndex < line.Length; ++charIndex)
                            {
                                if (!char.IsWhiteSpace(line[charIndex]) && line[charIndex] != '-')
                                {
                                    paramSummaryIndex = charIndex;
                                    break;
                                }
                            }

                            string paramName = paramNameEndIndex == 0 ? string.Empty : line.ToString(0, paramNameEndIndex);
                            if (Settings.DocUpdateParamCasing)
                            {
                                paramName = GetParamName(paramName);
                            }

                            string paramSummary = paramSummaryIndex <= 0 ? string.Empty : line.ToString(paramSummaryIndex, line.Length - paramSummaryIndex);
                            if (nextLineBeginSummary)
                            {
                                paramSummary += "</param>";
                            }

                            builder.AppendLine("/// <param name=\"" + paramName + "\">" + paramSummary);
                        }
                    }
                    else if (subSummaryType == DocSubSummary.Return ||
                        subSummaryType == DocSubSummary.Note ||
                        subSummaryType == DocSubSummary.See)
                    {
                        string returnSummary = line.ToString();
                        if (nextLineBeginSummary)
                        {
                            returnSummary += GetDocSubSummaryTag(subSummaryType, true, false);
                        }
                        string openTag = GetDocSubSummaryTag(subSummaryType, true, true);

                        if (injectReturnSummaryIndex != -1 && subSummaryType == DocSubSummary.Return)
                        {
                            injectReturnSummaryHeader = !line.ToString().StartsWith("return");
                            if (nextLineBeginSummary)
                            {
                                StringBuilder injectedSummary = new StringBuilder(line.ToString());
                                if (injectReturnSummaryHeader)
                                {
                                    if (injectedSummary.Length > 0)
                                    {
                                        injectedSummary[0] = char.ToUpperInvariant(injectedSummary[0]);
                                    }
                                    injectedSummary.Insert(0, "Returns: ");
                                }
                                builder.Insert(injectReturnSummaryIndex, injectedSummary.ToString());
                                injectReturnSummaryIndex = -1;
                            }
                            else
                            {
                                // Skip newline, the indent, the comment and the tag
                                returnSummaryStart = builder.Length + builder.GetNewLineLength() +
                                    builder.GetIndentLength() + ("/// ").Length + openTag.Length;
                            }
                        }

                        builder.AppendLine("/// " + openTag + returnSummary);
                    }
                }
                else if (nextLineBeginSummary && subSummaryType != DocSubSummary.None)
                {
                    string closeTag = GetDocSubSummaryTag(subSummaryType, true, false);
                    builder.AppendLine("/// " + line.ToString() + closeTag);

                    if (injectReturnSummaryIndex != -1 && subSummaryType == DocSubSummary.Return)
                    {
                        // Skip the close tag
                        int returnSummaryEnd = builder.Length - closeTag.Length;
                        StringBuilder injectedSummary = new StringBuilder(builder.GetStringBetween(returnSummaryStart, returnSummaryEnd));
                        if (injectReturnSummaryHeader)
                        {
                            if (injectedSummary.Length > 0)
                            {
                                injectedSummary[0] = char.ToUpperInvariant(injectedSummary[0]);
                            }
                            injectedSummary.Insert(0, "Returns: ");
                        }
                        builder.Insert(injectReturnSummaryIndex, injectedSummary.ToString());
                        injectReturnSummaryIndex = -1;
                    }
                }
                else if (subSummaryType != DocSubSummary.None)
                {
                    // Multi-line sub summary (and also not the last line for this sub summary)
                    builder.AppendLine("/// " + line.ToString());
                }
                else
                {
                    // Regular summary
                    if (i == 0)
                    {
                        if (nextLineBeginSummary || lines.Count == 1)
                        {
                            builder.AppendLine("/// <summary>" + line.ToString() + "</summary>");
                        }
                        else
                        {
                            builder.AppendLine("/// <summary>");
                            builder.AppendLine("/// " + line.ToString());
                        }
                    }
                    else
                    {
                        builder.AppendLine("/// " + line.ToString());
                    }
                }
            }

            if (subSummaryType == DocSubSummary.None && lines.Count > 1)
            {
                builder.AppendLine("/// </summary>");
            }
        }

        private string GetDocSubSummaryTag(DocSubSummary subSummary, bool managed = false, bool open = true)
        {
            if (managed)
            {
                switch (subSummary)
                {
                    case DocSubSummary.Out:
                    case DocSubSummary.outparam:
                    case DocSubSummary.Param: return open ? "<param>" : "</param>";
                    case DocSubSummary.Returns:
                    case DocSubSummary.Return: return open ? "<returns>" : "</returns>";
                    case DocSubSummary.Note: return open ? "<remarks>" : "</remarks>";
                    case DocSubSummary.See: return open ? "<see cref=\"" : "\"/>";
                    default: return open ? "<!--" : "-->";
                }
            }
            else
            {
                switch (subSummary)
                {
                    case DocSubSummary.Out: return "@out";
                    case DocSubSummary.outparam: return "@outparam";
                    case DocSubSummary.Param: return "@param";
                    case DocSubSummary.Return: return "@return";
                    case DocSubSummary.Returns: return "@returns";
                    case DocSubSummary.Note: return "@note";
                    case DocSubSummary.See: return "@see";
                    default: return "@unknown";
                }
            }
        }

        private DocSubSummary GetDocSubSummaryFromTag(string tag)
        {
            if (tag == GetDocSubSummaryTag(DocSubSummary.Param) ||
                tag == GetDocSubSummaryTag(DocSubSummary.Out) ||
                tag == GetDocSubSummaryTag(DocSubSummary.outparam))
            {
                return DocSubSummary.Param;
            }
            if (tag == GetDocSubSummaryTag(DocSubSummary.Return) ||
                tag == GetDocSubSummaryTag(DocSubSummary.Returns))
            {
                return DocSubSummary.Return;
            }
            if (tag == GetDocSubSummaryTag(DocSubSummary.Note))
            {
                return DocSubSummary.Note;
            }
            if (tag == GetDocSubSummaryTag(DocSubSummary.See))
            {
                return DocSubSummary.See;
            }
            return DocSubSummary.None;
        }

        private void DocTimTrailingChars(StringBuilder str)
        {
            if (!Settings.DocTrimTrailingChars)
            {
                return;
            }

            for (int i = str.Length - 1; i >= 0; --i)
            {
                if (!char.IsWhiteSpace(str[i]) && str[i] != '*')
                {
                    int trimIndex = i + 1;
                    int trimCount = str.Length - trimIndex;
                    if (trimCount > 0)
                    {
                        str.Remove(trimIndex, trimCount);
                    }
                    break;
                }
            }
        }

        enum DocSubSummary
        {
            None,
            Param,
            Return,
            Returns,
            Note,
            See,
            Out,
            outparam
        }
    }
}
