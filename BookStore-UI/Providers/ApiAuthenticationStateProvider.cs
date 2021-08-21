using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStore_UI.Providers
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        /// <summary>
        /// Will be checking local storage so inject it 
        /// </summary>
        /// <param name="localStorage"></param>
        public ApiAuthenticationStateProvider(ILocalStorageService localStorage, JwtSecurityTokenHandler tokenHandler)
        {
            _localStorage = localStorage;
            _tokenHandler = tokenHandler; 
        }


        /// <summary>
        /// Is the person autheticated or not?
        /// Overriding becuase we are using tokens
        /// </summary>
        /// <returns></returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var savedToken = await _localStorage.GetItemAsync<string>("authToken");
                if (string.IsNullOrWhiteSpace(savedToken))
                {
                    //nobody here, all empty - tell blazor app nobody is logged in
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }
                //if token is expired remove it and refuse authentication
                var tokenContent = _tokenHandler.ReadJwtToken(savedToken);

                //Has token expired?
                var expiring = tokenContent.ValidTo;
                if(expiring < DateTime.Now)
                {
                    //remove from storage token expired
                    await _localStorage.RemoveItemAsync("authToken");
                    //nobody here, all empty - tell blazor app nobody is logged in
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }
                //Get claims from the token (create method for this as it will be reused (ParseClaims below)
                //Build authenticated user object
                var claims = ParseClaims(tokenContent);
                var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
                //return authenticated person
                return new AuthenticationState(new ClaimsPrincipal(user));
            }
            catch (Exception)
            {
                 //log out everyone
                 return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

        }

        public async Task LoggedIn()
        {
            //set authentication state
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");
            var tokenContent = _tokenHandler.ReadJwtToken(savedToken);
            var claims = ParseClaims(tokenContent);
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }

        /// <summary>
        /// Change state to 'nobody there'
        /// </summary>
        public void LoggedOut()
        {
            var nobody = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(nobody));
            NotifyAuthenticationStateChanged(authState);
        }

        private IList<Claim> ParseClaims(JwtSecurityToken tokenContent)
        {
            var claims = tokenContent.Claims.ToList(); //INumerable of tokens in Claim
            claims.Add(new Claim(ClaimTypes.Name, tokenContent.Subject)); //take "sub" field from token
            return claims;
        }
    }
}
