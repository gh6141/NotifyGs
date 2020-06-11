mshta vbscript:execute("MsgBox(""次の画面で「チェック停止＆設定の削除」をクリックしてください""):close")
c:\NotifyGs\NotifySet.exe
rd /q /s c:\NotifyGs
del %USERPROFILE%\Desktop\gsession.url
del %USERPROFILE%\Desktop\NotifySet.bat