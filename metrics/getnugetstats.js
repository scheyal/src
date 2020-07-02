'use strict';
/*
console.log("Args:")
for (let j = 0; j < process.argv.length; j++) {
    console.log(j + ' -> ' + (process.argv[j]));
}*/


/// helper functions

function showObject(obj) {
  var result = "";
  for (var p in obj) {
    if( obj.hasOwnProperty(p) ) {
      result += p + " , " + obj[p] + "\n";
    } 
  }              
  return result;
}

function parseWeekObj(wobj)
{
        // console.log(wobj);
        var count = wobj["count"];
        var week = wobj["week"];
        var toks = week.split(/([-T])/);
        // datetoks.forEach(x => console.log("t: " + x));
        var year = toks[0];
        var month = toks[2];
        return { "month": month, "year" : year, "count" : count };

}

function processDownloads(json)
{
  if(json["downloads"].length < 1)
  {
    throw("Not enough data to process downloads");
  }

  var weekLine = json["downloads"][0];
  var week = parseWeekObj(weekLine);
  // init first week
  var lastmonth = week.month;
  var lastyear = week.year;
  var lastcount = week.count;
  var mtotal = 0;

  console.log("Downloads , month")
  for(var i=0; i<json["downloads"].length; i++)
    {
        weekLine = json["downloads"][i];
        week = parseWeekObj(weekLine);
//        console.log(week);
        var year = week.year;
        var month = week.month;
        var count = week.count;
        var delta = count - lastcount;
        lastcount = count;

        // console.log(" %s, %s", month, delta);

        if(lastmonth == month)
        {
          // sum up
          mtotal += delta;
        }
        else
        {
          console.log("%s , %s-%s", mtotal, lastyear, lastmonth);
          // reset
          lastmonth = month;
          lastyear = year;
          mtotal = delta;

        }
    }
    if(mtotal != 0)
    {
      console.log("%s , %s-%s", mtotal, lastyear, lastmonth);
    }



}

///

// "globals"
var nugeturl = "https://nugettrends.com/api/package/history/"
var pkgname = "Microsoft.Bot.Builder";
var months = "7";
var targeturl = nugeturl + pkgname + "?months=" + months;


// process cmd line
if(process.argv.length == 3)
{
  const helpkey = new Set(["-h", "-?", "--help", "help", "/?", "/h"]);
  var a2 = process.argv[2];
  if(helpkey.has(a2) == true)
  {
    console.log("getnugetstats  <packagename> where default is " + pkgname)    
    process.exit(-1);
  }
  else
  {
    pkgname = a2;
  }
}

console.log("Stats from: " + targeturl);

// get stats 
const fetch = require('node-fetch');
const { Console } = require('console');
let url = targeturl;
let settings = { method: "Get" };

fetch(url, settings)
    .then(res => res.json())
    .then((json) => {
      try
      {
        processDownloads(json);
      }
      catch(e)
      {
        console.error(e);        
      }
    });