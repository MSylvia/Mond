﻿namespace Mond.Compiler.Expressions
{
    class BoolExpression : Expression, IConstantExpression
    {
        public bool Value { get; private set; }

        public BoolExpression(Token token, bool value)
            : base(token.FileName, token.Line)
        {
            Value = value;
        }

        public override int Compile(FunctionContext context)
        {
            context.Line(FileName, Line);

            if (Value)
                context.LoadTrue();
            else
                context.LoadFalse();

            return 1;
        }

        public override Expression Simplify()
        {
            return this;
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public MondValue GetValue()
        {
            return Value;
        }
    }
}
