using System.Text;

namespace StatefulUI.Editor.Core
{
    public class TypeGenerator
    {
        private StringBuilder _result = new StringBuilder();
        private bool _hasNameSpace;

        public TypeGenerator()
        {
            _result.AppendLine("// ReSharper disable CheckNamespace");
            _result.AppendLine("// ReSharper disable UnusedMember.Global");
        }

        public TypeGenerator AddNameSpace(string value)
        {
            _result.AppendLine($"namespace {value}");
            _result.AppendLine("{");
            _hasNameSpace = true;
            return this;
        }

        public TypeGenerator AddUsing(string value)
        {
            _result.AppendLine($"using {value};");
            return this;
        }

        public TypeGenerator AddAttribute(string attributeName)
        {
            _result.AppendLine($"[{attributeName}]");
            return this;
        }

        public TypeGenerator SetEnum(string className)
        {
            _result.AppendLine($"public enum {className}");
            _result.AppendLine("{");
            return this;
        }

        public TypeGenerator SetStaticClass(string className)
        {
            _result.AppendLine($"{GetIndent(IndentType.Class)}public static class {className}");
            _result.AppendLine($"{GetIndent(IndentType.Class)}{{");
            return this;
        }

        public TypeGenerator SetClass(string className, bool abstractClass = false)
        {
            _result.AppendLine($"{GetIndent(IndentType.Class)}public {(abstractClass ? "abstract " : "")}class {className}");
            _result.AppendLine($"{GetIndent(IndentType.Class)}{{");
            return this;
        }

        public TypeGenerator AddEnumField(string name, object value)
        {
            if (!_result.ToString().Contains($"    {name} = "))
            {
                _result.AppendLine($"    {name} = {value},");
            }
            return this;
        }

        public TypeGenerator AddBottom()
        {
            _result.AppendLine(GetIndent(IndentType.Class));
            AddEndBracket();
            if (_hasNameSpace) AddEndBracket();
            return this;
        }

        public TypeGenerator AddStatement(string statement)
        {
            statement = statement.Replace("=>", $"\n{GetIndent(IndentType.Statement)}    =>");
            _result.AppendLine($"{GetIndent(IndentType.Statement)}{statement}");
            _result.AppendLine();
            return this;
        }

        private void AddEndBracket()
        {
            _result.AppendLine("}");
        }

        public override string ToString()
        {
            return _result.ToString();
        }

        public string GetIndent(IndentType type)
        {
            switch (type)
            {
                case IndentType.Statement: return _hasNameSpace ? "        " : "    ";
                case IndentType.Class: return _hasNameSpace ? "    " : "";
                default: return "";
            }
        }
    }

    public enum IndentType
    {
        Statement,
        Class,
    }
}