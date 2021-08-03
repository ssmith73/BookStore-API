using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.DTOs
{
    /// <summary>
    /// DTO = Data Transfer Object
    /// DTO's carry data between processes, they encapsulate data and
    /// send it from one subsystem fo an application to another
    /// </summary>
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int? Year { get; set; }
        public string Isbn { get; set; }
        public string Summary { get; set; }
        public string Image { get; set; }
        public decimal? Price { get; set; }

        //nullable, allowed to be null - isNull in the database
        public int? AuthorId { get; set; }

        //DTO's only talk to DTO's
        public virtual AuthorDTO Author { get; set; }
    }

    /// <summary>
    /// client save mapping
    /// </summary>
    public class BookCreateDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Isbn { get; set; }
        [StringLength(500)]
        public string Summary { get; set; }
        public string Image { get; set; }
        public decimal? Price { get; set; }

        //nullable, allowed to be null - isNull in the database
        [Required]
        public int AuthorId { get; set; }

        //DTO's only talk to DTO's
    }

    /// <summary>
    /// Define what the client can update in a record
    /// </summary>
    public class BookUpdateDTO
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public int? Year { get; set; }
        [StringLength(500)]
        public string Summary { get; set; }
        public string Image { get; set; }
        public decimal? Price { get; set; }

        public int? AuthorId { get; set; }

        //DTO's only talk to DTO's
    }
}
