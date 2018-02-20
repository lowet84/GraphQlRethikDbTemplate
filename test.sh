#!/bin/bash
docker build -t gqltest .
docker rm -f gqltest
docker run -d -it --name gqltest --net=homeserver2 -e DATABASE=rethinkdb gqltest
docker exec -it gqltest sh
