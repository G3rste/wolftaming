#!/bin/bash
version=$(grep 'version' resources/modinfo.json | sed -r 's/\s*"version":\s*"(([0-9]\.*)*)",?/\1/')
petai=$(grep 'petai' resources/modinfo.json | sed -r 's/\s*"petai":\s*"(([0-9]\.*)*)",?/\1/' | sed -r 's/[0-9]*$/+/')
releasefile='bin/wolftaming_v'$version'_petai_v'$petai'.zip'
dotnet build -c release
mv bin/wolftaming.zip $releasefile
gh release create 'v'$version $releasefile