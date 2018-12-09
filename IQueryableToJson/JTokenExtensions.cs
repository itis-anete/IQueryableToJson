using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace IQueryableToJson
{
	public static class JTokenExtensions
	{
		public static bool IsInArray(this JToken token)
		{
			if (token == null)
				return false;

			while (token.Parent != null)
			{
				if (token.Parent.Type == JTokenType.Array)
					return true;
				token = token.Parent;
			}
			return false;
		}

		public static List<JToken> GetAllTokens(this JToken jsonObject)
		{
			if (jsonObject == null)
				throw new ArgumentNullException(nameof(jsonObject));

			var allTokens = new List<JToken>();
			var nextTokens = new Stack<JToken>();
			nextTokens.Push(jsonObject);

			while (nextTokens.Count > 0)
			{
				var currentToken = nextTokens.Pop();
				allTokens.Add(currentToken);

				foreach (var child in currentToken)
					nextTokens.Push(child);
			}

			return allTokens;
		}

		public static KeyValuePair<string, string> ToKeyValuePair(this JToken token)
		{
			return new KeyValuePair<string, string>(token.Path, token.ToString());
		}
	}
}
