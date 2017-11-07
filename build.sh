#!/bin/bash
# assumes 64 bit for now
target=${1:-Debug}
xbuild -property:Configuration=$target
pushd PlexNative
make
popd
cp PlexNative/x64/* ShiftOS.Frontend/bin/DesktopGL/AnyCPU/$target
