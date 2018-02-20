FROM microsoft/aspnetcore-build:2.0.0
RUN apt-get update && apt-get install -y jq
