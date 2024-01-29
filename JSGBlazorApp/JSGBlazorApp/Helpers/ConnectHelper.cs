using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JSGBlazorApp.Helpers
{
    public class ConnectHelper
    {
        private static ConnectHelper _instance;
        public static ConnectHelper Instance => _instance ?? (_instance = new ConnectHelper());

        private Task _initTask = null;
        private object _taskLock = new object();

        private ConnectHelper() { }

        public static DatabaseViz DBServer = new DatabaseViz();
        public string apkDownloadUrl = @"D:\";

        private static Task InitializeTask = null;
        private static object taskLock = new object();

        public async Task InitializeAsync()
        {
            try
            {
                bool waitFlag1 = false, waitFlag2 = false;
                if (InitializeTask == null)
                {
                    lock (taskLock)
                        InitializeTask = Task.Run((Action)InitializeCommon);
                    waitFlag1 = true;
                }

                if (waitFlag1)
                    await InitializeTask;
            }
            catch (Exception ex)
            {
                //Common.Instance.WriteErrorLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.Message);
            }
            finally
            {
                lock (taskLock)
                    InitializeTask = null;
            }
        }

        public void InitializeCommon()
        {
            DatabaseViz dbServer = DatabaseViz.Core = DBServer;

            var _url = "http://192.9.44.68:15117";
            //if (string.IsNullOrEmpty(_url))
            //{
            //    _url = Properties.AppResources.WebApiUrl;
            //}

            DBServer.Url = _url;

            //var _urlSignalR = Preferences.Get("SignalRUrl", string.Empty);
            //if (string.IsNullOrEmpty(_urlSignalR))
            //{
            //    _urlSignalR = Properties.AppResources.SignalRUrl;
            //}
        }


        public async Task WaitInitializeCommon()
        {
            Task task;
            lock (_taskLock)
            {
                task = _initTask;
            }
            if (task != null)
            {
                await task;
            }
        }

    }
}
