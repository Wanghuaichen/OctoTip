﻿/*
 * Created by SharpDevelop.
 * User: oferfrid
 * Date: 21/09/2011
 * Time: 10:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

using OctoTip.OctoTipLib;
using OctoTip.OctoTipExperiments.Core;



namespace OctoTip.OctoTipTest
{
	class Program
	{
		public static void Main(string[] args)
		{
//			Console.WriteLine("start init RobotJobsQueueServiceClient!");
//			
//			
//			RobotJobsQueueServiceClient RC = new RobotJobsQueueServiceClient();
//			
//			
//			Console.WriteLine("Robot Status: {0}",RC.GetRobotStatus());
//				
//				
//			
//			OctoTip.OctoTipLib.RobotJob RP = new OctoTip.OctoTipLib.RobotJob(@"C:\Users\Public\Documents\Learn\BioLab\programing\OctoTip\SampleData\" + "NewScript1.esc");
//			Random r = new Random();
//			RP.Priority = (double)r.Next()/int.MaxValue;
//			//RC.TestConnection("tt");
//			RC.AddRobotJob(RP);
//			
//		
			
			foreach (Type ProtocolData in ProtocolHostProvider.ProtocolsData)
            {
				Console.WriteLine(ProtocolData.Name);
            }

  
			
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
}

