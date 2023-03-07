using System.Diagnostics;
using System.IO.Ports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;


namespace Refactor.Controllers;

public class HomeController : Controller
{
   private readonly IHubContext<DataHub> _hubContext;
   

        public HomeController(IHubContext<DataHub> hubContext)
        {
            _hubContext = hubContext;
        }

    public IActionResult Index()
    {
          
            
        return View();
    }
    
    public IActionResult privacy()
    {
        return View();
    }


    public IActionResult Gauge()
    {
        // TODO: Your code here
        return View();
    }
    public IActionResult Gaugetwo()
    {
        // TODO: Your code here
        return View();
    }
    

     public async Task StartReading()
        {
            ///dev/ttyUSB0
            using (SerialPort serialPort = new SerialPort("COM10"))
            {
                    serialPort.BaudRate = 19200;
                    serialPort.Parity = Parity.Odd;
                    serialPort.DataBits = 8;
                    serialPort.StopBits = StopBits.One;

                serialPort.Open();

                  byte[] tx_Data = new byte[10];
                    tx_Data[0]= 0xFA;
                    tx_Data[1] = 0x0A;
                    tx_Data[2] = 0x03;
                    tx_Data[3] = 0x01;
                    tx_Data[4] = 0x01;
                    tx_Data[5] = 0x00;
                    tx_Data[6] = 0x00;
                    tx_Data[7] = 0x00;
                    tx_Data[8] = 0x00;
                    tx_Data[9] = 0x0F;
                    serialPort.Write(tx_Data, 0, tx_Data.Length);

                    byte[] buffer = new byte[17];
                    string data="0";
                    string data1="0";
                    string data2="0";


                while(true)
                {
                     while (serialPort.BytesToRead > 14)
                 {
                      buffer[1] = (byte)serialPort.ReadByte();

                        if (buffer[1] == 0xFA) //250
                         {

                                buffer[2] = (byte)serialPort.ReadByte();    
                                buffer[3] = (byte)serialPort.ReadByte();
                                serialPort.Read(buffer, 4, 10);

                                if (buffer[2] == 13 && buffer[4] == 4 && buffer[5] == 132)
                                {
                                    Task.Run(() =>
                                    {
                                        if (buffer[10] == 255) data="30";
                                        else data=(buffer[10] * 3).ToString();
                                    }).Wait();
                                }

                               if (buffer[2] == 17 && buffer[4] == 4 && buffer[5] == 133)
                                {
                                    int pulse = (buffer[11] << 8) | buffer[10];
                                    int spo = buffer[12];
                                    if (pulse == 511) Task.Run(() =>data1=null).Wait();
                                    else Task.Run(() =>data1=(pulse).ToString()).Wait();
                                    if (spo == 127) Task.Run(() =>data2=(null)).Wait();
                                    else Task.Run(() => data2=(spo).ToString()).Wait();
                                }

                         }     

                      
                    await _hubContext.Clients.All.SendAsync("ReceiveData", data,data1,data2);


                   
                }
                }


            }
        }

    
}
