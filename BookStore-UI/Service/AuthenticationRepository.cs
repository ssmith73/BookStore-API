using Blazored.LocalStorage;  
using BookStore_UI.Contracts;  
using BookStore_UI.Models;  
using BookStore_UI.Providers;  
using BookStore_UI.Static;  
using Microsoft.AspNetCore.Components.Authorization;  
using Newtonsoft.Json;  
using System;  
using System.Collections.Generic;  
using System.Linq;  
using System.Net.Http;  
using System.Net.Http.Headers;  
using System.Text;  
using System.Threading.Tasks;  
  
namespace BookStore_UI.Service  
{  
    public class AuthenticationRepository : IAuthenticationRepository  
    {  
        private readonly IHttpClientFactory _client;  
        private readonly ILocalStorageService _localStorage;  
        private readonly Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider _authenticationStateProvider;  
  
        public AuthenticationRepository(IHttpClientFactory client,  
                                        ILocalStorageService localStorageService,  
                                        AuthenticationStateProvider authenticationStateProvider)  
        {  
            _client = client;  
            _localStorage = localStorageService;  
            _authenticationStateProvider = authenticationStateProvider;  
        }  
  
        /// <summary>  
        /// Forms will feed data in here for login  
        /// </summary>  
        /// <param name="user"></param>  
        /// <returns></returns>  
        public async Task<bool> Login(LoginModel user) {  
            //like the create from the baserepository  
            //create request  
            var request = new HttpRequestMessage(HttpMethod.Post,  
                Endpoints.LoginEndpoint);  
  
            //Fill it with content  
            request.Content = new StringContent(JsonConvert.SerializeObject(user),  
                Encoding.UTF8, "application/json");  
  
            var client = _client.CreateClient();  
            HttpResponseMessage response = await client.SendAsync(request);  
  
            try  
            {  
                //Get back a token for a login, need to store it for later use in app  
                if(response.IsSuccessStatusCode == false)  
                    return false; //kill the operation  
                //else get data from response and deserialize  
                //response will have both response code and token string  
  
                var content = await response.Content.ReadAsStringAsync();  
                var token = JsonConvert.DeserializeObject<TokenResponse>(content);  
  
                //need to store the token somewhere  
                //use library to use (local storage) store token (use localstorage for this)  
                //note stored in browser - some security concerns  with this approach  
                await _localStorage.SetItemAsync("authToken", token.Token);  
  
  
                //override default authentication for (built in provider - blazor)  
                //change authentication state of app (override default)  
                await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedIn();  
  
                client.DefaultRequestHeaders.Authorization =  
                    new AuthenticationHeaderValue("bearer", token.Token);  
  
            }  
            catch (Exception e)  
            {  
  
                throw;  
            }  
  
  
  
            //return true state - success - everything worked  
            return true;  
        }  
  
        public async Task Logout()  
        {  
            await _localStorage.RemoveItemAsync("authToken");  
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedOut();  
        }  
  
        public async Task<bool> Register(RegistrationModel user)  
        {  
            //like the create from the baserepository  
            //create request  
            var request = new HttpRequestMessage(HttpMethod.Post,  
                Endpoints.RegisterEndpoint);  
  
            //Fill it with content  
            request.Content = new StringContent(JsonConvert.SerializeObject(user),  
                Encoding.UTF8, "application/json");  
  
            var client = _client.CreateClient();  
            HttpResponseMessage response = await client.SendAsync(request);  
  
            return response.IsSuccessStatusCode;  
  
        }  
    }  
}  
