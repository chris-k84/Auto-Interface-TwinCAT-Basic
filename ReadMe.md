# Automation Interface Basic Implementation

## Project: 
This project is to create a set of classes that capture the functionality of the [Automation Interface](https://infosys.beckhoff.com/content/1033/tc3_automationinterface/index.html?id=3954232867334285510). The project is designed to give you a set of classes which can be used as the basis of your own implementations or simply provide a set of examples for how to perform different tasks.

for more detailed info on how the Automation Interface works, please check out the WIKI.

## Requirements: 
TwinCAT 3 4022 or higher

## Creator: 
ChrisK

## Contributors: 
ChrisK - LOL All by myself!

## Comments:

## Details:
The set of wrapper classes to allow the easy implementation of the Automation Interface.
- IO Handler class deals with Fieldbus IO handling
- PLC Handler class deals with the PLC elements, creating PLC projects and elements also level 2 interfaces which allow code modification, library control etc.
- TwinCAT Handler class deals with the level 1 interface for system set up, creating tasks, core assignment, linking etc.
- Visual Studio Handler class deals with the DTE Visual Studio interaction
- ADS Handler class deals with ADS comms, including target detection and route creation.


## Notice:
