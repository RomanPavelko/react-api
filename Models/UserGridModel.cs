using System.Collections.Generic;

namespace api.Models
{
    public class UserGridModel
    {
        public int TotalRecords {get;set;}
        public IList<User> Users { get; set; }        
    }
}