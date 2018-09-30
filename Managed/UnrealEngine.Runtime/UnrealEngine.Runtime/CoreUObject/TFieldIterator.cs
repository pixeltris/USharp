using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnrealEngine.Runtime
{
    /// <summary>
    /// For iterating through a linked list of fields.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TFieldIterator<T> : IEnumerator<T>, IEnumerable<T> where T : UObject
    {
        /// <summary>
        /// The T UClass
        /// </summary>
        private UClass typeClass;

        /// <summary>
        /// The target struct which is held onto for .Reset() which resets unrealStruct to targetStruct
        /// </summary>
        private UStruct targetStruct;

        /// <summary>
        /// The object being searched for the specified field
        /// </summary>
        private UStruct unrealStruct;

        /// <summary>
        /// The current location in the list of fields being iterated
        /// </summary>
        private UField field;

        /// <summary>
        /// The index of the current interface being iterated
        /// </summary>
        private int interfaceIndex;

        /// <summary>
        /// Whether to include the super class or not
        /// </summary>
        private bool includeSuper;

        /// <summary>
        /// Whether to include deprecated fields or not
        /// </summary>
        private bool includeDeprecated;

        /// <summary>
        /// Whether to include interface fields or not
        /// </summary>
        private bool includeInterface;

        private EFieldIteratorType iteratorType;
        private bool first = false;

        public TFieldIterator(UStruct unrealStruct,
            bool includeSuper = true,
            bool includeDeprecated = true,
            bool includeInterface = false) 
            : this(unrealStruct, EFieldIteratorType.Children, includeSuper, includeDeprecated, includeInterface)
        {
        }

        public TFieldIterator(UStruct unrealStruct,
            EFieldIteratorType iteratorType,
            bool includeSuper = true,
            bool includeDeprecated = true,
            bool includeInterface = false)
        {
            typeClass = UClass.GetClass<T>();
            targetStruct = unrealStruct;            
            this.iteratorType = iteratorType;
            this.unrealStruct = unrealStruct;
            field = GetField(unrealStruct);
            interfaceIndex = -1;
            this.includeSuper = includeSuper;
            this.includeDeprecated = includeDeprecated;
            this.includeInterface = includeInterface;
            first = true;
        }

        public UStruct GetStruct()
        {
            return unrealStruct;
        }

        public T Current
        {
            get { return field as T; }
        }

        object IEnumerator.Current
        {
            get { return field; }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (first)
            {
                first = false;
            }
            else
            {
                field = field.Next;
            }

            UField currentField = field;
            UStruct currentStruct = unrealStruct;

            while (currentStruct != null)
            {
                while (currentField != null)
                {
                    UClass fieldClass = currentField.GetClass();

                    if (fieldClass.HasAllCastFlags(typeClass.ClassCastFlags) && (includeDeprecated ||
                        !fieldClass.HasAllCastFlags(EClassCastFlags.UProperty) ||
                        !(currentField as UProperty).HasAllPropertyFlags(EPropertyFlags.Deprecated)))
                    {
                        unrealStruct = currentStruct;
                        field = currentField;
                        return true;
                    }

                    currentField = currentField.Next;
                }

                if (includeInterface)
                {
                    // We shouldn't be able to get here for non-classes
                    UClass currentClass = currentStruct as UClass;
                    ++interfaceIndex;
                    FImplementedInterface[] interfaces = currentClass.Interfaces;
                    if (interfaces != null && interfaceIndex < interfaces.Length)
                    {
                        UClass interfaceClass = interfaces[interfaceIndex].InterfaceClass;
                        currentField = GetField(interfaceClass);
                        continue;
                    }
                }

                if (includeSuper)
                {
                    currentStruct = currentStruct.GetInheritanceSuper();
                    if (currentStruct != null)
                    {
                        currentField = GetField(currentStruct);
                        interfaceIndex = -1;
                        continue;
                    }
                }

                break;
            }

            unrealStruct = currentStruct;
            field = currentField;

            return field != null;
        }

        public void Reset()
        {
            unrealStruct = targetStruct;
            field = GetField(unrealStruct);
            interfaceIndex = -1;
            first = true;
        }

        private UField GetField(UStruct unrealStruct)
        {
            if (unrealStruct == null)
            {
                return null;
            }

            switch (iteratorType)
            {
                case EFieldIteratorType.Children:
                    return unrealStruct.Children;

                case EFieldIteratorType.Property:
                    return unrealStruct.PropertyLink;

                case EFieldIteratorType.Ref:
                    return unrealStruct.RefLink;

                case EFieldIteratorType.Destructor:
                    return unrealStruct.DestructorLink;

                case EFieldIteratorType.PostConstruct:
                    return unrealStruct.PostConstructLink;                
            }
            return null;
        }

        public IEnumerator GetEnumerator()
        {
            return this;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this;
        }
    }

    public enum EFieldIteratorType
    {
        Children,
        Property,
        Ref,
        Destructor,
        PostConstruct
    }
}
