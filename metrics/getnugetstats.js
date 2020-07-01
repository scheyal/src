'use strict';
/*
console.log("Args:")
for (let j = 0; j < process.argv.length; j++) {
    console.log(j + ' -> ' + (process.argv[j]));
}*/


// helper functions
function showObject(obj) {
  var result = "";
  for (var p in obj) {
    if( obj.hasOwnProperty(p) ) {
      result += p + " , " + obj[p] + "\n";
    } 
  }              
  return result;
}



var nugeturl = "https://nugettrends.com/api/package/history/"
var pkgname = "Microsoft.Bot.Builder";
var months = "6";

var targeturl = nugeturl + pkgname + "?months=" + months;

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


const fetch = require('node-fetch');

let url = targeturl;

let settings = { method: "Get" };

fetch(url, settings)
    .then(res => res.json())
    .then((json) => {
        console.log(json);
        for(var i=0; i<json["downloads"].length; i++)
        {
            var mtotal = 0;
            var wobj = json["downloads"][i];
            console.log(wobj);

        }

    });