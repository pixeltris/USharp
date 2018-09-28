using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public partial class CodeGenerator
    {
        // <EnumPath, EnumValuePrefix>   EnumPath=Enum->GetPathName()   EnumValuePrefix = Value prefix e.g. CPF_
        private Dictionary<string, string> enumValuePrefixCache = new Dictionary<string, string>();

        private string GetEnumValuePrefix(UEnum unrealEnum)
        {
            string enumPrefix;
            if (enumValuePrefixCache.TryGetValue(unrealEnum.GetPathName(), out enumPrefix))
            {
                return enumPrefix;
            }

            GetEnumValues(unrealEnum, false);

            if (enumValuePrefixCache.TryGetValue(unrealEnum.GetPathName(), out enumPrefix))
            {
                return enumPrefix;
            }
            return null;
        }

        private string GetEnumValueName(UEnum unrealEnum, int index)
        {
            string qualifiedValueName = unrealEnum.GetNameByIndex((byte)index).ToString();
            int colonPos = qualifiedValueName.IndexOf("::");

            string rawName = null;
            if (colonPos >= 0)
            {
                rawName = qualifiedValueName.Substring(colonPos + 2);
            }
            else
            {
                rawName = qualifiedValueName;
            }
            return rawName;
        }

        private List<EnumValueInfo> GetEnumValues(UEnum unrealEnum, bool getDocumentation)
        {
            int valueCount = unrealEnum.NumEnums();
            bool isBlueprintEnum = unrealEnum.IsA<UUserDefinedEnum>();

            List<EnumValueInfo> enumValues = new List<EnumValueInfo>(valueCount);

            // Try to identify a common prefix of the form PRE_, so we can strip it from all values.
            // We'll only strip it if it's present on all values not explicitly skipped.
            string commonPrefix = null;
            int commonPrefixCount = 0;
            int skippedValueCount = 0;

            int numMax = 0;
            if (Settings.RemoveEnumMAX)
            {
                // Skip all ending "MAX" values (Some enums have duplicate MAX (DORN_MAX / ENetDormancy_MAX))
                for (int i = valueCount - 1; i >= 0; --i)
                {
                    string rawName = GetEnumValueName(unrealEnum, i);

                    // Using case sensitive here to avoid removing genuine "Max" values (still may be removing genuine "MAX" values)
                    if (rawName.EndsWith("MAX"))
                    {
                        ++numMax;
                        ++skippedValueCount;

                        // Duplicate MAX should be "XXX_MAX" for last value and "MAX" for second to last value
                        if (i < valueCount - 1 || !rawName.EndsWith("_MAX"))
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            //if (numMax > 1)
            //{
            //    FMessage.Log("Duplicate MAX in enum " + unrealEnum.GetPathName());
            //}

            for (int i = 0; i < valueCount - numMax; ++i)
            {
                string rawName = GetEnumValueName(unrealEnum, i);

                EnumValueInfo enumValue = new EnumValueInfo();
                enumValue.Index = i;
                enumValue.Value = unrealEnum.GetValueByIndex(i);
                enumValue.Name = rawName;
                enumValue.DisplayName = MakeValidName(unrealEnum.GetDisplayNameTextStringByIndex(i));
                if (getDocumentation)
                {
                    enumValue.DocCommentSummary = unrealEnum.GetToolTipByIndex(i);
                }
                enumValues.Add(enumValue);

                // We can skip all of the common prefix checks for enums that are already namespaced in C++.
                // In the cases where a namespaced enum does have a common prefix for its values, it doesn't
                // match the PRE_* pattern, and it's generally necessary for syntactic reasons, 
                // i.e. Touch1, Touch2, and so on in ETouchIndex.
                if (unrealEnum.GetCppForm() == UEnum.ECppForm.Regular)
                {
                    // A handful of enums have bad values named this way in C++.
                    if (rawName.StartsWith("TEMP_BROKEN"))
                    {
                        ++skippedValueCount;
                    }
                    // UHT inserts spacers for sparse enums.  Since we're omitting the _MAX value, we'll
                    // still export these to ensure that C# reflection gives an accurate value count, but
                    // don't hold them against the common prefix count.
                    else if (rawName.StartsWith("UnusedSpacer_"))
                    {
                        ++skippedValueCount;
                    }
                    // Infer the prefix from the first unskipped value.
                    else if (string.IsNullOrEmpty(commonPrefix))
                    {
                        int underscorePos = rawName.IndexOf("_");
                        if (underscorePos >= 0)
                        {
                            commonPrefix = rawName.Substring(underscorePos + 1);
                            ++commonPrefixCount;
                        }
                    }
                    else if (rawName.StartsWith(commonPrefix))
                    {
                        ++commonPrefixCount;
                    }
                }
            }

            if (valueCount != (commonPrefixCount + skippedValueCount))
            {
                //if (!string.IsNullOrEmpty(commonPrefix))
                //{
                //    FMessage.Log(string.Format("Rejecting common prefix '{0}' for '{1}' ({2}). ValueCount={3}, CommonPrefixCount={4}, SkippedValueCount={5}",
                //        commonPrefix, unrealEnum.GetName(), unrealEnum.GetFName().DisplayIndex, valueCount, commonPrefixCount, skippedValueCount));
                //}

                commonPrefix = null;
            }

            foreach (EnumValueInfo enumValue in enumValues)
            {
                if (!string.IsNullOrEmpty(commonPrefix))
                {
                    enumValue.Name = enumValue.Name.RemoveFromStart(commonPrefix);
                }

                //one enum has a member called "float" which isn't valid in C#. That said, C# enum values should be PascalCase anyway, so just uppercase it.
                if (char.IsLower(enumValue.Name[0]))
                {
                    enumValue.Name = char.ToUpperInvariant(enumValue.Name[0]) + enumValue.Name.Substring(1);
                }
            }

            // Update the enum prefix cache for lookup with default function params
            enumValuePrefixCache[unrealEnum.GetPathName()] = commonPrefix;

            return enumValues;
        }

        class EnumValueInfo
        {
            public int Index { get; set; }
            public long Value { get; set; }
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public string DocCommentSummary { get; set; }
        }
    }
}
