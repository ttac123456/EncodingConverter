setlocal

rem SKIP: CALL ConvertEncoding.bat shift-jis
rem SKIP: call :CheckConvertError

CALL DeleteH.bat

CALL Build.bat

pause

CALL ConvertEncoding.bat utf-8
call :CheckConvertError

CALL CopyH.bat

pause

:ExitBatch
endlocal
exit

:CheckConvertError
@echo off
rem �����R�[�h�ϊ��G���[�I�����̃o�b�`�I���m�F���b�Z�[�W�\���T�u���[�`�����J�n
if %ERRORLEVEL% neq 0 (
  choice /M "�o�b�`���I�����܂����H"
  if errorlevel 2 (
    rem �N���I�����̓T�u���[�`���Ăяo�����ɖ߂��ăo�b�`�p��
    echo �p�����܂��B
  ) else (
    rem �Y���I�����̓o�b�`�I��
    echo �o�b�`���I�����܂��B
    call :ExitBatch
  )
)
@echo on
goto :eof
