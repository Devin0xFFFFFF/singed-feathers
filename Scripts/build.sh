#! /bin/sh

project="SingedFeathers"

#echo "Attempting to build $project for Windows"
#/Applications/Unity/Unity.app/Contents/MacOS/Unity \
#  -batchmode \
#  -nographics \
#  -silent-crashes \
#  -logFile $(pwd)/unity.log \
#  -projectPath $(pwd) \
#  -buildWindowsPlayer "$(pwd)/Build/windows/$project.exe" \
#  -quit

echo "Attempting to build $project for OS X"
/Applications/Unity/Unity.app/Contents/MacOS/Unity \
  -batchmode \
  -nographics \
  -silent-crashes \
  -logFile $(pwd)/SingedFeathers/Unity.log \
  -projectPath=$(pwd) \
  -buildOSXUniversalPlayer "$(pwd)/SingedFeathers/Build/osx/$project.app" \
  -quit

#echo "Attempting to build $project for Linux"
#/Applications/Unity/Unity.app/Contents/MacOS/Unity \
#  -batchmode \
#  -nographics \
#  -silent-crashes \
#  -logFile $(pwd)/unity.log \
#  -projectPath $(pwd) \
#  -buildLinuxUniversalPlayer "$(pwd)/Build/linux/$project.exe" \
#  -quit

echo 'Logs from build'
cat $(pwd)/SingedFeathers/Unity.log


#echo 'Attempting to zip builds'å
#zip -r $(pwd)/Build/linux.zip $(pwd)/Build/linux/
#zip -r $(pwd)/SingedFeathers/Build/mac.zip . -i $(pwd)/SingedFeathers/Build/osx/
#zip -r $(pwd)/Build/windows.zip $(pwd)/Build/windows/
