using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SyncTrackLog
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SyncTrackLogService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SyncTrackLogService.svc or SyncTrackLogService.svc.cs at the Solution Explorer and start debugging.
    public class SyncTrackLogService : ISyncTrackLogService
    {
       public string GetInterval()
        {
            return "{LocationInterval : 40, SyncInterval : 90 }";

        }

        public string SendTrackLog(List<LocationLog> log)
        {
            return log.Count+" records inserted successfully";
        }
    }
}
