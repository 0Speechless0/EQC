@echo off

mkdir ..\output_folder;

echo "�ؿ����b�إߤ��A�еy��..."
::xcopy .\ ..\output_folder /T /E


for /f "delims=" %%i in ('git diff-tree -r --no-commit-id --name-only --diff-filter=ACMRT HEAD~%1 HEAD~%2 ') do  @cp  --parents "%%i" "..\output_folder" 


rm ..\change.txt
git diff --name-status HEAD~%1 HEAD~%2 >> ..\change.txt


echo "����..�A�ɮ׿�X�P�M�ץؿ��P�h�Aoutput_folder�O�ܧ��ɮסAchange.txt�O�ܧ����"