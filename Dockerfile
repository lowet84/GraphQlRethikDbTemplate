FROM microsoft/dotnet-nightly:2.1-sdk as build
ADD GraphQlRethinkDbCore /app/GraphQlRethinkDbCore
ADD GraphQlRethinkDbHttp /app/GraphQlRethinkDbHttp
ADD SampleAppHttp /app/SampleAppHttp
WORKDIR /app/SampleAppHttp
RUN dotnet restore
RUN dotnet publish --output out/ --configuration Release

FROM microsoft/dotnet-nightly:2.1-runtime-alpine
COPY --from=build /app/SampleAppHttp/out /app
WORKDIR /app
CMD dotnet SampleAppHttp.dll
