#! /bin/sh

BASE_URL=https://s3-us-west-2.amazonaws.com/singedfeathers-unity-builds/
HASH=88d00a7498cd
VERSION=5.5.1f1

download() {
  file=$1
  url="$BASE_URL/$HASH/$package"

  echo "Downloading from $url: "
  curl -o `basename "$package"` "$url"
}

install() {
  package=$1
  download "$package"

  echo "Installing "`basename "$package"`
  sudo installer -dumplog -package `basename "$package"` -target /
}

# See $BASE_URL/$HASH/unity-$VERSION-$PLATFORM.ini for complete list
# of available packages, where PLATFORM is `osx` or `win`

install "MacEditorInstaller/Unity-$VERSION.pkg"
install "MacEditorTargetInstaller/UnitySetup-WebGL-Support-for-Editor-$VERSION.pkg"

# Source: http://blog.stablekernel.com/continuous-integration-for-unity-5-using-travisci
