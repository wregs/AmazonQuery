using System.Collections.Generic;

namespace AmazonQuery.Models
{
    public class AmazonItem
    {
        public string Title { get; set; }
        public string ImageLink { get; set; }
        public string Currency { get; set; }    
        public int oPrice { get; set; } //original price from server (USD server)
        public int aPrice { get; set; } //adjusted prices accrourding to selected currency
    }

    public class AmazonItems
    {
        public List<AmazonItem> items { get; set;}
    }
}