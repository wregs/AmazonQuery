using System.Net;
using System.Collections.Generic;
using System.Web.Helpers;
using System.Web.Mvc;

namespace AmazonQuery
{
    public class CurrencyList
    {
        // preset currency names correspond to fixer.io server currency name list
        public static List<string> curs = new List<string>(new string[]{"AUD","BGN","BRL","CAD","CHF","CNY","CZK","DKK",
                                                          "GBP","HKD","HUF","IDR","ILS","INR","JPY","KRW",
                                                          "MXN","MYR","NOK","NZD","PHP","PLN","RON","RUB",
                                                          "SEK","SGD","THB","TRY","USD","ZAR","EUR"});

        // function to create list with preset currency values
        // returns list for dropdownbox in _Layout.cshtml
        public static List<SelectListItem> populateList(string cur)
        {
            var list = new List<SelectListItem>() { };
            foreach (var i in curs)
            {
                // preselected value for dropdownbox, cur= USD
                if (i == cur)
                {
                    list.Add(new SelectListItem() { Text = i, Value = i, Selected = true });
                }
                else
                {
                    list.Add(new SelectListItem() { Text = i, Value = i });
                }
            }
            return list;

        }
        // gets currency rates for <new currency> ,EUR, and USD
        // returns rates based on EUR as Array[3]
        public static decimal[] getCurrencyRate(string new_cur,string eur,string cur_cur)
        {
            var arr = new decimal[3] { 0, 1, 0 };
            if (new_cur == eur) arr[0] = 1; //EUR rate is 1 always(rates based from server)
            if (cur_cur == eur) arr[2] = 1; //EUR rate is 1 always(rates based from server)

            using (var webClient = new WebClient())
            {                
                dynamic json = webClient.DownloadString("http://api.fixer.io/latest");
                
                var ob=Json.Decode<Dictionary<string,object>>(json);

                foreach(var kvp1 in ob)
                {
                    if (kvp1.Key == "rates")
                    {

                        foreach(var kvp2 in kvp1.Value)
                        {
                            if(arr[0]==0)
                            if (kvp2.Key == new_cur){ arr[0] = kvp2.Value; }
                            if(arr[2]==0)
                            if (kvp2.Key == cur_cur) { arr[2] = kvp2.Value; }

                        }                        
                    }
                }               
            }

            return arr;
        }
    }
}