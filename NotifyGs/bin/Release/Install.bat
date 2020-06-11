mkdir c:\NotifyGs
copy \\172.16.125.197\public\NotifyGs\*.*  c:\NotifyGs\
copy \\172.16.125.197\public\NotifyGs\gsession.* %USERPROFILE%\Desktop\
copy \\172.16.125.197\public\NotifyGs\NotifySet.bat %USERPROFILE%\Desktop\
c:\NotifyGs\NotifySet.exe