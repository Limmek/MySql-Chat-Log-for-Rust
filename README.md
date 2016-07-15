# MySql Chat Log for Rust
Rust plugin and Web template to view real time chat log.


This plugin logs all chat messages sent by players to a MySql Database.
Database is created on connection if not exist.


###How to use and install the plugin:
------
1. Download and move MCLog.cs inside plugin folder.
2. Edit MCLog.json in config folder.
3. Reload plugin so MySql connection opened.

###How to use and install the web template:
------
1. Download the latest web source.
2. Open up your web folder and move the files.
3. Edit the config.php.


###Console Commands
1. **mclog.empty** - Truncate table. Clear table from data.
2. **mclog.drop** - Drops table. Removes table from database.
After Drop Table plugin must be reloaded for a new table to be created!


###Table structure
| ID |     time    |      steam_id     | player_name | chat_msg | is_admin | player_ip |
|----|-------------|-------------------|-------------|----------|----------|-----------|
| 1  | y-m-d h.m.s | 76561198047099745 |    Limmek   | Hello :) |     1    | 127.0.0.1 |


##Web Template.
![IMAGE ALT TEXT HERE](https://raw.githubusercontent.com/Limmek/MySql-Chat-Log-for-Rust/master/template_image.jpg)

Live demo (http://rust.swedon.nu/chat)


###TO-DO List:
..


------
If you have any problems with the plugin please leave a comment or send me a message!
Keep in mind this is my first plugin and first time coding in CSharp!
