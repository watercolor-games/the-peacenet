#!/bin/bash -e

git pull
./updatepackages.sh
for dir in peace-engine .
do
	pushd $dir
	git submodule init
	git submodule update
	xbuild
	popd
done

