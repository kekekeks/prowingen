#!/bin/sh
set -e
mkdir -p native/bin
rm -rf build
mkdir build
cd build
cmake ../native
make
cd ../
find proxygen/|grep '\.so\.'|grep -v 0.0$|while read lib; do cp $lib native/bin; done;
for i in `find native/bin|grep '\.so'`
do
	patchelf/src/patchelf --set-rpath '$ORIGIN' $i
done

