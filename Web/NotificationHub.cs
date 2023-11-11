﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Web
{
	public class NotificationHub : Hub
	{
		public async Task ReceivedWebHook()
		{
			await Clients.Others.SendAsync("ProcessPayment");
		}

		public async Task NewWithdrawalRequest()
		{
			await Clients.Others.SendAsync("NewWithdrawalRequest");
		}
	}
}