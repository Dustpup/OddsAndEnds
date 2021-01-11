 /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *                             
 *  ///////////////////////////////////////////////////////////////////////////////////  *
 *  //--███████--███████---███████--██---██--███████----------████████--████████-----//  *
 *  //-███------██-----██-██-----██-██--██--██-------------------██--------██--------//  * 
 *  //-██-------██-----██-██-----██-████---- ██████ -------------██--------██--------//  * 
 *  //-███------██-----██-██-----██-██--██--------██-------------██--------██--------//  *
 *  //--███████--███████---███████--██---██- ██████-----██----████████-----██--------//  *
 *  //===============================================================================//  *
 *  ///////////////////////////////////////////////////////////////////////////////////  *
 *  // Script: CMySqlManager.cs                                                      //  *
 *  // Author: Brandon Cook                                                          //  *
 *  //                                                                               //  *
 *  ///////////////////////////////////////////////////////////////////////////////////  *
 * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */
using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ProjectTrack
{
    internal struct MySQLVar
    {
        public string VaribleName;
        public bool ContainsLength;
        public int Length;
        public MySqlDbType Type;
        public string Extra; 
    }
    struct MySqlParam
    {
        public string VarName;
        public string LengthName;
        public string TypeName;
        public string ExtraName;

    }

    internal partial class CMySqlManager
    {
        
        const int DataUpdateRateSec = 2;
        const int MaxQueuedTask     = 3;
        private MySqlConnection mySQLConnection;
        private int taskQueued = 0;

        private bool isConnected { get; set; }
        private bool isBusy      { get; set; }


        /// <summary>
        /// Creates New Tables in a My SQL Database
        /// </summary>
        /// <param name="newTableName"> Name of table</param>
        /// <param name="newTabVars"> Columns of table</param>
        /// <returns></returns>
        public async Task CreateTable(string newTableName,params MySQLVar[] newTabVars)
        {
            await Task.Run(async () =>
            {
                taskQueued++;               // Update Thread Count
                int inline = taskQueued;    // Thread Positon

                string incommingTable;
                string incommingVars = "";

                List<MySQLVar>   Sqlvars  = new List<MySQLVar>();
                List<MySqlParam> Sqlparam = new List<MySqlParam>();

                MySqlCommand mySQLCommand;
                MySqlConnectionStringBuilder mysqlStrBuilder;

                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Check if the current thread is busy.//////////////////////////////////////////////////////////////                                                            
                while (isBusy)
                {
#if DEBUG && CONSOLE  // Stay silent when in release Mode                                     
                         Console.WriteLine(inline + ": " + "Waiting For Other thread to Complete");               
#endif
                    await Task.Delay((int)TimeSpan.FromSeconds(DataUpdateRateSec * inline).TotalMilliseconds);
                }
                if (mySQLConnection != null)
                    if (mySQLConnection.State != System.Data.ConnectionState.Closed)
                    {
                        mySQLConnection.Close();
                        isBusy = true;
                    }
                //////////////////////////////////////////////////////////////////////////////////////////////////////

                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Build Connection to SQL Server /////////////////////////////////////////////////////////////////// 

                mysqlStrBuilder          = new MySqlConnectionStringBuilder(); // Build string file
                mysqlStrBuilder.Server   = CSettingsManager.MYSQLServerAddress;
                mysqlStrBuilder.Database = CSettingsManager.MYSQLDatabase;
                mysqlStrBuilder.UserID   = CSettingsManager.MYSQLUser;
                mysqlStrBuilder.Password = CSettingsManager.MYSQLPassword;
                mysqlStrBuilder.Port     = Convert.ToUInt16(CSettingsManager.MYSQLPort);
                MySqlSslMode mode;
                Enum.TryParse(CSettingsManager.MYSQLSsl, out mode);
                mysqlStrBuilder.SslMode  = mode;


                mySQLConnection = new MySqlConnection(mysqlStrBuilder.ToString());
                mysqlStrBuilder = null;

                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Setup Vars /////////////////////////////////////////////////////////////////////////////////////// 

#region SETUP VARS
                // Cleaned Up By Mark in Source Engine Discord. 
                // I did not hit her, I did Not! Oh hi Mark!w

                StringBuilder varBuild = new StringBuilder();
                foreach(MySQLVar sqlV in newTabVars)
                {
                    varBuild.AppendFormat("{0} {1}", sqlV.VaribleName, sqlV.Type);

                    if (sqlV.ContainsLength)
                        varBuild.AppendFormat("({0})", sqlV.Length);
                    if (!String.IsNullOrEmpty(sqlV.Extra))
                        varBuild.AppendFormat(" {0}", sqlV.Extra);

                    varBuild.Append(',');
                }

                incommingVars = varBuild.ToString().TrimEnd(',');


                incommingTable = String.Format("CREATE TABLE IF NOT EXISTS {1} ( id INT AUTO_INCREMENT PRIMARY KEY, {0});", incommingVars,newTableName);

#if DEBUG && CONSOLE
                Console.WriteLine(incommingTable);
#endif
#endregion

                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Setup Params ///////////////////////////////////////////////////////////////////////////////////// 
                mySQLCommand = new MySqlCommand(incommingTable, mySQLConnection);

                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Start SQL STUFF ////////////////////////////////////////////////////////////////////////////////// 
                try
                {

                    mySQLConnection.Open();
                    mySQLCommand.ExecuteNonQuery();
                    isConnected = true;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    mySQLCommand.Connection.Close(); // Try to close the connection
                    isConnected = false;
                    isBusy = false;
                    taskQueued--;
                }

            });

        }
        /// <summary>
        /// Removes the table from the database
        /// </summary>
        /// <param name="tableName">table to remove.</param>
        /// <returns></returns>
        public async Task DropTable(string tableName)
        {
            await Task.Run(async () =>
            {
                taskQueued++;             // Update Thread Count
                int inline = taskQueued;  // Thread Positon

                string outgoingTable;

                List<MySQLVar> Sqlvars = new List<MySQLVar>();
                List<MySqlParam> Sqlparam = new List<MySqlParam>();

                MySqlCommand mySQLCommand;
                MySqlConnectionStringBuilder mysqlStrBuilder;

                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Check if the current thread is busy.//////////////////////////////////////////////////////////////                                                            
                while (isBusy)
                {
#if DEBUG && CONSOLE // Stay silent when in release Mode                                     
                         Console.WriteLine(inline + ": " + "Waiting For Other thread to Complete");               
#endif
                    await Task.Delay((int)TimeSpan.FromSeconds(DataUpdateRateSec * inline).TotalMilliseconds);
                }
                if (mySQLConnection != null)
                    if (mySQLConnection.State != System.Data.ConnectionState.Closed)
                    {
                        mySQLConnection.Close();
                        isBusy = true;
                    }
                //////////////////////////////////////////////////////////////////////////////////////////////////////

                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Build Connection to SQL Server /////////////////////////////////////////////////////////////////// 

                mysqlStrBuilder = new MySqlConnectionStringBuilder(); // Build string file
                mysqlStrBuilder.Server = CSettingsManager.MYSQLServerAddress;
                mysqlStrBuilder.Database = CSettingsManager.MYSQLDatabase;
                mysqlStrBuilder.UserID = CSettingsManager.MYSQLUser;
                mysqlStrBuilder.Password = CSettingsManager.MYSQLPassword;
                mysqlStrBuilder.Port = Convert.ToUInt16(CSettingsManager.MYSQLPort);
                MySqlSslMode mode;
                Enum.TryParse(CSettingsManager.MYSQLSsl, out mode);
                mysqlStrBuilder.SslMode = mode;


                mySQLConnection = new MySqlConnection(mysqlStrBuilder.ToString());
                mysqlStrBuilder = null;

          

                outgoingTable = String.Format("DROP TABLE IF EXISTS {0};", tableName);
                /////////////////////////////////////////////////////////////////////////////////////////////////////
#if DEBUG && CONSOLE
                Console.WriteLine(incommingTable);
#endif
                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Command Exicution //////////////////////////////////////////////////////////////////////////////// 
                mySQLCommand = new MySqlCommand(outgoingTable, mySQLConnection);

                try
                {
                    mySQLConnection.Open();
                    mySQLCommand.ExecuteNonQuery();
                    isConnected = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    mySQLCommand.Connection.Close(); // Try to close the connection
                    isConnected = false;
                    isBusy = false;
                    taskQueued--;
                }

            });
        }
        /// <summary>
        /// Creates a table based off of a structure object. Only works with paticular structures.
        /// </summary>
        /// <typeparam name="T">The type of object being created in the database.</typeparam>
        /// <param name="tableName">The name of the table.</param>
        public async void CreateTableObject<T>(string tableName)
        {
            Type mainType = typeof(T);
            FieldInfo[] p = mainType.GetFields();
            MySQLVar[] tableVars = null;

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Limit Checks ///////////////////////////////////////////////////////////////////////////////////// 
            Type[] limitArry = new Type[5] {typeof(BillTo),
                                            typeof(Customer),
                                            typeof(DocumentLayout),
                                            typeof(ShipInfo),
                                            typeof(Order)};

            Predicate<Type> matching = new Predicate<Type>(
                (Type typeIn) =>
                {
                    return (typeIn == mainType);
                });
            if (!Array.Exists(limitArry, matching))
                throw new ApplicationException("CRITICAL: Unhandeled Type When Creating A New Database Object!");

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // Build Order vars ///////////////////////////////////////////////////////////////////////////////// 

            tableVars = BuildSqlVarArray<T>();

            await CreateTable(tableName, tableVars);
        }
        /// <summary>
        /// Deep Selection of a single item.
        /// </summary>
        /// <typeparam name="T">The structure that the table uses to name columns</typeparam>
        /// <param name="tableName">The table name that the objec uses</param>
        /// <param name="id">Unique ID that the row uses</param>
        /// <returns></returns>
        public async Task<Dictionary<string,object>> SelectRow<T>(string tableName,string id)
        {
            return await Task.Run(async () =>
            {
                taskQueued++;                                 // Update Thread Count
                int inline                     = taskQueued;  // Thread Positon

                string incomingRow;
                string selectString            = null;

                MySqlCommand mySQLCommand;
                MySqlConnectionStringBuilder mysqlStrBuilder;

                StringBuilder selectVarsString = new StringBuilder();
                MySQLVar[] typeVars            = BuildSqlVarArray<T>();
                Dictionary<string, object> rturnDic = new Dictionary<string, object>();

                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Check if the current thread is busy.//////////////////////////////////////////////////////////////                                                            
                while (isBusy)
                    await Task.Delay((int)TimeSpan.FromSeconds(DataUpdateRateSec * inline).TotalMilliseconds);

                if (mySQLConnection != null)
                    if (mySQLConnection.State != System.Data.ConnectionState.Closed)
                    {
                        mySQLConnection.Close();
                        isBusy = true;
                    }

                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Build Connection to SQL Server /////////////////////////////////////////////////////////////////// 

                #region BUILD CONNECTION -----
                mysqlStrBuilder                 = new MySqlConnectionStringBuilder(); // Build string file
                mysqlStrBuilder.Server          = CSettingsManager.MYSQLServerAddress;
                mysqlStrBuilder.Database        = CSettingsManager.MYSQLDatabase;
                mysqlStrBuilder.UserID          = CSettingsManager.MYSQLUser;
                mysqlStrBuilder.Password        = CSettingsManager.MYSQLPassword;
                mysqlStrBuilder.Port            = Convert.ToUInt16(CSettingsManager.MYSQLPort);
                MySqlSslMode mode;
                Enum.TryParse(CSettingsManager.MYSQLSsl, out mode);
                mysqlStrBuilder.SslMode         = mode;

                mySQLConnection                 = new MySqlConnection(mysqlStrBuilder.ToString());
                mysqlStrBuilder                 = null;
                #endregion

                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Build Select string ////////////////////////////////////////////////////////////////////////////// 

                #region SELECT STRING    -----
                foreach (MySQLVar var in typeVars)
                    selectVarsString.AppendFormat(" \'{0}\',", var.VaribleName);

                // TODO ADD PARAMETER SYSTEM... Oct 31 2018 ~ Brandon

                selectString = selectVarsString.ToString();
                selectString.TrimEnd(',');

                incomingRow = String.Format("SELECT {0} FROM {1} WHERE id=@id;", selectString,tableName);

                #endregion

                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Command Execution //////////////////////////////////////////////////////////////////////////////// 

                mySQLCommand = new MySqlCommand(incomingRow, mySQLConnection);
                mySQLCommand.Parameters.AddWithValue("id", id);

                // TODO ADD PARAMETER SYSTEM... Oct 31 2018 ~ Brandon

                try
                {
                    mySQLConnection.Open();
                    isConnected = true;
                    MySqlDataReader reader = mySQLCommand.ExecuteReader();

                    /////////////////////////////////////////////////////////////////////////////////////////////////
                    // Pull Object Type  //////////////////////////////////////////////////////////////////////////// 

                    while (reader.Read())
                        for (int i = 0; i < reader.FieldCount; i++)
                            rturnDic.Add(reader.GetName(i), reader.GetValue(i));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
                finally
                {
                    mySQLCommand.Connection.Close(); // Try to close the connection
                    isConnected = false;
                    isBusy = false;
                    taskQueued--;
                }
                return rturnDic;
            });
        }

        public async Task<object[]> SelectRow(string tableName,string id,string[] varnames)
        {
            return await Task.Run(async ()=> {

                List<object> data = new List<object>();
                /////////////////////////////////////////////////////////////////////////////
                
                taskQueued++;                                 // Update Thread Count
                int inline = taskQueued;  // Thread Positon

                MySqlCommand mySQLCommand;
                MySqlConnectionStringBuilder mysqlStrBuilder;

                StringBuilder selectVarsString = new StringBuilder();

                string selectCommand;
                string selectString;
                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Check if the current thread is abusy.//////////////////////////////////////////////////////////////                                                            
                while (isBusy) await Task.Delay((int)TimeSpan.FromSeconds(DataUpdateRateSec * inline).TotalMilliseconds);

                if (mySQLConnection != null)
                    if (mySQLConnection.State != System.Data.ConnectionState.Closed)
                    {
                        mySQLConnection.Close();
                        isBusy = true;
                    }

                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Build Connection to SQL Server /////////////////////////////////////////////////////////////////// 

                #region BUILD CONNECTION -----
                mysqlStrBuilder = new MySqlConnectionStringBuilder(); // Build string file
                mysqlStrBuilder.Server = CSettingsManager.MYSQLServerAddress;
                mysqlStrBuilder.Database = CSettingsManager.MYSQLDatabase;
                mysqlStrBuilder.UserID = CSettingsManager.MYSQLUser;
                mysqlStrBuilder.Password = CSettingsManager.MYSQLPassword;
                mysqlStrBuilder.Port = Convert.ToUInt16(CSettingsManager.MYSQLPort);
                MySqlSslMode mode;
                Enum.TryParse(CSettingsManager.MYSQLSsl, out mode);
                mysqlStrBuilder.SslMode = mode;

                mySQLConnection = new MySqlConnection(mysqlStrBuilder.ToString());
                mysqlStrBuilder = null;
                #endregion

                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Command Execution //////////////////////////////////////////////////////////////////////////////// 
                foreach (string var in varnames)
                    selectVarsString = selectVarsString.AppendFormat(" \'{0}\',", var);

                // TODO ADD PARAMETER SYSTEM... Oct 31 2018 ~ Brandon

                selectString = selectVarsString.ToString();
                selectString.TrimEnd(',');

                selectCommand = String.Format("SELECT {0} FROM {1} WHERE id=@id;", selectString,tableName);

                mySQLCommand = new MySqlCommand(selectCommand, mySQLConnection);
                mySQLCommand.Parameters.AddWithValue("id", id);

                // TODO ADD PARAMETER SYSTEM... Oct 31 2018 ~ Brandon
                
                try
                {
                    mySQLConnection.Open();
                    isConnected = true;
                    MySqlDataReader reader = mySQLCommand.ExecuteReader();
                   
                    /////////////////////////////////////////////////////////////////////////////////////////////////
                    // Pull Object Type  //////////////////////////////////////////////////////////////////////////// 

                    while (reader.Read())
                        for (int i = 0; i < reader.FieldCount; i++)
                           data.Add(reader.GetValue(i));

                }
                catch (Exception ex)
                {  
                    Console.WriteLine(ex);
                    return null;
                }
                finally
                {
                    mySQLCommand.Connection.Close(); // Try to close the connection
                    isConnected = false;
                    isBusy = false;
                    taskQueued--;
                }

                /////////////////////////////////////////////////////////////////////////////
                return data.ToArray();
            });
        }
        public async Task InsertRow(string tableName,string[] varnames,string[] values)
        {
            await Task.Run(async () =>
            {
                taskQueued++;             // Update Thread Count
                int inline = taskQueued;  // Thread Positon

                string rowToInsert;
                string comValues = null;
                StringBuilder valueStrBldr = new StringBuilder();

                List<MySQLVar> Sqlvars = new List<MySQLVar>();
                List<MySqlParam> Sqlparam = new List<MySqlParam>();

                MySqlCommand mySQLCommand;
                MySqlConnectionStringBuilder mysqlStrBuilder;

                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Check if the current thread is busy.//////////////////////////////////////////////////////////////                                                            
                while (isBusy)   await Task.Delay((int)TimeSpan.FromSeconds(DataUpdateRateSec * inline).TotalMilliseconds);

                if (mySQLConnection != null)
                    if (mySQLConnection.State != System.Data.ConnectionState.Closed)
                    {
                        mySQLConnection.Close();
                        isBusy = true;
                    }

                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Build Connection to SQL Server /////////////////////////////////////////////////////////////////// 

                mysqlStrBuilder          = new MySqlConnectionStringBuilder(); // Build string file
                mysqlStrBuilder.Server   = CSettingsManager.MYSQLServerAddress;
                mysqlStrBuilder.Database = CSettingsManager.MYSQLDatabase;
                mysqlStrBuilder.UserID   = CSettingsManager.MYSQLUser;
                mysqlStrBuilder.Password = CSettingsManager.MYSQLPassword;
                mysqlStrBuilder.Port     = Convert.ToUInt16(CSettingsManager.MYSQLPort);
                MySqlSslMode mode;
                Enum.TryParse(CSettingsManager.MYSQLSsl, out mode);
                mysqlStrBuilder.SslMode = mode;

                mySQLConnection = new MySqlConnection(mysqlStrBuilder.ToString());
                mysqlStrBuilder = null;
                

                foreach (string var in values)
                    valueStrBldr = valueStrBldr.AppendFormat(" \'{0}\',", var);
                comValues = valueStrBldr.ToString().Trim(',');
                
                rowToInsert = String.Format("INSERT INTO {0}({1}) VALUES({2});", tableName,String.Join(',', varnames), comValues);


                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Command Exicution //////////////////////////////////////////////////////////////////////////////// 

                mySQLCommand = new MySqlCommand(rowToInsert, mySQLConnection);

                try
                {
                    mySQLConnection.Open();
                    isConnected = true;
                    mySQLCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    mySQLCommand.Connection.Close(); //   Try to close the connection
                    isConnected = false;
                    isBusy = false;
                    taskQueued--;
                }

            });
        }

        public async Task DropRow(string tableName,string id)
        {
            await Task.Run(async () =>
            {
                taskQueued++;             // Update Thread Count
                int inline = taskQueued;  // Thread Positon

                string rowToInsert;
                string comValues = null;
                StringBuilder valueStrBldr = new StringBuilder();

                List<MySQLVar> Sqlvars = new List<MySQLVar>();
                List<MySqlParam> Sqlparam = new List<MySqlParam>();

                MySqlCommand mySQLCommand;
                MySqlConnectionStringBuilder mysqlStrBuilder;

                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Check if the current thread is busy.//////////////////////////////////////////////////////////////                                                            
                while (isBusy) await Task.Delay((int)TimeSpan.FromSeconds(DataUpdateRateSec * inline).TotalMilliseconds);

                if (mySQLConnection != null)
                    if (mySQLConnection.State != System.Data.ConnectionState.Closed)
                    {
                        mySQLConnection.Close();
                        isBusy = true;
                    }

                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Build Connection to SQL Server /////////////////////////////////////////////////////////////////// 

                mysqlStrBuilder          = new MySqlConnectionStringBuilder(); // Build string file
                mysqlStrBuilder.Server   = CSettingsManager.MYSQLServerAddress;
                mysqlStrBuilder.Database = CSettingsManager.MYSQLDatabase;
                mysqlStrBuilder.UserID   = CSettingsManager.MYSQLUser;
                mysqlStrBuilder.Password = CSettingsManager.MYSQLPassword;
                mysqlStrBuilder.Port     = Convert.ToUInt16(CSettingsManager.MYSQLPort);
                MySqlSslMode mode;
                Enum.TryParse(CSettingsManager.MYSQLSsl, out mode);
                mysqlStrBuilder.SslMode = mode;

                mySQLConnection = new MySqlConnection(mysqlStrBuilder.ToString());
                mysqlStrBuilder = null;

                rowToInsert = String.Format("DELETE FROM {0} WHERE {1};", tableName, id);


                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Command Exicution //////////////////////////////////////////////////////////////////////////////// 

                mySQLCommand = new MySqlCommand(rowToInsert, mySQLConnection);

                try
                {
                    mySQLConnection.Open();
                    isConnected = true;
                    mySQLCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    mySQLCommand.Connection.Close(); //   Try to close the connection
                    isConnected = false;
                    isBusy = false;
                    taskQueued--;
                }

            });
        }
        
        /// <summary>
        /// Makes Usable Mysql Vars so that they can be converted to table info.
        /// </summary>
        /// <typeparam name="T">The structure to use. !!!Limited!!!</typeparam>
        /// <returns>Sql Vars</returns>
        private MySQLVar[] BuildSqlVarArray<T>()
        {
            Type mainType = typeof(T);
            FieldInfo[] p = mainType.GetFields();
            List<MySQLVar> tableVars = new List<MySQLVar>();

            for (int fi = 0; fi < p.Length; fi++)
                if (p[fi].FieldType == typeof(bool))
                    tableVars.Add(new MySQLVar()
                    {
                        VaribleName = p[fi].Name,
                        ContainsLength = true,
                        Length = 16,
                        Type = MySqlDbType.Int16
                    });
                else if (p[fi].FieldType == typeof(int))
                    tableVars.Add(new MySQLVar()
                    {
                        VaribleName = p[fi].Name,
                        ContainsLength = true,
                        Length = 255,
                        Type = MySqlDbType.Int64
                    });
                else if (p[fi].FieldType == typeof(string) ||
                         p[fi].FieldType == typeof(char) ||
                         p[fi].FieldType == typeof(BillTo) ||
                         p[fi].FieldType == typeof(ShipInfo) ||
                         p[fi].FieldType == typeof(ShipInfo[]) ||
                         p[fi].FieldType == typeof(FileInfo) ||
                         p[fi].FieldType == typeof(Customer) ||
                         p[fi].FieldType == typeof(PaperType) ||
                         p[fi].FieldType == typeof(Binding) ||
                         p[fi].FieldType == typeof(Folding) ||
                         p[fi].FieldType == typeof(ShipingType))
                    tableVars.Add(new MySQLVar()
                    {
                        VaribleName = p[fi].Name,
                        ContainsLength = true,
                        Length = 255,
                        Type = MySqlDbType.VarChar
                    });

                else if (p[fi].FieldType == typeof(DateTime))
                    tableVars.Add(new MySQLVar()
                    {
                        VaribleName = p[fi].Name,
                        ContainsLength = false,
                        Length = 16,
                        Type = MySqlDbType.DateTime
                    });

                else throw new ApplicationException(
                        String.Format("!!Could Not Find Type In Order ProjectTrack.StObject.cs!! UNKNOWN TYPE == {0}", p[fi].GetType().FullName));
            return tableVars.ToArray();
        }

        #region OBSOLETE Functions
#if DEBUG

        /// <summary>
        /// Opens a connection from mysql. Sends a command, then inserts a row in the orders databse.
        /// </summary>
        /// <param name="localOrder"> The order information that is to be inserted to the database</param>
        /// <returns>Task</returns>
        [Obsolete]
        public async Task CreateOrder(Order localOrder)
        {
            await Task.Run(async () =>
            {
                taskQueued++;
                int inline = taskQueued;

                /////////////////////////////////////////////////////////////////////////////////////////////////////
                // Check if the current thread is busy.//////////////////////////////////////////////////////////////                                                            
                while (isBusy)
                {
#if DEBUG && CONSOLE // Stay silent when in release Mode                                     
                         Console.WriteLine(inline + ": " + "Waiting For Other thread to Complete");               
#endif
                    await Task.Delay((int)TimeSpan.FromSeconds(DataUpdateRateSec * inline).TotalMilliseconds);
                }
                if (mySQLConnection != null)
                    if (mySQLConnection.State != System.Data.ConnectionState.Closed)
                    {
                        mySQLConnection.Close();
                        isBusy = true;
                    }
                /////////////////////////////////////////////////////////////////////////////////////////////////////


                // BIG FAT COMMAND :P
                string s = "INSERT INTO orders (`uname`, `pickedup`, `keeponfile`," +
                " `isrush`, `onhold`, `completed`, `packaged`, `produced`, `recieved`," +
                " `jobname`, `billtoid`, `shipid`, `customerid`, `jobdatein`," +
                " `jobdateout`, `documentname`, `notes`, `writtenby`, `printedby`, `checkedby`)" +
                " VALUES (NULL, @pick, @keep, @rush, @hold, @comp, @pack, @prod, @reciev, @jobnam, @billto," +
                " @shipto, @cust, @datein, @dateout, @documentn, @note, @written, @printby, @checkby)"; // Command 

                // Setup the connection string then destroy it.
                MySqlConnectionStringBuilder sqlStrBuilder = new MySqlConnectionStringBuilder(); // Build string file
                sqlStrBuilder.Server    = CSettingsManager.MYSQLServerAddress;
                sqlStrBuilder.Database  = CSettingsManager.MYSQLDatabase;
                sqlStrBuilder.UserID    = CSettingsManager.MYSQLUser;
                sqlStrBuilder.Password  = CSettingsManager.MYSQLPassword;
                sqlStrBuilder.Port      = Convert.ToUInt16(CSettingsManager.MYSQLPort);
                MySqlSslMode mode;
                Enum.TryParse(CSettingsManager.MYSQLSsl, out mode);
                sqlStrBuilder.SslMode   = mode;


                mySQLConnection = new MySqlConnection(sqlStrBuilder.ToString());
                sqlStrBuilder = null;

                // Bool Conversion [boolIn ? true = 1 : false = 0]
                MySqlCommand sqlCommand = new MySqlCommand(s, mySQLConnection);
                sqlCommand.Parameters.AddWithValue("@pick",      localOrder.isCustomerPickup ? "1" : "0");
                sqlCommand.Parameters.AddWithValue("@keep",      localOrder.isKeepOnFile     ? "1" : "0");
                sqlCommand.Parameters.AddWithValue("@rush",      localOrder.isRush           ? "1" : "0");
                sqlCommand.Parameters.AddWithValue("@hold",      localOrder.isOnHold         ? "1" : "0");
                sqlCommand.Parameters.AddWithValue("@comp",      localOrder.isCompleted      ? "1" : "0");
                sqlCommand.Parameters.AddWithValue("@pack",      localOrder.isPackaged       ? "1" : "0");
                sqlCommand.Parameters.AddWithValue("@prod",      localOrder.isProduced       ? "1" : "0");
                sqlCommand.Parameters.AddWithValue("@reciev",    localOrder.isRecieved       ? "1" : "0");
                sqlCommand.Parameters.AddWithValue("@jobnam",    localOrder.JobName          .ToString());
                //sqlCommand.Parameters.AddWithValue("@billto",    localOrder.BillTo_id        .ToString());
                //sqlCommand.Parameters.AddWithValue("@shipto",    localOrder.Ship_id          .ToString());
                //sqlCommand.Parameters.AddWithValue("@cust",      localOrder.Custom_id        .ToString());
                sqlCommand.Parameters.AddWithValue("@datein",    localOrder.DateIn           .ToString("yyyy-MM-dd HH:mm:ss"));
                sqlCommand.Parameters.AddWithValue("@dateout",   localOrder.DateOut          .ToString("yyyy-MM-dd HH:mm:ss"));
                sqlCommand.Parameters.AddWithValue("@documentn", localOrder.DocumentName     .ToString());
                sqlCommand.Parameters.AddWithValue("@note",      localOrder.AdditionalNotes  .ToString());
                sqlCommand.Parameters.AddWithValue("@written",   localOrder.WrittenBy        .ToString());
                sqlCommand.Parameters.AddWithValue("@printby",   localOrder.PrintedBy        .ToString());
                sqlCommand.Parameters.AddWithValue("@checkby",   localOrder.CheckedBy        .ToString());

#if DEBUG && CONSOLE
                Console.WriteLine("// Connecting to Database...");
#endif
                try
                {
                    mySQLConnection.Open();
                    sqlCommand.ExecuteNonQuery();
                    isConnected = true;

#if DEBUG && CONSOLE
                            Console.WriteLine("Connected!");
#endif
                }
                /* In Debug Mode Return error TODO: [Plan to spit error in log] */
                catch (Exception ex)
                {
#if DEBUG && CONSOLE
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.WriteLine("ERROR: " + ex.Message);
                            Console.ResetColor();
#endif
                }
                finally
                {


#if DEBUG && CONSOLE
                          if (isBusy && taskQueue == 1)
                          Console.WriteLine("Done Importing");
#endif

                    sqlCommand.Connection.Close(); // Try to close the connection
                    isConnected = false;
                    isBusy = false;
                    taskQueued--;

                }
            });
        }
        /// <summary>
        /// Opens a Connection From MySql, Sends a command to select, Then returns a 
        /// </summary>
        /// <param name="ordernum">The order id that is to be sleceted from the database.</param>
        /// <returns>Order information</returns>
        [Obsolete]
        public async Task<Order> SelectOrder(string ordernum)
        {
            return await Task.Run(async () =>
            {
                if (taskQueued + 1 > MaxQueuedTask)
                {
                    taskQueued++;
                    int inline = taskQueued;
                    if (inline > MaxQueuedTask)

                        while (isBusy)
                        {
#if DEBUG && CONSOLE // Stay silent when in release Mode                                     
                                Console.WriteLine(inline + ": " + "Waiting For Other thread to Complete");
#endif
                            await Task.Delay((int)TimeSpan.FromSeconds(DataUpdateRateSec * inline).TotalMilliseconds);
                        }
                    if (mySQLConnection.State != System.Data.ConnectionState.Closed)
                        mySQLConnection.Close();

                    isBusy = true;

                    string s = "SELECT uname,jobname,jobdatein,jobdateout,billtoid,shipid,customerid,documentname,notes,writtenby,pickedup,keeponfile,isrush,onhold,completed,packaged,produced,recieved,printedby,checkedby FROM orders WHERE uname=@match"; // Command Screen

                    // Setup the connection string then destroy it.
                    MySqlConnectionStringBuilder stringBuilder = new MySqlConnectionStringBuilder(); // Build string file
                    stringBuilder.Server = CSettingsManager.MYSQLServerAddress;
                    stringBuilder.Database = CSettingsManager.MYSQLDatabase;
                    stringBuilder.UserID = CSettingsManager.MYSQLUser;
                    stringBuilder.Password = CSettingsManager.MYSQLPassword;
                    stringBuilder.Port = Convert.ToUInt16(CSettingsManager.MYSQLPort);
                    stringBuilder.SslMode = MySqlSslMode.Required;

                    mySQLConnection = new MySqlConnection(stringBuilder.ToString());

                    try
                    {
                        // Start a new Thread

                        // Run along side of Base :P
                        Order order = new Order();
                        MySqlCommand cmd = new MySqlCommand(s, mySQLConnection);

                        cmd.Parameters.AddWithValue("@match", ordernum); // Order number provided by method

#if DEBUG && CONSOLE
                               Console.WriteLine("// Connecting to Database...");
#endif
                        try
                        {
                            mySQLConnection.Open();
                            MySqlDataReader reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                isConnected = true;
#if DEBUG && CONSOLE // Console Write Out.
                                      Console.WriteLine("Connected!");
#endif
                                // Get Order Information //
                                order.JobName = reader.GetString("jobname");
                                order.DateIn = reader.GetDateTime("jobdatein");
                                order.DateOut = reader.GetDateTime("jobdateout");
                                //order.UName = reader.GetString("uname");
                                //order.BillTo_id = reader.GetString("billtoid");
                                //order.Ship_id = reader.GetString("shipid");
                                //order.Custom_id = reader.GetString("customerid");
                                order.isCustomerPickup = reader.GetBoolean("pickedup");
                                order.isKeepOnFile = reader.GetBoolean("keeponfile");
                                order.isRush = reader.GetBoolean("isrush");
                                order.isOnHold = reader.GetBoolean("onhold");
                                order.isCompleted = reader.GetBoolean("completed");
                                order.isPackaged = reader.GetBoolean("packaged");
                                order.isProduced = reader.GetBoolean("produced");
                                order.isRecieved = reader.GetBoolean("recieved");
                                order.DocumentName = reader.GetString("documentname");
                                order.AdditionalNotes = reader.GetString("notes");
                                order.WrittenBy = reader.GetString("writtenBy");
                                order.PrintedBy = reader.GetString("printedby");
                                order.CheckedBy = reader.GetString("checkedBy");
                            }
                        }

                        catch (Exception ex) // Show any Connection Errors or SQL Errors
                        {
#if DEBUG && CONSOLE
                                 Console.BackgroundColor = ConsoleColor.Red;
                                 Console.ForegroundColor = ConsoleColor.Black;
                                 Console.WriteLine("ERROR: " + ex.Message);
                                 Console.ResetColor();
#endif
                        }
                        finally
                        {
                            cmd.Connection.Close(); // Try to close the connection
                            isConnected = false;
                            isBusy = false;
                        }
                        return order;

                    }
                    catch (Exception ex)// Show any Connection Errors or SQL Errors 
                    {
#if DEBUG && CONSOLE
                             Console.BackgroundColor = ConsoleColor.Red;
                             Console.ForegroundColor = ConsoleColor.Black;
                             Console.WriteLine("ERROR: " + ex.Message);
                             Console.ResetColor();
#endif
                        return new Order();
                    }
                    finally
                    {
                        isConnected = false;
                        isBusy = false;
                        taskQueued--;
                    }
                }
                else
                {
                    throw new ApplicationException(String.Format("Cannot Create anymore new mysql threads. Max {0} Only", MaxQueuedTask));
                }
            });
        }
        [Obsolete]
        public async Task SelectCustomer(string custnum)
        {
            await Task.Run(async () =>
            {
                if (taskQueued + 1 > MaxQueuedTask)
                {
                    // Add to task queued
                    if (taskQueued + 1 > MaxQueuedTask)
                    {
                        int inline;
                        string selectString = "SELECT customerid FROM customers WHERE customerid=@match";

                        inline = taskQueued++;

                        // Twidle thumbs until number comes up
                        if (inline > MaxQueuedTask)
                            while (isBusy)
                                await Task.Delay((int)TimeSpan.FromSeconds(DataUpdateRateSec * inline).TotalMilliseconds);

                        // Check if connection closed before begining a new session.
                        if (mySQLConnection.State != System.Data.ConnectionState.Closed)
                            mySQLConnection.Close();

                        isBusy = true;

                        // setup connection string then destroy it.
                        MySqlConnectionStringBuilder stringBuilder = new MySqlConnectionStringBuilder(); // Build string file
                        stringBuilder.Server = CSettingsManager.MYSQLServerAddress;
                        stringBuilder.Database = CSettingsManager.MYSQLDatabase;
                        stringBuilder.UserID = CSettingsManager.MYSQLUser;
                        stringBuilder.Password = CSettingsManager.MYSQLPassword;
                        stringBuilder.Port = Convert.ToUInt16(CSettingsManager.MYSQLPort);
                        stringBuilder.SslMode = MySqlSslMode.Required;

                        mySQLConnection = new MySqlConnection(stringBuilder.ToString());
                    }


                }
                else
                {
                    throw new ApplicationException(String.Format("Cannot Create anymore new mysql threads. Max {0} Only", MaxQueuedTask));
                }
            });

        }
#endif
        #endregion
    }
}