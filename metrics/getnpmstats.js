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

///




var pkgname = "botbuilder";

if(process.argv.length == 3)
{
  const helpkey = new Set(["-h", "-?", "--help", "help", "/?", "/h"]);
  var a2 = process.argv[2];
  if(helpkey.has(a2) == true)
  {
    console.log("getnpmstats <packagename> where default is " + pkgname)    
    process.exit(-1);
  }
  else
  {
    pkgname = a2;
  }
}

console.log("NPM stats for package: " + pkgname)
var stats = require('download-stats');


var start = new Date('2020-01-01');
var end = new Date('2020-06-30');
var downloads = [];
stats.get(start, end, pkgname)
 .on('error', console.error)
 .on('data', function(data) {
    downloads.push(data);
    // console.log(data);
  })
  .on('end', function() {
    console.log('done.');
    var monstats = stats.calc.monthly(downloads);
    console.log("Monthly Stats:");
    console.log("Month   , Downloads")
    console.log(showObject(monstats));
  });

