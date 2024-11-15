using EPiServer.Core;
using EPiServer.DataAnnotations;
using System;

namespace AlloySampleSite.Models.Media
{
    [ContentType(GUID = "EE3BD195-7CB0-4756-AB5F-E5E223CD9820")]
    public class GenericMedia : MediaData
    {
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public virtual String Description { get; set; }
    }
}