@echo off
echo MOGRIFY CURRENT DIRECTORY:
@echo on
magick mogrify -trim +repage *.png
@echo off
echo:
echo MOGRIFY SUBDIRECTORIES:
@echo on
FOR /D /R %%A IN (*) DO magick mogrify -trim +repage "%%A\*.png"
@echo off
echo:
echo SCRIPT COMPLETE
pause