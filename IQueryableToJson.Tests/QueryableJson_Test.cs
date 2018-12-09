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
		public void SimpleJson()
		{
			const string json = @"
			{
				""Name"": ""Vasya""
			}";

			var result = new QueryableJson(json);

			result.Should().BeEquivalentTo(new Dictionary<string, string>
			{
				["Name"] = "Vasya"
			});
		}

		[Test]
		public void Select_Works()
		{
			const string json = @"
			{
				""Country"": ""Oblivion""
			}";

			var result = new QueryableJson(json)
				.Select(x => QueryableJson.All);

			result.Should().BeEquivalentTo(new Dictionary<string, string>
			{
				["Country"] = "Oblivion"
			});
		}

		[Test]
		public void Select_Works_WithArray()
		{
			const string json = @"
			{
				""IsTrue"": true,
				""Useless"": [ ""JavaScript"", ""Front-end"" ]
			}";

			var result = new QueryableJson(json)
				.Select(x => QueryableJson.AllFromArrays);

			result.Should().BeEquivalentTo(new Dictionary<string, string>
			{
				["Useless[0]"] = "JavaScript",
				["Useless[1]"] = "Front-end"
			});
		}

		[Test]
		public void Select_Works_WithNestedObjectsInArray()
		{
			const string json = @"
			{
				""Game"": ""Minecraft"",
				""Magic"": [
				{
					""Spell"": ""Fireball"",
					""Description"": ""No comments""
				},
				{
					""Spell"": ""Healing"",
					""Description"": "".... you all""
				}]
			}";

			var result = new QueryableJson(json)
				.Select(x => QueryableJson.AllFromArrays);

			result.Should().BeEquivalentTo(new Dictionary<string, string>
			{
				["Magic[0].Spell"] = "Fireball",
				["Magic[0].Description"] = "No comments",
				["Magic[1].Spell"] = "Healing",
				["Magic[1].Description"] = ".... you all",
			});
		}

		[Test]
		public void Where_Works()
		{
			const string json = @"
			{
				""Name"": ""Vasya"",
				""Sex"": ""undefined""
			}";

			var result = new QueryableJson(json)
				.Where(pair => pair.Value == "undefined");

			result.Should().BeEquivalentTo(new Dictionary<string, string>
			{
				["Sex"] = "undefined"
			});
		}

		[Test]
		public void Where_Works_WithTwoCalls()
		{
			const string json = @"
			{
				""Name"": ""Vasya"",
				""Sex"": ""undefined"",
				""FullName"": ""Vasya Pumpkin""
			}";

			var result = new QueryableJson(json)
				.Where(pair => pair.Key.Length > 3)
				.Where(pair => pair.Key.Length < 5);

			result.Should().BeEquivalentTo(new Dictionary<string, string>
			{
				["Name"] = "Vasya"
			});
		}

		[Test]
		public void SelectAndWhere_WorkTogether()
		{
			const string json = @"
			{
				""IsTrue"": true,
				""Useless"": [ ""JavaScript"", ""Front-end"" ]
			}";

			var result = new QueryableJson(json)
				.Select(x => QueryableJson.AllFromArrays)
				.Where(pair => pair.Key.EndsWith("[1]"));

			result.Should().BeEquivalentTo(new Dictionary<string, string>
			{
				["Useless[1]"] = "Front-end"
			});
		}
	}
}
