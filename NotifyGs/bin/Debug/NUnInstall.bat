mshta vbscript:execute("MsgBox(""次の画面で「チェック停止＆設定の削除」をクリックしてください""):close")
c:\NotifyGs\NotifyGs.exe /gui
rd /q /s c:\NotifyGs
del %USERPROFILE%\Desktop\グループセッション.url