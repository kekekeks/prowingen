#/bin/bash
set -e

git clone git@github.com:NixOS/patchelf.git
cd patchelf
./bootstrap.sh && ./configure && make
cd ../

git clone git@github.com:facebook/proxygen.git
cd proxygen/proxygen
./deps.sh
