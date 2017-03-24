#! /bin/sh

project="SingedFeathers"

cd SingedFeathers

echo "Attempting to generate project files"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -logFile $(pwd)/../Travis/Unity.log \
  -projectPath=$(pwd)/SingedFeathers \
  -executeMethod Assets.Editor.Build.AutoBuilder.GenerateProjects \
  -quit

cd ..

echo 'Logs from project generation'
cat $(pwd)/Travis/Unity.log


