using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore_API.Data
{
    [Table("Authors")]
    public partial class Author
    {
        //Columns in Author db
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Bio { get; set; }

        public virtual IList<Book> Books { get; set; }

    }
}