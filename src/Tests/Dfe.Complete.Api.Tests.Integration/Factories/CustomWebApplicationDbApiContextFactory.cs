using System.Collections.Specialized;
using DfE.CoreLibs.Testing.Mocks.WebApplicationFactory;
using Newtonsoft.Json;
using WireMock.Logging;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Util;
using Xunit.Abstractions;

namespace Dfe.Complete.Api.Tests.Integration.Factories;

public class CustomWebApplicationDbApiContextFactory<TProgram> : CustomWebApplicationDbContextFactory<TProgram> where TProgram : class
{
   private static int _currentPort = 5080;
   private static readonly object Sync = new();

   public WireMockServer MockApiServer { get; set; }

   protected override void ConfigureClient(HttpClient client)
   {
      int port = AllocateNext();
      MockApiServer = WireMockServer.Start(port);
      MockApiServer.LogEntriesChanged += EntriesChanged;
      base.ConfigureClient(client);
   }

   public ITestOutputHelper DebugOutput { get; set; }


   public IReadOnlyList<ILogEntry> GetMockServerLogs(string path, HttpMethod verb = null)
   {
      IRequestBuilder requestBuilder = Request.Create().WithPath(path);
      if (verb is not null) requestBuilder.UsingMethod(verb.Method);
      return MockApiServer.FindLogEntries(requestBuilder);
   }

   private void EntriesChanged(object sender, NotifyCollectionChangedEventArgs e)
   {
      DebugOutput.WriteLine($"API Server change: {JsonConvert.SerializeObject(e)}");
   }

   public void AddGetWithJsonResponse<TResponseBody>(string path, TResponseBody responseBody)
   {
      MockApiServer
         .Given(Request.Create()
            .WithPath(path)
            .UsingGet())
         .RespondWith(Response.Create()
            .WithStatusCode(200)
            .WithHeader("Content-Type", "application/json")
            .WithBody(JsonConvert.SerializeObject(responseBody)));
   }

   public void AddPatchWithJsonRequest<TRequestBody, TResponseBody>(string path, TRequestBody requestBody, TResponseBody responseBody)
   {
      MockApiServer
         .Given(Request.Create()
            .WithPath(path)
            .WithBody(new JsonMatcher(JsonConvert.SerializeObject(requestBody), true))
            .UsingPatch())
         .RespondWith(Response.Create()
            .WithStatusCode(200)
            .WithHeader("Content-Type", "application/json")
            .WithBody(JsonConvert.SerializeObject(responseBody)));
   }

   public void AddApiCallWithBodyDelegate<TResponseBody>(string path, Func<IBodyData, bool> bodyDelegate, TResponseBody responseBody, HttpMethod verb = null)
   {
      MockApiServer
         .Given(Request.Create()
            .WithPath(path)
            .WithBody(bodyDelegate)
            .UsingMethod(verb == null ? HttpMethod.Post.ToString() : verb.ToString()))
         .RespondWith(Response.Create()
            .WithStatusCode(200)
            .WithHeader("Content-Type", "application/json")
            .WithBody(JsonConvert.SerializeObject(responseBody)));
   }

   public void AddPutWithJsonRequest<TRequestBody, TResponseBody>(string path, TRequestBody requestBody, TResponseBody responseBody)
   {
      MockApiServer
         .Given(Request.Create()
            .WithPath(path)
            .WithBody(new JsonMatcher(JsonConvert.SerializeObject(requestBody), true))
            .UsingPut())
         .RespondWith(Response.Create()
            .WithStatusCode(200)
            .WithHeader("Content-Type", "application/json")
            .WithBody(JsonConvert.SerializeObject(responseBody)));
   }

   public void AddPostWithJsonRequest<TRequestBody, TResponseBody>(string path, TRequestBody requestBody, TResponseBody responseBody)
   {
      MockApiServer
         .Given(Request.Create()
            .WithPath(path)
            .WithBody(new JsonMatcher(JsonConvert.SerializeObject(requestBody), true))
            .UsingPost())
         .RespondWith(Response.Create()
            .WithStatusCode(200)
            .WithHeader("Content-Type", "application/json")
            .WithBody(JsonConvert.SerializeObject(responseBody)));
   }

   public void AddAnyPostWithJsonRequest<TResponseBody>(string path, TResponseBody responseBody)
   {
      MockApiServer
         .Given(Request.Create()
            .WithPath(path)
            .UsingPost())
         .RespondWith(Response.Create()
            .WithStatusCode(200)
            .WithHeader("Content-Type", "application/json")
            .WithBody(JsonConvert.SerializeObject(responseBody)));
   }

   public void AddErrorResponse(string path, string method)
   {
      MockApiServer
         .Given(Request.Create()
            .WithPath(path)
            .UsingMethod(method))
         .RespondWith(Response.Create()
            .WithStatusCode(500));
   }

   public void Reset()
   {
      MockApiServer.Reset();
   }

   private static int AllocateNext()
   {
      lock (Sync)
      {
         int next = _currentPort;
         _currentPort++;
         return next;
      }
   }

   public new void Dispose()
   {
      MockApiServer.Stop();
      base.Dispose();
   }
}