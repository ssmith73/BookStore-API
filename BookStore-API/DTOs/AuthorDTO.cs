using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.DTOs
{

    /// <summary>
    /// Use this DTO class to enforce certain validations
    /// Users interact with this class - not the Author class
    /// Set any limits on user interaction with database objects here
    /// </summary>
    public class AuthorDTO
    {
        //Columns in Author db
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Bio { get; set; }

        //Note, not interacting with Book, but with the DTO
        public virtual IList<BookDTO> Books { get; set; }

    }
}
