using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_UI.Models
{
    public class ResponseModel
    {
        /// <summary>
        /// Was the call successful or not
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Any error message from the response
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Any content from response
        /// </summary>
        public string Content { get; set; }
    }
}
