using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IQueryableToJson
{
	public class JsonQueryProvider : IQueryProvider
	{
		public JToken JsonSource { get; }

		public JsonQueryProvider(JToken jsonSource)
		{
			JsonSource = jsonSource ?? throw new ArgumentNullException(nameof(jsonSource));
		}

		public IQueryable CreateQuery(Expression expression)
		{
			return CreateQuery<KeyValuePair<string, string>>(expression);
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			if (typeof(TElement) != typeof(KeyValuePair<string, string>))
				throw new NotSupportedException("Element type must be " +
				                                typeof(KeyValuePair<string, string>).ToString());

			return (IQueryable<TElement>)new QueryableJson(expression, this);
		}

		public object Execute(Expression expression)
		{
			return ExpressionExecutor.Execute(expression);
		}

		public TResult Execute<TResult>(Expression expression)
		{
			if (!typeof(TResult).IsAssignableFrom(typeof(IDictionary<string, string>)))
				throw new NotSupportedException("Execution result must be of type " +
				                                typeof(IDictionary<string, string>).ToString());

			return (TResult)Execute(expression);
		}

		private static readonly JsonQueryExecutor ExpressionExecutor = new JsonQueryExecutor();
	}
}
