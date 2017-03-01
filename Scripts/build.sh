#! /bin/sh

project="SingedFeathers"

echo "Attempting to build $project for Webgl"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/SingedFeathers/Unity.log \
  -projectPath=$(pwd)/SingedFeathers \
  -executeMethod Assets.Editor.Build.AutoBuilder.PerformWebBuild \
  -quit

echo 'Logs from build'
cat $(pwd)/SingedFeathers/Unity.log


#echo 'Attempting to zip builds'å
#zip -r $(pwd)/Build/linux.zip $(pwd)/Build/linux/
#zip -r $(pwd)/SingedFeathers/Build/mac.zip . -i $(pwd)/SingedFeathers/Build/osx/
#zip -r $(pwd)/Build/windows.zip $(pwd)/Build/windows/
