using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        // DTO = Data Transfer Object
        // DTO's carry data between processes, they encapsulate data and
        // send it from one subsystem fo an application to another

        //Columns in Author db
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Bio { get; set; }

        //Note, not interacting with Book, but with the DTO
        public virtual IList<BookDTO> Books { get; set; }
    }

    public class AuthorCreateDTO
    {
        //https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations.keyattribute?view=net-5.0

        [Required(ErrorMessage = "Author First Name is required")]
        public string Firstname { get; set; }
        [Required(ErrorMessage = "Author Last Name is required")]
        [StringLength(25, ErrorMessage ="Author's last name cannot exceed 25 characters")]
        public string Lastname { get; set; }
        public string Bio { get; set; }
    }
    public class AuthorUpdateDTO
    {
        // DTO = Data Transfer Object
        // DTO's carry data between processes, they encapsulate data and
        // send it from one subsystem fo an application to another

        //Columns in Author db
        [Required(ErrorMessage = "Author Id is required")]
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Bio { get; set; }

    }
}
