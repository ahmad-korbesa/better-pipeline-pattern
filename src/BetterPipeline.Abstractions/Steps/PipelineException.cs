namespace BetterPipeline.Abstractions.Orchestration
{

    [Serializable]
    public class PipelineException : Exception
    {
        public PipelineException() { }
        public PipelineException(string stepName) : base($"Error at step {stepName}") { }
        public PipelineException(string message, Exception inner) : base(message, inner) { }
        protected PipelineException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}
