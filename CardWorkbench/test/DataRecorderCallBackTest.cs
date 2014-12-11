using CardWorkbench.AcroInterface;
using CardWorkbench.ViewModels.MenuControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardWorkbench.test
{
    public class DataRecorderCallBackTest
    {
        public static void testCallBack()
        {
            String json = @"{
                   ""StartDataRecord"" : {
                      ""RecordLength"": 16384,  
                      ""FileName"": ""192.168.0.69:8080"",
                      ""bControlRunState"": 0,
                      ""bFrameMode"": 0,
                      ""bEnableTime"": 0,
                      ""bEnableStatus"": 0
                   }
                }";
            String dataStr = @"{
                       ""DataReceiver"" : {
	                    ""addr"": ""192.168.0.69"",   
	                    ""port"": 8080,
	                    ""IDPosition"": 2,  
	                    ""frameLength"": 32,    
	                    ""wordSize"": 16,	  
	                    ""bEnableTime"": 1,   
	                    ""bEnableStatus"": 1  
                       }
                    }"; 
            try 
	        {
                acro.Acro1626P acro1626P = Acro1626pHelper.getCurrentAcro1626PInstance();

                acro1626P.startDataRecord(2, 1, json);

                acro.DataReceiver acroDataReceiver = new acro.DataReceiver();
                //acroDataReceiver.startReceive(dataStr, new DataRecorderReceiveDataCallBack());
		
	        }
	        catch (Exception ex)
	        {

                Console.WriteLine(ex.Message);
                throw;
	        }
        
        }
    }
}
