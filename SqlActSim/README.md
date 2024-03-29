# SQL Simulated Activity Hydration

## Introduction
The following program generated synthetic activities for Microsoft Sustainability Manager. 

It currently supports two activity types:
* Purchased Electricity (power)
* Transportation and Distribution (TD)

## Usage

```
Usage: SqlActSim  ActivityType Total Watermark
where:
 ActivityType: [PE | ME | WO ]
        PE = Purchased Electricity
        ME = Mobile Combustion Precalc Emissions
        WO = Waste generated in Operations
 Total: count of activities
 Watermark: Stamp activity name, description. One word. Optional. 'TestOnly3Lines' for testing.
```

## Setup
* Provision a SQL server
* Create tables for supported activity types
* Edit App.Config.txt and copy over to App.Config.txt for connection string specifications
* Build and run
