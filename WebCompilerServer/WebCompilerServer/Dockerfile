FROM mcr.microsoft.com/dotnet/sdk:6.0

LABEL author="jimmybarnes"

RUN mkdir -p /usr/local/WebCompiler
RUN mkdir -p /usr/local/WebCompiler/Stage
RUN mkdir -p /usr/local/WebCompiler/Build
RUN mkdir -p /usr/local/WebCompiler/Templates
RUN mkdir -p /usr/local/WebCompiler/Projects
RUN mkdir -p /usr/local/WebCompiler/Data
RUN mkdir -p /bin

WORKDIR /bin

ADD bin/Publish .

ADD _templates_ /usr/local/WebCompiler/Templates

CMD ["dotnet", "WebCompilerServer.dll"]
#CMD ["/bin/bash"]