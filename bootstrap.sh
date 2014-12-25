#/bin/bash
set -e

git clone git@github.com:NixOS/patchelf.git
cd patchelf
./bootstrap.sh && ./configure && make
cd ../

git clone git@github.com:facebook/proxygen.git
sed -i 's/^make check$//' proxygen/proxygen/deps.sh
cd proxygen
cat ../patches/proxygen/01_mono-stack-frame.patch | patch -p1
cd proxygen
./deps.sh
