; 安装程序初始定义常量
!define PRODUCT_NAME "ACF上传工具"
!define PRODUCT_VERSION "1.0"
!define PRODUCT_PUBLISHER "启程网络(7-cheng.com), Inc."
!define PRODUCT_WEB_SITE "http://www.7-cheng.com/"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\DoubleX.Upload.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"
!define PRODUCT_STARTMENU_REGVAL "NSIS:StartMenuDir"


SetCompressor lzma


; ------ MUI 现代界面定义 (1.67 版本以上兼容) ------
!include "MUI.nsh"

; MUI 预定义常量
!define MUI_ABORTWARNING
!define MUI_ICON "..\图标\favicon.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; 欢迎页面
!insertmacro MUI_PAGE_WELCOME
; 许可协议页面
;!insertmacro MUI_PAGE_LICENSE "..\文件\Licence.txt"
; 安装目录选择页面
!insertmacro MUI_PAGE_DIRECTORY
; 开始菜单设置页面
var ICONS_GROUP
!define MUI_STARTMENUPAGE_NODISABLE
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "ACF上传工具"
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "${PRODUCT_UNINST_ROOT_KEY}"
!define MUI_STARTMENUPAGE_REGISTRY_KEY "${PRODUCT_UNINST_KEY}"
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "${PRODUCT_STARTMENU_REGVAL}"
!insertmacro MUI_PAGE_STARTMENU Application $ICONS_GROUP

!define Code "0002"
!define Edition "Pro"
;Air/Pro

; 安装过程页面
!insertmacro MUI_PAGE_INSTFILES
; 安装完成页面
!define MUI_FINISHPAGE_RUN "$INSTDIR\DoubleX.Upload.exe"
!insertmacro MUI_PAGE_FINISH

; 安装卸载过程页面
!insertmacro MUI_UNPAGE_INSTFILES

; 安装界面包含的语言设置
!insertmacro MUI_LANGUAGE "SimpChinese"

; 安装预释放文件
!insertmacro MUI_RESERVEFILE_INSTALLOPTIONS
; ------ MUI 现代界面定义结束 ------


Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "setup\${Code}\${Edition}.exe"
InstallDir "$PROGRAMFILES\ACF上传工具"
InstallDirRegKey HKLM "${PRODUCT_UNINST_KEY}" "UninstallString"
ShowInstDetails show
ShowUnInstDetails show
BrandingText " "
RequestExecutionLevel admin

Section -.NET Framework
  ;检测是否是需要的.NET Framework版本
  Call GetNetFrameworkVersion
  Pop $R1
  ;${If} $R1 < '2.0.50727'
  ;${If} $R1 < '3.5.30729.4926'
  ;${If} $R1 < '4.0.30319'
	${If} $R1 < '4.5.52747'
    MessageBox MB_YESNO|MB_ICONQUESTION "此软件运行需要.NET Framework 4.5 运行环境，但您机器上似乎没有安装此环境。$\r$\n您是否：$\r$\n1.使用此安装程序在线下载并安装.NET Framework 4.5$\r$\n2.使用百度搜索或QQ/360管家下载安装.NET Framework 4.5 $\r$\n$\r$\n是否在线下载并安装.NET Framework 4.5,请确认您的机器已连接互联网？" IDNO +2
      Call DownloadNetFramework45
  ${ENDIF}
SectionEnd

Section "MainSection" SEC01

  SetOutPath "$INSTDIR"
  SetOverwrite on
  File "..\..\..\..\Release\上传工具\DoubleX.Upload.exe.config"
  File "..\..\..\..\Release\上传工具\DoubleX.Upload.exe"
  SetOutPath "$INSTDIR\Bin"
  File /r "..\..\..\..\Release\上传工具\Bin\*.*"
  SetOutPath "$INSTDIR\Data"
  File /r "..\..\..\..\Release\上传工具\Data\*.*"
  File "file\${Code}\config.xml"
  File "license\${Edition}\license.key"
  SetOutPath "$INSTDIR\x64"
  File /r "..\..\..\..\Release\上传工具\x64\*.*"
  SetOutPath "$INSTDIR\x86"
  File /r "..\..\..\..\Release\上传工具\x86\*.*"
  SetOutPath "$INSTDIR\Help"
  File /r "..\..\..\..\Release\上传工具\Help\*.*"
  

; 创建开始菜单快捷方式
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
  CreateDirectory "$SMPROGRAMS\$ICONS_GROUP"
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\ACF上传工具.lnk" "$INSTDIR\DoubleX.Upload.exe"
  SetOutPath "$INSTDIR"
  CreateShortCut "$DESKTOP\ACF上传工具.lnk" "$INSTDIR\DoubleX.Upload.exe"
  !insertmacro MUI_STARTMENU_WRITE_END
SectionEnd


