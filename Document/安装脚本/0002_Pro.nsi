; ��װ�����ʼ���峣��
!define PRODUCT_NAME "ACF�ϴ�����"
!define PRODUCT_VERSION "1.0"
!define PRODUCT_PUBLISHER "��������(7-cheng.com), Inc."
!define PRODUCT_WEB_SITE "http://www.7-cheng.com/"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\DoubleX.Upload.exe"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"
!define PRODUCT_STARTMENU_REGVAL "NSIS:StartMenuDir"


SetCompressor lzma


; ------ MUI �ִ����涨�� (1.67 �汾���ϼ���) ------
!include "MUI.nsh"

; MUI Ԥ���峣��
!define MUI_ABORTWARNING
!define MUI_ICON "..\ͼ��\favicon.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; ��ӭҳ��
!insertmacro MUI_PAGE_WELCOME
; ���Э��ҳ��
;!insertmacro MUI_PAGE_LICENSE "..\�ļ�\Licence.txt"
; ��װĿ¼ѡ��ҳ��
!insertmacro MUI_PAGE_DIRECTORY
; ��ʼ�˵�����ҳ��
var ICONS_GROUP
!define MUI_STARTMENUPAGE_NODISABLE
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "ACF�ϴ�����"
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "${PRODUCT_UNINST_ROOT_KEY}"
!define MUI_STARTMENUPAGE_REGISTRY_KEY "${PRODUCT_UNINST_KEY}"
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "${PRODUCT_STARTMENU_REGVAL}"
!insertmacro MUI_PAGE_STARTMENU Application $ICONS_GROUP

!define Code "0002"
!define Edition "Pro"
;Air/Pro

; ��װ����ҳ��
!insertmacro MUI_PAGE_INSTFILES
; ��װ���ҳ��
!define MUI_FINISHPAGE_RUN "$INSTDIR\DoubleX.Upload.exe"
!insertmacro MUI_PAGE_FINISH

; ��װж�ع���ҳ��
!insertmacro MUI_UNPAGE_INSTFILES

; ��װ�����������������
!insertmacro MUI_LANGUAGE "SimpChinese"

; ��װԤ�ͷ��ļ�
!insertmacro MUI_RESERVEFILE_INSTALLOPTIONS
; ------ MUI �ִ����涨����� ------


Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "setup\${Code}\${Edition}.exe"
InstallDir "$PROGRAMFILES\ACF�ϴ�����"
InstallDirRegKey HKLM "${PRODUCT_UNINST_KEY}" "UninstallString"
ShowInstDetails show
ShowUnInstDetails show
BrandingText " "
RequestExecutionLevel admin

Section -.NET Framework
  ;����Ƿ�����Ҫ��.NET Framework�汾
  Call GetNetFrameworkVersion
  Pop $R1
  ;${If} $R1 < '2.0.50727'
  ;${If} $R1 < '3.5.30729.4926'
  ;${If} $R1 < '4.0.30319'
	${If} $R1 < '4.5.52747'
    MessageBox MB_YESNO|MB_ICONQUESTION "�����������Ҫ.NET Framework 4.5 ���л����������������ƺ�û�а�װ�˻�����$\r$\n���Ƿ�$\r$\n1.ʹ�ô˰�װ�����������ز���װ.NET Framework 4.5$\r$\n2.ʹ�ðٶ�������QQ/360�ܼ����ذ�װ.NET Framework 4.5 $\r$\n$\r$\n�Ƿ��������ز���װ.NET Framework 4.5,��ȷ�����Ļ��������ӻ�������" IDNO +2
      Call DownloadNetFramework45
  ${ENDIF}
SectionEnd

