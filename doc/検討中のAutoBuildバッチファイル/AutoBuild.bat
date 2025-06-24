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
rem 文字コード変換エラー終了時のバッチ終了確認メッセージ表示サブルーチンを開始
if %ERRORLEVEL% neq 0 (
  choice /M "バッチを終了しますか？"
  if errorlevel 2 (
    rem ｢N｣を選択時はサブルーチン呼び出し元に戻ってバッチ継続
    echo 継続します。
  ) else (
    rem ｢Y｣を選択時はバッチ終了
    echo バッチを終了します。
    call :ExitBatch
  )
)
@echo on
goto :eof
