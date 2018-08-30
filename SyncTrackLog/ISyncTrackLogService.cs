using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SyncTrackLog
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISyncTrackLogService" in both code and config file together.
    [ServiceContract]
    public interface ISyncTrackLogService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/GetInterval/", BodyStyle = WebMessageBodyStyle.Wrapped, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        string GetInterval();

        [WebInvoke(Method = "POST", UriTemplate = "/SendTrackLog", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        [OperationContract] string SendTrackLog(List<LocationLog> logs);


    }

    [DataContract]
    
    public class LocationLog
    {
        [DataMember]
        public int IMEINo;
        [DataMember]
        public decimal Latitude;
        [DataMember]
        public decimal Longitude;
        [DataMember]
        public string CreatedOn;




    }

}
