namespace Kronos.Server.RequestProcessing
{
    internal interface IRequestProcessor
    {
        void ProcessRequest(byte[] request);
    }
}
