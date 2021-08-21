using BookStore_UI.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_UI.Service
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IHttpClientFactory _client;

        public BaseRepository(IHttpClientFactory client)
        {
            _client = client;
        }

        //Generic methods so it can be used for Authors, books etc
        public async Task<bool> Create(string url, T obj)
        {
            //request - - user causes some event on the UI
            //create request is a post
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            if (obj == null)
                return false; //failed attempt

            //build content for the request
            //obj represents the data to post
            request.Content = new StringContent(JsonConvert.SerializeObject(obj));
            var client = _client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> Delete(string url, int id)
        {
            if (id < 1)
                return false;

            //request - - user causes some event on the UI
            //create request is a post
            var request = new HttpRequestMessage(HttpMethod.Delete, url + id); //concatonate url and id for endpoint

            var client = _client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent) //NoContent is a success for a delete
            {
                return true;
            }

            return false;
        }

        public async Task<T> Get(string url, int id)
        {
            //request - - user causes some event on the UI
            //create request is a post
            var request = new HttpRequestMessage(HttpMethod.Get, url + id);

            //build content for the request
            //obj represents the data to post
            var client = _client.CreateClient();
            //Send request get response
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //Get - so want content back
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(content);
            }
            return null;
        }

        public async Task<IList<T>> Get(string url)
        {
            //request - - user causes some event on the UI
            //create request is a post
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            //build content for the request
            //obj represents the data to post
            var client = _client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IList<T>>(content);
            }
            return null;
        }

        public async Task<bool> Update(string url, T obj)
        {
            //put and patch handle updates
            //put - replaces existing information - will create record if necessary
            //patch - only replaces what's necessary

            //request - - user causes some event on the UI
            //create request is a post
            var request = new HttpRequestMessage(HttpMethod.Put, url);
            if (obj == null)
                return false; //failed attempt

            //build content for the request
            //obj represents the data to post
            request.Content = new StringContent(content: JsonConvert.SerializeObject(obj),
                                                encoding: Encoding.UTF8,
                                                mediaType: "application/json");
            var client = _client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }
            return false;
        }
    }
}
