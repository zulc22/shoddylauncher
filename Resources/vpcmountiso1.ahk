#NoEnv

SetTitleMatchMode 2
WinWait - Microsoft Virtual PC,, 0.5
vpcWin := WinExist()
if (vpcWin = 0) {
	ExitApp
}

WinMenuSelectItem,,, CD, Release