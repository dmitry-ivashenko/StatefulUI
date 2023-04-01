namespace StatefulUI.Editor.Core
{
    public class TypeGenerator
    {
        private string _result;
        private bool _hasNameSpace;

        public TypeGenerator()
        {
            _result += "// ReSharper disable CheckNamespace\n" +
                       "// ReSharper disable UnusedMember.Global\n";
        }
        
        public TypeGenerator AddNameSpace(string value)
        {
            _result += $"namespace {value}\n{{\n";
            _hasNameSpace = true;
            return this;
        }

        public TypeGenerator AddUsing(string value)
        {
            _result += $"using {value};\n";
            return this;
        }
        
        public TypeGenerator AddAttribute(string attributeName)
        {
            _result += $"[{attributeName}]\n";
            return this;
        }
        
        public TypeGenerator SetEnum(string className)
        {
            _result += $"public enum {className}\n{{\n";
            return this;
        }

        public TypeGenerator SetStaticClass(string className)
        {
            _result += $"{GetIndent(IndentType.Class)}public static class {className}\n" +
                       $"{GetIndent(IndentType.Class)}{{\n";
            return this;
        }

        public TypeGenerator SetClass(string className, bool abstractClass = false)
        {
            _result += $"{GetIndent(IndentType.Class)}public {(abstractClass ? "abstract " : "")}class {className}\n" +
                       $"{GetIndent(IndentType.Class)}{{\n";
            return this;
        }

        public TypeGenerator AddEnumField(string name, object value)
        {
            if (!_result.Contains($"    {name} = "))
            {
                _result += $"    {name} = {value},\n";    
            }
            return this;
        }

        public TypeGenerator AddBottom()
        {
            _result += GetIndent(IndentType.Class);
            AddEndBracket();
            if (_hasNameSpace) AddEndBracket();
            return this;
        }

        public TypeGenerator AddStatement(string statement)
        {
            statement = statement.Replace("=>", $"\n{GetIndent(IndentType.Statement)}    =>");
            _result += $"{GetIndent(IndentType.Statement)}{statement}\n\n";
            return this;
        }

        private void AddEndBracket()
        {
            _result += "}\n";
        }

        public override string ToString()
        {
            return _result;
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