'use strict';
  
// imports
const fetch = require('node-fetch');
const { Console } = require('console');
  

// globals
var githuburl = "https://api.github.com/repos/Microsoft/";
var orchestratorLabels = [ "Area:%20AI-Orchestrator", "Area:%20Orchestrator", "Orchestrator" ]
var issuefilter = "/issues?state=open&labels=";
var repos = [ "botframework-cli", "BotBuilder-Samples", "botframework-sdk", "BotFramework-Composer"];
var token = '<removed>';

function processLine(line)
{

    var issue;

    issue = line;
    var owner = line.assignee.login;
    var id = line.number;
    var url = line.html_url;
    var created = line.created_at;
    var title = line.title;
	var milestone = line.milestone == null ? "none" : line.milestone.title;
	
	var labels = "";
	
	var len = line.labels.length;
	for(var i=0; i<len; i++)
	{
		labels += line.labels[i].name; 
		if(i < len-1)
		{
			labels += ", ";
		}
	}
		

    issue = {
      "id": id,
      "owner": owner,
      "title": title,
      "url": url,
      "created": created,
	  "labels": labels,
	  "Milestone": milestone
    }

    return issue;

}


function processIssues(json)
{
  var len = json.length;
  if(len < 1)
  {
    return;
    // throw("Not enough data to process issues");
  }

  // console.log("Received %s issues", len);

  for(var i=0; i<len; i++)
    {
        var issue = processLine(json[i]);
        if(issue != "")
        {
          console.log(issue);
        }
    }
}


function printRepoIssues(url)
{
  // console.log("Repo Url: " + url);
  
  var tokenValue = 'token ' + token;
  let settings = { 
	method: "Get", 
    headers: {
		'Authorization': tokenValue
	}
  };
  
  fetch(url, settings)
      .then(res => res.json())
      .then((json) => {
        try
        {
			if(json.hasOwnProperty('message'))
			{
				console.log(json);
			}
			else
			{
				processIssues(json);
			}
        }
        catch(e)
        {
          console.error(e);        
        }
      });
}



function main()
{

  console.log("Repos:");
  orchestratorLabels.forEach( olabel =>
    {
	  repos.forEach( repo => 
		{
	      var url = githuburl + repo + issuefilter + olabel;
		  console.log(url);
		}
	  ) 

	  console.log("Issues:");
	  repos.forEach( repo => 
		{
	      var url = githuburl + repo + issuefilter + olabel;
		  printRepoIssues(url);
		} 
	  )
    }
  )

}




/// MAIN ///

main();


/**

console.log("Args (not using):")
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
    console.log("Apply no filter")
    noFilter = false;
  }
}

var targeturl = githuburl + repo + issuefilter;

console.log("Source URL: " + targeturl);



let url = targeturl;
let settings = { method: "Get" };

fetch(url, settings)
    .then(res => res.json())
    .then((json) => {
      try
      {
        processIssues(json);
      }
      catch(e)
      {
        console.error(e);        
      }
    });


**/
