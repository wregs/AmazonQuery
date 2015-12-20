using System.ComponentModel.DataAnnotations;

namespace AmazonQuery.Models
{
    public class SearchReq
    {      
        [Required]
        public string Query { get; set; }        
    }
}