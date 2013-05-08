﻿/*
 * Created by SharpDevelop.
 * User: oferfrid
 * Date: 06/05/2012
 * Time: 12:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using OctoTip.Lib.ExperimentsCore.Attributes;
using OctoTip.Lib.ExperimentsCore.Base;

namespace GrowKillCurvePlaiting
{
	/// <summary>
	/// Description of GKCPWaitState.
	/// </summary>
	[State("Wait 4 sample","Wait for next sample")]
	public class GKCPWaitState:WaitState
	{

		public GKCPWaitState(double MinutesOfWait):base(DateTime.Now.AddMinutes(MinutesOfWait))
		{

		}
		
		protected override void OnWaitStart()
		{
			
		}
		
		protected override void OnWaitEnd()
		{
			
		}
		#region static
		public static new List<Type> NextStates()
		{
			return new List<Type>{typeof(GKCPSampleState)};
		}
		#endregion
	}
}

