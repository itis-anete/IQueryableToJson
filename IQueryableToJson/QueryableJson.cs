using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IQueryableToJson
{
	public class QueryableJson : IQueryable<KeyValuePair<string, string>>
	{
		public QueryableJson(string jsonContent)
			: this(JsonConvert.DeserializeObject<JToken>(jsonContent)) { }

		public QueryableJson(JToken jsonObject)
		{
			Expression = Expression.Constant(this);
			Provider = new JsonQueryProvider(jsonObject);
		}

		public QueryableJson(IQueryProvider provider)
		{
			Expression = Expression.Constant(this);
			Provider = provider;
		}

		public QueryableJson(Expression expression, IQueryProvider provider)
		{
			Expression = expression;
			Provider = provider;
		}

		public Type ElementType => typeof(KeyValuePair<string, string>);
		public Expression Expression { get; }
		public IQueryProvider Provider { get; }

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			var executionResult = Provider.Execute<IDictionary<string, string>>(Expression);
			return executionResult.GetEnumerator();
		}
	}
}
