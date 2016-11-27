﻿namespace App.Common.Connector
{
    using System;
    using App.Common.Http;
    using System.Net.Http;
    using App.Common.Configurations;
    using System.Net.Http.Headers;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;
    using Logging;
    using DI;

    public class RESTConnector : IConnector
    {
        public IResponseData<TRespone> Put<TRequest, TRespone>(string uri, TRequest data)
        {
            using (HttpClient client = this.CreateHttpClient(Configuration.Current.IntegrationTest.BaseUrl))
            {
                HttpContent content = new JsonContent<TRequest>(data);
                HttpResponseMessage responseMessage = client.PutAsync(uri, content).Result;
                IResponseData<TRespone> result = this.GetResponseAs<ResponseData<TRespone>>(responseMessage.Content);
                return result;
            }
        }

        public IResponseData<TResponse> Post<TRequest, TResponse>(string uri, TRequest data)
        {
            using (HttpClient client = this.CreateHttpClient(Configuration.Current.IntegrationTest.BaseUrl))
            {
                HttpContent content = new JsonContent<TRequest>(data);
                HttpResponseMessage responseMessage = client.PostAsync(uri, content).Result;
                IResponseData<TResponse> result = this.GetResponseAs<ResponseData<TResponse>>(responseMessage.Content);
                return result;
            }
        }

        public IResponseData<TResponse> Delete<TResponse>(string uri)
        {
            using (HttpClient client = this.CreateHttpClient(Configuration.Current.IntegrationTest.BaseUrl))
            {
                HttpResponseMessage responseMessage = client.DeleteAsync(uri).Result;
                IResponseData<TResponse> result = this.GetResponseAs<ResponseData<TResponse>>(responseMessage.Content);
                return result;
            }
        }

        public IResponseData<TResponse> Get<TResponse>(string uri)
        {
            using (HttpClient client = this.CreateHttpClient(Configuration.Current.IntegrationTest.BaseUrl))
            {
                HttpResponseMessage responseMessage = client.GetAsync(uri).Result;
                IResponseData<TResponse> result = this.GetResponseAs<ResponseData<TResponse>>(responseMessage.Content);
                return result;
            }
        }

        private TResponse GetResponseAs<TResponse>(HttpContent content)
        {
            try
            {
                Stream receiveStream = content.ReadAsStreamAsync().Result;
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                string contentStr = readStream.ReadToEnd();
                return JsonConvert.DeserializeObject<TResponse>(contentStr);
            }
            catch (Exception ex) {
                ILogger logger = IoC.Container.Resolve<ILogger>();
                logger.Error(ex);
                throw ex;
            }

            //TResponse result = content.ReadAsAsync<TResponse>().Result;
            //return result;
        }

        private HttpClient CreateHttpClient(string baseUrl)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
    }
}
