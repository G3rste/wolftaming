Remove-Item "bin/vsdebug" -Force -Recurse;
New-Item -Path "bin/vsdebug" -Name "wolftaming" -ItemType "directory";
Copy-Item -Path "resources/*" -Destination "bin/vsdebug/wolftaming" -Recurse;
Copy-Item -Path "bin/Debug/net10.0/*" -Destination "bin/vsdebug/wolftaming";
