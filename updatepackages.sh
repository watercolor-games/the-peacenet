#!/bin/sh -e
# Download nuget packages on linux

mkdir -p packages
cd packages
for fname in $(find .. -iname "packages.config")
do
	nuget install "$fname"
done
