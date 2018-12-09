using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IQueryableToJson.Tests
{
	[TestFixture]
	public class QueryableJson_Test
	{
		[Test]
		public void Works_WithSimpleJson()
		{
			const string json = @"{""Name"": ""Vasya""}";

			var result = new QueryableJson(json);

			result.Should().BeEquivalentTo(new Dictionary<string, string>
			{
				["Name"] = "Vasya"
			});
		}

		[Test]
		public void Works_WithWhere()
		{
			const string json = @"{""Name"": ""Vasya"", ""Sex"": ""undefined""}";

			var result = new QueryableJson(json)
				.Where(pair => pair.Value == "undefined");

			result.Should().BeEquivalentTo(new Dictionary<string, string>
			{
				["Sex"] = "undefined"
			});
		}

		[Test]
		public void Works_WithTwoWhere()
		{
			const string json = @"{""Name"": ""Vasya"", ""Sex"": ""undefined"", ""FullName"": ""Vasya Pumpkin""}";

			var result = new QueryableJson(json)
				.Where(pair => pair.Key.Length > 3)
				.Where(pair => pair.Key.Length < 5);

			result.Should().BeEquivalentTo(new Dictionary<string, string>
			{
				["Name"] = "Vasya"
			});
		}
	}
}
