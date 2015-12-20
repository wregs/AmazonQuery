/**********************************************************************************************
 * Copyright 2009 Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"). You may not use this file 
 * except in compliance with the License. A copy of the License is located at
 *
 *       http://aws.amazon.com/apache2.0/
 *
 * or in the "LICENSE.txt" file accompanying this file. This file is distributed on an "AS IS"
 * BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under the License. 
 *
 * ********************************************************************************************
 *
 *  Amazon Product Advertising API
 *  Signed Requests Sample Code
 *
 *  API Version: 2009-03-31
 *
 */
/*******************************
* Source file has been modiefied for AmazonQuery project by Pavel Vavilov
*
* 
*/

using AmazonQuery.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;

namespace AmazonQuery
{
    public class AItemLookup
    {
        private const string AWS_ACCESS_KEY_ID = "";
        private const string AWS_SECRET_KEY = "";
        private const string AWS_ASSOC_TAG = "";
        private const string DESTINATION = "webservices.amazon.com";

        private const string NAMESPACE = "http://webservices.amazon.com/AWSECommerceService/2011-08-01";
        
        // functions queries amazon server for a single page of max 10 items
        // returns the list of AmazonItem
        public static List<AmazonItem> itemsReq(string keyword="",int itempage=1)
        {
            SignedRequestHelper helper = new SignedRequestHelper(AWS_ACCESS_KEY_ID, AWS_SECRET_KEY, DESTINATION);

            String requestUrl;           
     
            IDictionary<string, string> r1 = new Dictionary<string, String>();
            r1["AssociateTag"] = AWS_ASSOC_TAG;
            r1["Service"] = "AWSECommerceService";
            r1["Operation"] = "ItemSearch";
            r1["SearchIndex"] = "All";  //all categories
            r1["Keywords"] = keyword;   // keyword search
            r1["ItemPage"] = itempage.ToString();
            r1["ResponseGroup"] = "Images,ItemAttributes,Offers"; // get imagelinks and prices

            requestUrl = helper.Sign(r1);           

            return FetchItems(requestUrl);
        }
        // function parses retrieved xml responce
        // retrieves the list of AmazonItems
        private static List<AmazonItem> FetchItems(string url)
        {
            try
            {
                WebRequest request = HttpWebRequest.Create(url);
                WebResponse response = request.GetResponse();
                XmlDocument doc = new XmlDocument();
                doc.Load(response.GetResponseStream());

                XmlNodeList errorMessageNodes = doc.GetElementsByTagName("Message", NAMESPACE);
                if (errorMessageNodes != null && errorMessageNodes.Count > 0)
                {
                    String message = errorMessageNodes.Item(0).InnerText;
                    return null;
                }

                XmlNodeList items = doc.GetElementsByTagName("Item");
                List<AmazonItem> aitems = new List<AmazonItem> { };
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("ns0", NAMESPACE);
                foreach( XmlNode item in items)
                {
                    string title = "";
                    string imageLink = "";
                    int price = -1;
                    string currency = "";

                    try { title = item.SelectSingleNode(".//ns0:Title", nsmgr).InnerText; }
                    catch (Exception) { }
                    try { imageLink = item.SelectSingleNode(".//ns0:MediumImage//ns0:URL", nsmgr).InnerText; }
                    catch (Exception) { }
                    try { Int32.TryParse(item.SelectSingleNode(".//ns0:Offer//ns0:Price//ns0:Amount", nsmgr).InnerText, out price); }
                    catch (Exception) { }
                    try { currency = item.SelectSingleNode(".//ns0:Offer//ns0:Price//ns0:CurrencyCode", nsmgr).InnerText; }
                    catch (Exception) { }
                   
                    
                    aitems.Add(new AmazonItem { Title = title, ImageLink = imageLink,Currency=currency, oPrice = price, aPrice=price });
                }

                return aitems; 
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Caught Exception: " + e.Message);
                System.Console.WriteLine("Stack Trace: " + e.StackTrace);
            }

            return null;
        }
    }
}