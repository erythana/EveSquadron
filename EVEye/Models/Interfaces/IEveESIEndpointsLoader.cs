namespace EVEye.Models.Interfaces
{
    public interface IEveESIEndpointsLoader
    {
        public string UniverseEndpoint { get; }
        public string PortraitEndpoint { get; }
    }
}