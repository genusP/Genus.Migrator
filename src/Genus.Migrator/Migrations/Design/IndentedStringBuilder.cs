using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genus.Migrator.Migrations.Design
{
    public class IndentedStringBuilder
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private int _indent = 0;
        private const char indentChar = ' ';
        private const int indentSize = 4;

        private string NewLine => Environment.NewLine + new string(indentChar, indentSize * _indent);

        public IndentedStringBuilder Append(object value, bool ignoreIndent=false)
        {
            var strVal = value.ToString();
            if(!ignoreIndent)
                strVal.Replace(Environment.NewLine, this.NewLine);
            _builder.Append(value);
            return this;
        }

        public IndentedStringBuilder AppendNewLine(object value)
        {
            return AppendLine("").Append(value);
        }

        public IndentedStringBuilder AppendLine(object value)
        {
            Append(value);
            _builder.Append(this.NewLine);
            return this;
        }

        public void Clear() => _builder.Clear();

        public IndentedStringBuilder Indent()
        {
            _indent++;
            return this;
        }

        public IndentedStringBuilder DeIndent()
        {
            if (_indent > 0)
                _indent--;
            return this;
        }

        public Indenter Indenter() => new Indenter(this);

        public override string ToString() => _builder.ToString();
    }

    public class Indenter : IDisposable
    {
        private readonly IndentedStringBuilder _builder;
        public Indenter(IndentedStringBuilder builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
            _builder = builder;
            builder.Indent();
        }
        public void Dispose() => _builder.DeIndent();
    }
}
