# WeatherReport

1. It's an MVC application. 
2. It has UI where we can upload .csv file which includes a list of cities.
3. There is a file 'city.list.json' in the project solution which has a list of cities with details like ID, Country etc.
4. Ids are extracted from this file for the provided cities and passed to an openweathermap API.
5. Received information is stored in files created for each city named as city_cityname,countrycode_today'sdate(in format yyyyMMdd).txt
   E.g. city_Mumbai,IN_20190713.txt
6. These files are getting saved in a folder 'WeatherReport' which is included in the project solution.
