@echo off

mkdir ..\output_folder;

echo "目錄正在建立中，請稍後..."
::xcopy .\ ..\output_folder /T /E


for /f "delims=" %%i in ('git diff-tree -r --no-commit-id --name-only --diff-filter=ACMRT HEAD~%1 HEAD~%2 ') do  @cp  --parents "%%i" "..\output_folder" 


rm ..\change.txt
git diff --name-status HEAD~%1 HEAD~%2 >> ..\change.txt


echo "完成..，檔案輸出與專案目錄同層，output_folder是變更檔案，change.txt是變更明細"