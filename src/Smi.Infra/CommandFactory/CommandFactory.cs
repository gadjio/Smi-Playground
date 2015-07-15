namespace Smi.Infra.CommandFactory
{
	using System;
	using Newtonsoft.Json;
	using Smi.Infra.Messages;

	public class CommandFactory : ICommandFactory
    {
        public ICommand Create(string commandName, string payload)
        {
            Type commandType = Type.GetType(commandName + "Command");

            if (commandType == null)
                throw new ArgumentException("Command Type " + commandName + "Command could not be found.");
            
            if (typeof (ICommand).IsAssignableFrom(commandType))
                throw new ArgumentException("Command Type " + commandName + "Command does not implement ICommand interface.");

            try
            {
                return (ICommand) JsonConvert.DeserializeObject(payload, commandType);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Payload could not be deserialized into Command Type " + commandName + "Command.", e);
            }
        }
    }

    public interface ICommandFactory
    {
        ICommand Create(string commandName, string payload);
    }
}