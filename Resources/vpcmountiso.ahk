#NoEnv

SetTitleMatchMode 2
WinWait - Microsoft Virtual PC,, 0.5
vpcWin := WinExist()
if (vpcWin = 0) {
	MsgBox 0, ShoddyLauncher, No VPC2004/2007 VMs appear to be open.`nI'll open up the console; Start one up and try again.
	Try {
		Run "%ProgramFiles%\Microsoft Virtual PC\Virtual PC.exe", %ProgramFiles%\Microsoft Virtual PC, Min
	}
	ExitApp
}

WinMenuSelectItem,,, CD, Release
WinMenuSelectItem,,, CD, Capture ISO Image...
WinWait ahk_exe Virtual PC.exe ahk_class #32770
ControlSetText Edit1, %1%
; Loop pressing the "Open" button because sometimes it just doesn't acknowledge it.
fileDialog := WinExist()
While (WinExist("ahk_id " . fileDialog)) {
	ControlClick Button2
	Sleep 20
}
WinWaitClose
WinWait ahk_id %vpcWin%
WinActivate