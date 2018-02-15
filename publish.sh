#!/bin/bash

cd $1
apt-get update && apt-get install -y jq
VERSION=$(cat GraphQlRethinkDbCore.csproj | grep PackageVersion | sed -n -r -e 's/.*?>(.*?)<.*/\1/p')
NUGET_VERSION=$(curl https://api-v2v3search-0.nuget.org/query?q=GraphQlRethinkDbCore | jq '.data[0].versions[-1].version' --raw-output)
echo $VERSION
echo $NUGET_VERSION
if [ 1 = 1 ]
then
  echo "Same version, exiting build"
  exit 0
fi
echo New version detected. Building and publishing
