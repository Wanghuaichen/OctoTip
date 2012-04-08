﻿/*
 * Created by SharpDevelop.
 * User: oferfrid
 * Date: 08/04/2012
 * Time: 14:03
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;
using OctoTip.Lib;
using OctoTip.Lib.ExperimentsCore.Attributes;
using OctoTip.Lib.ExperimentsCore.Base;

namespace CyclingInAmp
{
	/// <summary>
	/// Description of CAGetOD.
	/// </summary>
	[State("Get OD","Reading OD in Infinite")]
	public class CAGetOD:ReadState
	{
		int Empty384WellInd;
		int Lic6PlateInd;
		
		double ReadResults ;
		
		public CAGetOD(int Empty384WellInd,int Lic6PlateInd,string OutputFilePath):base()
		{
			this.Empty384WellInd = Empty384WellInd;
			this.Lic6PlateInd = Lic6PlateInd;
		}
		
		
			#region static
		public static new List<Type> NextStates()
		{
			return new List<Type>{typeof(CADilut),typeof(CAGrowToOD)};
		}
		#endregion
		protected override RobotJob BeforeRobotRun()
		{
			List<RobotJobParameter> RJP = new List<RobotJobParameter>(2);
			
			LicPos LP = Utils.Ind2LicPos(Lic6PlateInd);

			RJP.Add(new RobotJobParameter("Liconic6PlateCart",RobotJobParameter.ParameterType.Number,LP.Cart));
			RJP.Add(new RobotJobParameter("Liconic6PlatePos",RobotJobParameter.ParameterType.Number,LP.Pos));
			RJP.Add(new RobotJobParameter("Empty384WellInd",RobotJobParameter.ParameterType.Number,Empty384WellInd));
			
			RobotJob RJ = new RobotJob(
				@"D:\OctoTip\Protocols\CyclingInAmp\Scripts\MeasureWellOD.esc",RJP);
			
			return RJ;
		}
		
		
		
		protected override void AfterRobotRun()
		{
			
			
			//rename the results file
			
			try
			{
				FileInfo MyFileInfo = GetMeasurementsResultsFile();
			}
			catch (Exception ex)
			{
				throw(new Exception("Unable to GetMeasurementsResultsFile!",ex));
			}
			
			
			XPathDocument  ResultsXPathDocument = this.GetXPathMeasurementsResults();
			
			
			XPathNavigator navigator = ResultsXPathDocument.CreateNavigator();

			
			XPathNodeIterator DataNodes = navigator.Select("MeasurementResultData/Section/Data"); //reads
			
			
			
			
			foreach (XPathNavigator DataNode in DataNodes)
			{
				
				foreach (XPathNavigator node in DataNode.SelectChildren("Well",""))
				{
					node.MoveToAttribute("Pos",string.Empty);
					
					int WellInd =  CalcIndFromPlatePos(node.Value);
					if (Empty384WellInd == WellInd)
					{
						node.MoveToParent();
						Log(WellInd + "=" + node.Value);
						ReadResults=Convert.ToDouble(node.Value);
					}
				}
			}
			
			
		}
		
		public double GetReadResult()
		{
			return ReadResults;
		}
		
		private int CalcIndFromPlatePos(string Pos)
		{
			//384
			int Row = char.Parse(Pos.Substring(0,1))-'A';
			int Col =  Convert.ToInt32(Pos.Substring(1,Pos.Length-1))-1;
			int ind = Row + Col*4*4+1;
			return ind;
		}
		
	
		
		

	}
}