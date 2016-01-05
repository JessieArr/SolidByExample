using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Web.Script.Serialization;
using SolidByExample.Logging;

namespace SolidByExample
{
	public class RestService : IRestService
	{
		private Dictionary<int, RestPost> _PostCache = new Dictionary<int, RestPost>();
		private Dictionary<int, RestUser> _UserCache = new Dictionary<int, RestUser>();
		private const string _ApiUrl = "http://jsonplaceholder.typicode.com";
		private const string _PostRoute = "posts";
		private const string _UserRoute = "users";

		private IRestClient _restClient;
		private ILogHelper _logHelper;

		public RestService()
		{
			// Opportunity for dependency inversion: consumer of our Service should
			// be able to specify which instance of the Rest Client and Log we use.
			_restClient = new RestClient(_ApiUrl);
			var logHelper = new NLogLogHelper("logName");
			_logHelper = logHelper;
		}

		public RestPost GetPost(int postNumber)
		{
			RestPost returnValue;
			// Responsibility #1: Logging
			_logHelper.LogInfo("GetPost called with argument: " + postNumber);

			// Responsibility #2: Caching
			if (_PostCache.ContainsKey(postNumber))
			{
				returnValue = _PostCache[postNumber];
			}
			else
			{
				try
				{
					// Responsibility #3: Network Requests
					var request = new RestRequest(_PostRoute, Method.GET);
					request.AddParameter("id", postNumber);

					var response = _restClient.Execute(request);
					var parser = new JavaScriptSerializer();
					var post = parser.Deserialize<RestPost[]>(response.Content);
					returnValue = post[0];

					// Responsibility #2: Caching
					_PostCache.Add(postNumber, returnValue);
				}
				catch (Exception ex)
				{
					// Responsibility #1: Logging
					_logHelper.LogError("GetPost failed to fetch post " + postNumber + ": " + ex.Message);
					throw ex;
				}
			}

			// Responsibility #1: Logging
			_logHelper.LogInfo("GetPost returned Post with title: " + returnValue.title);

			return returnValue;
		}

		public RestUser GetUser(int userNumber)
		{
			RestUser returnValue;
			// Responsibility #1: Logging
			_logHelper.LogInfo("GetUser called with argument: " + userNumber);

			// Responsibility #2: Caching
			if (_UserCache.ContainsKey(userNumber))
			{
				returnValue = _UserCache[userNumber];
			}
			else
			{
				try
				{
					// Responsibility #3: Network Requests
					var request = new RestRequest(_UserRoute, Method.GET);
					request.AddParameter("id", userNumber);

					var response = _restClient.Execute(request);
					var parser = new JavaScriptSerializer();
					var post = parser.Deserialize<RestUser[]>(response.Content);
					returnValue = post[0];

					// Responsibility #2: Caching
					_UserCache.Add(userNumber, returnValue);
                }
				catch (Exception ex)
				{
					// Responsibility #1: Logging
					_logHelper.LogError("GetUser failed to fetch user " + userNumber + ": " + ex.Message);
					throw ex;
				}
			}

			// Responsibility #1: Logging
			_logHelper.LogInfo("GetUser returned User with name: " + returnValue.name);

			return returnValue;
		}
	}

	public interface IRestService
	{
		// Opportunity for interface segregation as the interface grows larger:
		// We could create two service interfaces grouped by return object type.
		RestPost GetPost(int postNumber);
		RestUser GetUser(int userNumber);
	}
}
