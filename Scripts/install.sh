#! /bin/sh

nuget restore GoogleSheet2Json.sln
nuget install NUnit.Runners -Version 2.6.4 -OutputDirectory testrunner
