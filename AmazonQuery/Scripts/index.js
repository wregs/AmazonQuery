var nextPagesBuffer = null;     //buffer to store nextpages
var prevPagesBuffer = null;     //buffer to store prevpages
var current = null;             // current page of items
var currency = "USD";           // selected currency
var rate = 1.0;                 // current currency rate 

// function to initially display the first page of max 13 items from amazon
// called on keyword submit on ajax responce
function post_request(result) {

    if (result.length == 0) { $('#initmsg').show(); return; }

    prevPagesBuffer = null;
    nextPagesBuffer = null;

    var raw_data = getNext13();
    current = result;
    apply_cur_rate(current);

    display_results(current);    

    if (raw_data.length != 0) {
        nextPagesBuffer = Array(jQuery.parseJSON(raw_data));
        activate_nextpagelink();
    }

}

// function to display current page of max 13 items
function display_results(result) {

    var html_result = "";

    for(var i=0;i<result.length;i++)
    {
        var price_val = parseInt(result[i].aPrice);
        var price = "";
        if (price_val < 0) price = "<b>N/A</b>";
        else {
            price = (price_val / 100)+" <span class=\"price_tag\">"+currency+"</span>";
        }

         var item="<hr/><table><tr><td rowspan=\"2\"><image src="+result[i].ImageLink+" /></td>"+
                  "<td valign=\"top\"><div style=\"margin-left:20px;\"><b> "+result[i].Title+"</b></div></td></tr>"+
                  "<tr><td><div style=\"margin-left:20px;\"><span class=\"price\">" + price + "</span> </td></tr></table>";
         html_result += item;
    }
    var html_pageselect = "<br/><div><center><span id='prevPageLink'><- Prev Page</span> | <span id='nextPageLink'>Next Page -></span></center></div>";

    $('#search_result').html(html_result);
    $('#search_result').append(html_pageselect).show();

}
// function to active a link for the next page of max 13 items
function activate_nextpagelink() {
   
    $('#nextPageLink').wrap("<a href='javascript:nextPage();'></a>");
    
}
// function to active a link for the prev page of max 13 items
function activate_prevpagelink() {

    $('#prevPageLink').wrap("<a href='javascript:prevPage();'></a>");

}
// function to switch to the next page of max 13 items
// if there is a page in nextPageBuffer then get from the buffer
// else get next page from server
function nextPage() {
    if (nextPagesBuffer.length == 0) { reset(); return; }

    if (prevPagesBuffer === null) { prevPagesBuffer = Array(current); }
    else { prevPagesBuffer = prevPagesBuffer.concat(Array(current)); };
    
    current = nextPagesBuffer[0];
    
    nextPagesBuffer = nextPagesBuffer.slice(1);
    
    if (nextPagesBuffer.length == 0)
    {
        var raw_data = getNext13();
        if (raw_data.length != 0) {
            nextPagesBuffer = Array(jQuery.parseJSON(raw_data));
        }
    }
    apply_cur_rate(current);
    display_results(current);
    
    
    if (nextPagesBuffer.length != 0) {       
        activate_nextpagelink();
    }
    if(prevPagesBuffer.length!=0)
    {
        activate_prevpagelink();
    }

}
// function to switch to prev page of max 13 items if there is a page in the prevPagesBuffer
// else call is faulty - do reset of variables
function prevPage()
{
    if (prevPagesBuffer === null) { reset(); return; }
    nextPagesBuffer = nextPagesBuffer.concat(Array(current));
    var prev13 = prevPagesBuffer[(prevPagesBuffer.length - 1)];
    current = prev13;
    apply_cur_rate(current);
    display_results(current); 
    prevPagesBuffer = prevPagesBuffer.slice(0, (prevPagesBuffer.length - 1));

    if (prevPagesBuffer.length != 0) {
        
        activate_prevpagelink();
    }
    if (nextPagesBuffer.length != 0) {
        activate_nextpagelink();
    }
}
// function to retrieve the next page of max 13 items from server
// call is synchronous
function getNext13() {
    return $.ajax({
        dataType:'json',
        type: 'POST',
        url: '/Home/Next',
        async: false
    }).responseText;
}
// resets variables and search data of the page
function reset() {
    next13 = null;
    current = null;
    prevPages = null;
    $("#search_result").hide();
    $("#initmsg").show();
}

// functions to apply currency rate to 1 set of items
function apply_cur_rate(set)
{
    if (set == null) return;

    for(var i=0;i<set.length;i++)
    {
        var oprice = parseInt(set[i].oPrice);
        if (oprice < 0) continue;

        var aprice = oprice * rate;
        set[i].aPrice = aprice.toString(10);
    }
    
}
// functions to apply currency rate to all stored pages and current page
function apply_cur_rateAll(rate) {
    if(nextPagesBuffer != null)
        if(nextPagesBuffer.length>0)
        {
            for(var i=0;i<nextPagesBuffer.length;i++)
            {
                apply_cur_rate(nextPagesBuffer[i]);
            }
        }
    if(prevPagesBuffer!=null)
        if(prevPagesBuffer.length>0)
        {
            for(var i=0;i<prevPagesBuffer.length;i++)
            {
                apply_cur_rate(prevPagesBuffer[i]);
            }
        }
    if(current!=null){
        apply_cur_rate(current);
        display_results(current);
    }
    
    reactivate_pagelinks();
}
// function to reactivate page links after currency modification
function reactivate_pagelinks(){
    if(nextPagesBuffer != null)
        if(nextPagesBuffer.length>0)
        {
            activate_nextpagelink();
        }
    if(prevPagesBuffer!=null)
        if(prevPagesBuffer.length>0)
        {
            activate_prevpagelink();
        }
}