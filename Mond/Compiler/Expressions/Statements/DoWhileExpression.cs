﻿using Mond.Compiler.Visitors;

namespace Mond.Compiler.Expressions.Statements
{
    class DoWhileExpression : Expression, IStatementExpression
    {
        public BlockExpression Block { get; private set; }
        public Expression Condition { get; private set; }

        public DoWhileExpression(Token token, BlockExpression block, Expression condition)
            : base(token.FileName, token.Line)
        {
            Block = block;
            Condition = condition;
        }

        public override int Compile(FunctionContext context)
        {
            context.Line(FileName, Line);

            var stack = 0;
            var start = context.MakeLabel("doWhileStart");
            var cont = context.MakeLabel("doWhileContinue");
            var brk = context.MakeLabel("doWhileBreak");
            var end = context.MakeLabel("doWhileEnd");

            var containsFunction = new LoopContainsFunctionVisitor();
            Block.Accept(containsFunction);

            var loopContext = containsFunction.Value ? new LoopContext(context) : context;

            // body
            loopContext.PushLoop(cont, containsFunction.Value ? brk : end);

            stack += loopContext.Bind(start);

            if (containsFunction.Value)
                stack += loopContext.Enter();

            stack += Block.Compile(loopContext);
            loopContext.PopLoop();

            // condition check
            stack += context.Bind(cont); // continue

            if (containsFunction.Value)
                stack += context.Leave();

            stack += Condition.Compile(context);
            stack += context.JumpTrue(start);

            if (containsFunction.Value)
            {
                stack += context.Jump(end);

                stack += context.Bind(brk); // break (with function)
                stack += context.Leave();
            }

            stack += context.Bind(end); // break (without function)

            CheckStack(stack, 0);
            return stack;
        }

        public override Expression Simplify()
        {
            Block = (BlockExpression)Block.Simplify();
            Condition = Condition.Simplify();

            return this;
        }

        public override void SetParent(Expression parent)
        {
            base.SetParent(parent);

            Block.SetParent(this);
            Condition.SetParent(this);
        }

        public override T Accept<T>(IExpressionVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
