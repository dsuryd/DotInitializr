@echo off
if "%1"=="local" goto :local
goto :heroku

:local
echo --- Remove any existing image
docker rmi dotinitializr -f

echo --- Build a new image
docker build -t dotinitializr -f ./Dockerfile . --build-arg aspnetenv=Production

echo --- Remove build images
docker image prune -f --filter label=stage=build
rd __tmp__ /q /s

echo --- Run a container on port 8090
docker run -it --rm -p:8090:80 --name dotinitializr_8090 dotinitializr

goto :end

:heroku
rem heroku login
call heroku container:login
call heroku container:push web -a dotInitializr
call heroku container:release web -a dotInitializr

:end