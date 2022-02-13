How To
===============

Access the source code:
  There are 2 packages on GitHub. The source and the exported package that 

To give access to database:
	GRANT ALL PRIVILEGES ON *.* TO 'root'@'%' IDENTIFIED BY 'piepasri';
	FLUSH PRIVILEGES;
	
To set web site open to lan
	http://gilesey.wordpress.com/2013/04/21/allowing-remote-access-to-your-iis-express-service/
	ipad : http://stackoverflow.com/questions/14725455/connecting-to-visual-studio-debugging-iis-express-server-over-the-lan


Setup Info
===============
1. install MySQL 
	A) MYSQL Server 5.6
		user : root
		password piepasri
	B) MySQL WorkBench (optional)
	Make sure to not use SSL with workbench
		windows10, Workbench 8.0.27
		close the MySQL Workbench (important, otherwise it will overwrite the edit)
		in ~\AppData\Roaming\MySQL\Workbench\connections.xml
		set <value type="int" key="useSSL">0</value> and save.




2.Install IIS 
- open control panel
- open program and feature
- click on Turn windows features on or off
- enable all Internet Iinformation Service


3. open IIS manager
- set pool to wineman
- add priviledge to the folder with the wineman pool 

4. set the database:
"C:\Program Files\MySQL\MySQL Server 5.6\bin\mysql"
	GRANT ALL PRIVILEGES ON *.* TO 'root'@'%' IDENTIFIED BY 'piepasri';
	FLUSH PRIVILEGES;

"C:\Program Files\MySQL\MySQL Server 5.6\bin\mysql" < "C:\GitHub\WineManPub\App_Data\dumpdb.sql"
"C:\Program Files\MySQL\MySQL Server 5.6\bin\mysql" < "C:\GitHub\WineManPub\App_Data\version102.sql"

