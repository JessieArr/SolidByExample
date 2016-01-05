using System;
using System.Web.Script.Serialization;
using Moq;
using NUnit.Framework;
using RestSharp;
using SolidByExample.Logging;

namespace SolidByExample.Test
{
	[TestFixture]
	public class RestServiceTests
	{
		private Mock<IRestClient> _mockRestClient;
		private Mock<ILogHelper> _mockLogHelper;
		private JavaScriptSerializer _javaScriptSerializer;
		private IRestService _SUT;

		[SetUp]
		public void BeforeEachTest()
		{
			_javaScriptSerializer = new JavaScriptSerializer();

			_mockRestClient = new Mock<IRestClient>();
			var expectedPost = new []
			{
				new RestPost() {
					id = 1,
					userId = 1,
					title = "Test title!",
					body = "Test body!"
				}
			};
			var postJSON = _javaScriptSerializer.Serialize(expectedPost);
			var mockResponse = new RestResponse()
			{
				Content = postJSON
			};
			new Mock<IRestResponse>().SetupGet(x => x.Content)
				.Returns(postJSON);

			_mockRestClient.Setup(x => x.Execute(It.IsAny<IRestRequest>()))
				.Returns(mockResponse);

			_mockLogHelper = new Mock<ILogHelper>();

			// Now that we have inverted the RestService's dependency on the RestService
			// and LogHelper we gain three big benefits: 
			// 1- we no longer make network requests and write to disk while executing our
			// tests. Tests that fail when the internet is out aren't very useful!
			// 2- We are no longer limited to assertions about our methods' return values
			// and can verify the way our RestService interacts with other objects.
			// 3- We can simulate failure scenarios such as exceptions and be sure that
			// our class handles them gracefully. 
			_SUT = new RestService(_mockRestClient.Object, _mockLogHelper.Object);
		}

		[Test]
		public void GetPost_Returns_ExpectedData()
		{
			var post = _SUT.GetPost(1);
			Assert.NotNull(post);
			Assert.That(post.title.Length > 0);

			// Since we now take advantage of mocks, we can assert things about the way
			// Our RestService interacts with other objects like the RestService and
			// LogHelper. This would have been very difficult to test before applying
			// the Dependency Inversion Principle.

			// We verify that a RestRequest is executed once.
			_mockRestClient.Verify(x => x.Execute(It.IsAny<IRestRequest>()), Times.Once());

			// We verify that LogHelper.LogInfo is called twice.
			_mockLogHelper.Verify(x => x.LogInfo(It.IsAny<string>()), Times.Exactly(2));
		}

		[Test]
		public void GetPost_ExecutesGetRequest()
		{
			var post = _SUT.GetPost(1);

			// Since we now take advantage of mocks, we can assert things about the way
			// Our RestService interacts with other objects like the RestService and
			// LogHelper. This would have been very difficult to test before applying
			// the Dependency Inversion Principle.

			// We verify that a RestRequest is executed once.
			_mockRestClient.Verify(x => x.Execute(It.Is<IRestRequest>(y => y.Method == Method.GET)), Times.Once());
		}

		[Test]
		public void GetPost_LogsInfoMessages_TwoTimes()
		{
			var post = _SUT.GetPost(1);

			// We verify that LogHelper.LogInfo is called twice.
			_mockLogHelper.Verify(x => x.LogInfo(It.IsAny<string>()), Times.Exactly(2));
		}

		[Test]
		public void GetPost_RestClientException_IsRethrown()
		{
			var expectedException = new Exception("TEST EXCEPTION!");
			_mockRestClient.Setup(x => x.Execute(It.IsAny<IRestRequest>()))
				.Throws(expectedException);
			Exception caughtException = null;
			try
			{
				var post = _SUT.GetPost(1);
			}
			catch (Exception ex)
			{
				caughtException = ex;
			}

			// We make sure that exceptions are properly rethrown.
			Assert.That(caughtException == expectedException);
		}

		[Test]
		public void GetPost_RestClientException_IsLogged()
		{
			var expectedException = new Exception("TEST EXCEPTION!");
			_mockRestClient.Setup(x => x.Execute(It.IsAny<IRestRequest>()))
				.Throws(expectedException);
			try
			{
				var post = _SUT.GetPost(1);
			}
			catch (Exception)
			{
			}

			// We make sure that exceptions are properly rethrown.
			_mockLogHelper.Verify(x => x.LogError(It.IsAny<string>()), Times.Once);
		}
	}
}
