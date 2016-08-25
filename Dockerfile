FROM microsoft/dotnet:1.0.0-preview2-sdk
RUN mkdir -p /usr/src/app
COPY src/GithubReleaseHook /usr/src/app/
RUN cd /usr/src/app && dotnet restore
WORKDIR /usr/src/app
EXPOSE 8081
CMD dotnet run