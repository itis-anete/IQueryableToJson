using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IQueryableToJson
{
	public class JsonExpressionExecutor : ExpressionVisitor
	{
		public Dictionary<string, string> Execute(Expression jsonExpression)
		{
			var executionResult = (ConstantExpression)Visit(jsonExpression);
			return (Dictionary<string, string>)executionResult.Value;
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			if (!(node.Value is QueryableJson json))
				throw new NotSupportedException($"Value of type {typeof(QueryableJson)} expected");
			
			return Expression.Constant(((JsonQueryProvider)json.Provider).JsonSource);
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			switch (node.Method.Name)
			{
				case "Where":
					return HandleWhereMethod(node);
				default:
					throw new NotSupportedException($"Method \"{node.Method.Name}\" is not supported");
			}
		}

		private Expression HandleWhereMethod(MethodCallExpression node)
		{
			var argument = (ConstantExpression)Visit(node.Arguments[0]);
			var values = (Dictionary<string, string>)argument.Value;

			var lambda = GetLambda(node.Arguments[1]);
			var predicate = lambda.Compile();

			var filteredValues = values
				.Where(predicate)
				.ToDictionary(pair => pair.Key, pair => pair.Value);
			return Expression.Constant(filteredValues);
		}

		private static Expression<Func<KeyValuePair<string, string>, bool>> GetLambda(Expression expression)
		{
			var lambdaExpression = expression.NodeType == ExpressionType.Quote
				? ((UnaryExpression)expression).Operand
				: expression;

			return (Expression<Func<KeyValuePair<string, string>, bool>>)lambdaExpression;
		}
	}
}
