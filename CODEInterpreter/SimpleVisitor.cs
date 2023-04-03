using Antlr4.Runtime.Misc;
using CODEInterpreter.Content;

public class SimpleVisitor: SimpleBaseVisitor<object?>
{
        Dictionary<string, object?> Variable { get; } = new();

        public SimpleVisitor()
        {
            Variable["Write"] = new Func<object?[], object?>(Write);
        }

        private object? Write(object?[] args)
        {
            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }
            return null;
        }

        public override object? VisitFunctionCall([NotNull] SimpleParser.FunctionCallContext context)
        {
            var name = context.IDENTIFIER().GetText();
            var args = context.expression().Select(Visit).ToArray();

            if (!Variable.ContainsKey(name))
            {
                throw new Exception($"Function {name} is not defined.");
            }

            if (Variable[name] is not Func<object?[], object?> func)
            {
                throw new Exception($"Variable {name} is not a function.");
            }

            return func(args);

        }

        public override object? VisitAssignment(SimpleParser.AssignmentContext context)
        {
            var varName = context.IDENTIFIER().GetText();

            var value = Visit(context.expression());

            Variable[varName] = value;

            return null;
        }

        public override object? VisitIdentifierExpression(SimpleParser.IdentifierExpressionContext context)
        {
            var varName = context.IDENTIFIER().GetText();

            if (!Variable.ContainsKey(varName))
            {
                throw new Exception($"Variable {varName} is not defined");
            }

            return Variable[varName];
        }

        public override object? VisitConstant(SimpleParser.ConstantContext context)
        {
            if (context.INTEGER() is { } i)
                return int.Parse(i.GetText());

            if (context.FLOAT() is { } f)
                return float.Parse(f.GetText());

            if (context.STRING() is { } s)
                return s.GetText()[1..^1];

            if (context.BOOL() is { } b)
                return b.GetText() == "true";

            if (context.NULL() is { })
                return null;

            throw new NotImplementedException();
        }

        public override object? VisitAdditiveExpression(SimpleParser.AdditiveExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var op = context.addOp().GetText();

            return op switch
            {
                "+" => Add(left, right),
                "-" => Subtract(left, right),
                _ => throw new NotImplementedException(),
            }; ;
        }

        public override object? VisitWhileBlock(SimpleParser.WhileBlockContext context)
        {
            Func<object?, bool> condition = context.WHILE().GetText() == "while"
                ? IsTrue
                : IsFalse
            ;

            if ((condition(Visit(context.expression()))))
            {
                do
                {
                    Visit(context.block());
                } while ((condition(Visit(context.expression()))));
            }
            else
            {
                Visit(context.elseIfBlock());
            }

            return null;
        }

        public override object VisitComparisonExpression(SimpleParser.ComparisonExpressionContext context)
        {
            var left = Visit(context.expression(0));
            var right = Visit(context.expression(1));

            var op = context.compareOp().GetText();

            return op switch
            {
                //"==" => IsEquals(left, right),
                //"!=" => NotEquals(left, right),
                "<" => LessThan(left, right),
                //">" => GreaterThan(left, right),
                //">=" => GreaterThanOrEqual(left, right),
                //"<=" => LessThanOrEqual(left, right),
                _ => throw new NotImplementedException()
            };
        }

        private object? Add(object? left, object? right)
        {
            if (left is int i && right is int j)
                return i + j;

            if (left is float f && right is float g)
                return f + g;

            if (left is int lInt && right is float rFloat)
                return lInt + rFloat;

            if (left is float lFloat && right is int rInt)
                return lFloat + rInt;

            //concatenate
            if (left is string || right is string)
                return $"{left}{right}";

            throw new Exception($"Cannot add values of types {left?.GetType()} and {right?.GetType()}");
        }

        private object? Subtract(object? left, object? right)
        {
            if (left is int i && right is int j)
                return i - j;

            if (left is float f && right is float g)
                return f - g;

            if (left is int lInt && right is float rFloat)
                return lInt - rFloat;

            if (left is float lFloat && right is int rInt)
                return lFloat - rInt;

            throw new Exception($"Cannot subtract values of types {left?.GetType()} and {right?.GetType()}");
        }

        private bool LessThan(object? left, object? right)
        {
            if (left is int i && right is int j)
                return i < j;

            if (left is float f && right is float g)
                return f < g;

            if (left is int lInt && right is float rFloat)
                return lInt < rFloat;

            if (left is float lFloat && right is int rInt)
                return lFloat < rInt;

            throw new Exception($"Cannot compare values of types {left?.GetType()} and {right?.GetType()}");
        }

        private bool IsTrue(object? value)
        {
            if (value is bool b)
            {
                return b;
            }

            throw new Exception("Value is not boolean");
        }

        private bool IsFalse(object? value) => !IsTrue(value);
    
}