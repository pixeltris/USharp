using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    public partial class CodeGenerator
    {
        /// <summary>
        /// All types to export (struct/class/enum) depending on the Settings.ExportMode (if mode is "All" ExportTypes will be left empty)
        /// </summary>
        private HashSet<UField> availableTypes = new HashSet<UField>();

        private void UpdateAvailableTypes()
        {
			availableTypes.Clear();

            if (Settings.ExportMode == CodeGeneratorSettings.CodeExportMode.All)
            {
				return;
            }

            foreach (UStruct unrealStruct in UObject.GetObjects<UStruct>())
            {
                if (!unrealStruct.IsA<UFunction>() && IsBlueprintVisibleStruct(unrealStruct))
                {
                    UpdateAvailableTypes(unrealStruct);
                }
            }

            foreach (UEnum unrealEnum in UObject.GetObjects<UEnum>())
            {
                if (CanExportEnum(unrealEnum) && IsBlueprintVisibleEnum(unrealEnum))
                {
                    UpdateAvailableTypes(unrealEnum);
                }
            }

			// cache UPackage class
			UClass packageClass = UClass.GetClass<UPackage>();

            if (Settings.ExportMode == CodeGeneratorSettings.CodeExportMode.Referenced)
            {
                // Get struct refs from global functions (delegates)
                // - Should we check if the delegate is even referenced?
                foreach (UFunction function in UObject.GetObjects<UFunction>())
                {
					bool isGlobal = function.GetOuter().GetClass() == packageClass;
                    if (isGlobal && function.HasAnyFunctionFlags(EFunctionFlags.Delegate | EFunctionFlags.MulticastDelegate))
                    {
                        UpdateAvailableTypes(function);
                    }
                }
            }
        }

        private void UpdateAvailableTypes(UField field)
        {
            if (field == null)
            {
				return;
            }

            System.Diagnostics.Debug.Assert(!field.IsA<UProperty>(), "Shouldn't have UProperty here");

            if (field.IsA<UStruct>() || field.IsA<UEnum>())
            {
				bool isNewElement = availableTypes.Add(field);
                if (!isNewElement)
                {
					return;
                }
            }

            if (Settings.ExportMode != CodeGeneratorSettings.CodeExportMode.Referenced)
            {
				return;
            }

            // Get all of the references from this struct
			UStruct unrealStruct = field as UStruct;
            if (unrealStruct != null)
            {
				bool isBlueprintType = unrealStruct.IsA<UUserDefinedStruct>() || unrealStruct.IsA<UBlueprintGeneratedClass>();

                // Get struct references from parent class chain
				UStruct parentStruct = unrealStruct.GetSuperStruct();
                while (parentStruct != null)
                {
					UpdateAvailableTypes(parentStruct);
					parentStruct = parentStruct.GetSuperStruct();
                }

                // Get references from interfaces
                UClass unrealClass = field as UClass;
                if (unrealClass != null)
                {
                    foreach (FImplementedInterface implementedInterface in unrealClass.Interfaces)
                    {
						UpdateAvailableTypes(implementedInterface.InterfaceClass);
                    }
                }
				
                // Get struct references from members
                foreach (UProperty property in unrealStruct.GetFields<UProperty>(false))
                {
                    if (CanExportProperty(property, unrealStruct, isBlueprintType))
                    {
                        UpdateAvailableTypesProp(property);
                    }
                }

                // Get struct references from function params (and return type)
                foreach (UFunction unrealFunction in unrealStruct.GetFields<UFunction>(false))
                {
                    if (CanExportFunction(unrealFunction, isBlueprintType))
                    {
                        foreach (UProperty parameter in unrealFunction.GetFields<UProperty>())
                        {
                            UpdateAvailableTypesProp(parameter);
                        }
                    }
                }
            }

            // This should be for global functions only (delegates)
			UFunction function = field as UFunction;
            if (function != null)
            {
                if (CanExportFunction(function, false))
                {
					UStruct functionOwner = function.GetOuter() as UStruct;
                    if (functionOwner != null)
                    {
                        UpdateAvailableTypes(functionOwner);
                    }

                    foreach (UProperty parameter in function.GetFields<UProperty>())
                    {
                        UpdateAvailableTypesProp(parameter);
                    }
                }
            }
        }

        private void UpdateAvailableTypesProp(UProperty property)
        {
			UField field1 = null;
			UField field2 = null;
            GetStructEnumOrFuncFromProp(property, out field1, out field2);
            if (field1 != null)
            {
				UpdateAvailableTypes(field1);
            }
            if (field2 != null)
            {
				UpdateAvailableTypes(field2);
            }
        }

        private bool IsAvailableType(UField field)
        {
			return Settings.ExportMode == CodeGeneratorSettings.CodeExportMode.All || availableTypes.Contains(field);
        }
    }
}
