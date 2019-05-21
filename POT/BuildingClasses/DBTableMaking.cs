using POT.WorkingClasses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POT.BuildingClasses
{
    class MakeDataBase
    {
        public Boolean MakeDB(String _DBName)
        {
            ConnectionHelper cn = new ConnectionHelper();
            SqlCommand command = new SqlCommand();
            Boolean executed = false;
            int result = 0;

            using (SqlConnection cnn = cn.Connect("admin", "0000"))
            {
                if (!_DBName.Equals(""))
                {
                    try
                    {
                        command.Connection = cnn;
                        command.CommandText = "SELECT database_id FROM sys.databases WHERE Name = '" + _DBName + "'";
                        result = Convert.ToInt32(command.ExecuteScalar());

                        if (result <= 0)
                        {
                            command.CommandText = "CREATE DATABASE " + _DBName;
                            command.ExecuteNonQuery();
                        }

                        Properties.Settings.Default.DBBuilded = true;
                        Properties.Settings.Default.Save();

                        command.CommandText = "select count(*) from [" + _DBName + "].information_schema.tables where table_name = 'ErrorCode'";

                        Properties.Settings.Default.DBTabelsBuilded = (int)command.ExecuteScalar() > 0;
                        Properties.Settings.Default.Save();

                        executed = true;
                    }
                    catch (Exception e1)
                    {
                        new LogWriter(e1);
                        throw;
                    }
                    finally
                    {
                        cnn.Close();
                    }
                }
            }

            return executed;
        }

        public Boolean MakeTables(String _DBName)
        {
            ConnectionHelper cn = new ConnectionHelper();
            SqlCommand command = new SqlCommand();
            Boolean executed = false;
            
            using (SqlConnection cnn = cn.Connect("admin", "0000"))
            {
                SqlTransaction transaction = null;

                if (Properties.Settings.Default.DBBuilded && !Properties.Settings.Default.DBTabelsBuilded)
                {
                    try
                    {
                        command.Connection = cnn;

                        transaction = cnn.BeginTransaction();
                        command.Transaction = transaction;

                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.ErrorCode ([Item][nchar](50) NULL,[Value] [nchar] (50) NULL) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.ErrorCodeOther ([ErrorValue][nvarchar](50) NULL,[Item] [nvarchar] (50) NULL,[Value] [nvarchar] (50) NULL,[Category] [varchar] (50) NULL) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.ErrorLog ([ErrorDescription][nvarchar](500) NULL,[Module] [nvarchar] (50) NULL,[FunctionName] [nvarchar] (50) NULL,[UserName] [nvarchar] (50) NULL,[Date] [nvarchar] (10) NULL,[Time] [nvarchar] (10) NULL) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.IISparts ([iisID][numeric](18, 0) NOT NULL,[partID] [numeric] (18, 0) NOT NULL,[date] [nvarchar] (11) NOT NULL,[rb] [numeric] (18, 0) NULL,[times] [nvarchar] (5) NULL,[iusID] [numeric] (18, 0) NULL,[customerID] [numeric] (18, 0) NULL,[napomena][nvarchar](200) NULL) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.IUSparts ([iusID][numeric](18, 0) NOT NULL,[partID] [numeric] (18, 0) NOT NULL,[date] [nvarchar] (11) NOT NULL,[rb] [numeric] (18, 0) NULL,[customerID] [numeric] (18, 0) NULL, [napomena] [nvarchar](200) NULL) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.MUIzvjestaj (" +
                            "[TicketIDRNmu][numeric](18, 0) NULL," +
                            "[Branch] [nvarchar] (50) NULL," +
                            "[CCN] [nvarchar] (50) NULL," +
                            "[City] [nvarchar] (50) NULL," +
                            "[Address] [nvarchar] (50) NULL," +
                            "[Country] [nvarchar] (50) NULL," +
                            "[MonthYear] [nvarchar] (4) NULL," +
                            "[DatPrijavemu] [nvarchar] (11) NULL," +
                            "[VriPrijavemu] [nvarchar] (5) NULL," +
                            "[NazivUredajamu] [nvarchar] (400) NULL," +
                            "[OpisKvaramu] [nvarchar] (400) NULL," +
                            "[RMA] [nvarchar] (50) NULL," +
                            "[PartNumber] [nvarchar] (50) NULL," +
                            "[SNN] [nvarchar] (50) NULL," +
                            "[SNO] [nvarchar] (50) NULL," +
                            "[DatSLAmu] [nvarchar] (11) NULL," +
                            "[VriSLAmu] [nvarchar] (5) NULL," +
                            "[DatZavrsiomu] [nvarchar] (11) NULL," +
                            "[VriDrivemu] [nvarchar] (5) NULL," +
                            "[VriPoceomu] [nvarchar] (5) NULL," +
                            "[VriZavrsiomu] [nvarchar] (5) NULL," +
                            "[RNDescriptionmu] [nvarchar] (400) NULL," +
                            "[FullPartCodemu] [numeric] (33, 0) NULL," +
                            "[UserIDRegionID] [nvarchar] (50) NULL," +
                            "[ErrorCodemu] [nvarchar] (50) NULL) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        //POTversion
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.POTversion ([version][nvarchar](50) NULL) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        //PovijestLog
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.PovijestLog ([logID][numeric](18, 0) IDENTITY(1, 1) NOT NULL,[DatumUpisa] [nvarchar] (11) NOT NULL,[DatumRada] [nvarchar] (11) NOT NULL," +
                            "[SifraNovi] [numeric] (18, 0) NULL,[NazivNovi] [nvarchar] (100) NULL,[Opis] [nvarchar] (400) NULL,[SNNovi] [nvarchar] (50) NULL,[SNStari] [nvarchar] (50) NULL,[CN] [nvarchar] (50) NULL," +
                            "[UserID] [numeric] (18, 0) NOT NULL,[Customer] [nvarchar] (50) NULL,[CCN] [nvarchar] (50) NULL,[CI] [nvarchar] (50) NULL,[Dokument] [nvarchar] (50) NULL,[SifraStari] [numeric] (18, 0) NULL," +
                            "[NazivStari] [nvarchar] (100) NULL,[gNg] [nvarchar] (3) NULL,CONSTRAINT[PK__Povijest__7839F62DD8309478] PRIMARY KEY CLUSTERED([logID])" +
                            "WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        //TecajnaLista
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.TecajnaLista ([ID][numeric](18, 0) NOT NULL,[DateUpdated] [nvarchar] (11) NOT NULL,[KupovniE] [numeric] (18, 4)" +
                            " NOT NULL,[SrednjiE] [numeric] (18, 4) NOT NULL,[ProdajniE] [numeric] (18, 4) NOT NULL,CONSTRAINT[PK_TecajnaLista] PRIMARY KEY CLUSTERED([ID] ASC)" +
                            "WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        //VersionTable
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.VersionTable ([Version][nvarchar](50) NULL) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        /*
                        //Zamjena
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.Zamjena ([zamjenaID][numeric](18, 0) NOT NULL,[DatIzrade] [nvarchar] (11) NOT NULL,[VriIzrade] [nvarchar] (5) NOT NULL,[RegijaID] [numeric] (18, 0) NOT NULL," +
                            "[PartID] [numeric] (18, 0) NOT NULL,[VriRada] [nchar] (10) NOT NULL,[UserID] [numeric] (18, 0) NOT NULL,CONSTRAINT[PK_Zamjena] PRIMARY KEY CLUSTERED([zamjenaID] ASC)" +
                            "WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        //ZamjenaParts
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.ZamjenaParts ([ZamjenaID][numeric](18, 0) NOT NULL,[PartIDg] [numeric] (18, 0) NOT NULL,[PartIDng] [numeric] (18, 0) NOT NULL) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.ZamjenaParts WITH CHECK ADD  CONSTRAINT [FK_ZamjenaParts_Zamjena] FOREIGN KEY([ZamjenaID])REFERENCES[dbo].[Zamjena]([zamjenaID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.ZamjenaParts CHECK CONSTRAINT [FK_ZamjenaParts_Zamjena]";
                        command.ExecuteNonQuery();
                        */

                        //Sifrarnik
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.Sifrarnik " +
                                "([CategoryCode][numeric](9, 0) NOT NULL DEFAULT((0))," +
                                "[CategoryName] [nvarchar] (50) NOT NULL DEFAULT(' ')," +
                                "[PartCode] [numeric] (6, 0) NOT NULL DEFAULT((0))," +
                                "[PartName] [nvarchar] (50) NOT NULL DEFAULT(' ')," +
                                "[SubPartCode] [numeric] (3, 0) NULL DEFAULT((0))," +
                                "[SubPartName] [nvarchar] (100) NULL DEFAULT(' ')," +
                                "[PartNumber] [nvarchar] (50) NULL," +
                                "[PriceInKn] [numeric] (18, 2) NOT NULL DEFAULT((0))," +
                                "[PriceOutKn] [numeric] (18, 2) NOT NULL DEFAULT((0))," +
                                "[PriceInEur] [numeric] (18, 2) NOT NULL DEFAULT((0))," +
                                "[PriceOutEur] [numeric] (18, 2) NOT NULL DEFAULT((0))," +
                                "[FullCode] AS(([CategoryCode]+[PartCode])+[SubPartCode]) PERSISTED," +
                                "[FullName] AS(((([CategoryName]+' ')+[PartName])+' ')+[SubPartName]) PERSISTED," +
                                "[Packing] [nvarchar] (50) NULL," +
                                "CONSTRAINT[UQ_Sifrarnik_FullCode] UNIQUE NONCLUSTERED([FullCode] ASC)" +
                                "WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        //Regija
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.Regija (" +
                            "[RegionID][numeric](18, 0) NOT NULL," +
                            "[Region] [nvarchar] (3) NOT NULL," +
                            "[FullRegion] [nvarchar] (50) NOT NULL," +
                            "CONSTRAINT[PK_Regija] PRIMARY KEY CLUSTERED" +
                            "([RegionID] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        //Users
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.Users ([UserID][numeric](18, 0) IDENTITY(1, 1) NOT NULL," +
                            "[Name] [nvarchar] (50) NOT NULL," +
                            "[Surename] [nvarchar] (50) NOT NULL," +
                            "[Username] [nvarchar] (20) NOT NULL," +
                            "[Password] [nvarchar] (50) NOT NULL," +
                            "[Phone] [nvarchar] (50) NOT NULL," +
                            "[Email] [nvarchar] (50) NOT NULL," +
                            "[RegionID] [numeric] (18, 0) NULL," +
                            "[AdminRights] [numeric] (18, 0) NOT NULL DEFAULT((0))," +
                            "[HashPswd][nvarchar](max) NULL," +
                            "CONSTRAINT[PK_Worker] PRIMARY KEY CLUSTERED([UserID] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]," +
                            "CONSTRAINT[UQ_Username] UNIQUE NONCLUSTERED([Username] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]" +
                            ") ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Users WITH CHECK ADD CONSTRAINT [FK_Users_Regija] FOREIGN KEY([RegionID])REFERENCES[dbo].[Regija]([RegionID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Users CHECK CONSTRAINT [FK_Users_Regija]";
                        command.ExecuteNonQuery();

                        //Tvrtke
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.Tvrtke ([ID][numeric](18, 0) IDENTITY(1, 1) NOT NULL, [Name] [nvarchar] (50) NOT NULL," +
                            "[Address] [nvarchar] (50) NOT NULL,[City] [nvarchar] (50) NOT NULL,[PB] [nvarchar] (50) NOT NULL,[OIB] [nvarchar] (50) NULL," +
                            "[Contact] [nvarchar] (50) NULL,[BIC] [nvarchar] (50) NULL DEFAULT((0)),[KN] [numeric] (18, 2) NULL DEFAULT((0)),[Eur] [numeric] (18, 2) NULL DEFAULT((0))," +
                            "[Code] [nvarchar] (4) NOT NULL,[Country] [nchar] (10) NULL,[RegionID] [numeric] (18, 0) NULL DEFAULT((8)),[email] [nvarchar] (50) NULL," +
                            "CONSTRAINT[PK_Tvrtke] PRIMARY KEY CLUSTERED([ID] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]," +
                            "CONSTRAINT[Code_exist] UNIQUE NONCLUSTERED([Code] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]," +
                            "CONSTRAINT[Name_UNIQ_1] UNIQUE NONCLUSTERED([Name] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]" +
                            ") ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Tvrtke WITH CHECK ADD  CONSTRAINT [FK_Tvrtke_Regija] FOREIGN KEY([RegionID])REFERENCES[dbo].[Regija]([RegionID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Tvrtke CHECK CONSTRAINT [FK_Tvrtke_Regija]";
                        command.ExecuteNonQuery();

                        //Ticket
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.Ticket(" +
                            "[TicketID][numeric](18, 0) NOT NULL," +
                            "[TvrtkeID] [numeric] (18, 0) NOT NULL," +
                            "[Prio] [numeric] (18, 0) NOT NULL," +
                            "[Filijala] [nvarchar] (50) NOT NULL," +
                            "[CCN] [nvarchar] (50) NOT NULL," +
                            "[CID] [nvarchar] (50) NOT NULL," +
                            "[DatPrijave] [nvarchar] (11) NOT NULL," +
                            "[VriPrijave] [nvarchar] (5) NOT NULL," +
                            "[DatSLA] [nvarchar] (11) NOT NULL," +
                            "[VriSLA] [nvarchar] (5) NOT NULL," +
                            "[Drive] [numeric] (18, 0) NOT NULL," +
                            "[NazivUredaja] [nvarchar] (400) NOT NULL," +
                            "[OpisKvara] [nvarchar] (400) NOT NULL," +
                            "[Prijavio] [nvarchar] (50) NOT NULL," +
                            "[UserIDPreuzeo] [numeric] (18, 0) NULL," +
                            "[DatPreuzeto] [nvarchar] (11) NULL," +
                            "[VriPreuzeto] [nvarchar] (5) NULL," +
                            "[UserIDDrive] [numeric] (18, 0) NULL," +
                            "[DatDrive] [nvarchar] (11) NULL," +
                            "[VriDrive] [nvarchar] (5) NULL," +
                            "[UserIDPoceo] [numeric] (18, 0) NULL," +
                            "[DatPoceo] [nvarchar] (11) NULL," +
                            "[VriPoceo] [nvarchar] (5) NULL," +
                            "[UserIDZavrsio] [numeric] (18, 0) NULL," +
                            "[DatZavrsio] [nvarchar] (11) NULL," +
                            "[VriZavrsio] [nvarchar] (5) NULL," +
                            "[UserIDUnio] [numeric] (18, 0) NULL," +
                            "[DatReport] [nvarchar] (11) NULL," +
                            "[VriReport] [nvarchar] (50) NULL," +
                            "[RNID] [nvarchar] (50) NULL," +
                            "[UserIDSastavio] [numeric] (18, 0) NULL, " +
                            "CONSTRAINT [PK_Ticket_1] PRIMARY KEY CLUSTERED " +
                            "([TicketID] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]," +
                            "CONSTRAINT[UQ_TicketID] UNIQUE NONCLUSTERED([TicketID] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Ticket  WITH CHECK ADD  CONSTRAINT [FK_Ticket_Tvrtke] FOREIGN KEY([TvrtkeID])REFERENCES[dbo].[Tvrtke]([ID])";
                        command.ExecuteNonQuery();               
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Ticket CHECK CONSTRAINT [FK_Ticket_Tvrtke]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Ticket  WITH CHECK ADD  CONSTRAINT [FK_Ticket_Users] FOREIGN KEY([UserIDPreuzeo])REFERENCES[dbo].[Users]([UserID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Ticket CHECK CONSTRAINT [FK_Ticket_Users]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Ticket  WITH CHECK ADD  CONSTRAINT [FK_Ticket_Users1] FOREIGN KEY([UserIDUnio])REFERENCES[dbo].[Users]([UserID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Ticket CHECK CONSTRAINT [FK_Ticket_Users1]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Ticket  WITH CHECK ADD  CONSTRAINT [FK_Ticket_Users2] FOREIGN KEY([UserIDDrive])REFERENCES[dbo].[Users]([UserID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Ticket CHECK CONSTRAINT [FK_Ticket_Users2]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Ticket  WITH CHECK ADD  CONSTRAINT [FK_Ticket_Users3] FOREIGN KEY([UserIDPoceo])REFERENCES[dbo].[Users]([UserID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Ticket CHECK CONSTRAINT [FK_Ticket_Users3]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Ticket  WITH CHECK ADD  CONSTRAINT [FK_Ticket_Users4] FOREIGN KEY([UserIDZavrsio])REFERENCES[dbo].[Users]([UserID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Ticket CHECK CONSTRAINT [FK_Ticket_Users4]";
                        command.ExecuteNonQuery();

                        //RN
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.RN ([TicketIDRN][numeric](18, 0) NOT NULL,[ErrorCode] [nvarchar] (50) NOT NULL,[Description] [nvarchar] (400) NOT NULL," +
                            "[RB] [numeric] (18, 0) NOT NULL,[PartIDNew] [numeric] (18, 0) NOT NULL,[PartIDOld] [numeric] (18, 0) NOT NULL) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.RN WITH CHECK ADD  CONSTRAINT [FK_RN_Ticket] FOREIGN KEY([TicketIDRN])REFERENCES[dbo].[Ticket] ([TicketID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.RN CHECK CONSTRAINT [FK_RN_Ticket]";
                        command.ExecuteNonQuery();

                       //OTP
                       command.CommandText = "CREATE TABLE " + _DBName + ".dbo.OTP ([otpID][numeric](18, 0) NOT NULL,[customerID] [numeric] (18, 0) NOT NULL,[dateCreated] [nvarchar] (11) NOT NULL," +
                            "[napomena] [nvarchar] (200) NULL,[primID] [numeric] (18, 0) NULL,[userID] [numeric] (18, 0) NULL, [branchID] [numeric] (18, 0) NULL, CONSTRAINT[UQ_OTPid] UNIQUE NONCLUSTERED([otpID] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, " +
                            "IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.OTP WITH CHECK ADD CONSTRAINT [FK_OTP_Tvrtke] FOREIGN KEY([customerID])REFERENCES[dbo].[Tvrtke]([ID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.OTP CHECK CONSTRAINT [FK_OTP_Tvrtke]";
                        command.ExecuteNonQuery();

                        //OTPparts
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.OTPparts ([otpID][numeric](18, 0) NOT NULL,[partID] [numeric] (18, 0) NOT NULL,[trackingNumber] [nvarchar] (50) NULL) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.OTPparts WITH CHECK ADD  CONSTRAINT [FK_OTPparts_OTP] FOREIGN KEY([otpID])REFERENCES[dbo].[OTP] ([otpID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.OTPparts CHECK CONSTRAINT [FK_OTPparts_OTP]";
                        command.ExecuteNonQuery();

                        //Filijale
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.Filijale ([FilID][numeric](18, 0) IDENTITY(1, 1) NOT NULL,[TvrtkeCode] [nvarchar] (4) NOT NULL,[FilNumber] [nvarchar] (50) NOT NULL," +
                            "[RegionID] [numeric] (18, 0) NOT NULL,[Address] [nvarchar] (50) NULL,[City] [nvarchar] (50) NULL,[PB] [nvarchar] (50) NULL,[Phone] [nvarchar] (50) NULL,[Country] [nvarchar] (50) NULL," +
                            "CONSTRAINT[PK_Filijale] PRIMARY KEY CLUSTERED([FilID] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Filijale  WITH CHECK ADD  CONSTRAINT [FK_Filijale_Regija] FOREIGN KEY([RegionID])REFERENCES[dbo].[Regija]([RegionID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Filijale CHECK CONSTRAINT [FK_Filijale_Regija]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Filijale  WITH CHECK ADD CONSTRAINT [FK_Filijale_Tvrtke] FOREIGN KEY([TvrtkeCode])REFERENCES[dbo].[Tvrtke] ([Code])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Filijale CHECK CONSTRAINT [FK_Filijale_Tvrtke]";
                        command.ExecuteNonQuery();

                        //Legenda
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.Legenda ([PartID][numeric](18, 0) IDENTITY(1, 1) NOT NULL,[CodePartFull]  AS((CONVERT([numeric],[CompanyO]) * (100000000000.) + CONVERT([numeric],[CompanyC]) * (1000000000))+[PartialCode]) PERSISTED,[PartialCode] [numeric] (11, 0) NOT NULL,[SN] [nvarchar] (50) NOT NULL,[CN] [nvarchar] (50) NULL DEFAULT((0)),[DateIn] [nvarchar] (50) NOT NULL, [DateOut] [nvarchar] (50) NULL,[DateSend] [nvarchar] (50) NULL,[StorageID] [numeric] (18, 0) NOT NULL,[State] [nvarchar] (3) NOT NULL,[CompanyO] [nvarchar] (4) NOT NULL,[CompanyC] [nvarchar] (4) NOT NULL,CONSTRAINT[PK__Parts_co__7C3F0D30BC33483A] PRIMARY KEY CLUSTERED([PartID] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Legenda WITH CHECK ADD  CONSTRAINT [FK_Legenda_Sifrarnik] FOREIGN KEY([PartialCode])REFERENCES[dbo].[Sifrarnik]([FullCode])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Legenda CHECK CONSTRAINT [FK_Legenda_Sifrarnik]";
                        command.ExecuteNonQuery();

                        //Narudzbenica
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.Narudzbenica ([NarudzbenicaID][numeric](18, 0) NOT NULL,[userID] [numeric] (18, 0) NOT NULL,[DateMaked] [nvarchar] (11) NOT NULL,[otpID] [numeric] (18, 0) NULL,CONSTRAINT[UK_NarudzbenicaID] UNIQUE NONCLUSTERED([NarudzbenicaID] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Narudzbenica WITH CHECK ADD CONSTRAINT [FK_Narudzbenica_OTP] FOREIGN KEY([otpID])REFERENCES[dbo].[OTP]([otpID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Narudzbenica CHECK CONSTRAINT [FK_Narudzbenica_OTP]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Narudzbenica WITH CHECK ADD CONSTRAINT [FK_Narudzbenica_Users] FOREIGN KEY([userID])REFERENCES[dbo].[Users]([UserID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Narudzbenica CHECK CONSTRAINT [FK_Narudzbenica_Users]";
                        command.ExecuteNonQuery();

                        //NarudzbenicaParts
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.NarudzbenicaParts ([NarudzbenicaID][numeric](18, 0) NOT NULL,[FullCode] [numeric] (11, 0) NOT NULL,[CompanyO] [nvarchar] (4) NOT NULL,[CompanyC] [nvarchar] (4) NOT NULL) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.NarudzbenicaParts WITH CHECK ADD CONSTRAINT [FK_NarudzbenicaParts_Narudzbenica] FOREIGN KEY([NarudzbenicaID])REFERENCES[dbo].[Narudzbenica]([NarudzbenicaID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.NarudzbenicaParts CHECK CONSTRAINT [FK_NarudzbenicaParts_Narudzbenica]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.NarudzbenicaParts WITH CHECK ADD CONSTRAINT [FK_NarudzbenicaParts_Sifrarnik] FOREIGN KEY([FullCode])REFERENCES[dbo].[Sifrarnik]([FullCode])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.NarudzbenicaParts CHECK CONSTRAINT [FK_NarudzbenicaParts_Sifrarnik]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.NarudzbenicaParts WITH CHECK ADD CONSTRAINT [FK_NarudzbenicaParts_Tvrtke] FOREIGN KEY([CompanyO])REFERENCES[dbo].[Tvrtke]([Code])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.NarudzbenicaParts CHECK CONSTRAINT [FK_NarudzbenicaParts_Tvrtke]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.NarudzbenicaParts WITH CHECK ADD  CONSTRAINT [FK_NarudzbenicaParts_Tvrtke1] FOREIGN KEY([CompanyC])REFERENCES[dbo].[Tvrtke]([Code])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.NarudzbenicaParts CHECK CONSTRAINT [FK_NarudzbenicaParts_Tvrtke1]";
                        command.ExecuteNonQuery();

                        //Parts
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.Parts (" +
                            "[PartID][numeric](18, 0) IDENTITY(1, 1) NOT NULL," +
                            "[CodePartFull]  AS((CONVERT([numeric],[CompanyO]) * (100000000000.) + CONVERT([numeric],[CompanyC]) * (1000000000))+[PartialCode]) PERSISTED," +
                            "[PartialCode] [numeric] (11, 0) NOT NULL,[SN] [nvarchar] (50) NOT NULL,[CN] [nvarchar] (50) NULL DEFAULT((0)),[DateIn] [nvarchar] (50) NOT NULL," +
                            "[DateOut] [nvarchar] (50) NULL,[DateSend] [nvarchar] (50) NULL,[StorageID] [numeric] (18, 0) NOT NULL,[State] [nvarchar] (3) NOT NULL," +
                            "[CompanyO] [nvarchar] (4) NOT NULL,[CompanyC] [nvarchar] (4) NOT NULL,CONSTRAINT[PK_Parts] PRIMARY KEY CLUSTERED([PartID] ASC)" +
                            "WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Parts WITH CHECK ADD  CONSTRAINT [FK_Parts_Regija] FOREIGN KEY([StorageID])REFERENCES[dbo].[Regija]([RegionID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Parts CHECK CONSTRAINT [FK_Parts_Regija]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Parts WITH CHECK ADD  CONSTRAINT [FK_Parts_Sifrarnik] FOREIGN KEY([PartialCode])REFERENCES[dbo].[Sifrarnik]([FullCode])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Parts CHECK CONSTRAINT [FK_Parts_Sifrarnik]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Parts WITH CHECK ADD  CONSTRAINT [FK_Parts_Tvrtke] FOREIGN KEY([CompanyO])REFERENCES[dbo].[Tvrtke]([Code])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Parts CHECK CONSTRAINT [FK_Parts_Tvrtke]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Parts WITH CHECK ADD  CONSTRAINT [FK_Parts_Tvrtke1] FOREIGN KEY([CompanyC])REFERENCES[dbo].[Tvrtke]([Code])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Parts CHECK CONSTRAINT [FK_Parts_Tvrtke1]";
                        command.ExecuteNonQuery();

                        //PartsPoslano
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.PartsPoslano ([PartID][numeric](18, 0) NOT NULL,[CodePartFull] [numeric] (33, 0) NULL,[PartialCode] [numeric] (11, 0) NOT NULL," +
                            "[SN] [nvarchar] (50) NULL,[CN] [nvarchar] (50) NULL,[DateIn] [nvarchar] (50) NOT NULL,[DateOut] [nvarchar] (50) NULL,[DateSend] [nvarchar] (50) NULL,[StorageID] [numeric] (18, 0) NOT NULL," +
                            "[State] [nvarchar] (3) NOT NULL,[CompanyO] [nvarchar] (4) NOT NULL,[CompanyC] [nvarchar] (4) NOT NULL,CONSTRAINT[UK_PartsPoslano] UNIQUE NONCLUSTERED([PartID] ASC)" +
                            "WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.PartsPoslano  ADD  DEFAULT ((0)) FOR [CN]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.PartsPoslano WITH CHECK ADD  CONSTRAINT [FK__Parts_cop__Parti__0A888742] FOREIGN KEY([PartialCode])REFERENCES[dbo].[Sifrarnik]([FullCode])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.PartsPoslano CHECK CONSTRAINT [FK__Parts_cop__Parti__0A888742]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.PartsPoslano WITH CHECK ADD  CONSTRAINT [FK__Parts_cop__Stora__09946309] FOREIGN KEY([StorageID])REFERENCES[dbo].[Regija]([RegionID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.PartsPoslano CHECK CONSTRAINT [FK__Parts_cop__Stora__09946309]";
                        command.ExecuteNonQuery();

                        //PartsZamijenjeno
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.PartsZamijenjeno ([PartID][numeric](18, 0) NOT NULL,[CodePartFull] [numeric] (33, 0) NULL,[PartialCode] [numeric] (11, 0) NOT NULL," +
                            "[SN] [nvarchar] (50) NULL,[CN] [nvarchar] (50) NULL,[DateIn] [nvarchar] (50) NOT NULL,[DateOut] [nvarchar] (50) NULL,[DateSend] [nvarchar] (50) NULL,[StorageID] [numeric] (18, 0) NOT NULL," +
                            "[State] [nvarchar] (3) NOT NULL,[CompanyO] [nvarchar] (4) NOT NULL,[CompanyC] [nvarchar] (4) NOT NULL,CONSTRAINT[UQ__PartsPos__7C3F0D31317DA89A] UNIQUE NONCLUSTERED([PartID] ASC)WITH(PAD_INDEX = OFF, " +
                            "STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.PartsZamijenjeno  ADD  DEFAULT ((0)) FOR [CN]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.PartsZamijenjeno WITH CHECK ADD CONSTRAINT [FK__PartsPosl__Parti__5E3FF0B0] FOREIGN KEY([PartialCode])REFERENCES[dbo].[Sifrarnik]([FullCode])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.PartsZamijenjeno CHECK CONSTRAINT [FK__PartsPosl__Parti__5E3FF0B0]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.PartsZamijenjeno WITH CHECK ADD CONSTRAINT [FK__PartsPosl__Stora__5F3414E9] FOREIGN KEY([StorageID])REFERENCES[dbo].[Regija]([RegionID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.PartsZamijenjeno CHECK CONSTRAINT [FK__PartsPosl__Stora__5F3414E9]";
                        command.ExecuteNonQuery();

                        //PRIM
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.PRIM ([primID][numeric](18, 0) NOT NULL,[customerID] [numeric] (18, 0) NOT NULL,[dateCreated] [nvarchar] (11) NOT NULL,[napomena] [nvarchar] (200) NULL," +
                            "[userID] [numeric] (18, 0) NULL,CONSTRAINT[UQ_PRIMID] UNIQUE NONCLUSTERED([primID] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.PRIM WITH CHECK ADD  CONSTRAINT [FK_PRIM_Tvrtke] FOREIGN KEY([customerID])REFERENCES[dbo].[Tvrtke]([ID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.PRIM CHECK CONSTRAINT [FK_PRIM_Tvrtke]";
                        command.ExecuteNonQuery();

                        //PRIMparts
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.PRIMparts ([primID][numeric](18, 0) NOT NULL,[partID] [numeric] (18, 0) NOT NULL,[trackingNumber] [nvarchar] (50) NULL) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.PRIMparts WITH CHECK ADD  CONSTRAINT [FK_PRIMparts_PRIM] FOREIGN KEY([primID])REFERENCES[dbo].[PRIM] ([primID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.PRIMparts CHECK CONSTRAINT [FK_PRIMparts_PRIM]";
                        command.ExecuteNonQuery();

                        //Transport
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.Transport ([TransactionID][numeric](18, 0) NOT NULL,[TransportDateIn] [nvarchar] (50) NULL,[TransportDateOut] [nvarchar] (50) NOT NULL," +
                            "[RegionIDOut] [numeric] (18, 0) NOT NULL,[RegionIDIn] [numeric] (18, 0) NOT NULL,[UsersUserIDOut] [numeric] (18, 0) NOT NULL,[UsersUserIDIn] [numeric] (18, 0) NULL,[haveTrackingNumbers] [numeric] (18, 0) NULL," +
                            "CONSTRAINT[PK_Transport] PRIMARY KEY CLUSTERED([TransactionID] ASC)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Transport WITH CHECK ADD  CONSTRAINT [FK_Transport_OTP] FOREIGN KEY([TransactionID])REFERENCES[dbo].[OTP]([otpID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Transport CHECK CONSTRAINT [FK_Transport_OTP]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Transport WITH CHECK ADD  CONSTRAINT [FK_Transport_Regija] FOREIGN KEY([RegionIDOut])REFERENCES[dbo].[Regija]([RegionID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Transport CHECK CONSTRAINT [FK_Transport_Regija]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Transport WITH CHECK ADD  CONSTRAINT [FK_Transport_Tvrtke] FOREIGN KEY([RegionIDIn])REFERENCES[dbo].[Tvrtke]([ID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Transport CHECK CONSTRAINT [FK_Transport_Tvrtke]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Transport WITH CHECK ADD  CONSTRAINT [FK_Transport_Users] FOREIGN KEY([UsersUserIDOut])REFERENCES[dbo].[Users]([UserID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Transport CHECK CONSTRAINT [FK_Transport_Users]";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Transport WITH CHECK ADD  CONSTRAINT [FK_Transport_Users1] FOREIGN KEY([UsersUserIDIn])REFERENCES[dbo].[Users]([UserID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.Transport CHECK CONSTRAINT [FK_Transport_Users1]";
                        command.ExecuteNonQuery();

                        //WorkerInfo
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.WorkerInfo ([UserIDFK][numeric](18, 0) NOT NULL,[StartWorking] [nvarchar] (50) NULL,[Birthday] [nvarchar] (50) NULL,[FatherName] [nvarchar] (50) NULL," +
                            "[MotherName] [nvarchar] (50) NULL,[MotherSurname] [nvarchar] (50) NULL,[OIB] [nvarchar] (50) NULL,[JMBG] [nvarchar] (50) NULL,[Address] [nvarchar] (50) NULL,[City] [nvarchar] (50) NULL," +
                            "[Country] [nvarchar] (50) NULL,[ENC] [nvarchar] (50) NULL,[PhoneType] [nvarchar] (50) NULL,[LaptopType] [nvarchar] (50) NULL,CONSTRAINT[PK_WorkerInfo] PRIMARY KEY CLUSTERED([UserIDFK] ASC)" +
                            "WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.WorkerInfo WITH CHECK ADD  CONSTRAINT [FK_WorkerInfo_Worker] FOREIGN KEY([UserIDFK])REFERENCES[dbo].[Users]([UserID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.WorkerInfo CHECK CONSTRAINT [FK_WorkerInfo_Worker]";
                        command.ExecuteNonQuery();


                        transaction.Commit();

                        transaction = cnn.BeginTransaction();
                        command.Transaction = transaction;

                        //MainCmp
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.MainCmp([ID][numeric](18, 0) IDENTITY(1, 1) NOT NULL,[CmpName] [nvarchar] (50) NOT NULL,[Address] [nvarchar] (50) NULL,[VAT] [nvarchar] (50) NOT NULL," +
                            "[www] [nvarchar] (50) NULL,[Phone] [nvarchar] (50) NULL,[Email] [nvarchar] (50) NULL,[MB] [nvarchar] (50) NOT NULL,[IBAN] [nvarchar] (50) NOT NULL,[SWIFT] [nvarchar] (50) NOT NULL," +
                            "[CompanyCode] [nvarchar] (50) NOT NULL,[SupportEmail] [nvarchar] (50) NULL,[IDTvrtke] [numeric] (18, 0) NULL) ON[PRIMARY]";
                        command.ExecuteNonQuery();

                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.MainCmp  WITH CHECK ADD  CONSTRAINT [FK_MainCmp_Tvrtke] FOREIGN KEY([IDTvrtke])REFERENCES[dbo].[Tvrtke]([ID])";
                        command.ExecuteNonQuery();
                        command.CommandText = "ALTER TABLE  " + _DBName + ".dbo.MainCmp CHECK CONSTRAINT [FK_MainCmp_Tvrtke]";
                        command.ExecuteNonQuery();

                        transaction.Commit();


                        //ISS
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.ISS([ID] [numeric](18, 0) NOT NULL,[Date] [nvarchar](50) NOT NULL,[UserID] [numeric](18, 0) NOT NULL,[CustomerID] [numeric](18, 0) NOT NULL," +
                            "[PartID] [numeric](18, 0) NOT NULL,[Closed] [numeric](1, 0) NOT NULL,PRIMARY KEY CLUSTERED ([ID] ASC)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON," +
                            "ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY]";
                        command.ExecuteNonQuery();


                        //ISSparts
                        command.CommandText = "CREATE TABLE " + _DBName + ".dbo.ISSparts([ISSid] [numeric](18, 0) NOT NULL,[RB] [numeric](18, 0) NOT NULL,[oldPartID] [numeric](18, 0) NULL,[newPartID] [numeric](18, 0) NULL," +
                            "[Work] [nvarchar](200) NULL,[Comment] [nvarchar](200) NULL,[Time] [nvarchar](50) NULL) ON [PRIMARY]";
                        command.ExecuteNonQuery();


                        transaction = cnn.BeginTransaction();
                        command.Transaction = transaction;

                        //Popunjavanje inicijalno potrebnih podataka
                        command.CommandText = "Insert into " + _DBName + ".dbo.Regija(RegionID, Region, FullRegion) values(1, 'S', 'Servis')";
                        command.ExecuteNonQuery();
                        command.CommandText = "Insert into " + _DBName + ".dbo.Regija(RegionID, Region, FullRegion) values(2, 'T', 'Transport')";
                        command.ExecuteNonQuery();
                        command.CommandText = "Insert into " + _DBName + ".dbo.Regija(RegionID, Region, FullRegion) values(3, 'O', 'Ostali')";
                        command.ExecuteNonQuery();

                        command.CommandText = "Insert into " + _DBName + ".dbo.Users(Name, Surename, Username, Password, Phone, Email, RegionID, AdminRights, HashPswd) values('Baza', 'Baza', 'Baza', '9999', '', '', 3, 1, 'QXCsKieCoVFv6eE9cyKuSCwb1ZQ=')";
                        command.ExecuteNonQuery();

                        command.CommandText = "Insert into " + _DBName + ".dbo.Users(Name, Surename, Username, Password, Phone, Email, RegionID, AdminRights, HashPswd) values('STORNO', 'STORNO', 'storno', '9999', '', '', 3, 1, 'QXCsKieCoVFv6eE9cyKuSCwb1ZQ=')";
                        command.ExecuteNonQuery();

                        command.CommandText = "Insert into " + _DBName + ".dbo.Users(Name, Surename, Username, Password, Phone, Email, RegionID, AdminRights, HashPswd) values('Administrator', 'Admin', 'admin', '0000', '+385914456454', 'service@exception.hr', 3, 1, 'Od+lUoMxjTGv5aP/Sg4yU+IEXkM=')";
                        command.ExecuteNonQuery();

                        transaction.Commit();

                        Properties.Settings.Default.Catalog = _DBName;
                        Properties.Settings.Default.DBTabelsBuilded = true;
                        Properties.Settings.Default.Save();

                        executed = true;
                    }
                    catch (Exception e1)
                    {
                        new LogWriter(e1);
                        try
                        {
                            if (transaction != null)
                                transaction.Rollback();
                            throw;
                        }
                        catch (Exception e2)
                        {
                            new LogWriter(e2);
                            throw;
                        }
                    }
                    finally
                    {
                        cnn.Close();
                    }
                }else
                    executed = true;
            }

            return executed;
        }
    }
}
