@echo off
rem �o�b�`�N����������G���R�[�f�B���O���擾
set ENCODING=%1

rem �e��p�X�����擾
set CONVERTER_PATH=..\..\Tool\EncodingConverter\bin
set CONVERTER_EXE=%CONVERTER_PATH%\EncodingConverter.exe
set TARGET_FILE_PATH=%CONVERTER_PATH%\ConvertTargetFileList.csv

rem EncodingConverter�p�̊e��p�����[�^��ݒ�
set OPT_NOGUI=--nogui
set OPT_TARGET_LIST=--targetlist=%TARGET_FILE_PATH%
set OPT_ENCODING=--encoding=%ENCODING%
set OPT_ENCODING_SJIS=--encoding=shift_jis
set OPT_ENCODING_UTF8=--encoding=utf-8
set OPT_BOM=--bom=false
set OPT_NEW_LINE=--newline=CR+LF
set OPT_QUIET=--quiet

rem EncodingConverter�p�̕����R�[�h�ϊ��ݒ���e��p�����[�^��g�ݍ��킹�Đݒ�
set CONVERTER_PARAM=%OPT_NOGUI% %OPT_TARGET_LIST% %OPT_ENCODING% %OPT_BOM% %OPT_NEW_LINE% %OPT_QUIET%
set CONVERTER_PARAM_SJIS=%OPT_NOGUI% %OPT_TARGET_LIST% %OPT_ENCODING_SJIS% %OPT_BOM% %OPT_NEW_LINE% %OPT_QUIET%
set CONVERTER_PARAM_UTF8=%OPT_NOGUI% %OPT_TARGET_LIST% %OPT_ENCODING_UTF8% %OPT_BOM% %OPT_NEW_LINE% %OPT_QUIET%
set CONVERTER_PARAM_NOT_QUIET=%OPT_NOGUI% %OPT_TARGET_LIST% %OPT_ENCODING% %OPT_BOM% %OPT_NEW_LINE%

rem echo �����R�[�h�ϊ��ݒ���m�F
rem echo CONVERTER_EXE        = "%CONVERTER_EXE%"
rem echo CONVERTER_PARAM_SJIS = "%CONVERTER_PARAM_SJIS%"
rem echo CONVERTER_PARAM_UTF8 = "%CONVERTER_PARAM_UTF8%"
rem echo CONVERTER_PARAM_NOT_QUIET = "%CONVERTER_PARAM_NOT_QUIET%"
rem echo �����R�[�h�ϊ����J�n

rem �����R�[�h�ϊ��A�v���̎��s�R�}���h�Q
rem echo %ENCODING%�ւ̕����R�[�h�ϊ� (���o�b�`�t�@�C���N��������ENCODING���w��)
set CONVERT=%CONVERTER_EXE% %CONVERTER_PARAM%

rem echo Shift-JIS�ւ̕����R�[�h�ϊ�
set CONVERT_SJIS=%CONVERTER_EXE% %CONVERTER_PARAM_SJIS%

rem echo UTF-8�ւ̕����R�[�h�ϊ�
set CONVERT_UTF8=%CONVERTER_EXE% %CONVERTER_PARAM_UTF8%

rem echo %ENCODING%�ւ̕����R�[�h�ϊ� (���o�b�`�t�@�C���N��������ENCODING���w��)(�����O�\���p�R���\�[����ʂ��N��)
set CONVERT_NOT_QUIET=%CONVERTER_EXE% %CONVERTER_PARAM_NOT_QUIET%

@echo on


%CONVERT%


@echo off
rem �����R�[�h�ϊ����ʂƂ��Ĉُ�I����͋[�����f�o�b�O�m�F�p�ɁAERRORLEVEL�������I�ɢ1��ɐݒ肷�邱�Ƃ����s
rem set ERRORLEVEL=1

rem �����R�[�h�ϊ����ʃ��b�Z�[�W�\��
if %ERRORLEVEL%==0 (
    echo %ENCODING%�ւ̕����R�[�h�ϊ�������I�����܂����B
) else (
    echo %ENCODING%�ւ̕����R�[�h�ϊ����ُ�I�����܂����B
    echo �����R�[�h�ϊ��Ώۃt�@�C�����X�g�ۑ��t�@�C�� %TARGET_FILE_PATH% �̎w���肪�Ȃ������m�F���Ă��������B
)
@echo on
