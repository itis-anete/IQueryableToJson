using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace IQueryableToJson
{
	public class JsonQueryExecutor : ExpressionVisitor
	{
		public Dictionary<string, string> Execute(Expression jsonQuery)
		{
			var visitationResult = (ConstantExpression)Visit(jsonQuery);
			var returnedTokens = (IEnumerable<JToken>)visitationResult.Value;
			return returnedTokens
				.Select(token => token.ToKeyValuePair())
				.ToDictionary(pair => pair.Key, pair => pair.Value);
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			if (!(node.Value is QueryableJson json))
				throw new NotSupportedException($"Value of type {typeof(QueryableJson)} expected");

			var source = ((JsonQueryProvider)json.Provider).JsonSource;
			var tokens = source.GetAllTokens()
				.Where(token => !token.HasValues);
			return Expression.Constant(tokens);
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			switch (node.Method.Name)
			{
				case "Select":
					return HandleSelectMethod(node);
				case "Where":
					return HandleWhereMethod(node);
				default:
					throw new NotSupportedException($"Method \"{node.Method.Name}\" is not supported");
			}
		}

		private Expression HandleSelectMethod(MethodCallExpression node)
		{
			var argument = (ConstantExpression)Visit(node.Arguments[0]);
			var tokens = (IEnumerable<JToken>)argument.Value;

			var lambda = GetLambda(node.Arguments[1]);
			var selectOption = ((PropertyInfo)((MemberExpression)lambda.Body).Member).Name;
			switch (selectOption)
			{
				case "All":
					break;
				case "AllFromArrays":
					tokens = tokens.Where(token => token.IsInArray());
					break;
				default:
					throw new NotSupportedException("Unknown option in Select method");
			}

			return Expression.Constant(tokens);
		}

		private Expression HandleWhereMethod(MethodCallExpression node)
		{
			var argument = (ConstantExpression)Visit(node.Arguments[0]);
			var tokens = (IEnumerable<JToken>)argument.Value;

			var lambda = (Expression<Func<KeyValuePair<string, string>, bool>>)
				GetLambda(node.Arguments[1]);
			var predicate = lambda.Compile();

			tokens = tokens
				.Where(token => predicate(token.ToKeyValuePair()));
			return Expression.Constant(tokens);
		}

		private static LambdaExpression GetLambda(Expression expression)
		{
			var lambdaExpression = expression.NodeType == ExpressionType.Quote
				? ((UnaryExpression)expression).Operand
				: expression;

			return (LambdaExpression)lambdaExpression;
		}
	}
}
