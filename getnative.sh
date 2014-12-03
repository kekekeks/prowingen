#!/bin/sh
find proxygen/|grep '\.so\.'|grep -v 0.0$|while read lib; do cp $lib native/bin/Debug; done;
for i in `find native/bin/Debug|grep '\.so'`
do
	patchelf/src/patchelf --set-rpath '$ORIGIN' $i
done

