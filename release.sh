#!/bin/bash
version=$(grep -o '"version":\s*"\([0-9]\.*\)*"' resources/modinfo.json | grep -o '\([0-9]\.*\)*')
petai=$(grep -o '"petai":\s*"\([0-9]\.*\)*"' resources/modinfo.json | grep -o '\([0-9]\.*\)*' | sed -r 's/[0-9]*$/+/')
releasefile='bin/wolftaming_v'$version'_petai_v'$petai'.zip'
dotnet build -c release
mv bin/wolftaming.zip $releasefile
gh release create --generate-notes 'v'$version $releasefile 