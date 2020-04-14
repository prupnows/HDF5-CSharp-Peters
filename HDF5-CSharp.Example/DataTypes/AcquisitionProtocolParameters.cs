using Newtonsoft.Json;

namespace HDF5CSharp.Example.DataTypes
{
    public class AcquisitionProtocolParameters
    {
        // Scan Protocol the scan is based on => change might be apply on top of it
        public string BaseScanProtocolName { get; set; } = string.Empty;

        // Description of the acquisition 
        public string AcquisitionDescription { get; set; } = string.Empty;
        public string RecordingName { get; set; } = string.Empty;


        public string AsJson() => JsonConvert.SerializeObject(this);

        public static AcquisitionProtocolParameters FromJson(string data)
        {
            return JsonConvert.DeserializeObject<AcquisitionProtocolParameters>(data);
        }
    }
}
