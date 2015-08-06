@ECHO OFF
ECHO Stop Service
sc stop JavaServiceHostTest 

ECHO Uninstall Service
sc delete JavaServiceHostTest 

ECHO Install Service
sc create JavaServiceHostTest binpath= "JavaWindowsServiceHost\JavaWindowsServiceHost\bin\Debug\za.co.fnbs.JavaWindowsServiceHost.exe"

ECHO Start Service
sc start JavaServiceHostTest 

PAUSE

ECHO Stop on Exit
sc stop JavaServiceHostTest 
