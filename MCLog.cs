using System;
using System.Text;
using System.Collections.Generic;
using Oxide.Core;
using Oxide.Core.Database;
using Oxide.Core.MySql;

namespace Oxide.Plugins
{
    [Info("MCLog", "Limmek", "1.1.1"/*, ResourceId = 123*/)]
    [Description("MySql Chat Log")]
    internal class MCLog : RustPlugin
    {
        readonly Core.MySql.Libraries.MySql _mySql = new Core.MySql.Libraries.MySql();
        private Connection _mySqlConnection = null;
        //private readonly Ext.MySql.Libraries.MySql _mySql = Interface.GetMod().GetLibrary<Ext.MySql.Libraries.MySql>();     
        private const string CreateTable = "CREATE TABLE IF NOT EXISTS mclog (id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, time TIMESTAMP NULL DEFAULT NULL, steam_id BIGINT(255), player_name VARCHAR(255), chat_msg TEXT DEFAULT NULL, is_admin INT(2) default '0', player_ip VARCHAR(255));";
        private Dictionary<string, object> dbConnect = null;
        protected override void LoadDefaultConfig() { }
        
        // Execute query
        public void executeQuery(string query, params object[] data) {
            var sql = Sql.Builder.Append(query, data);
            _mySql.Insert(sql, _mySqlConnection);
        }

        static string EncodeNonAsciiCharacters(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    string encodedValue = "";
                    sb.Append(encodedValue);
                }
                else {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private T checkCfg<T>(string conf, T def)
        {
            if (Config[conf] != null)
            {
                return (T)Config[conf];
             }
            else
            {
                Config[conf] = def;
                return def;
            }
        }

        private string getDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void MySqlConnect()
        {
            if (_mySqlConnection == null)
            {   
                Puts("Connecting to MySql databse.");
                _mySqlConnection = _mySql.OpenDb(dbConnect["Host"].ToString(), Convert.ToInt32(dbConnect["Port"]), dbConnect["Database"].ToString(), dbConnect["Username"].ToString(), dbConnect["Password"].ToString(), this);
            }
        }

        private void MySQLCreateTableOnConnect()
        {
            try 
            { 
                MySqlConnect();
                var sql = Core.Database.Sql.Builder.Append(@CreateTable);
                _mySql.Insert(sql, _mySqlConnection);
            }
            catch (Exception ex)
            {   
                Puts(ex.Message);
            }   
        }

        void Loaded() 
        {
            dbConnect = checkCfg<Dictionary<string, object>>("dbConnect", new Dictionary<string, object>{
                {"Database", "db"},
                {"Port", 3306 },
                {"Host", "127.0.0.1"}, 
                {"Username", "username"},
                {"Password", "password"},
            });
            SaveConfig();
            MySQLCreateTableOnConnect();
        }

        void Unloaded()
        {

            _mySql.CloseDb(_mySqlConnection);
            _mySqlConnection = null;

        }

        // Log Chat messages
        void OnPlayerChat(ConsoleSystem.Arg arg)  {   
                BasePlayer player = (BasePlayer)arg.Connection.player;
                var pname = EncodeNonAsciiCharacters(player.displayName);
                var pid = player.userID.ToString();
                var pip = player.net.connection.ipaddress;
                string message = arg.GetString(0);
                if (player.IsAdmin) {
                    // Admin
                    //PrintWarning(pid+" "+pname+" "+pip+" "+message+" 1 "+getDateTime());
                    executeQuery("INSERT INTO mclog (`steam_id`, `player_name`, `chat_msg`, `is_admin`, `time`, `player_ip`) VALUES (@0, @1, @2, @3, @4, @5);",
                                 pid, pname, pip, message,"1",getDateTime());
                }
                else {
                    // Player
                    executeQuery("INSERT INTO server_log_chat (player_id, player_name, player_ip, chat_message, admin, time) VALUES (@0, @1, @2, @3, @4, @5)",
                                 pid, pname, pip, message,"0",getDateTime());
                }               
        }

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////// CONSOLE COMMANDS //////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [ConsoleCommand("mclog.empty")]
        private void EmptyTableCommand(ConsoleSystem.Arg arg) {
            executeQuery("TRUNCATE mclog");
            PrintWarning("Empty table successful!");
        }

        [ConsoleCommand("mclog.drop")]
        private void DropTableCommand(ConsoleSystem.Arg arg) {
            executeQuery("DROP mclog");
            PrintWarning("Drop table successful! Please reload the Plugin!");
        }
    }
}