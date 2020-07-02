'use strict';
/*
console.log("Args:")
for (let j = 0; j < process.argv.length; j++) {
    console.log(j + ' -> ' + (process.argv[j]));
}*/

// "globals"
var nugeturl = "https://api.github.com/repos/Microsoft/";
var pkgname = "BotFramework-Emulator";
var minver = 4;
var noFilter = true;

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

function getAssetType(asset)
{
    const pkgTypes =  new Set([".AppImage", ".dmg", ".zip", ".exe"]);

    if(asset.endsWith(".dmg") || asset.endsWith(".zip"))
    {
        return "mac";
    }
    else if(asset.endsWith(".AppImage"))
    {
        return "linux";
    }
    else if(asset.endsWith("exe"))
    {
        return "windows";
    }

    return "unknown";
}

function packageLine(pkg)
{
    var vername = pkg["name"];
    var mmver = pkg["mmver"];
    var date = pkg["published_at"];
    var assetsCount = pkg["assets"].length;
    var assets = new Array(assetsCount);
    var actualCount = 0;
    for(var i=0;i<assetsCount;i++)
    {
        var assetName = pkg["assets"][i]["name"];
        var assetDownloads = pkg["assets"][i]["download_count"];
        var assetType = getAssetType(assetName);
        if(assetType != "unknown")
        {
            assets[i] = { "asset": assetName, assetType, "downloads": assetDownloads}; 
            actualCount++;
        }
    }    
    assets = assets.slice(0, actualCount);
    return {"mmver": mmver, "vername": vername, "date": date, "assets": assetsCount, assets};
}

function processLine(pkg)
{
    var re = "^("+minver+").\\d+.\\d+$";
    var verfilter = new RegExp(re);
    var ver = pkg["name"];
    var ret = "";
    var match = ver.match(verfilter);
    var match = verfilter.exec(ver);
    if(noFilter == false || 
       (match != null && match.length > 1 && match[1] >= minver))
    {
        ver = ver.replace(/(\d+.\d+)(.\d+)/, "$1");
        pkg["mmver"] = ver;
        ret = packageLine(pkg);
    }
    return ret;

}

function processDownloads(json)
{
  var len = json.length;
  if(len < 1)
  {
    throw("Not enough data to process downloads");
  }

  console.log("Received %s releases", len);

  console.log("majminver, version, release_date, package_name, os_type, downloads");

  for(var i=0; i<len; i++)
    {
        var pkg = processLine(json[i]);
        if(pkg != "")
        {
            // console.log(pkg);
            for(var a=0; a<pkg.assets.length; a++)
            {
                console.log("%s, %s, %s, %s, %s, %s", pkg.mmver, pkg.vername, pkg.date, 
                            pkg.assets[a].asset, pkg.assets[a].assetType, pkg.assets[a].downloads);
            }
        }
    }
}

///

console.log("Args:")
for (let j = 0; j < process.argv.length; j++) {
    console.log(j + ' -> ' + (process.argv[j]));
}

// process cmd line
if(process.argv.length > 2)
{
  console.log("Processing parameters (%s)", process.argv.length);
  const helpkey = new Set(["-h", "-?", "--help", "help", "/?", "/h"]);
  var a2 = process.argv[2];
  if(helpkey.has(a2) == true)
  {
    console.log("getgithubtats  <packagename> where default is " + pkgname)    
    process.exit(-1);
  }
  else
  {
    pkgname = a2;
    console.log("Selected package: ", pkgname);
  }

  if(process.argv[3] == "nofilter")
  {
    noFilter = false;
  }
}

var targeturl = nugeturl + pkgname + "/releases";


console.log("Source URL: " + targeturl);


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