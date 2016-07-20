using System;
using System.Text;
using System.Collections.Generic;
using Oxide.Core;
using Oxide.Ext.MySql;

namespace Oxide.Plugins
{
    [Info("MCLog", "Limmek", "1.0.0")]
    [Description("MySql Chat Log")]
    internal class MCLog : RustPlugin
    {
        private readonly Ext.MySql.Libraries.MySql _mySql = Interface.GetMod().GetLibrary<Ext.MySql.Libraries.MySql>();
        private const string EmptyTable = "TRUNCATE mclog";
        private const string DropTable = "DROP TABLE mclog";         
        private const string InsertData = "INSERT INTO mclog (`steam_id`, `player_name`, `chat_msg`, `is_admin`, `time`, `player_ip`) VALUES (@0, @1, @2, @3, @4, @5);";
        private const string CreateTable = "CREATE TABLE IF NOT EXISTS mclog (id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, time TIMESTAMP NULL DEFAULT NULL, steam_id BIGINT(255), player_name VARCHAR(255), chat_msg TEXT DEFAULT NULL, is_admin INT(2) default '0', player_ip VARCHAR(255));";
        private Core.Database.Connection _mySqlConnection = null;
        private Dictionary<string, object> dbConnect = null;
        protected override void LoadDefaultConfig() { }

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
            try { 
                MySqlConnect();
                var sql = Core.Database.Sql.Builder.Append(@CreateTable);
                _mySql.Insert(sql, _mySqlConnection);
            }
            catch (Exception ex)
            {   

                return;

            }   
        }

        void Loaded() 
        {
            dbConnect = checkCfg<Dictionary<string, object>>("dbConnect", new Dictionary<string, object>{
                {"Database", "db"},
                {"Port", 3306 },
                {"Host", "127.0.0.1"}, 
                {"Username", "user"},
                {"Password", "password"},
            });
            SaveConfig();
            MySQLCreateTableOnConnect();
        }

        void Unloaded()
        {
            timer.Once(5, () =>
            {
                _mySql.CloseDb(_mySqlConnection);
                _mySqlConnection = null;
            });
        }

        void OnPlayerChat(ConsoleSystem.Arg arg) 
        {   
            BasePlayer player = (BasePlayer)arg.connection.player;
            string message = arg.GetString(0);
            MySqlConnect();
            if (hasPermission(player, "mod"))
            {
                var sql = Core.Database.Sql.Builder.Append(@InsertData, player.userID, EncodeNonAsciiCharacters(player.displayName), message, 1, getDateTime(), player.net.connection.ipaddress);
                _mySql.Insert(sql, _mySqlConnection);
            }else 
            {
                var sql = Core.Database.Sql.Builder.Append(@InsertData, player.userID, EncodeNonAsciiCharacters(player.displayName), message, 0, getDateTime(), player.net.connection.ipaddress);
                _mySql.Insert(sql, _mySqlConnection);
            }  
        }
        bool hasPermission(BasePlayer player, string permissionName)
        {
            if (player.net.connection.authLevel > 1) return true;
                return permission.UserHasPermission(player.userID.ToString(), permissionName);
        }

        [ConsoleCommand("mclog.empty")]
        private void EmptyTableCommand(ConsoleSystem.Arg arg)
        {
            MySqlConnect();
            var sql = Core.Database.Sql.Builder.Append(@EmptyTable);
            _mySql.ExecuteNonQuery(sql, _mySqlConnection);
            PrintWarning("Empty table successful!");
        }

        [ConsoleCommand("mclog.drop")]
        private void DropTableCommand(ConsoleSystem.Arg arg)
        {
            MySqlConnect();
            var sql = Core.Database.Sql.Builder.Append(@DropTable);
            _mySql.ExecuteNonQuery(sql, _mySqlConnection);
            PrintWarning("Drop table successful! Please reload the Plugin!");
        }
    }
}