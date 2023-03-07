using Microsoft.AspNetCore.SignalR;

public class DataHub : Hub
    {
        public async Task SendData(string data)
        {
            //write data to serial port
            System.Console.WriteLine("Listening to serial port");

    
            await Clients.All.SendAsync("ReceiveData", data);
        }
    }