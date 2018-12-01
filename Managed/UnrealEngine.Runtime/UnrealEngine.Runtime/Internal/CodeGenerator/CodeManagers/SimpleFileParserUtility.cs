using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnrealEngine.Runtime;

namespace UnrealEngine.Runtime.Utilities
{
    class SimpleFileParserUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_filePath"></param>
        /// <param name="_lineContentLookFor"></param>
        /// <param name="_columnContentLookFrom"></param>
        /// <param name="_openAndCloseParseRef">Example Would Be " Or { Or [</param>
        /// <param name="_closeParseRef">Example Would Be " Or } Or ]</param>
        /// <returns></returns>
        public static string ObtainStringFromSharedFileContent(string _filePath, string _lineContentLookFor, string _columnContentLookFrom, char _openParseRef, char _closeParseRef)
        {
            return ObtainStringFromContentArray(ObtainContentFromSharedFile(_filePath), _lineContentLookFor, _columnContentLookFrom, _openParseRef, _closeParseRef);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_content"></param>
        /// <param name="_lineContentLookFor"></param>
        /// <param name="_columnContentLookFrom"></param>
        /// <param name="_openAndCloseParseRef">Example Would Be " Or { Or [</param>
        /// <param name="_closeParseRef">Example Would Be " Or } Or ]</param>
        /// <returns></returns>
        public static string ObtainStringFromContentArray(string[] _content, string _lineContentLookFor, string _columnContentLookFrom, char _openParseRef, char _closeParseRef)
        {
            string _foundPath = "";
            if (_content == null || _content.Length <= 0 || string.IsNullOrEmpty(_lineContentLookFor) ||
                string.IsNullOrEmpty(_columnContentLookFrom))
            {
                LogFromSimpleFileParser("Couldn't Obtain String From Content Array Due To Parameters Being Invalid For Checking");
                return _foundPath;
            }

            int _lineContentLookForIndex = ObtainLastIndexInAllLineContains(_content, _lineContentLookFor);
            if(_lineContentLookForIndex != -1)
            {
                string _fulllineFound = _content[_lineContentLookForIndex];
                int _columnContentLookFromIndex = _fulllineFound.IndexOf(_columnContentLookFrom);
                if(_columnContentLookFromIndex != -1)
                {
                    string _foundPathInCharacters = ObtainStringFromLine(_openParseRef, _closeParseRef, _fulllineFound, _columnContentLookFromIndex);
                }
                else
                {
                    LogFromSimpleFileParser("Didn't find ColumnContentLookFrom: " + _columnContentLookFrom + " in Line: " + _fulllineFound);
                }
            }
            else
            {
                LogFromSimpleFileParser("Couldn't Find Instance of LineContentLookFor: " + _lineContentLookFor);
            }
            return _foundPath;
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
            int _firstLineCheckIndex = FindFirstIndexOfGivenCharacter(lineToObtainCharsFrom, columnToStartFromIndex, openParseRef);
            List<char> _foundPathInCharacters = new List<char>();
            if (_firstLineCheckIndex != -1)
            {
                for (int i = _firstLineCheckIndex; i < lineToObtainCharsFrom.Length; i++)
                {
                    //Checks Char After Current One For CloseParseRef(Such as Quotation Mark)
                    if (lineToObtainCharsFrom[i + 1] == closeParseRef)
                    {
                        break;
                    }
                    else
                    {
                        _foundPathInCharacters.Add(lineToObtainCharsFrom[i + 1]);
                    }
                }
            }
            else
            {
                LogFromSimpleFileParser("Couldn't Find Instance of OpenParseRef: " + openParseRef + " Inside found Line: " + lineToObtainCharsFrom);
            }

            return _foundPathInCharacters;
        }

        public static string[] ObtainContentFromSharedFile(string _filePath)
        {
            if (!string.IsNullOrEmpty(_filePath) && _filePath.Length > 2 && File.Exists(_filePath))
            {
                string _fileStreamLine;
                List<string> _logContentList = new List<string>();
                using (var _stream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader _reader = new StreamReader(_stream))
                {
                    while ((_fileStreamLine = _reader.ReadLine()) != null)
                    {
                        _logContentList.Add(_fileStreamLine);
                    }
                }

                string[] _logContent = _logContentList.ToArray();
                return _logContent;
            }
            return new string[] { };
        }

        public static int ObtainFirstIndexIfContains(string[] _content, string _contains)
        {
            for (int i = 0; i < _content.Length; i++)
            {
                if (_content[i].Contains(_contains))
                {
                    return i;
                }
            }
            return -1;
        }

        public static List<int> ObtainAllLineIndexsIfContains(string[] _content, string _contains)
        {
            List<int> _allIndexes = new List<int>();
            for (int i = 0; i < _content.Length; i++)
            {
                if (_content[i].Contains(_contains))
                {
                    _allIndexes.Add(i);
                }
            }
            return _allIndexes;
        }

        public static int ObtainLastIndexInAllLineContains(string[] _content, string _contains)
        {
            var _indexes = ObtainAllLineIndexsIfContains(_content, _contains);
            int _last = -1;
            foreach (var _index in _indexes)
            {
                if (_index > _last)
                {
                    _last = _index;
                }
            }
            return _last;
        }

        public static int FindFirstIndexOfGivenCharacter(string _line, int _startIndex, char _contains)
        {
            if (_startIndex < _line.Length)
            {
                for (int i = _startIndex; i < _line.Length; i++)
                {
                    if (_line[i] == _contains) return i;
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
