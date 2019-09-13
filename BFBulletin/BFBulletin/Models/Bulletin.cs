using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using System.ComponentModel.DataAnnotations;

namespace BFBulletin.Models
{
    public class Bulletin : TableEntity, IEquatable<Bulletin>, IComparable<Bulletin>
    {
        static string DefaultPartition = "BFBV1";
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime PublishDate { get; set; }
        public string Tags { get; set; }
        public string ReferenceUrl { get; set; }
        public string SeeAlso { get; set; }

        public Bulletin()
        {
            // Initialize default fields
            PartitionKey = DefaultPartition;
            RowKey = "0";
            Title = "Welcome to Bot Framework SDK";
            Body = "There are no announcements at the moment. Stay tuned...";
            Tags = "Default";
            ReferenceUrl = "https://aka.ms/bfcli";
            SeeAlso = "https://dev.botframework.com/";
        }


        public static bool Equal(Bulletin b1, Bulletin b2)
        {
            return (b1.PartitionKey.ToUpper() == b2.PartitionKey.ToUpper() &&
                    b1.RowKey.ToUpper() == b2.RowKey.ToUpper());
        }

        public bool Equals(Bulletin other)
        {
            return Bulletin.Equal(this, other);
        }

        public int CompareTo(Bulletin other)
        {
            // A null value means that this object is greater.
            if (other == null)
            {
                return 1;
            }
            else
            {
                int thisId = 0;
                int otherId = 0;
                bool thisOk = Int32.TryParse(this.RowKey, out thisId);
                bool otherOk = Int32.TryParse(other.RowKey, out otherId);
                if (thisOk && otherOk)
                {
                    // do int comparison
                    return -1 * thisId.CompareTo(otherId);
                }
                else
                {
                    // do string comparison
                    return this.RowKey.CompareTo(other.RowKey);
                }
            }
        }

    }
}
