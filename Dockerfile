FROM debian:stretch-slim

RUN apt-get update
RUN apt-get install -y wget gpg apt-transport-https
RUN wget -qO- https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.asc.gpg
RUN mv microsoft.asc.gpg /etc/apt/trusted.gpg.d/
RUN wget -q https://packages.microsoft.com/config/debian/9/prod.list
RUN mv prod.list /etc/apt/sources.list.d/microsoft-prod.list
RUN apt-get update
RUN apt-get update
RUN apt-get install -y dotnet-sdk-3.1

RUN mkdir /opt/test-task
WORKDIR /opt/test-task

COPY TestTask/bin/Release/netcoreapp3.1/linux-x64 .
EXPOSE 3000
CMD ./TestTask
