﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.5446
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OctoTip.Lib
{

	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
	[System.ServiceModel.ServiceContractAttribute(ConfigurationName="IRobotJobsQueueService")]
	public interface IRobotJobsQueueService
	{
		
		[System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRobotJobsQueueService/TestConnection", ReplyAction="http://tempuri.org/IRobotJobsQueueService/TestConnectionResponse")]
		string TestConnection(string name);
		
		[System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRobotJobsQueueService/AddRobotJob", ReplyAction="http://tempuri.org/IRobotJobsQueueService/AddRobotJobResponse")]
		System.Guid AddRobotJob(OctoTip.Lib.RobotJob RJ);
		
		[System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRobotJobsQueueService/GetJobStatus", ReplyAction="http://tempuri.org/IRobotJobsQueueService/GetJobStatusResponse")]
		OctoTip.Lib.RobotJob.Status GetJobStatus(System.Guid UniqueID);
		
		[System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IRobotJobsQueueService/GetRobotStatus", ReplyAction="http://tempuri.org/IRobotJobsQueueService/GetRobotStatusResponse")]
		string GetRobotStatus();
	}

	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
	public interface IRobotJobsQueueServiceChannel : IRobotJobsQueueService, System.ServiceModel.IClientChannel
	{
	}

	[System.Diagnostics.DebuggerStepThroughAttribute()]
	[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "3.0.0.0")]
	public partial class RobotJobsQueueServiceClient : System.ServiceModel.ClientBase<IRobotJobsQueueService>, IRobotJobsQueueService
	{
		
		public RobotJobsQueueServiceClient()
		{
		}
		
		public RobotJobsQueueServiceClient(string endpointConfigurationName) :
			base(endpointConfigurationName)
		{
		}
		
		public RobotJobsQueueServiceClient(string endpointConfigurationName, string remoteAddress) :
			base(endpointConfigurationName, remoteAddress)
		{
		}
		
		public RobotJobsQueueServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
			base(endpointConfigurationName, remoteAddress)
		{
		}
		
		public RobotJobsQueueServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
			base(binding, remoteAddress)
		{
		}
		
		public string TestConnection(string name)
		{
			return base.Channel.TestConnection(name);
		}
		
		public System.Guid AddRobotJob(OctoTip.Lib.RobotJob RJ)
		{
			return base.Channel.AddRobotJob(RJ);
		}
		
		public OctoTip.Lib.RobotJob.Status GetJobStatus(System.Guid UniqueID)
		{
			return base.Channel.GetJobStatus(UniqueID);
		}
		
		public string GetRobotStatus()
		{
			return base.Channel.GetRobotStatus();
		}
	}
}