Section "MainSection" SEC01

  SetOutPath "$INSTDIR"
  SetOverwrite on
  File "..\..\..\..\Release\�ϴ�����\DoubleX.Upload.exe.config"
  File "..\..\..\..\Release\�ϴ�����\DoubleX.Upload.exe"
  SetOutPath "$INSTDIR\Bin"
  File /r "..\..\..\..\Release\�ϴ�����\Bin\*.*"
  SetOutPath "$INSTDIR\Data"
  File /r "..\..\..\..\Release\�ϴ�����\Data\*.*"
  File "file\${Code}\config.xml"
  File "license\${Edition}\license.key"
  SetOutPath "$INSTDIR\x64"
  File /r "..\..\..\..\Release\�ϴ�����\x64\*.*"
  SetOutPath "$INSTDIR\x86"
  File /r "..\..\..\..\Release\�ϴ�����\x86\*.*"
  SetOutPath "$INSTDIR\Help"
  File /r "..\..\..\..\Release\�ϴ�����\Help\*.*"
  

; ������ʼ�˵���ݷ�ʽ
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
  CreateDirectory "$SMPROGRAMS\$ICONS_GROUP"
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\ACF�ϴ�����.lnk" "$INSTDIR\DoubleX.Upload.exe"
  SetOutPath "$INSTDIR"
  CreateShortCut "$DESKTOP\ACF�ϴ�����.lnk" "$INSTDIR\DoubleX.Upload.exe"
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
 *  �����ǰ�װ�����ж�ز���  *
 ******************************/

Section Uninstall
  !insertmacro MUI_STARTMENU_GETFOLDER "Application" $ICONS_GROUP
  Delete "$INSTDIR\${PRODUCT_NAME}.url"
  Delete "$INSTDIR\uninst.exe"
  Delete "$INSTDIR\DoubleX.Upload.exe"
  Delete "$INSTDIR\DoubleX.Upload.exe.config"

  Delete "$SMPROGRAMS\$ICONS_GROUP\Uninstall.lnk"
  Delete "$SMPROGRAMS\$ICONS_GROUP\Website.lnk"
  Delete "$DESKTOP\ACF�ϴ�����.lnk"
  Delete "$SMPROGRAMS\$ICONS_GROUP\ACF�ϴ�����.lnk"

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

#-- ���� NSIS �ű��༭�������� Function ���α�������� Section ����֮���д���Ա��ⰲװ�������δ��Ԥ֪�����⡣--#

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "��ȷʵҪ��ȫ�Ƴ� $(^Name) ���������е������" IDYES +2
  Abort
FunctionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) �ѳɹ��ش����ļ�����Ƴ���"
FunctionEnd


/******************************
 *  .NetFramework �汾���  *
 ******************************/

Function GetNetFrameworkVersion
;��ȡ.Net Framework�汾֧��
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
;���� .NET Framework 4.5
  NSISdl::download /TRANSLATE2 '�������� %s' '��������...' '(ʣ�� 1 ��)' '(ʣ�� 1 ����)' '(ʣ�� 1 Сʱ)' '(ʣ�� %u ��)' '(ʣ�� %u ����)' '(ʣ�� %u Сʱ)' '����ɣ�%skB(%d%%) ��С��%skB �ٶȣ�%u.%01ukB/s' /TIMEOUT=7500 /NOIEPROXY 'http://download.microsoft.com/download/E/2/1/E21644B5-2DF2-47C2-91BD-63C560427900/NDP452-KB2901907-x86-x64-AllOS-ENU.exe' '$TEMP\NDP452-KB2901907-x86-x64-AllOS-ENU.exe'
  Pop $R0
  StrCmp $R0 "success" 0 +2

  SetDetailsPrint textonly
  DetailPrint "���ڰ�װ .NET Framework 4.5.2 ..."
  SetDetailsPrint listonly
  ExecWait '$TEMP\NDP452-KB2901907-x86-x64-AllOS-ENU.exe /quiet /norestart' $R1
  Delete "$TEMP\NDP452-KB2901907-x86-x64-AllOS-ENU.exe"

FunctionEnd

