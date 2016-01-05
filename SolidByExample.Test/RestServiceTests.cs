using System;
using NUnit.Framework;

namespace SolidByExample.Test
{
	[TestFixture]
	public class RestServiceTests
	{
		private IRestService _SUT;

		[SetUp]
		public void BeforeEachTest()
		{
			_SUT = new RestService();
        }

		[Test]
		public void TestMethod1()
		{
			var post = _SUT.GetPost(1);
			Assert.NotNull(post);
			Assert.That(post.title.Length > 0);
		}
	}
}
