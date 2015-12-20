using AmazonQuery.Models;
using System.Collections.Generic;
using System.Linq;

namespace AmazonQuery
{

    public class AmazonFinder
    {
        public List<AmazonItem> result;     // holds 50 max items from amazon server
        private int offset;                 
        private int last_step;
        public  const int STEP13 = 13;
        private const int firstPage = 1;    // first page to query on amazon
        private const int lastPage = 5;     // last and max page to query on amazon

        public AmazonFinder(string name)
        {
            result = new List<AmazonItem>{ };
            offset = 0;
            last_step = 0;
            findItems(name);
        }
        // function find and fills the list with items from amazon
        private void findItems(string name)
        {
            // fill in the list with max 50 items
            for(var i = firstPage; i <= lastPage; i++)
            {
                result.AddRange(AItemLookup.itemsReq(name,i));
            }
            

        }
        // function returns a single page of max STEP items (13)
        public List<AmazonItem> getNext(int STEP)
        {
            int items_n = result.Count();
            if (offset >= items_n) return null;

            int items_left = items_n - offset;
            int step = items_left < STEP ? items_left : STEP;

            var listSTEP = result.GetRange(offset, step);


            offset += step;
            last_step = step;

            return listSTEP;
        }       
        
    }
}