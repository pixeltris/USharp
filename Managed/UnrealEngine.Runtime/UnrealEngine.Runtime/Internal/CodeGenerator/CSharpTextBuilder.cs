using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public class CSharpTextBuilder
    {
        private StringBuilder text = new StringBuilder();
        private int numPrevEmptyLines;// Number of lines just written which were empty
        private int indent;
        private IndentType indentType;

        public int Length
        {
            get { return text.Length; }
        }

        public int IndentCount
        {
            get { return indent; }
            set { indent = value; }
        }

        public CSharpTextBuilder(IndentType indentType = IndentType.Spaces)
        {
            this.indentType = indentType;
            indent = 0;
        }

        public string GetIndentStr()
        {
            return indentType == IndentType.Spaces ? "    " : "\t";
        }

        public int GetIndent()
        {
            return indent;
        }

        public int GetIndentLength()
        {
            return indentType == IndentType.Spaces ? indent * 4 : indent;
        }

        public int GetNewLineLength()
        {
            // Assuming StringBuilder uses Environment.NewLine
            return Environment.NewLine.Length;
        }

        public void Indent()
        {
            ++indent;
        }

        public void Unindent()
        {
            --indent;
        }

        public void AppendLine()
        {
            numPrevEmptyLines++;

            if (text.Length != 0)
            {
                text.AppendLine();
            }

            string indentStr = GetIndentStr();
            for (int i = 0; i < indent; i++)
            {
                text.Append(indentStr);
            }
        }

        public void AppendLine(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                AppendLine();
            }
            else
            {
                AppendLine();
                text.Append(line);
                numPrevEmptyLines = 0;
            }
        }

        public void AppendLine(FName line)
        {
            AppendLine(line.ToString());
        }

        public void InsertLine(int index, string line)
        {
            InsertLine(index, 0, line);
        }

        public void InsertLine(int index, int indent = 0, string line = null)
        {
            if (index >= text.Length)
            {
                numPrevEmptyLines = 0;
            }

            StringBuilder indentedLine = new StringBuilder();

            string indentStr = GetIndentStr();
            for (int i = 0; i < indent; i++)
            {
                indentedLine.Append(indentStr);
            }

            indentedLine.AppendLine(line);
            text.Insert(index, indentedLine.ToString());
        }

        public void Insert(int index, string str)
        {
            text.Insert(index, str);
        }

        public void RemovePreviousEmptyLine()
        {
            if (numPrevEmptyLines <= 0)
            {
                return;
            }
            int removeCount = GetIndentLength() + GetNewLineLength();
            if (removeCount > 0)
            {
                text.Remove(text.Length - removeCount, removeCount);
            }
            numPrevEmptyLines--;
        }

        public void RemovePreviousEmptyLines()
        {
            while (numPrevEmptyLines > 0)
            {
                RemovePreviousEmptyLine();
            }
        }

        public void OpenBrace()
        {
            AppendLine("{");
            Indent();
        }

        public void CloseBrace()
        {
            Unindent();
            AppendLine("}");
        }

        public void Clear()
        {
            text.Clear();
            numPrevEmptyLines = 0;
        }

        public override string ToString()
        {
            return text.ToString();
        }

        public string GetStringBetween(int start, int end)
        {
            return text.ToString().Substring(start, end - start);
        }

        public void InsertNamespaces(string currentNamespace, List<string> namespaces, bool sortNamespaces)
        {
            if (sortNamespaces)
            {
                // Reversed as InsertLine will insert each line at line index 0
                namespaces.Sort((x, y) => -x.CompareTo(y));
            }

            bool hasNamespace = false;

            foreach (string namespaceName in namespaces)
            {
                if (namespaceName != currentNamespace)
                {
                    if (!hasNamespace)
                    {
                        InsertLine(0);
                        hasNamespace = true;
                    }
                    InsertLine(0, "using " + namespaceName + ";");
                }
            }
        }

        public enum IndentType
        {
            Spaces,
            Tabs
        }
    }
}
