using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Engine;

namespace UnrealEngine.Runtime
{
    public partial class CodeGenerator
    {
        private bool CanExportEnum(UEnum unrealEnum)
        {
            // Skip enums which are already defined in this project
            if (projectDefinedTypes.ContainsKey(unrealEnum.GetPathName()))
            {
                return false;
            }

            return true;
        }

        private bool IsBlueprintVisibleEnum(UEnum unrealEnum)
        {
            return unrealEnum.GetBoolMetaData(MDEnum.BlueprintType);
        }

        private void GenerateCodeForEnums(UnrealModuleInfo module, UEnum[] enums, bool combine)
        {
            if (enums.Length == 0)
            {
                return;
            }

            if (combine)
            {
                // Put all enums into a single file prefixed with the module name
                string enumsName = module.Name + "Enums";

                UnrealModuleType moduleAssetType;
                string currentNamespace = GetModuleNamespace(enums[0], out moduleAssetType, false);
                List<string> namespaces = GetDefaultNamespaces();

                CSharpTextBuilder builder = new CSharpTextBuilder(Settings.IndentType);
                if (!string.IsNullOrEmpty(currentNamespace))
                {
                    builder.AppendLine("namespace " + currentNamespace);
                    builder.OpenBrace();
                }

                UEnum lastEnum = enums.Last();

                foreach (UEnum unrealEnum in enums)
                {
                    SlowTaskStep(unrealEnum);
                    GenerateCodeForEnum(module, builder, unrealEnum);

                    if (unrealEnum != lastEnum)
                    {
                        builder.AppendLine();
                    }
                }

                if (!string.IsNullOrEmpty(currentNamespace))
                {
                    builder.CloseBrace();
                }

                builder.InsertNamespaces(currentNamespace, namespaces, Settings.SortNamespaces);

                OnCodeGenerated(module, moduleAssetType, enumsName, null, builder);
            }
            else
            {
                foreach (UEnum unrealEnum in enums)
                {
                    SlowTaskStep(unrealEnum);
                    GenerateCodeForEnum(module, unrealEnum);
                }
            }
        }

        private void GenerateCodeForEnum(UnrealModuleInfo module, CSharpTextBuilder builder, UEnum unrealEnum)
        {
            bool isBlueprintType = unrealEnum.IsA<UUserDefinedEnum>();

            AppendDocComment(builder, unrealEnum, isBlueprintType);
            AppendAttribute(builder, unrealEnum, module);

            // Set the underlying enum type if this enum is tagged as a BlueprintType
            string enumUnderlyingType = string.Empty;
            if (unrealEnum.HasMetaData(MDEnum.BlueprintType))
            {
                enumUnderlyingType = " : byte";
            }

            builder.AppendLine("public enum " + GetTypeName(unrealEnum) + enumUnderlyingType);
            builder.OpenBrace();

            // TODO: Blueprint value bitflags
            // According to issue UE-32816 "enum values are currently assumed to be flag indices and not actual flag mask values"
            List<EnumValueInfo> enumValues = GetEnumValues(unrealEnum, true);

            int lastEnumIndex = enumValues.Count;
            foreach (EnumValueInfo enumValue in enumValues)
            {
                AppendDocComment(builder, enumValue.DocCommentSummary);
                if (isBlueprintType)
                {
                    builder.AppendLine("[EnumValueName(\"" + enumValue.Name + "\")]");
                }
                builder.AppendLine(string.Format("{0}={1}{2}",
                    isBlueprintType ? enumValue.DisplayName : enumValue.Name,
                    enumValue.Value,
                    --lastEnumIndex > 0 ? "," : string.Empty));
            }

            builder.CloseBrace();
        }

        private void GenerateCodeForEnum(UnrealModuleInfo module, UEnum unrealEnum)
        {
            UnrealModuleType moduleAssetType;
            string currentNamespace = GetModuleNamespace(unrealEnum, out moduleAssetType);
            List<string> namespaces = GetDefaultNamespaces();

            CSharpTextBuilder builder = new CSharpTextBuilder(Settings.IndentType);
            if (!string.IsNullOrEmpty(currentNamespace))
            {
                builder.AppendLine("namespace " + currentNamespace);
                builder.OpenBrace();
            }

            GenerateCodeForEnum(module, builder, unrealEnum);

            if (!string.IsNullOrEmpty(currentNamespace))
            {
                builder.CloseBrace();
            }

            builder.InsertNamespaces(currentNamespace, namespaces, Settings.SortNamespaces);

            OnCodeGenerated(module, moduleAssetType, GetTypeName(unrealEnum), unrealEnum.GetPathName(), builder);
        }
    }
}
