﻿/*
 * Created by SharpDevelop.
 * User: oferfrid
 * Date: 26/09/2011
 * Time: 20:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading;
using OctoTip.Lib.ExperimentsCore.Interfaces;
using OctoTip.Lib;

namespace OctoTip.Lib.ExperimentsCore.Base
{
	/// <summary>
	/// Description of Protocol.
	/// </summary>
	public abstract class Protocol:IProtocol
	{
		
		#region Static
		public static  List<Type> ProtocolStates()
		{
			return new List<Type>(0);
		}
		#endregion
		
				
		public State CurrentState;
		
		private Thread RunningThread;
		
		
		protected int StateSamplelingRate;
		
		public  ProtocolParameters ProtocolParameters;
		
		public  Protocol(ProtocolParameters ProtocolParameters)
		{
			this.ProtocolParameters = ProtocolParameters;
			RunningThread = new Thread(_DoWork);
			RunningThread.IsBackground = true;
		}
		public  Protocol()
		{
			StateSamplelingRate = Convert.ToInt32(ConfigurationManager.AppSettings["StateSamplelingRate"]);
			if (StateSamplelingRate == 0)
			{
				throw new NullReferenceException("AppSettings key for StateSamplelingRate is null");
			}
		}
		~Protocol()
		{
			CurrentState = null;
		}
		
		
		public void SetNewProtocolParameters(ProtocolParameters ProtocolParameters)
		{
			this.ProtocolParameters = ProtocolParameters;
		}
		#region Public Propertis
		public Statuses Status
		{
			get {return _CurrentStatus;}
		}

		
		#endregion
		#region status Handeling
		
		// Volatile is used as hint to the compiler that this data
		// member will be accessed by multiple threads.
		protected volatile bool ShouldStop = false;
		protected volatile bool ShouldPause = false;
		
		
		private Statuses _CurrentStatus = Statuses.Stopped;
		
		
		public void ChangeState(State NewState)
		{
			if (CurrentState !=null)
			{
				//remove event handeling from the recent state.
				CurrentState.StateDisplayedDataChange -=  OnProtocolStateDisplayedDataChange;
				CurrentState.StateStatusChange -=	OnProtocolStateStatusChange;
			}
			
			if(!this.ShouldStop)
			{
				//do not move to the next state if paused.
				if (ShouldPause && !ShouldStop)
				{
					SetCurrentStatus( Statuses.Paused,"Paused between states");
					while (ShouldPause && !ShouldStop)
					{
						Thread.Sleep(StateSamplelingRate);
					}
					
				}
				if(!this.ShouldStop)
				{
					
					SetCurrentStatus( Statuses.Started,"Started");
					CurrentState = NewState;
					CurrentState.StateDisplayedDataChange += OnProtocolStateDisplayedDataChange;
					CurrentState.StateStatusChange +=	OnProtocolStateStatusChange;
					CurrentState.Start();
					
					while (!this.ShouldStop && CurrentState is IRestartableState && CurrentState.CurrentStatus == State.Statuses.FatalError)
					{//test if restartblr after failier and maintain a pause like state if failed
						
						SetCurrentStatus( Statuses.Paused,"Paused after FatalError");
						while (ShouldPause && !ShouldStop)
						{
							Thread.Sleep(StateSamplelingRate);
						}
						
						if (!ShouldStop)
						{
							SetCurrentStatus( Statuses.Started,"resumed after FatalError");
							IRestartableState RestartableCurrentState = CurrentState as IRestartableState;
							RestartableCurrentState.Restart();
						}
					}
					
				}
				
			}
		}

		protected void SetCurrentStatus(Statuses CurrentStatus,string Message )
		{
			//Log the status change And Raise the event.
			Log(string.Format("{0}>{1} {2}",_CurrentStatus , CurrentStatus , Message),Logging.LoggingEntery.EnteryTypes.Debug);
			_CurrentStatus = CurrentStatus;
			OnStatusChanged(new  ProtocolStatusChangeEventArgs(CurrentStatus,Message));
		}
		

		public event EventHandler<ProtocolStatusChangeEventArgs> StatusChanged;
		public event EventHandler<ProtocolDisplayedDataChangeEventArgs> DisplayedDataChange;
		public event EventHandler<StateDisplayedDataChangeEventArgs> StateDisplayedDataChange;
		public event EventHandler<StateStatusChangeEventArgs> StateStatusChange;
		
	
		private void OnStatusChanged(ProtocolStatusChangeEventArgs e)
		{
			EventHandler<ProtocolStatusChangeEventArgs> handler = StatusChanged;
			if (handler != null)
			{
				handler(this, e);
			}
		}
		
		protected  void DisplayData(string Message)
		{
			ProtocolDisplayedDataChangeEventArgs e = new ProtocolDisplayedDataChangeEventArgs(Message);
			EventHandler<ProtocolDisplayedDataChangeEventArgs> handler = DisplayedDataChange;
			if (handler != null)
			{
				handler(this, e);
			}
		}
				
		private void OnProtocolStateDisplayedDataChange(object sender,StateDisplayedDataChangeEventArgs e)
		{
			EventHandler<StateDisplayedDataChangeEventArgs> handler = StateDisplayedDataChange;
			if (handler != null)
			{
				handler(this, e);
			}
		}
		
		protected  virtual void OnProtocolStateStatusChange(object sender,StateStatusChangeEventArgs e)
		{
			EventHandler<StateStatusChangeEventArgs> handler = StateStatusChange;
			if (handler != null)
			{
				handler(this, e);
			}

			switch(e.StateStatus)
			{
					case State.Statuses.Paused:
					this.SetCurrentStatus(Statuses.Paused,"Paused from state:" + e.Message );
					break;
					case State.Statuses.Started:
					this.SetCurrentStatus(Statuses.Started,"started from state:" + e.Message );
					break;
					case State.Statuses.Stopped:
					this.SetCurrentStatus(Statuses.Stopped,"Stopped From State:" + e.Message );
					break;
					case State.Statuses.FatalError:
					this.SetCurrentStatus(Statuses.FatalError,"FatalError From State:" + e.Message );
					ShouldPause  = true;
					break;
					
					case State.Statuses.RuntimeError:
					this.SetCurrentStatus(Statuses.RuntimeError,"RuntimeError From State:" + e.Message );
					break;
					
					case State.Statuses.EndedSuccessfully:
					this.SetCurrentStatus(Statuses.EndedSuccessfully,"Ended Successfully From State:" + e.Message );
					break;
		
			//Stoping ??,
			//Starting, ??
			// Pausing, ??
			

			}
			
			
			this.Log(string.Format("State {0} status changed to: {1}",e.CurrentState.GetType().Name,e.StateStatus),Logging.LoggingEntery.EnteryTypes.Debug);
		}
		
		
		
		public void   RequestStop()
		{
			SetCurrentStatus(Statuses.Stoping,"Stop Requested , Trying to Stopped");
			ShouldStop = true;
			ShouldPause  = false;
			CurrentState.RequestStop("Stop Reqested From Protocol");
		}
		public void RequestPause()
		{
			SetCurrentStatus(Statuses.Pausing,"Pause Requested, Trying to Pause");
			ShouldPause  = true;
			CurrentState.RequestPause();
		}
		public void RequestStart()
		{
			switch(this.Status)
			{
				case(Statuses.Paused):
					SetCurrentStatus(Statuses.Starting,"Resuming");
					ShouldPause  = false;
					if (CurrentState.CurrentStatus==State.Statuses.Paused)
					{
						CurrentState.RequestRestart();
					}
					break;
				case(Statuses.Stopped):
					SetCurrentStatus(Statuses.Starting,"Startting");
					RunningThread.Start();
					break;
			}
			
			
			
		}
		
		public enum Statuses
		{
			Stopped,
			Stoping,
			Started,
			Starting,
			Paused,
			Pausing,
			Error,
			FatalError,
			RuntimeError,
			EndedSuccessfully
		}
		
		
		
		#endregion
		
		protected void ProtocolLog(string title, Logging.LoggingEntery.EnteryTypes ET)
		{
			Logging.Log.LogEntery(new Logging.LoggingEntery(this.GetType().Name,this.ProtocolParameters.Name,title,ET));
		}
		protected void ProtocolLog(string Title,string Messege , Logging.LoggingEntery.EnteryTypes ET)
		{
			Logging.Log.LogEntery(new Logging.LoggingEntery(this.GetType().Name,this.ProtocolParameters.Name,Title,Messege,ET));
		}
		
		private void Log(string title,Logging.LoggingEntery.EnteryTypes ET)
		{
			string SubSendor;
			if (this.ProtocolParameters.Name==null)
			{
				SubSendor = this.GetType().Name;
			}
			else 
			{
				SubSendor = string.Format("{0}({1})",this.GetType().Name,this.ProtocolParameters.Name);
			}
				
			Logging.Log.LogEntery(new Logging.LoggingEntery("OctoTipPlus Appilcation",SubSendor ,title,ET));
		}
		
		public void Start()
		{
			OnStatusChanged(new ProtocolStatusChangeEventArgs(Statuses.Started,"Started"));
			Log("Started",Logging.LoggingEntery.EnteryTypes.Debug);
			try {
				this.Start();
				OnStatusChanged(new ProtocolStatusChangeEventArgs(Statuses.Stopped,"Ended Successfully!"));
			}
			catch (Exception e)
			{
				OnStatusChanged(new ProtocolStatusChangeEventArgs(Statuses.Error,"Error:" + e.Message ));
				Log("Error:" + e.Message,Logging.LoggingEntery.EnteryTypes.Error);
				RunningThread.Abort();
				
			}
			
			
			
			
		}
		
		
		private void _DoWork()
		{
			try
			{
				DoWork();
				this.SetCurrentStatus(Statuses.EndedSuccessfully,"Ended at " +  DateTime.Now.ToString());
			}
			catch(Exception e)
			{
				SetCurrentStatus(Statuses.FatalError,"Exception:" +  e.ToString());
			}
		}
		
		protected abstract   void DoWork();
		

		
	}
	
	
	public class ProtocolStatusChangeEventArgs : EventArgs
	{
		private Protocol.Statuses _NewStatus;
		private string _Message;

		public ProtocolStatusChangeEventArgs(Protocol.Statuses NewStatus,string Message)
		{
			this._NewStatus = NewStatus;
			this._Message = Message;
		}
		public Protocol.Statuses NewStatus
		{
			get { return _NewStatus; }
		}
		public string Message
		{
			get { return _Message; }
		}
	}
	

	#region EventArgs
	
	public class ProtocolDisplayedDataChangeEventArgs : EventArgs
	{
		
		private string _Message;

		public ProtocolDisplayedDataChangeEventArgs(string Message)
		{
			
			this._Message = Message;
		}
		public string Message
		{
			get { return _Message; }
		}
	}
	
	#endregion
}

