using System;
using System.Collections.Generic;
using NLog;

namespace DS4WinWPF.DraculaView.MVVMHandler
{
    public static class Mediator
    {
        public static DS4Windows.ControlService rootHub;
        private static LoggerHolder logHolder;

        static IDictionary<string, List<Action<object>>> pl_dict = new Dictionary<string, List<Action<object>>>();

        public static void Register(string token, Action<object> callback)
        {
            try
            {
                if (!pl_dict.ContainsKey(token))
                {
                    var list = new List<Action<object>>();
                    list.Add(callback);
                    pl_dict.Add(token, list);
                }
                else
                {
                    if (!pl_dict[token].Contains(callback))
                    {
                        pl_dict[token].Add(callback);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger logger = logHolder.Logger;
                logger.Error("Register: " + ex);
            }
        }

        public static void Unregister(string token, Action<object> callback)
        {
            try
            {
                if (pl_dict.ContainsKey(token))
                    pl_dict[token].Remove(callback);
            }
            catch (Exception ex)
            {
                Logger logger = logHolder.Logger;
                logger.Error("Unregister: " + ex);
            }
        }

        public static void NotifyColleagues(string token, object args)
        {
            try
            {
                if (pl_dict.ContainsKey(token))
                    foreach (var callback in pl_dict[token])
                        callback(args);
            }
            catch (Exception ex)
            {
                Logger logger = logHolder.Logger;
                logger.Error("NotifyColleagues: " + ex);
            }
        }
    }
}
