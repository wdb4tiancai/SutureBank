using System;

namespace UIFramework.Editor
{
    public delegate SupportedTypeData DefineSupportedTypeDelegate();

    [AttributeUsage(AttributeTargets.Method)]
    public class SupportedComponentTypeAttribute : Attribute { }

    public class SupportedTypeData
    {
        public readonly Type Type;
        public readonly int Priority;
        public readonly string ShowName;
        public readonly string NameSpace;
        public readonly string CodeTypeName;
        public readonly string VariableName;
        public SupportedTypeData(Type type, int priority, string showName, string nameSpace, string codeTypeName, string variableName)
        {
            this.Type = type;
            this.Priority = priority;
            this.ShowName = showName;
            this.NameSpace = nameSpace;
            this.CodeTypeName = codeTypeName;
            this.VariableName = variableName;
        }
    }

}