Section -AdditionalIcons
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
  WriteIniStr "$INSTDIR\${PRODUCT_NAME}.url" "InternetShortcut" "URL" "${PRODUCT_WEB_SITE}"
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\Website.lnk" "$INSTDIR\${PRODUCT_NAME}.url"
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\Uninstall.lnk" "$INSTDIR\uninst.exe"
  !insertmacro MUI_STARTMENU_WRITE_END
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr HKLM "${PRODUCT_DIR_REGKEY}" "" "$INSTDIR\DoubleX.Upload.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\DoubleX.Upload.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd

/******************************
 *  以下是安装程序的卸载部分  *
 ******************************/

Section Uninstall
  !insertmacro MUI_STARTMENU_GETFOLDER "Application" $ICONS_GROUP
  Delete "$INSTDIR\${PRODUCT_NAME}.url"
  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\DoubleX.Upload.exe"
  Delete "$INSTDIR\DoubleX.Upload.exe.config"

  Delete "$SMPROGRAMS\$ICONS_GROUP\Uninstall.lnk"
  Delete "$SMPROGRAMS\$ICONS_GROUP\Website.lnk"
  Delete "$DESKTOP\ACF上传工具.lnk"
  Delete "$SMPROGRAMS\$ICONS_GROUP\ACF上传工具.lnk"

  RMDir "$SMPROGRAMS\$ICONS_GROUP"

  RMDir /r "$INSTDIR\temp"
  RMDir /r "$INSTDIR\Bin"
  RMDir /r "$INSTDIR\Data"
  RMDir /r "$INSTDIR\x64"
  RMDir /r "$INSTDIR\x86"

  RMDir "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKLM "${PRODUCT_DIR_REGKEY}"
  SetAutoClose true
SectionEnd

#-- 根据 NSIS 脚本编辑规则，所有 Function 区段必须放置在 Section 区段之后编写，以避免安装程序出现未可预知的问题。--#

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "您确实要完全移除 $(^Name) ，及其所有的组件？" IDYES +2
  Abort
FunctionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) 已成功地从您的计算机移除。"
FunctionEnd


/******************************
 *  .NetFramework 版本测检  *
 ******************************/

Function GetNetFrameworkVersion
;获取.Net Framework版本支持
    Push $1
    Push $0
    ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Install"
    ReadRegDWORD $1 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Version"
    StrCmp $0 1 KnowNetFrameworkVersion +1
    ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5" "Install"
    ReadRegDWORD $1 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5" "Version"
    StrCmp $0 1 KnowNetFrameworkVersion +1
    ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.0\Setup" "InstallSuccess"
    ReadRegDWORD $1 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.0\Setup" "Version"
    StrCmp $0 1 KnowNetFrameworkVersion +1
    ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727" "Install"
    ReadRegDWORD $1 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727" "Version"
    StrCmp $1 "" +1 +2
    StrCpy $1 "2.0.50727.832"
    StrCmp $0 1 KnowNetFrameworkVersion +1
    ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v1.1.4322" "Install"
    ReadRegDWORD $1 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v1.1.4322" "Version"
    StrCmp $1 "" +1 +2
    StrCpy $1 "1.1.4322.573"
    StrCmp $0 1 KnowNetFrameworkVersion +1
    ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\.NETFramework\policy\v1.0" "Install"
    ReadRegDWORD $1 HKLM "SOFTWARE\Microsoft\.NETFramework\policy\v1.0" "Version"
    StrCmp $1 "" +1 +2
    StrCpy $1 "1.0.3705.0"
    StrCmp $0 1 KnowNetFrameworkVersion +1
    StrCpy $1 "not .NetFramework"
    KnowNetFrameworkVersion:
    Pop $0
    Exch $1
FunctionEnd

Function DownloadNetFramework45
;下载 .NET Framework 4.5
  NSISdl::download /TRANSLATE2 '正在下载 %s' '正在连接...' '(剩余 1 秒)' '(剩余 1 分钟)' '(剩余 1 小时)' '(剩余 %u 秒)' '(剩余 %u 分钟)' '(剩余 %u 小时)' '已完成：%skB(%d%%) 大小：%skB 速度：%u.%01ukB/s' /TIMEOUT=7500 /NOIEPROXY 'http://download.microsoft.com/download/E/2/1/E21644B5-2DF2-47C2-91BD-63C560427900/NDP452-KB2901907-x86-x64-AllOS-ENU.exe' '$TEMP\NDP452-KB2901907-x86-x64-AllOS-ENU.exe'
  Pop $R0
  StrCmp $R0 "success" 0 +2

  SetDetailsPrint textonly
  DetailPrint "正在安装 .NET Framework 4.5.2 ..."
  SetDetailsPrint listonly
  ExecWait '$TEMP\NDP452-KB2901907-x86-x64-AllOS-ENU.exe /quiet /norestart' $R1
  Delete "$TEMP\NDP452-KB2901907-x86-x64-AllOS-ENU.exe"

FunctionEnd

