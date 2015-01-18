@echo off 
set root=C:\WineMapPub\app_data
cd %root%
mysql -u root  -e "CREATE DATABASE IF NOT EXISTS wineman";
mysql -u root  wineman < *.sql