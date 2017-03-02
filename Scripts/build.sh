#! /bin/sh

project="SingedFeathers"

cd SingedFeathers

echo "Attempting to build $project for Webgl"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -logFile $(pwd)/Unity.log \
  -projectPath=$(pwd)/SingedFeathers \
  -executeMethod Assets.Editor.Build.AutoBuilder.PerformWebBuild \
  -quit

cd ..

echo 'Logs from build'
cat $(pwd)/SingedFeathers/Unity.log


