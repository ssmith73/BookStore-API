using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_UI.Static
{
    public static class Endpoints
    {
        /// <summary>
        /// Url's for our API
        /// </summary>
        public static string BaseUrl { get; set; } = "https://localhost:44387/";
        public static string AuthorsEndpoint { get; set; } = $"{BaseUrl}api/authors/";
        public static string BookEndpoint { get; set; } = $"{BaseUrl}api/books/";
        public static string RegisterEndpoint { get; set; } = $"{BaseUrl}api/users/register/";

        //Endpoint for logins
        public static string LoginEndpoint { get; set; } = $"{BaseUrl}api/users/login/";


    }
}
