#! /bin/sh

msbuild /p:Configuration=Release GoogleSheet2Json.sln
mono ./testrunner/NUnit.Runners.2.6.4/tools/nunit-console.exe ./GoogleSheet2JsonTest/bin/Release/GoogleSheet2JsonTest.dll