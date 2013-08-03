using System.Collections.Generic;
using CruiseControl.Models;
using Nancy;
using Nancy.IO;
using Newtonsoft.Json;
using System;

namespace CruiseControl
{
	public class HttpModule : NancyModule
	{
        public Commander _commander;

		public HttpModule(Commander commander)
		{
			Get["/"] = x => "Nancy";
			Post["/Command"] = x =>
				{
                    return ProcessData(Request.Body);
				};

            _commander = commander;
		}

        public string ProcessData(RequestStream requestBody)
		{
            var length = int.Parse(requestBody.Length.ToString());
            var bytes = new byte[length];
            requestBody.Read(bytes, 0, length);
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            // Process the status
            var board = JsonConvert.DeserializeObject<BoardStatus>(json);

            if (board.RoundNumber == 0) _commander.Reset(); 

            _commander.GetBoardStatus(board);

            // Create commands to do
            return JsonConvert.SerializeObject(_commander.GiveCommands());
		}

	}
}