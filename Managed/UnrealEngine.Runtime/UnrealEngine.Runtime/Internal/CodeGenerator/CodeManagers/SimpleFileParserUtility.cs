using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnrealEngine.Runtime.Utilities
{
    class SimpleFileParserUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lineContentLookFor"></param>
        /// <param name="columnContentLookFrom"></param>
        /// <param name="openParseRef">Example Would Be " Or { Or [</param>
        /// <param name="closeParseRef">Example Would Be " Or } Or ]</param>
        /// <returns></returns>
        public static string ObtainStringFromSharedFileContent(string filePath, string lineContentLookFor, string columnContentLookFrom, char openParseRef, char closeParseRef)
        {
            return ObtainStringFromContentArray(ObtainContentFromSharedFile(filePath), lineContentLookFor, columnContentLookFrom, openParseRef, closeParseRef);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="lineContentLookFor"></param>
        /// <param name="columnContentLookFrom"></param>
        /// <param name="openParseRef">Example Would Be " Or { Or [</param>
        /// <param name="closeParseRef">Example Would Be " Or } Or ]</param>
        /// <returns></returns>
        public static string ObtainStringFromContentArray(string[] content, string lineContentLookFor, string columnContentLookFrom, char openParseRef, char closeParseRef)
        {
            string foundPath = "";
            if (content == null || content.Length <= 0 || string.IsNullOrEmpty(lineContentLookFor) ||
                string.IsNullOrEmpty(columnContentLookFrom))
            {
                LogFromSimpleFileParser("Couldn't Obtain String From Content Array Due To Parameters Being Invalid For Checking");
                return foundPath;
            }

            int lineContentLookForIndex = ObtainLastIndexInAllLineContains(content, lineContentLookFor);
            if(lineContentLookForIndex != -1)
            {
                string fulllineFound = content[lineContentLookForIndex];
                int columnContentLookFromIndex = fulllineFound.IndexOf(columnContentLookFrom);
                if(columnContentLookFromIndex != -1)
                {
                    foundPath = ObtainStringFromLine(openParseRef, closeParseRef, fulllineFound, columnContentLookFromIndex);
                }
                else
                {
                    LogFromSimpleFileParser("Didn't find ColumnContentLookFrom: " + columnContentLookFrom + " in Line: " + fulllineFound);
                }
            }
            else
            {
                LogFromSimpleFileParser("Couldn't Find Instance of LineContentLookFor: " + lineContentLookFor);
            }
            return foundPath;
        }

        public static string ObtainStringFromLine(char openParseRef, char closeParseRef, string lineToObtainCharsFrom, int columnToStartFromIndex)
        {
            List<char> obtainedChars = ObtainCharactersFromLine(openParseRef, closeParseRef, lineToObtainCharsFrom, columnToStartFromIndex);
            if(obtainedChars.Count > 0)
            {
                return new string(obtainedChars.ToArray());
            }
            return null;
        }

        public static List<char> ObtainCharactersFromLine(char openParseRef, char closeParseRef, string lineToObtainCharsFrom, int columnToStartFromIndex)
        {
            //Check For First Instance of OpenParseRef
            //(Such as Quotation Mark) After Archived Directory Arg
            int firstLineCheckIndex = FindFirstIndexOfGivenCharacter(lineToObtainCharsFrom, columnToStartFromIndex, openParseRef);
            List<char> foundPathInCharacters = new List<char>();
            if (firstLineCheckIndex != -1)
            {
                for (int i = firstLineCheckIndex; i < lineToObtainCharsFrom.Length; i++)
                {
                    //Checks Char After Current One For CloseParseRef(Such as Quotation Mark)
                    if (lineToObtainCharsFrom[i + 1] == closeParseRef)
                    {
                        break;
                    }
                    else
                    {
                        foundPathInCharacters.Add(lineToObtainCharsFrom[i + 1]);
                    }
                }
            }
            else
            {
                LogFromSimpleFileParser("Couldn't Find Instance of OpenParseRef: " + openParseRef + " Inside found Line: " + lineToObtainCharsFrom);
            }

            return foundPathInCharacters;
        }

        public static string[] ObtainContentFromSharedFile(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && filePath.Length > 2 && File.Exists(filePath))
            {
                string fileStreamLine;
                List<string> logContentList = new List<string>();
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader reader = new StreamReader(stream))
                {
                    while ((fileStreamLine = reader.ReadLine()) != null)
                    {
                        logContentList.Add(fileStreamLine);
                    }
                }

                string[] _logContent = logContentList.ToArray();
                return _logContent;
            }
            return new string[] { };
        }

        public static int ObtainFirstIndexIfContains(string[] content, string contains)
        {
            for (int i = 0; i < content.Length; i++)
            {
                if (content[i].Contains(contains))
                {
                    return i;
                }
            }
            return -1;
        }

        public static List<int> ObtainAllLineIndexsIfContains(string[] content, string contains)
        {
            List<int> allIndexes = new List<int>();
            for (int i = 0; i < content.Length; i++)
            {
                if (content[i].Contains(contains))
                {
                    allIndexes.Add(i);
                }
            }
            return allIndexes;
        }

        public static int ObtainLastIndexInAllLineContains(string[] content, string contains)
        {
            var indexes = ObtainAllLineIndexsIfContains(content, contains);
            int last = -1;
            foreach (var index in indexes)
            {
                if (index > last)
                {
                    last = index;
                }
            }
            return last;
        }

        public static int FindFirstIndexOfGivenCharacter(string line, int startIndex, char contains)
        {
            if (startIndex < line.Length)
            {
                for (int i = startIndex; i < line.Length; i++)
                {
                    if (line[i] == contains) return i;
                }
            }
            return -1;
        }

        private static void LogFromSimpleFileParser(string msg)
        {
            FMessage.Log(ELogVerbosity.Log, "SimpleFileParser: " + msg);
        }
    }
}
