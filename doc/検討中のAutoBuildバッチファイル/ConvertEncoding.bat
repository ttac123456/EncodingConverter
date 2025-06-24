@echo off
rem バッチ起動引数からエンコーディングを取得
set ENCODING=%1

rem 各種パス情報を取得
set CONVERTER_PATH=..\..\Tool\EncodingConverter\bin
set CONVERTER_EXE=%CONVERTER_PATH%\EncodingConverter.exe
set TARGET_FILE_PATH=%CONVERTER_PATH%\ConvertTargetFileList.csv

rem EncodingConverter用の各種パラメータを設定
set OPT_NOGUI=--nogui
set OPT_TARGET_LIST=--targetlist=%TARGET_FILE_PATH%
set OPT_ENCODING=--encoding=%ENCODING%
set OPT_ENCODING_SJIS=--encoding=shift_jis
set OPT_ENCODING_UTF8=--encoding=utf-8
set OPT_BOM=--bom=false
set OPT_NEW_LINE=--newline=CR+LF
set OPT_QUIET=--quiet

rem EncodingConverter用の文字コード変換設定を各種パラメータを組み合わせて設定
set CONVERTER_PARAM=%OPT_NOGUI% %OPT_TARGET_LIST% %OPT_ENCODING% %OPT_BOM% %OPT_NEW_LINE% %OPT_QUIET%
set CONVERTER_PARAM_SJIS=%OPT_NOGUI% %OPT_TARGET_LIST% %OPT_ENCODING_SJIS% %OPT_BOM% %OPT_NEW_LINE% %OPT_QUIET%
set CONVERTER_PARAM_UTF8=%OPT_NOGUI% %OPT_TARGET_LIST% %OPT_ENCODING_UTF8% %OPT_BOM% %OPT_NEW_LINE% %OPT_QUIET%
set CONVERTER_PARAM_NOT_QUIET=%OPT_NOGUI% %OPT_TARGET_LIST% %OPT_ENCODING% %OPT_BOM% %OPT_NEW_LINE%

rem echo 文字コード変換設定を確認
rem echo CONVERTER_EXE        = "%CONVERTER_EXE%"
rem echo CONVERTER_PARAM_SJIS = "%CONVERTER_PARAM_SJIS%"
rem echo CONVERTER_PARAM_UTF8 = "%CONVERTER_PARAM_UTF8%"
rem echo CONVERTER_PARAM_NOT_QUIET = "%CONVERTER_PARAM_NOT_QUIET%"
rem echo 文字コード変換を開始

rem 文字コード変換アプリの実行コマンド群
rem echo %ENCODING%への文字コード変換 (※バッチファイル起動引数でENCODINGを指定)
set CONVERT=%CONVERTER_EXE% %CONVERTER_PARAM%

rem echo Shift-JISへの文字コード変換
set CONVERT_SJIS=%CONVERTER_EXE% %CONVERTER_PARAM_SJIS%

rem echo UTF-8への文字コード変換
set CONVERT_UTF8=%CONVERTER_EXE% %CONVERTER_PARAM_UTF8%

rem echo %ENCODING%への文字コード変換 (※バッチファイル起動引数でENCODINGを指定)(※ログ表示用コンソール画面を起動)
set CONVERT_NOT_QUIET=%CONVERTER_EXE% %CONVERTER_PARAM_NOT_QUIET%

@echo on


%CONVERT%


@echo off
rem 文字コード変換結果として異常終了を模擬したデバッグ確認用に、ERRORLEVELを強制的に｢1｣に設定することを試行
rem set ERRORLEVEL=1

rem 文字コード変換結果メッセージ表示
if %ERRORLEVEL%==0 (
    echo %ENCODING%への文字コード変換が正常終了しました。
) else (
    echo %ENCODING%への文字コード変換が異常終了しました。
    echo 文字コード変換対象ファイルリスト保存ファイル %TARGET_FILE_PATH% の指定誤りがないかを確認してください。
)
@echo on
