using System;
using UnityEngine;

namespace Renamer
{
    internal class LogUtils
    {
        private const string logName = "KerbalRenamer";
        internal static bool enabled = false;

        public static void Log(params object[] message)
        {
            Log(Array.ConvertAll(message, item => item.ToString()));
        }

        public static void Log(params string[] message)
        {
            #if !DEBUG
            if (!enabled) return;
            #endif

            var builder = StringBuilderCache.Acquire();
            builder.Append("[").Append(logName).Append("] ");
            foreach (string part in message) {
                builder.Append(part);
            }
            Debug.Log(builder.ToStringAndRelease());
        }
    }
}
