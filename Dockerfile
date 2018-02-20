FROM microsoft/dotnet-nightly:2.1-sdk as build
ADD GraphQlRethinkDbCore /app/GraphQlRethinkDbCore
ADD GraphQlRethinkDbHttp /app/GraphQlRethinkDbHttp
ADD SampleAppHttp /app/SampleAppHttp
WORKDIR /app/SampleAppHttp
RUN sed -i 's/netcoreapp2.0/netcoreapp2.1/g' SampleAppHttp.csproj
RUN dotnet restore
RUN dotnet publish --output out/ --configuration Release

FROM microsoft/dotnet-nightly:2.1-runtime-alpine
RUN apk add --no-cache curl
COPY --from=build /app/SampleAppHttp/out /app
ADD SampleAppHttp/static /app/static
WORKDIR /app
CMD dotnet SampleAppHttp.dll
