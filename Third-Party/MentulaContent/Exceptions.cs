using System;

namespace Mentula.Content
{
    public class ParameterNullException : Exception
    {
        public string Parameter;
        public override string StackTrace { get { return null; } }

        public ParameterNullException()
        {
            Parameter = "NULL";
        }

        public ParameterNullException(string parameter)
            : base(SetMessage(parameter))
        {
            Parameter = parameter;
        }

        public ParameterNullException(string parameter, Exception inner)
            : base(SetMessage(parameter), inner)
        {
            Parameter = parameter;
        }

        private static string SetMessage(string arg)
        {
            return $"Could not find parameter: '{arg}'.";
        }
    }

    public class ParameterException : Exception
    {
        public Type Type;
        public string Parameter;
        public string Value;
        public override string StackTrace { get { return null; } }

        public ParameterException()
        {
            Type = typeof(void);
            Parameter = Value = "NULL";
        }

        public ParameterException(string parameter, string value, Type type)
            : base(SetMessage(parameter, value, type))
        {
            Type = type;
            Parameter = parameter;
            Value = value;
        }

        public ParameterException(string parameter, string value, Type type, Exception inner)
            : base(SetMessage(parameter, value, type), inner)
        {
            Type = type;
            Parameter = parameter;
            Value = value;
        }

        private static string SetMessage(string parameter, string value, Type type)
        {
            return $"Could not process parameter '{parameter}' to {type}. Value='{value}'";
        }
    }

    public class ContainerException : Exception
    {
        public string Container;
        public override string StackTrace { get { return null; } }

        public ContainerException()
        {
            Container = "NULL";
        }

        public ContainerException(string container)
            : base("An error occured while processing container: " + container)
        {
            Container = container;
        }

        public ContainerException(string container, Exception inner)
            : base("An error occured while processing container: " + container, inner)
        {
            Container = container;
        }
    }

    public class BuildException : Exception
    {
        public override string StackTrace { get { return null; } }

        public BuildException()
            : base("An exception occured while builing content!")
        { }

        public BuildException(string message)
            : base(message)
        { }

        public BuildException(string message, Exception inner)
            :base(message, Retro(inner))
        { }

        private static BuildException Retro(Exception e)
        {
            if (e != null) return new BuildException(e.Message, Retro(e.InnerException));
            return null;
        }
    }
}