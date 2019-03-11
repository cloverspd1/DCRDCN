using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BEL.DCRDCNWorkflow.Models
{
    [Serializable]
    public class DCRNoCount
    {
        /// <summary>
        /// Item ID
        /// </summary>
        [DataMember]
        public int ID { get; set; }

        /// <summary>
        /// Title Business Unit
        /// </summary>
        [DataMember]
        public string BusinessUnit { get; set; }

        /// <summary>
        /// Year Value
        /// </summary>
        [DataMember]
        public int Year { get; set; }

        /// <summary>
        /// Current Value
        /// </summary>
        [DataMember]
        public int CurrentValue { get; set; }
    }
}