#!/bin/bash
docker build -t gqltest .
docker rm -f gqltest
docker run -d -it --name gqltest --net=homeserver2 -e DATABASE=rethinkdb gqltest
sleep 3
docker exec -it gqltest curl http://localhost:3000 -v
docker logs gqltest
