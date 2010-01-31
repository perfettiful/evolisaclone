using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EvoImage.Threads
{
    public class CommandChannel
    {
        private static int command = 0;
        /// <summary>
        /// 1 means stop; 0 go on;2 - timer stops
        /// </summary>
        /// <param name="command"></param>
        public static void SetCommand(int command)
        {
            CommandChannel.command = command;
        }

        public static int GetCommand()
        {
            return command;
        }
    }
}
