using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinhSharingFiles.Domain.SysVariables
{
    public class AwsConfiguration
    {
        public string? AWSAccessKey { get; set; }
        public string? AWSSecretKey { get; set; }
        public string? AWSRegion { get; set; }
        public string? AWSBucketName { get; set; }
    }
}
